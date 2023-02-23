using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScriptStorage : MonoBehaviour
{
    [SerializeField] private FieldOfView _fieldOfView;

    //GETTERS && SETTERS//
    public FieldOfView FieldOfView => _fieldOfView;
    
    ///////////////////////////////////////////////

    private void Awake()
    {
        _fieldOfView = GetComponent<FieldOfView>();
    }
}
