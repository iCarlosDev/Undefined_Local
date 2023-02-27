using System;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    public static Level1Manager instance;

    [Header("--- SAFE ROOM ---")]
    [Space(10)]
    [SerializeField] private List<Transform> safeRoomWaypointsList;
    
    [Header("--- ALARM PARAMETERS ---")]
    [Space(10)]
    [SerializeField] private Transform alarmWaypoint;
    [SerializeField] private bool alarmActivated;
    
    //GETTERS && SETTERS//
    public bool AlarmActivated => alarmActivated;
    public List<Transform> SafeRoomWaypointsList => safeRoomWaypointsList;

    ///////////////////////
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        safeRoomWaypointsList.AddRange(GameObject.FindWithTag("SafeRoomCollider").GetComponentsInChildren<Transform>());
        safeRoomWaypointsList.Remove(safeRoomWaypointsList[0]);
    }
}
