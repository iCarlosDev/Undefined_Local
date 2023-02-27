using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist_IA : Enemy_IA
{
    
    [SerializeField] private int randomSafeRoomWaypoint;
    
    public override void Start()
    {
        base.Start();
        
        //Seteamos un número Random que decidirá a que lugar de la sala segura irá el NPC;
        randomSafeRoomWaypoint = Random.Range(0, Level1Manager.instance.SafeRoomWaypointsList.Count);
    }

    public override void Update()
    {
        base.Update();
        
        //Si el player no ha sido detectado nunca hará la lógica restante;
        if (!isPlayerDetected) return;
        
        //Se comprueba si tiene que huir del Player;
        if (_enemyScriptStorage.FieldOfView.canSeePlayer && _navMeshAgent.hasPath)
        {
            RunOfPlayer();
        }
    }

    //Método para huir del player;
    private void RunOfPlayer()
    {
        _navMeshAgent.speed = 3f;
        _navMeshAgent.stoppingDistance = 0.1f;
        
        //Si la alarma está activada irá a la sala segura;
        if (Level1Manager.instance.AlarmActivated)
        {
            GoSafeRoom();
        }
        //Si la alarma no está activada irá a activarla;
        else
        {
            GoActivateAlarm();
        }

        //Si el NPC llega al waypoint se quedará quieto;
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < 0.1f)
        {
            _navMeshAgent.ResetPath();
        }
    }

    //Método para ir a la sala segura;
    private void GoSafeRoom()
    {
        _navMeshAgent.SetDestination(Level1Manager.instance.SafeRoomWaypointsList[randomSafeRoomWaypoint].position);
    }

    //Método para ir a activar la alarma;
    private void GoActivateAlarm()
    {
        
    }
    
    
}
