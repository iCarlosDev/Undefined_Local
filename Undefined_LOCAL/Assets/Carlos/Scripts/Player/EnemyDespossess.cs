using System;
using UnityEngine;

public class EnemyDespossess : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Enemy_IA enemy;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Despossess();
        }
    }

    public void StartUp(Enemy_IA enemy, GameObject player)
    {
        this.enemy = enemy;
        this.player = player;
    }

    private void Despossess()
    {
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;
        enemy.gameObject.SetActive(true);
        
        enemy.Die();
        
        player.SetActive(true);

        //Recorremos todos los enemigos en escena y cambiamos el player de referencia que tienen;
        foreach (Enemy_IA enemy in Level1Manager.instance.EnemiesList)
        {
            //Igualamos el player de referencia al EnemyFP;
            enemy.PlayerRef = player.transform;
        }

        gameObject.SetActive(false);
    }
}
