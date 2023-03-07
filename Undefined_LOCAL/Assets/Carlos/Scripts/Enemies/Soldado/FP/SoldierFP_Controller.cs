using System;
using UnityEngine;

public class SoldierFP_Controller : EnemyController
{
    [Header("--- ANIMATOR ---")] 
    [Space(10)] 
    [SerializeField] private Animator _animator;

    [Header("--- FIRE PARAMETERS ---")] 
    [Space(10)]
    [SerializeField] private bool canShoot;
    
    [Header("--- CAMERA SHAKE ---")] 
    [Space(10)]
    [SerializeField] private float magnitude;
    [SerializeField] private float roughnes;
    [SerializeField] private float fadeIn;
    [SerializeField] private float fadeOut;

    private void Start()
    {
        canShoot = true;
    }

    public override void Update()
    {
        base.Update();
        
        AnimationControll();
        
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            Fire(); 
        }
    }

    private void AnimationControll()
    {
        if (_characterController.isGrounded)
        {
            _animator.SetFloat("CharacterMovement", _characterController.velocity.magnitude);
        }
        
        _animator.SetFloat("CharacterSpeed", currentSpeed);
    }

    private void Fire()
    {
        _animator.SetTrigger("Fire");
        EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughnes, fadeIn, fadeOut);
        canShoot = false;
    }

    public void ResetShot()
    {
        canShoot = true;
    }
}
