using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScriptStorage : MonoBehaviour
{
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private Enemy_IA _enemyIa;

    //GETTERS && SETTERS//
    public FieldOfView FieldOfView => _fieldOfView;
    public Enemy_IA EnemyIa => _enemyIa;

    ///////////////////////////////////////////////

    private void Awake()
    {
        _fieldOfView = GetComponentInChildren<FieldOfView>();
        _enemyIa = GetComponent<Enemy_IA>();
    }
}
