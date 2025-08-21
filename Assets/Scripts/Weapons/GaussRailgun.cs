using UnityEngine;
using System.Collections;

/// <summary>
/// Implements the Sniper's high-damage, single-shot railgun.
/// Inherits from the abstract Weapon class.
/// </summary>
public class GaussRailgun : Weapon
{
    [Header("Railgun Visuals")]
    [Tooltip("The point from which the railgun fires.")]
    [SerializeField] private Transform firePoint;
    [Tooltip("Optional: Particle effect for firing.")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [Tooltip("Optional: LineRenderer to trace the shot.")]
    [SerializeField] private LineRenderer beamEffect;

    private Tank owner;

    void Awake()
    {
        // Set the specific stats for this weapon
        damage = 75f;
        cooldown = 2.0f;
        range = 250f;
        owner = GetComponentInParent<Tank>();
    }

    /// <summary>
    /// Fires a raycast forward to simulate a precise shot.
    /// </summary>
    public override void Attack()
    {
        if (!CanAttack()) return;

        lastAttackTime = Time.time;

        if (muzzleFlash != null) muzzleFlash.Play();

        Vector3 endPoint = firePoint.position + firePoint.forward * range;

        if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, range))
        {
            Debug.Log("GaussRailgun hit: " + hit.transform.name);
            endPoint = hit.point;

            // Check if the hit object has a Tank component and apply damage
            if (hit.transform.TryGetComponent<Tank>(out Tank target))
            {
                target.TakeDamage(damage, owner);
            }
        }
        else
        {
            Debug.Log("GaussRailgun missed.");
        }

        // Handle the visual trail of the shot
        if (beamEffect != null)
        {
            StartCoroutine(ShowBeamEffect(endPoint));
        }
    }

    /// <summary>
    /// A coroutine to briefly show the beam effect.
    /// </summary>
    private IEnumerator ShowBeamEffect(Vector3 endPoint)
    {
        beamEffect.SetPosition(0, firePoint.position);
        beamEffect.SetPosition(1, endPoint);
        beamEffect.enabled = true;
        yield return new WaitForSeconds(0.075f);
        beamEffect.enabled = false;
    }

    void OnValidate()
    {
        if (firePoint == null)
            firePoint = transform;
    }
}
