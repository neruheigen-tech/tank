using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the game state for Team Annihilation mode.
/// This is a singleton to ensure only one instance exists.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("Game Mode Settings")]
    [Tooltip("Number of kills required to win.")]
    public int scoreToWin = 30;
    [Tooltip("Time in seconds before a tank respawns.")]
    public float respawnDelay = 4.0f;

    [Header("Team Spawn Points")]
    public List<Transform> redTeamSpawns;
    public List<Transform> blueTeamSpawns;

    private Dictionary<Team, int> teamScores;

    void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        // Initialize scores
        teamScores = new Dictionary<Team, int>
        {
            { Team.Red, 0 },
            { Team.Blue, 0 }
        };
    }

    /// <summary>
    /// Called by a tank when it is destroyed.
    /// </summary>
    /// <param name="destroyedTank">The tank that was destroyed.</param>
    /// <param name="killerTank">The tank that got the kill.</param>
    public void ReportKill(Tank destroyedTank, Tank killerTank)
    {
        // Award score if it wasn't a suicide or teamkill
        if (killerTank != null && killerTank.team != destroyedTank.team)
        {
            teamScores[killerTank.team]++;
            Debug.Log(killerTank.team + " scores! Current score: Red " + teamScores[Team.Red] + " - Blue " + teamScores[Team.Blue]);

            // Check for win condition
            if (teamScores[killerTank.team] >= scoreToWin)
            {
                EndMatch(killerTank.team);
                return; // Stop further processing
            }
        }

        // Start the respawn process for the destroyed tank
        StartCoroutine(RespawnCoroutine(destroyedTank));
    }

    private IEnumerator RespawnCoroutine(Tank tankToRespawn)
    {
        yield return new WaitForSeconds(respawnDelay);

        List<Transform> spawns = (tankToRespawn.team == Team.Red) ? redTeamSpawns : blueTeamSpawns;
        Transform spawnPoint = null;

        if (spawns != null && spawns.Count > 0)
        {
            spawnPoint = spawns[Random.Range(0, spawns.Count)];
        }
        else
        {
            Debug.LogWarning("No spawn points configured for team " + tankToRespawn.team + "! Defaulting to origin.");
            spawnPoint = transform; // Default to the GameManager's position
        }

        tankToRespawn.Respawn(spawnPoint.position, spawnPoint.rotation);
        Debug.Log(tankToRespawn.name + " has respawned.");
    }

    private void EndMatch(Team winningTeam)
    {
        Debug.Log("GAME OVER! Team " + winningTeam + " is victorious!");
        // In a real game, this would load a post-match screen.
        Time.timeScale = 0f; // A simple way to pause the game
    }
}
