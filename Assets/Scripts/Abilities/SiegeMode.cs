using UnityEngine;

/// <summary>
/// Implements the Sniper's Siege Mode ability.
/// When active, it disables tank movement in exchange for stat boosts.
/// </summary>
public class SiegeMode : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("The controller to disable during siege mode.")]
    [SerializeField] private TankController tankController;
    [Tooltip("The weapon to modify when in siege mode.")]
    [SerializeField] private GaussRailgun railgun; // Assuming Sniper always has a railgun

    [Header("Siege Mode Stats")]
    [Tooltip("The damage multiplier to apply when in siege mode.")]
    [SerializeField] private float damageMultiplier = 1.5f;

    private bool isSieged = false;
    private float originalDamage;

    void Awake()
    {
        // Auto-assign components if not set in the inspector
        if (tankController == null) tankController = GetComponentInParent<TankController>();
        if (railgun == null) railgun = GetComponentInChildren<GaussRailgun>();

        if (railgun != null)
        {
            originalDamage = railgun.damage;
        }
    }

    /// <summary>
    /// Toggles Siege Mode on and off.
    /// </summary>
    public void Toggle()
    {
        isSieged = !isSieged;

        if (isSieged)
        {
            // Enter Siege Mode
            if (tankController != null)
            {
                tankController.enabled = false;
            }
            if (railgun != null)
            {
                railgun.damage = originalDamage * damageMultiplier;
            }
            Debug.Log("SIEGE MODE: Activated. Movement locked. Damage increased.");
        }
        else
        {
            // Exit Siege Mode
            if (tankController != null)
            {
                tankController.enabled = true;
            }
            if (railgun != null)
            {
                railgun.damage = originalDamage;
            }
            Debug.Log("SIEGE MODE: Deactivated. Movement restored. Damage normalized.");
        }
    }
}
