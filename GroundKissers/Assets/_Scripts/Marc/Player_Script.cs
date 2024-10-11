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

    //dash
    public float tackleForce;
    public bool isDashing;


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
        if (callbackContext.performed && isGrounded && !isDashing)
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

    public void Tackle(InputAction.CallbackContext callbackContext)
    {
        if(groundCheck&&!felt&& callbackContext.performed &&!isDashing)
        {
            StartCoroutine("TackleCorroutine");
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
        if(!isJumping && !isGrounded || !isGrounded && rb.velocity.y<0 || felt)
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
        if(!felt && !isDashing)
        {
            moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
            // Movimiento horizontal
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
            if (moveInput.x > 0)
            {

                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveInput.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
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

    IEnumerator TackleCorroutine()
    {
        isDashing = true;
        Vector2 distanciaTackle = new Vector2(transform.localScale.x * tackleForce, 0f);  // Puedes ajustar el valor 20f según la fuerza deseada
        rb.AddForce(distanciaTackle, ForceMode2D.Impulse);
        Debug.Log("Tackling with force: " + tackleForce);
        yield return new WaitForSeconds(0.8f);
        isDashing=false;
    }

}