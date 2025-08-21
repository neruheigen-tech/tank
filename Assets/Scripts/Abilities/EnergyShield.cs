using UnityEngine;

/// <summary>
/// Implements the Saber's Energy Shield ability.
/// Toggles a shield object and reduces movement speed when active.
/// </summary>
public class EnergyShield : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("The GameObject representing the visual shield. This object should have a Collider.")]
    [SerializeField] private GameObject shieldVisualObject;
    [Tooltip("The tank's controller for modifying speed.")]
    [SerializeField] private TankController tankController;

    [Header("Shield Properties")]
    [Tooltip("The speed multiplier applied when the shield is active (e.g., 0.5 for 50% speed).")]
    [SerializeField, Range(0f, 1f)] private float speedModifier = 0.6f;

    private bool isShieldActive = false;
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

    /// <summary>
    /// Toggles the shield's state.
    /// </summary>
    public void Toggle()
    {
        isShieldActive = !isShieldActive;

        shieldVisualObject.SetActive(isShieldActive);

        if (isShieldActive)
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
