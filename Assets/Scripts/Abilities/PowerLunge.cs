using UnityEngine;

/// <summary>
/// Implements the Saber's Power Lunge ability.
/// Provides a short forward dash.
/// </summary>
public class PowerLunge : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("The Rigidbody to apply the lunge force to.")]
    [SerializeField] private Rigidbody tankRigidbody;

    [Header("Lunge Properties")]
    [Tooltip("The magnitude of the forward impulse.")]
    [SerializeField] private float lungeForce = 800f;
    [Tooltip("The cooldown time in seconds for the lunge.")]
    [SerializeField] private float cooldown = 4f;

    private float lastLungeTimestamp = -99f;

    void Awake()
    {
        if (tankRigidbody == null) tankRigidbody = GetComponentInParent<Rigidbody>();
    }

    /// <summary>
    /// Activates the lunge if it is not on cooldown.
    /// </summary>
    public void Activate()
    {
        if (Time.time >= lastLungeTimestamp + cooldown)
        {
            lastLungeTimestamp = Time.time;
            // Use ForceMode.Impulse for an instantaneous burst of force
            tankRigidbody.AddForce(tankRigidbody.transform.forward * lungeForce, ForceMode.Impulse);
            Debug.Log("LUNGE: Activated!");
        }
        else
        {
            Debug.Log("LUNGE: On cooldown.");
        }
    }
}
