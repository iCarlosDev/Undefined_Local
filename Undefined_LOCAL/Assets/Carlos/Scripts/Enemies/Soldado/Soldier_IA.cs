using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier_IA : Enemy_IA
{
    private Coroutine findPlayerCooldown;
    private Coroutine findInRoomCooldown;
    
    [Header("--- ROOM WAYPOINTS ---")]
    [Space(10)]
    [SerializeField] private List<Transform> roomWaypoints;
    [SerializeField] private int indexRoomWaypoints;
    [SerializeField] private bool searchingInRoom;

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
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < 0.1f)
        {
            Debug.Log("Finding Player...");
            
            //Comprobamos si hay waypoints en la lista y si está buscando en una sala;
            if (roomWaypoints.Count != 0 && searchingInRoom)
            {
                UpdateRoomWaypoint();
                
                //Si la corrutina "findPlayerCooldown es nula la ejecutamos"
                if (findPlayerCooldown == null)
                {
                    findInRoomCooldown = StartCoroutine(FindInRoomCooldown_Coroutine()); 
                }
            }
            
            //Si el NPC está buscando en una sala no se ejecutará el código restante;
            if (searchingInRoom) return;

            //Ejecutamos la corrutina "FindPlayerCooldown" y reseteamos el Path;
            if (findPlayerCooldown == null)
            {
                _navMeshAgent.ResetPath();
                findPlayerCooldown = StartCoroutine(FindPlayerCooldown_Coroutine());   
            }   
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
    
    private void StopFindingPlayer()
    {
        //Si la corrutina "FindPlayerCooldown" estaba en ejecución y volvemos a encontrar al player la detenemos;
        if (findPlayerCooldown != null)
        {
            StopCoroutine(findPlayerCooldown);
            findPlayerCooldown = null;
                
            Debug.Log("Player Found");
        }

        //Si la corrutina "FindInRoomCooldown" estaba en ejecución y volvemos a encontrar al player la detenemos;
        if (findInRoomCooldown != null)
        {
            StopCoroutine(findInRoomCooldown);
            findInRoomCooldown = null;
        }
    }

    #region - ROOM FINDING -
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RoomCollider"))
        {
            SetRoomWaypoints(other);
            searchingInRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RoomCollider"))
        {
            searchingInRoom = false;
        }
    }

    //Método para recoger los waypoints que seguirá el NPC cuando esté buscando al player;
    private void SetRoomWaypoints(Collider other)
    {
        roomWaypoints = new List<Transform>();
        roomWaypoints.AddRange(other.GetComponentsInChildren<Transform>());
        roomWaypoints.Remove(roomWaypoints[0]);
    }
    
    //Método para actualizar el waypoint al que tiene que ir el NPC;
    private void UpdateRoomWaypoint()
    {
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < 0.3f)
        {
            indexRoomWaypoints = (indexRoomWaypoints + 1) % roomWaypoints.Count;
            _navMeshAgent.SetDestination(roomWaypoints[indexRoomWaypoints].position);
        }
    }

    //Corrutina para setear un tiempo límite para buscar en la sala en la que haya estado el Player;
    private IEnumerator FindInRoomCooldown_Coroutine()
    {
        yield return new WaitForSeconds(30f);
        searchingInRoom = false;
        
        Debug.LogWarning("Stop Searching In Room");
    }
    
    #endregion

    #endregion
}
