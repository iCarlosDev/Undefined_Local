using System;
using UnityEngine;

public class SoldierFP_Controller : EnemyController
{
    [SerializeField] private Transform arms;
    
    [Header("--- ANIMATOR ---")] 
    [Space(10)] 
    [SerializeField] private Animator _animator;

    [Header("--- FIRE PARAMETERS ---")] 
    [Space(10)]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private bool canShoot;
    
    [Header("--- AIM PARAMETERS ---")] 
    [Space(10)]
    [SerializeField] private Transform aimPivot;
    [SerializeField] private float smoothTime;
    
    [Header("--- RELOAD PARAMETERS ---")] 
    [Space(10)]
    [SerializeField] private bool canReload;
    
    [Header("--- CAMERA SHAKE ---")] 
    [Space(10)]
    [SerializeField] private float magnitude;
    [SerializeField] private float roughnes;
    [SerializeField] private float fadeIn;
    [SerializeField] private float fadeOut;

    private void Start()
    {
        canShoot = true;
        canReload = true;
    }

    public override void Update()
    {
        base.Update();
        
        MovementAnimationControll();
        
        //Si apretamos el click Izquierdo y podemos disparar llamaremos al método "Fire";
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            Fire(); 
        }

        //Si mantenemos el click Derecho llamaremos al método "AimIn" si no al método "AimOut";
        if (Input.GetButton("Fire2"))
        {
            AimIn();
        }
        else
        {
            AimOut();
        }

        if (Input.GetKeyDown(KeyCode.R) && canReload)
        {
            Reload();
        }
    }

    //Método para controlar las animaciones básicas;
    private void MovementAnimationControll()
    {
        //Si el character está en el suelo asignaremos su velocidad en el animator;
        if (_characterController.isGrounded)
        {
            _animator.SetFloat("CharacterMovement", _characterController.velocity.magnitude);
        }
        
        //Asignamos la currentSpeed en el animator;
        _animator.SetFloat("CharacterSpeed", currentSpeed);
    }

    #region - FIRE -
    
    //Método para disparar;
    private void Fire()
    {
        //Activamos el trigger "Fire";
        _animator.SetTrigger("Fire");
        
        Debug.DrawRay(cameraPivot.position, cameraPivot.forward * 10, Color.red, 3f);
        
        //Aplicamos un shake a la cámara para dar efecto de disparo;
        EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughnes, fadeIn, fadeOut);
        
        canShoot = false;
    }

    //Método que se llama al hacerse la animación de disparo;
    public void ResetShot()
    {
        canShoot = true;
    }
    #endregion

    #region - AIM -

    //Método para apuntar;
    private void AimIn()
    {
        //Movemos la posición de los brazos con un smooth hacia el pivote de apuntado;
        arms.position = Vector3.Lerp(arms.position, aimPivot.position, smoothTime * Time.deltaTime);
        //Bajamos la magnitud del shake para que la escopeta no atraviese la cámara;
        magnitude = 0.25f;

    }

    //Método para desapuntar;
    private void AimOut()
    {
        //Movemos la posición de los brazos con un smooth hacia su posición inicial;
        arms.localPosition = Vector3.Lerp(arms.localPosition, Vector3.zero, smoothTime * Time.deltaTime);
        //Dejamos el valor de la magnitud del shake al inicial;
        magnitude = 1f;
    }

    #endregion

    #region - RELOAD -

    private void Reload()
    {
        _animator.SetTrigger("Reload");
        canReload = false;
    }

    public void ResetReload()
    {
        canReload = true;
    }

    #endregion
}
