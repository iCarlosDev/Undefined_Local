using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Soldier_IA : Enemy_IA
{
    private Coroutine findPlayerCooldown;
    private Coroutine findInRoomCooldown;

    [Header("--- LOOK PLAYER PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private float dumping;
    
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

    private void ChangePlayerRef(Transform playerRef)
    {
        base.playerRef = playerRef;
    }

    #region - PLAYER DETECTED -

    //Método para seguir y disparar al player;
    private void ChasePlayer()
    {
        Debug.Log("<color=orange>Chasing Player...</color>");

        //Si el componente "NavMeshAgent" está activo...;
        if (_navMeshAgent.enabled)
        {
            _navMeshAgent.SetDestination(playerRef.position);
            _navMeshAgent.stoppingDistance = 4;
            lookPlayer();
        }

        //Depende de la distancia entre el NPC y el Player el NPC disparará o Pateará;
        if (Vector3.Distance(transform.position, playerRef.position) < 0.8f)
        {
            Kick();
        }
        else
        {
            Shoot(); 
        }
    }

    //Método para rotar al NPC para que mire al player en el eje "Y";
    private void lookPlayer()
    {
        //Asignamos la distancia entre el player y el NPC a la variable local;
        var lookPos = playerRef.position - transform.position;
        lookPos.y = 0;
        //Asignamos la creación de la rotación del NPC en el eje "Y" a la variable local;
        var rotation = Quaternion.LookRotation(lookPos);
        //Rotamos el NPC con smooth;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dumping);
    }

    private void Shoot()
    {
        Debug.Log("<color=purple>Shooting...</color>");
    }

    private void Kick()
    {
        Debug.Log("<color=purple>Kicking...</color>");
    }

    #endregion

    #region - FINDING PLAYER -

    private void FindPlayer()
    {
        Debug.Log("<color=blue>Checking Last Player Position</color>");
        _navMeshAgent.SetDestination(_navMeshAgent.destination);
        _navMeshAgent.stoppingDistance = 0f;
        
        //Comprobamos que el NPC ha llegado a la última posición donde ha visto al Player;
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < 0.1f)
        {
            Debug.Log("<color=blue>Finding Player...</color>");
            
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
                //findPlayerCooldown = StartCoroutine(FindPlayerCooldown_Coroutine());   
            }

            if (!Level1Manager.instance.AlarmActivated)
            {
                GoActivateAlarm();
            }
            else
            {
                SearchInRooms();
            }
        }
    }

    //Corrutina para dejar de buscar al player si se hace el waitForSeconds;
    private IEnumerator FindPlayerCooldown_Coroutine()
    {
        yield return new WaitForSeconds(5f);
        /*_navMeshAgent.SetDestination(waypointsList[waypointsListIndex].position);
        _navMeshAgent.stoppingDistance = 1f;
        isPlayerDetected = false;*/
    }
    
    private void StopFindingPlayer()
    {
        //Si la corrutina "FindPlayerCooldown" estaba en ejecución y volvemos a encontrar al player la detenemos;
        if (findPlayerCooldown != null)
        {
            StopCoroutine(findPlayerCooldown);
            findPlayerCooldown = null;
                
            Debug.Log("<color=orange>Player Found</color>");
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
        if (other.CompareTag("RoomCollider") || other.CompareTag("SafeRoomCollider"))
        {
            SetRoomWaypoints(other);
            searchingInRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RoomCollider") || other.CompareTag("SafeRoomCollider"))
        {
            searchingInRoom = false;
        }
    }
    
    //El NPC una vez la alarma esté activada se pondrá a buscar al player en las salas;
    private void SearchInRooms()
    {
        int randomRoom = Random.Range(1, Level1Manager.instance.RoomsList.Count);
        roomWaypoints.AddRange(Level1Manager.instance.RoomsList[randomRoom].GetComponentsInChildren<Transform>());
        roomWaypoints.Remove(roomWaypoints[0]);
        _navMeshAgent.speed = 1f;
        searchingInRoom = true;
    }

    //Método para recoger los waypoints de la room que seguirá el NPC cuando esté buscando al player;
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

        Debug.LogWarning("<color=orange>Stop Searching In Room</color>");
    }
    
    #endregion

    #endregion
}
