using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement; // Required for scene management

/// <summary>
/// Handles the UI logic for the main menu screen.
/// Provides methods for starting the game as a host, joining as a client, and quitting.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The name of the lobby scene to load after connecting.")]
    [SerializeField] private string lobbySceneName = "LobbyScene"; // Example scene name

    /// <summary>
    /// Starts the game as a Host (Server + Client).
    /// This would be called by a "Host Game" button.
    /// </summary>
    public void StartHost()
    {
        Debug.Log("Starting as Host...");
        NetworkManager.Singleton.StartHost();

        // The host loads the lobby scene directly. The network session will ensure
        // clients who connect later will also be transitioned to this scene.
        SceneManager.LoadScene(lobbySceneName);
    }

    /// <summary>
    /// Joins an existing game as a Client.
    /// This would be called by a "Join Game" button.
    /// </summary>
    public void JoinClient()
    {
        Debug.Log("Joining as Client...");
        NetworkManager.Singleton.StartClient();
        // Clients don't load the scene directly. The server will tell them which scene to load.
        // The NetworkManager handles this synchronization automatically.
    }

    /// <summary>
    /// Quits the application.
    /// This would be called by a "Quit" button.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
