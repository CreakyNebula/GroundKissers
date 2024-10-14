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
    public float dashTime;


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
        if (callbackContext.performed && isGrounded && !isDashing && !felt)
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
        if(isGrounded && !felt && callbackContext.performed &&!isDashing)
        {
            Debug.Log("true");

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
            if(isDashing)
            {
                animator.Play("tackle");
            }
            else if (isGrounded)
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
        if(collision.gameObject.tag == "Zancadilla" && isDashing)
        {
            felt = true;
            StopCoroutine("TackleCorroutine");
            StartCoroutine("StandUp");
            isDashing = false;
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

        float dashDuration = dashTime; // Duración total del dash
        float elapsedTime = 0f; // Tiempo transcurrido
        float startSpeed = 2f; // Velocidad inicial del dash
        float maxSpeed = tackleForce; // Velocidad máxima que quieres alcanzar

        Vector2 dashDirection = new Vector2(transform.localScale.x, 0f);  // Dirección del dash (derecha o izquierda)

        while (elapsedTime < dashDuration)
        {
            // Calcula la velocidad del dash usando una interpolación suave
            float speed = Mathf.Lerp(startSpeed, maxSpeed, elapsedTime / dashDuration);

            // Aplica la velocidad gradualmente al jugador
            rb.velocity = new Vector2(speed * dashDirection.x, rb.velocity.y);

            elapsedTime += Time.deltaTime;  // Incrementa el tiempo transcurrido
            yield return null;  // Espera al siguiente frame
        }

        isDashing = false;
    }


}