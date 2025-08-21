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
                localPlayerObject.TryGetComponent(out localAbility1);
                localPlayerObject.TryGetComponent(out localAbility2);
            }
        }

        UpdatePlayerHUD();
        UpdateGameStateHUD();
    }

    private void UpdatePlayerHUD()
    {
        if (localPlayerTank == null) return;

        // Update health
        if (healthText != null)
        {
            healthText.text = localPlayerTank.currentHealth.Value.ToString("0");
        }

        // In a real project, this would be cleaner, likely with an IAbility interface
        // This is a conceptual demonstration of how cooldowns would be displayed.
        if (ability1Icon != null && localAbility1 != null)
        {
            // This part is highly speculative as it depends on the ability's implementation
            // For now, let's assume we can't get cooldowns easily and just show icons.
        }
    }

    private void UpdateGameStateHUD()
    {
        if (GameManager.Instance == null) return;

        // This part is also conceptual as we don't have direct access to the score dictionary.
        // A real implementation would have the GameManager expose scores via public properties or events.
        if (redTeamScoreText != null)
        {
            // redTeamScoreText.text = GameManager.Instance.GetScore(Team.Red).ToString();
        }
        if (blueTeamScoreText != null)
        {
            // blueTeamScoreText.text = GameManager.Instance.GetScore(Team.Blue).ToString();
        }
    }
}
