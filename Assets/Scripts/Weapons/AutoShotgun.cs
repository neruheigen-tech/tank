using UnityEngine;

/// <summary>
/// Implements the Assault's close-range Auto Shotgun.
/// Fires multiple pellets in a spread pattern.
/// </summary>
public class AutoShotgun : Weapon
{
    [Header("Shotgun Specifics")]
    [Tooltip("The point from which the pellets are fired.")]
    [SerializeField] private Transform firePoint;
    [Tooltip("The number of individual pellets fired in one shot.")]
    [SerializeField] private int pelletsPerShot = 8;
    [Tooltip("The maximum angle of deviation from the center for pellet spread.")]
    [SerializeField] private float maxSpreadAngle = 10.0f;

    private Tank owner;

    void Awake()
    {
        // Set the specific stats for this weapon
        damage = 12f; // This is the damage per pellet
        cooldown = 0.8f;
        range = 15f;
        owner = GetComponentInParent<Tank>();
    }

    /// <summary>
    /// Fires multiple raycasts in a random cone to simulate a shotgun blast.
    /// </summary>
    public override void Attack()
    {
        if (!CanAttack()) return;
        lastAttackTime = Time.time;

        Debug.Log("AutoShotgun firing " + pelletsPerShot + " pellets.");

        for (int i = 0; i < pelletsPerShot; i++)
        {
            // Calculate a random direction within a cone.
            // We use Quaternion.Euler to create a random rotation and apply it to the forward vector.
            float spreadX = Random.Range(-maxSpreadAngle, maxSpreadAngle);
            float spreadY = Random.Range(-maxSpreadAngle, maxSpreadAngle);
            Vector3 direction = Quaternion.Euler(spreadX, spreadY, 0) * firePoint.forward;

            if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, range))
            {
                if (hit.transform.TryGetComponent<Tank>(out Tank target))
                {
                    target.TakeDamage(damage, owner);
                }
                // Could add a small impact particle effect here.
            }
        }
    }

    void OnValidate()
    {
        if (firePoint == null)
            firePoint = transform;
    }
}
