using UnityEngine;
using Unity.Netcode; // Assuming Netcode for GameObjects is used

/// <summary>
/// Handles the player input and physics-based movement for a tank.
/// This script now includes networking logic to ensure only the owner can control it,
/// and actions are sent to the server for processing.
/// </summary>
public class TankController : NetworkBehaviour
{
    [Header("Tank Attributes")]
    public float moveSpeed = 12f;
    public float turnSpeed = 180f; // Degrees per second

    [Header("Object References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform turretTransform; // The part of the tank that aims
    [SerializeField] private Weapon equippedWeapon;

    [Header("Ability Slots")]
    [Tooltip("Ability mapped to the 'Q' key.")]
    [SerializeField] private MonoBehaviour ability1;
    [Tooltip("Ability mapped to the 'E' key.")]
    [SerializeField] private MonoBehaviour ability2;


    private float moveInput;
    private float turnInput;
    private Vector3 mouseWorldPosition;

    void Update()
    {
        // This ensures that only the player who owns this tank object can control it.
        if (!IsOwner) return;

        // In a real Unity environment, this would read from the input manager
        // Vertical maps to W/S keys, Horizontal maps to A/D keys
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // Aiming is client-side for responsiveness, the actual shot is validated by the server.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            mouseWorldPosition = hit.point;
        }

        // --- Input that triggers server actions ---

        if (Input.GetButtonDown("Fire1"))
        {
            // Instead of calling Attack() directly, we call a ServerRpc.
            RequestAttackServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RequestAbility1ServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RequestAbility2ServerRpc();
        }
    }

    [ServerRpc]
    private void RequestAttackServerRpc()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.Attack();
        }
    }

    [ServerRpc]
    private void RequestAbility1ServerRpc()
    {
        ActivateAbility(ability1);
    }

    [ServerRpc]
    private void RequestAbility2ServerRpc()
    {
        ActivateAbility(ability2);
    }

    // This method is now only called on the server via an RPC
    private void ActivateAbility(MonoBehaviour ability)
    {
        if (ability == null) return;

        // This logic now runs on the server, ensuring actions are authoritative.
        switch (ability)
        {
            case SiegeMode siege:
                siege.Toggle();
                break;
            case EnergyShield shield:
                shield.Toggle();
                break;
            case PowerLunge lunge:
                lunge.Activate();
                break;
            case JetJump jump:
                jump.Activate();
                break;
            default:
                Debug.LogWarning("Ability type not recognized: " + ability.GetType().Name);
                break;
        }
    }

    void FixedUpdate()
    {
        // Movement should also only be processed for the owner.
        // The NetworkTransform component will handle synchronizing the result to other clients.
        if (!IsOwner) return;

        // Apply physics-based movement and rotation in FixedUpdate for stability
        MoveTank();
        TurnTank();
        AimTurret();
    }

    private void MoveTank()
    {
        // Calculate the movement vector and apply it to the rigidbody
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    private void TurnTank()
    {
        // Calculate the turn rotation and apply it to the rigidbody
        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void AimTurret()
    {
        if (turretTransform != null)
        {
            // Determine the direction from the turret to the mouse world position
            Vector3 turretDirection = mouseWorldPosition - turretTransform.position;
            turretDirection.y = 0; // Keep the turret aiming level with the ground

            // Create the desired rotation and apply it smoothly
            Quaternion lookRotation = Quaternion.LookRotation(turretDirection);
            turretTransform.rotation = lookRotation;
        }
    }

    // OnValidate is a Unity-specific method that runs in the editor when the script is loaded
    // or a value is changed in the Inspector. It's useful for auto-assigning components.
    void OnValidate()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }
}
