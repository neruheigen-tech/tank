using UnityEngine;

/// <summary>
/// Implements the Assault's Jet Jump ability.
/// Provides a powerful vertical boost.
/// </summary>
public class JetJump : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("The Rigidbody to apply the jump force to.")]
    [SerializeField] private Rigidbody tankRigidbody;

    [Header("Jump Properties")]
    [Tooltip("The magnitude of the upward impulse.")]
    [SerializeField] private float jumpForce = 1000f;
    [Tooltip("The cooldown time in seconds for the jump.")]
    [SerializeField] private float cooldown = 3f;

    // GDD says "Long press for higher jump", this could be a charge mechanic.
    // For this prototype, a fixed jump is sufficient.

    private float lastJumpTimestamp = -99f;

    public float CooldownRemaining => Mathf.Max(0f, (lastJumpTimestamp + cooldown) - Time.time);
    public bool IsOnCooldown => CooldownRemaining > 0;

    void Awake()
    {
        if (tankRigidbody == null) tankRigidbody = GetComponentInParent<Rigidbody>();
    }

    /// <summary>
    /// Activates the jump if it is not on cooldown.
    /// </summary>
    public void Activate()
    {
        // A more advanced version might check if the tank is grounded.
        if (Time.time >= lastJumpTimestamp + cooldown)
        {
            lastJumpTimestamp = Time.time;
            tankRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("JUMP: Activated!");
        }
        else
        {
            Debug.Log("JUMP: On cooldown.");
        }
    }
}
