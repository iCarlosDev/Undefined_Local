using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerScriptStorage _playerScriptStorage;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    private void Awake()
    {
        _playerScriptStorage = GetComponent<PlayerScriptStorage>();
    }

    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _playerScriptStorage.PlayerMovement.Animator.SetTrigger("Die");
        _playerScriptStorage.PlayerMovement.enabled = false;
        _playerScriptStorage.EnemyPossess.enabled = false;

        
        _playerScriptStorage.PlayerHealth.enabled = false;
    }
}
