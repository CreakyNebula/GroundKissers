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

    //animator
    private Animator animator;

    //tropiezo
    private bool felt;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
      
        CheckGround();
        UpdateMovement();
        GravityScale();
        AnimationManager();
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
        if(!felt)
        {
            moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
            // Movimiento horizontal
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
            if (moveInput.x > 0)
            {

                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (moveInput.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
         }
        
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);
    }

    public void AnimationManager()
    {
        if(!felt)
        {
            if (isGrounded)
            {
                if (moveInput.x != 0)
                {
                    animator.Play("walk");
                }
                else
                {
                    animator.Play("idle");
                }
            }
            else
            {
                if (!isJumping || rb.velocity.y < 0)
                {
                    animator.Play("fall");
                }
                else
                {
                    animator.Play("jump");
                }

            }
        }
        else
        {
            animator.Play("trip over");

        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Zancadilla")
        {
            felt = true;
            StartCoroutine("StandUp");
        }
    }

    IEnumerator StandUp()
    {
        yield return new WaitForSeconds(2);
        felt = false;
    }

}