using UnityEngine;

public class PlayerScriptStorage : MonoBehaviour
{

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private EnemyPossess _enemyPossess;
    [SerializeField] private PlayerHealth _playerHealth;
    
    //GETTERS && SETTERS//
    public PlayerMovement PlayerMovement => _playerMovement;
    public EnemyPossess EnemyPossess => _enemyPossess;
    public PlayerHealth PlayerHealth => _playerHealth;
    ///////////////////////////////
    
    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _enemyPossess = GetComponent<EnemyPossess>();
        _playerHealth = GetComponent<PlayerHealth>();
    }
}
