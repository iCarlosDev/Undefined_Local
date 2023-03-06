using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;

    [Header("--- MOVEMENT PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private float speed;

    [Header("--- JUMP PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;

    [Header("--- GROUND CHECK ---")] 
    [Space(10)]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private bool isGrounded;

    private Vector3 velocity;

    private void Update()
    {
        Movement();
        Jump();
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        _characterController.Move(move * speed * Time.deltaTime);
    }

    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;

        _characterController.Move(velocity * Time.deltaTime);
    }
    
}
