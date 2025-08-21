using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Base class for all tanks, managing shared stats like health and team affiliation.
/// Includes networking for health synchronization.
/// </summary>
public class Tank : NetworkBehaviour
{
    [Header("Tank Stats")]
    [Tooltip("The maximum health of the tank.")]
    public float maxHealth = 100f;
    [Tooltip("The team this tank belongs to.")]
    public Team team;

    public NetworkVariable<float> currentHealth = new NetworkVariable<float>();
    private Tank lastAttacker;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
    }

    /// <summary>
    /// Reduces the tank's health and records the attacker. Must be run on the server.
    /// </summary>
    /// <param name="amount">The amount of damage to take.</param>
    /// <param name="attacker">The tank that dealt the damage.</param>
    public void TakeDamage(float amount, Tank attacker)
    {
        // Damage logic should only execute on the server.
        if (!IsServer) return;

        // Don't take damage from teammates
        if (attacker != null && attacker.team == this.team)
        {
            return;
        }

        currentHealth.Value -= amount;
        lastAttacker = attacker;
        Debug.Log(transform.name + " takes " + amount + " damage from " + attacker.name + ". Current health: " + currentHealth.Value);

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles the tank's destruction by notifying the GameManager.
    /// </summary>
    private void Die()
    {
        Debug.Log(transform.name + " has been destroyed by " + (lastAttacker != null ? lastAttacker.name : "the environment") + ".");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReportKill(this, lastAttacker);
        }

        // Deactivate the tank object. The GameManager will reactivate it upon respawn.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets the tank's state for respawning. Must be run on the server.
    /// </summary>
    public void Respawn(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        currentHealth.Value = maxHealth;
        lastAttacker = null;

        // The GameManager handles activating/deactivating the object, but we might
        // need to tell clients to re-enable it visually.
        RespawnClientRpc(position, rotation);
    }

    [ClientRpc]
    private void RespawnClientRpc(Vector3 position, Quaternion rotation)
    {
        // If not the server, manually set position and activate the object
        if (!IsServer)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
        gameObject.SetActive(true);
    }
}
