using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_IA : MonoBehaviour
{
    [SerializeField] protected EnemyScriptStorage _enemyScriptStorage;
    
    [Header("--- NAVMESH ---")]
    [Space(10)]
    [SerializeField] protected NavMeshAgent _navMeshAgent;
    
    [Header("--- WAYPOINTS ---")]
    [Space(10)]
    [SerializeField] private Transform waypointStorage;
    [SerializeField] protected List<Transform> waypointsList;
    [SerializeField] protected int waypointsListIndex;
    
    [Header("--- DETECTION ---")]
    [Space(10)]
    [SerializeField] protected bool isPlayerDetected;

    private void Awake()
    {
        _enemyScriptStorage = GetComponent<EnemyScriptStorage>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        waypointStorage = transform.parent.GetChild(1);
        
        waypointsList.AddRange(waypointStorage.GetComponentsInChildren<Transform>());
        waypointsList.Remove(waypointsList[0]);
        waypointsListIndex = 0;
    }

    private void Start()
    {
        //Si hay waypoints en la lista se setea el destino en el primero de esta;
        if (waypointsList.Count != 0)
        {
            _navMeshAgent.SetDestination(waypointsList[waypointsListIndex].position);
        }
    }

    public virtual void Update()
    {
        Debug.Log(_navMeshAgent.destination);
        Debug.DrawLine(transform.position, _navMeshAgent.destination, Color.red, 0.1f);
        CheckPlayerDetectedStatus();
    }

    private void CheckPlayerDetectedStatus()
    {
        //Solo si el player no es detectado hara la lógica;
        if (!isPlayerDetected)
        {
            //Si el NPC tiene un path asignado y no ve al player hace su path;
            if (_navMeshAgent.hasPath && !_enemyScriptStorage.FieldOfView.canSeePlayer)
            {
                Debug.Log("Player Not Detected");
                UpdatePath();   
            }

            //Si el NPC ve al Player se activa el bool "isPlayerDetected";
            if (_enemyScriptStorage.FieldOfView.canSeePlayer)
            {
                isPlayerDetected = true;
            } 
        }
    }

    #region - PATH WITH WAYPOINTS -
    
    //Método para comprobar si el NPC tiene que actualizar el waypoint;
    private void UpdatePath()
    {
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < 0.3f)
        {
            UpdateWaypoint();
        }
    }

    //Método para actualizar el waypoint al que tiene que ir el NPC;
    private void UpdateWaypoint()
    {
        waypointsListIndex = (waypointsListIndex + 1) % waypointsList.Count;
        _navMeshAgent.SetDestination(waypointsList[waypointsListIndex].position);
    }
    
    #endregion
}
