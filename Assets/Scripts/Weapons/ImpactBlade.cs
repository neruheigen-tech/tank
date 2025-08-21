using UnityEngine;

/// <summary>
/// Implements the Saber's close-quarters melee swing.
/// Inherits from the abstract Weapon class.
/// </summary>
public class ImpactBlade : Weapon
{
    [Header("Blade Specifics")]
    [Tooltip("The center point of the melee swing area.")]
    [SerializeField] private Transform attackPoint;
    [Tooltip("The dimensions of the attack box, relative to the attack point.")]
    [SerializeField] private Vector3 attackBoxSize = new Vector3(4f, 2f, 3f); // Width, Height, Forward-Length
    [Tooltip("Which layers should this weapon be able to hit.")]
    [SerializeField] private LayerMask hittableLayers;

    private Tank owner;

    void Awake()
    {
        // Set the specific stats for this weapon
        damage = 90f;
        cooldown = 1.2f;
        range = 3f; // This is more of a forward-offset for the box
        owner = GetComponentInParent<Tank>();
    }

    /// <summary>
    /// Performs an overlap check in a box in front of the tank, damaging all valid targets.
    /// </summary>
    public override void Attack()
    {
        if (!CanAttack()) return;

        lastAttackTime = Time.time;

        // The actual position of the attack box in world space
        Vector3 attackCenter = attackPoint.position + attackPoint.forward * (attackBoxSize.z / 2);

        Collider[] hits = Physics.OverlapBox(attackCenter, attackBoxSize / 2, attackPoint.rotation, hittableLayers);

        Debug.Log("ImpactBlade swung, detected " + hits.Length + " colliders.");

        foreach (var hit in hits)
        {
            // We don't want to hit ourselves
            if (hit.transform.root == transform.root) continue;

            if (hit.TryGetComponent<Tank>(out Tank target))
            {
                target.TakeDamage(damage, owner);
                // GDD mentions knockback, which would be implemented here
                // e.g., if(target.TryGetComponent<Rigidbody>(out Rigidbody targetRb))
                // { targetRb.AddForce(attackPoint.forward * knockbackForce); }
            }
        }
    }

    // This is a special Unity method that draws gizmos in the scene view.
    // It's extremely useful for debugging and visualizing areas like this.
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        // Set the Gizmos' matrix to the attack point's transform to draw the box in local space
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, Vector3.one);

        // The center of the gizmo box is offset forward by half its z-size
        Vector3 boxCenter = new Vector3(0, 0, attackBoxSize.z / 2);
        Gizmos.DrawWireCube(boxCenter, attackBoxSize);

        Gizmos.matrix = oldMatrix;
    }
}
