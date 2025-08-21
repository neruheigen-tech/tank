using UnityEngine;
using Unity.Netcode;
using TMPro; // Assuming TextMeshPro for text rendering
using UnityEngine.UI; // For Image component

/// <summary>
/// Manages the game's Heads-Up Display (HUD).
/// Updates UI elements like health, score, and ability cooldowns.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Player HUD References")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image ability1Icon;
    [SerializeField] private TextMeshProUGUI ability1CooldownText;
    [SerializeField] private Image ability2Icon;
    [SerializeField] private TextMeshProUGUI ability2CooldownText;

    [Header("Game State References")]
    [SerializeField] private TextMeshProUGUI redTeamScoreText;
    [SerializeField] private TextMeshProUGUI blueTeamScoreText;
    [SerializeField] private TextMeshProUGUI gameTimerText;

    // We only want to display info for the tank we are controlling
    private Tank localPlayerTank;
    private MonoBehaviour localAbility1;
    private MonoBehaviour localAbility2;

    void Start()
    {
        // Subscribe to score changes
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RedScore.OnValueChanged += UpdateRedScore;
            GameManager.Instance.BlueScore.OnValueChanged += UpdateBlueScore;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RedScore.OnValueChanged -= UpdateRedScore;
            GameManager.Instance.BlueScore.OnValueChanged -= UpdateBlueScore;
        }
    }

    void Update()
    {
        // Find the local player's tank if we haven't already.
        // This is a simple approach; a more robust system would use events.
        if (localPlayerTank == null && NetworkManager.Singleton.LocalClient != null)
        {
            var localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (localPlayerObject != null)
            {
                localPlayerTank = localPlayerObject.GetComponent<Tank>();
                // This assumes abilities are on the same object or children
                localPlayerTank.TryGetComponent(out localAbility1);
                localPlayerTank.TryGetComponent(out localAbility2);
            }
        }

        UpdatePlayerHUD();
    }

    private void UpdatePlayerHUD()
    {
        if (localPlayerTank == null) return;

        // Update health
        if (healthText != null)
        {
            healthText.text = localPlayerTank.currentHealth.Value.ToString("0");
        }

        // Update Ability 1 UI
        UpdateAbilityUI(localAbility1, ability1Icon, ability1CooldownText);

        // Update Ability 2 UI
        UpdateAbilityUI(localAbility2, ability2Icon, ability2CooldownText);
    }

    private void UpdateAbilityUI(MonoBehaviour ability, Image icon, TextMeshProUGUI cooldownText)
    {
        if (ability == null || icon == null || cooldownText == null) return;

        icon.color = Color.white; // Default state
        cooldownText.text = "";

        switch (ability)
        {
            case PowerLunge lunge:
                if (lunge.IsOnCooldown)
                {
                    icon.color = Color.gray;
                    cooldownText.text = lunge.CooldownRemaining.ToString("0.0");
                }
                break;
            case JetJump jump:
                if (jump.IsOnCooldown)
                {
                    icon.color = Color.gray;
                    cooldownText.text = jump.CooldownRemaining.ToString("0.0");
                }
                break;
            case SiegeMode siege:
                if (siege.IsSieged.Value)
                {
                    icon.color = Color.red; // "Active" color
                    cooldownText.text = "ON";
                }
                break;
            case EnergyShield shield:
                if (shield.IsShieldActive.Value)
                {
                    icon.color = Color.cyan; // "Active" color
                }
                break;
        }
    }

    private void UpdateRedScore(int previous, int current)
    {
        if (redTeamScoreText != null) redTeamScoreText.text = "Red: " + current;
    }

    private void UpdateBlueScore(int previous, int current)
    {
        if (blueTeamScoreText != null) blueTeamScoreText.text = "Blue: " + current;
    }
}
