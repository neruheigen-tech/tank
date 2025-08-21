using UnityEngine;

/// <summary>
/// Implements the Saber's Energy Shield ability.
/// Toggles a shield object and reduces movement speed when active.
/// </summary>
public class EnergyShield : NetworkBehaviour
{
    [Header("Dependencies")]
    [Tooltip("The GameObject representing the visual shield. This object should have a Collider.")]
    [SerializeField] private GameObject shieldVisualObject;
    [Tooltip("The tank's controller for modifying speed.")]
    [SerializeField] private TankController tankController;

    [Header("Shield Properties")]
    [Tooltip("The speed multiplier applied when the shield is active (e.g., 0.5 for 50% speed).")]
    [SerializeField, Range(0f, 1f)] private float speedModifier = 0.6f;

    public NetworkVariable<bool> IsShieldActive { get; } = new NetworkVariable<bool>();
    private float originalMoveSpeed;

    void Awake()
    {
        if (tankController == null) tankController = GetComponentInParent<TankController>();

        if (tankController != null)
        {
            originalMoveSpeed = tankController.moveSpeed;
        }

        if (shieldVisualObject != null)
        {
            shieldVisualObject.SetActive(false); // Start with shield off
        }
    }

    public override void OnNetworkSpawn()
    {
        IsShieldActive.OnValueChanged += OnShieldStateChanged;
    }

    public override void OnNetworkDespawn()
    {
        IsShieldActive.OnValueChanged -= OnShieldStateChanged;
    }

    private void OnShieldStateChanged(bool previousValue, bool newValue)
    {
        shieldVisualObject.SetActive(newValue);

        // Movement speed change should only affect the owner
        if (IsOwner)
        {
            if (newValue)
            {
                tankController.moveSpeed = originalMoveSpeed * speedModifier;
                Debug.Log("SHIELD: Activated. Movement speed reduced.");
            }
            else
            {
                tankController.moveSpeed = originalMoveSpeed;
                Debug.Log("SHIELD: Deactivated. Movement speed restored.");
            }
        }
    }

    /// <summary>
    /// Toggles the shield's state. Should only be called on the server.
    /// </summary>
    public void Toggle()
    {
        IsShieldActive.Value = !IsShieldActive.Value;
    }
}
