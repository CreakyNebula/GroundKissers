using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;


public class Player_Script : MonoBehaviour
{
    //Inputs
    private PlayerInput playerInput;
    // Variables de movimiento
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Vector2 moveInput;

    public float gravityScale;
    public float fallGravityScale;

    private Rigidbody2D rb;


    // GroundCheck
    public Transform groundCheck;  // Punto desde donde se dispara el Raycast
    public float groundCheckDistance = 0.2f;  // Distancia del Raycast para verificar el suelo
    public LayerMask groundLayer;  // Capa que representa el suelo
    private bool isGrounded;
    private bool isJumping;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
      
        CheckGround();
        UpdateMovement();
        GravityScale();
    }

   

  
    public void Move(InputAction.CallbackContext callbackContext)
    {
        moveInput = callbackContext.ReadValue<Vector2>();
    }

    
    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && isGrounded)
        {
            // Salta solo si está en el suelo
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
        }
        if (callbackContext.canceled )
        {
            isJumping = false;
        }
    }

   void CheckGround()
    {
        // Verifica si está en el suelo usando un Raycast
        isGrounded = Physics2D.BoxCast(groundCheck.position, new Vector2(0.5f, 0.1f), 0f, Vector2.down, groundCheckDistance, groundLayer);

        // Dibuja el BoxCast en la escena para depuración
       
    }

    void GravityScale()
    {
        if(!isJumping && !isGrounded || !isGrounded && rb.velocity.y<0 )
        {
            rb.gravityScale = fallGravityScale;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

       
    }
    void UpdateMovement()
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        // Movimiento horizontal
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);
    }


}