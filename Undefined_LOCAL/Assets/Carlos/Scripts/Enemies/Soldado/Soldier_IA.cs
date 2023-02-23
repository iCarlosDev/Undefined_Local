using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Soldier_IA : Enemy_IA
{
    private Coroutine findPlayerCooldown;
    
    public override void Update()
    {
        base.Update();
        
        //Si el player no ha sido detectado nunca hará la lógica restante;
        if (!isPlayerDetected) return;
        
        //Se comprueba si se puede chasear al Player;
        if (_enemyScriptStorage.FieldOfView.canSeePlayer)
        {
            ChasePlayer();
            StopFindingPlayer();
        }
        else
        {
            FindPlayer();
        }
    }

    #region - PLAYER DETECTED -

    //Método para seguir y disparar al player;
    private void ChasePlayer()
    {
        Debug.Log("Chasing Player...");
        
        _navMeshAgent.SetDestination(_enemyScriptStorage.FieldOfView.playerRef.transform.position);
        _navMeshAgent.stoppingDistance = 4;

        //Depende de la distancia entre el NPC y el Player el NPC disparará o Pateará;
        if (Vector3.Distance(transform.position, _enemyScriptStorage.FieldOfView.playerRef.transform.position) < 0.8f)
        {
            Kick();
        }
        else
        {
            Shoot(); 
        }
    }

    private void Shoot()
    {
        Debug.Log("Shooting...");
    }

    private void Kick()
    {
        Debug.Log("Kicking...");
    }

    #endregion

    #region - FINDING PLAYER -

    private void FindPlayer()
    {
        Debug.Log("Checking Last Player Position");
        _navMeshAgent.SetDestination(_navMeshAgent.destination);
        _navMeshAgent.stoppingDistance = 0f;
        
        //Comprobamos que el NPC ha llegado a la última posición donde ha visto al Player;
        if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            //Ejecutamos la corrutina "FindPlayerCooldown" y reseteamos el Path;
            if (findPlayerCooldown == null)
            {
                Debug.Log("Finding Player...");
                _navMeshAgent.ResetPath();
                findPlayerCooldown = StartCoroutine(FindPlayerCooldown_Coroutine());   
            }   
        }
    }
    
    private void StopFindingPlayer()
    {
        //Si la corrutina "FindPlayerCooldown" estaba en ejecución y volvemos a encontrar al player la detenemos;
        if (findPlayerCooldown != null)
        {
            StopCoroutine(findPlayerCooldown);
            findPlayerCooldown = null;
                
            Debug.Log("Player Found");
        }
    }
    
    //Corrutina para dejar de buscar al player si se hace el waitForSeconds;
    private IEnumerator FindPlayerCooldown_Coroutine()
    {
        yield return new WaitForSeconds(5f);
        _navMeshAgent.SetDestination(waypointsList[waypointsListIndex].position);
        _navMeshAgent.stoppingDistance = 1f;
        isPlayerDetected = false;
        Debug.Log("Player not Found, Going Patroling");
    }

    #endregion
}
