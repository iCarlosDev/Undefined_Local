using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierFP_Controller : EnemyController
{
    [Header("--- ANIMATOR ---")] 
    [Space(10)] 
    [SerializeField] private Animator _animator;
    
    public override void Update()
    {
        base.Update();
        
        AnimationControll();
    }

    private void AnimationControll()
    {
        _animator.SetFloat("CharacterMovement", _characterController.velocity.magnitude);
        _animator.SetFloat("CharacterSpeed", currentSpeed);
    }
}
