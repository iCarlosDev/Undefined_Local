using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Variables
    [SerializeField] private CharacterController _characterController;
    
    [Header("--- MOVEMENT ---")]
    [Space(10)]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] private float speed;
    [SerializeField] private float turnSmoothTime;
    
    private float turnSmoothVelocity;

    [Header("--- JUMP ---")] 
    [Space(10)] 
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float sphereRadius;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;
    
    [Header("--- ANIMATOR ---")] 
    [Space(10)] 
    [SerializeField] private Animator _animmator;
    
    //GETTERS & SETTERS//
    

    /////////////////////////////////////////

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animmator = GetComponent<Animator>();
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform;
        groundCheck = transform.GetChild(2);
    }

    private void Start()
    {
        speed = 2f;
    }

    private void Update()
    {
        Movement();

        if (isGrounded && _characterController.velocity.magnitude >= 0.1f)
        {
            Sprint();   
        }
        else
        {
            speed = 2f;
            _animmator.SetFloat("SpeedAnimation", 2.25f);
        }
        
        Jump();
    }

    private void Movement()
    {
        //Guardo en estas variables las teclas WASD;
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        
        //Recogemos los valores WASD en positivo y los guardo como "direction";
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        //Si el jugador está en movimiento...;
        if(direction.magnitude >= 0.1f)
        {
            //Recogemos el ángulo del input entre la dirección "X" y "Z" y la proyección del angulo entre esos 2 valores lo convertimos en grados para saber hacia donde mira nuestro player;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            //Smoothea la posición actual a la posición obtenida en "targetAngle";
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, turnSmoothTime);
            //rota el personaje en el eje "Y";
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //guardamos donde rota en "Y" el player por su eje "Z" (dando así que siempre donde mire el player será al frente);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            //Movemos el player hacia el frente;
            _characterController.Move(moveDir.normalized * speed * Time.deltaTime);
            
            _animmator.SetBool("IsWalking", true);
        }
        else
        {
            _animmator.SetBool("IsWalking", false);
        }
    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 4f;
            _animmator.SetFloat("SpeedAnimation", 3f);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 2f;
            _animmator.SetFloat("SpeedAnimation", 2.25f);
        }
    }

    private void Jump()
    {
        //Seteamos el bool con una esfera invisible triggered;
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRadius, groundMask);
        _animmator.SetBool("IsGrounded", isGrounded);
        
        //si estamos en el suelo y no estamos cayendo el eje "Y" será "-2";
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Si presionamos "Espacio" y estamos en el suelo...;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //igualamos nuestro eje "Y" a los valores de la raíz cuadrada (el resultado debería ser valor positivo);
            velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
            _animmator.SetTrigger("Jump");
        }
      
        //El eje "Y" irá progresivamente a 0;
        velocity.y += gravity * Time.deltaTime;
        
        //Movemos al personaje en el eje "Y" cada vez que saltemos;
        _characterController.Move(velocity * Time.deltaTime);
    }
}
