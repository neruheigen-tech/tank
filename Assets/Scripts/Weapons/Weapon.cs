using UnityEngine;

/// <summary>
/// Abstract base class for all weapon systems.
/// Defines the core interface for attacking.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("Base Weapon Stats")]
    [Tooltip("Damage dealt per attack.")]
    public float damage = 10f;
    [Tooltip("Time in seconds between attacks.")]
    public float cooldown = 0.5f;
    [Tooltip("The range of the weapon in meters.")]
    public float range = 100f;

    // The time the last attack was performed.
    protected float lastAttackTime;

    /// <summary>
    /// Checks if the weapon is ready to be fired again.
    /// </summary>
    public virtual bool CanAttack()
    {
        return Time.time > lastAttackTime + cooldown;
    }

    /// <summary>
    /// Abstract method to be implemented by all concrete weapon classes.
    /// This will contain the logic for how the weapon fires.
    /// </summary>
    public abstract void Attack();
}
