using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Manages the lobby state, including displaying connected players
/// and handling the transition to the main game scene.
/// </summary>
public class LobbyManager : NetworkBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI References")]
    [Tooltip("A reference to a UI panel or layout group where player list items will be created.")]
    [SerializeField] private Transform playerListContent;
    [Tooltip("A prefab for displaying a single player in the lobby.")]
    [SerializeField] private GameObject playerListItemPrefab;
    [Tooltip("A button that is only visible to the host to start the game.")]
    [SerializeField] private GameObject startGameButton;

    // A list of connected clients, maintained by the server.
    private NetworkList<PlayerLobbyState> connectedPlayers;

    private void Awake()
    {
        connectedPlayers = new NetworkList<PlayerLobbyState>();
    }

    public override void OnNetworkSpawn()
    {
        // Only the host should see the start game button
        if (startGameButton != null)
        {
            startGameButton.SetActive(IsHost);
        }

        // Subscribe to events
        connectedPlayers.OnListChanged += OnPlayerListChanged;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        if (IsServer)
        {
            // Add the host to the list of players
            AddPlayerToList(NetworkManager.Singleton.LocalClientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to prevent memory leaks
        connectedPlayers.OnListChanged -= OnPlayerListChanged;
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // This is called on the server when a new client connects
        AddPlayerToList(clientId);
    }

    private void AddPlayerToList(ulong clientId)
    {
        connectedPlayers.Add(new PlayerLobbyState
        {
            ClientId = clientId,
            PlayerName = "Player " + clientId // Simple name for now
        });
    }

    private void OnPlayerListChanged(NetworkListEvent<PlayerLobbyState> changeEvent)
    {
        // This is called on all clients whenever the list changes.
        // We can now update the UI.
        UpdatePlayerListUI();
    }

    private void UpdatePlayerListUI()
    {
        // Clear the existing list
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Add a new UI item for each player in the list
        foreach (var player in connectedPlayers)
        {
            GameObject item = Instantiate(playerListItemPrefab, playerListContent);
            // In a real UI, you'd set the text here, e.g.:
            // item.GetComponent<TextMeshProUGUI>().text = player.PlayerName;
            Debug.Log("Lobby UI: Displaying " + player.PlayerName);
        }
    }

    public void StartGame()
    {
        // This method is called by the host's "Start Game" button
        if (!IsHost) return;

        // Use the NetworkSceneManager to load the scene for all connected clients
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}

/// <summary>
/// A simple struct to hold player data in the lobby.
/// Must implement INetworkSerializable to be used in a NetworkList.
/// </summary>
public struct PlayerLobbyState : INetworkSerializable
{
    public ulong ClientId;
    public Unity.Collections.FixedString64Bytes PlayerName; // Using a fixed string for network serialization

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
    }
}
