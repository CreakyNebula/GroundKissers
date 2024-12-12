using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Network_Player_Script : NetworkBehaviour
{
    // Inputs
    private PlayerInput playerInput;
    // Variables de movimiento
    public float speed = 5f;
    public float jumpForce = 7f;
    private Vector2 moveInput;

    public float gravityScale;
    public float fallGravityScale;

    private Rigidbody2D rb;

    // GroundCheck
    public Transform groundCheck;  // Punto desde donde se dispara el Raycast
    public float groundCheckDistance = 0.2f;  // Distancia del Raycast para verificar el suelo
    public LayerMask groundLayer;  // Capa que representa el suelo
    [SerializeField]private bool isGrounded;

    // Animator
    private Animator animator;

    // Tropiezo
    private bool felt;

    // Dash
    public float tackleForce;
    public bool isDashing;
    public float dashTime;

    //Jump


    // Coyote Time y Jump Buffering
    public float coyoteTime = 0.2f;   // Tiempo permitido para Coyote Time
    private float coyoteTimeCounter;  // Contador de Coyote Time

    public float jumpBufferTime = 0.2f;   // Tiempo permitido para Jump Buffering
    private float jumpBufferCounter;  // Contador de Jump Buffer

    //Died

    //States

    public enum States { dashing,idleing,walking,falling, jumping, triping}
    public States mystate;

    //Inputs
    private bool jumpPressed;
    private bool tacklePressed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        SetState(States.idleing);
    }

    private void Update()
    {
        if(!IsOwner) return;
        CheckGround();
        GravityScale();
        /*UpdateMovement();

        AnimationManager();
        ;*/
        ManageCoyoteTime();
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        switch (mystate)
        {
            case States.dashing:
                Dash();
                break;
            case States.idleing:
                Idle();
                break;
            case States.walking:
                Walk();
                break;
            case States.falling:
                Fall();
                break;
            case States.jumping:
                Jump();
                break;
            case States.triping:
                Trip();
                break;
        }
    }

     #region Estados
    public void SetState(States s)
    {

        mystate = s;
    }
    public void Idle()
    {
        animator.Play("idle");
        if (moveInput.x != 0)
        { SetState(States.walking); }
        if (tacklePressed) SetState(States.dashing);
    }
    public void Walk()
    {
        animator.Play("walk");
        UpdateMovement();

        if (moveInput.x == 0) SetState(States.idleing);
        if (tacklePressed) SetState(States.dashing);
    }
    public void Dash()
    {
        if (!isDashing) { StartCoroutine("TackleCorroutine"); }
        animator.Play("tackle");

    }
    public void Jump()
    {
        animator.Play("jump");
        UpdateMovement();
      //  SetState(States.falling);
    }
    public void Fall()
    {
        UpdateMovement();
        animator.Play("fall");
        if (isGrounded) SetState(States.idleing);
    }
    
    public void Trip()
    {
        animator.Play("trip over");
    }
    #endregion 
    //Metodos fuera de estados
    void UpdateMovement()
    {
            // Movimiento horizontal
            rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
            if (moveInput.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (moveInput.x < 0)
            {
              transform.eulerAngles = new Vector3(0, 180, 0);
             }
    }
    void CheckGround()
    {
        // Verifica si está en el suelo usando un BoxCast
        isGrounded = Physics2D.BoxCast(groundCheck.position, new Vector2(0.5f, 0.1f), 0f, Vector2.down, groundCheckDistance, groundLayer);
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);
    }
    void GravityScale()
    {
        if (!jumpPressed && !isGrounded || (!isGrounded && rb.velocity.y<0 &&coyoteTimeCounter!>0 && coyoteTimeCounter!=coyoteTime) || jumpPressed&& !isGrounded && rb.velocity.y < 0)
        {
            rb.gravityScale = fallGravityScale;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }
    private void ManageCoyoteTime()
    {
        // Reinicia el contador si está en el suelo
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            // Disminuye el contador si no está en el suelo
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void ManageJumpBuffering()
    {
        // Disminuye el contador si se presionó el botón de salto
        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            // Reduce el tiempo de buffer si no está presionado
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    //Corroutines
    IEnumerator TackleCorroutine()
    {
        isDashing = true;

        float dashDuration = dashTime;  // Duración total del dash
        float elapsedTime = 0f;  // Tiempo transcurrido
        float startSpeed = 2f;  // Velocidad inicial del dash
        float maxSpeed = tackleForce;  // Velocidad máxima que quieres alcanzar

        Vector2 dashDirection = new Vector2(transform.right.x, 0f);  // Dirección del dash (derecha o izquierda)

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
        UpdateMovement();
        SetState(States.idleing);
    }

    IEnumerator StandUp()
    {
        yield return new WaitForSeconds(2);
        SetState(States.idleing);
        rb.drag = 0;
    }

    //Collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Zancadilla" && isDashing)
        {
            felt = true;
            SetState(States.triping);
            StopCoroutine("TackleCorroutine");
            StartCoroutine("StandUp");
            isDashing = false;
            rb.drag = 5;
        }
    }
    //Inputs

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            jumpPressed = true;
            Debug.Log("hola");
           
        }
        if (callbackContext.canceled)
        {
            jumpPressed = false;
        }
        if ((callbackContext.started && mystate==States.idleing || callbackContext.started && mystate == States.walking) && coyoteTimeCounter>0)
        {
            SetState(States.jumping);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

    }
    public void Tackle(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            tacklePressed = true;
        }
        if (callbackContext.canceled)
        {
            tacklePressed = false;
        }
    }

    // Método para gestionar Coyote Time y Jump Buffering
    /*


      

    
      

      void ManageCoyoteTime()
      {
          // Gestiona el Coyote Time
          if (isGrounded && !isDashing)
          {
              coyoteTimeCounter = coyoteTime;  // Si está en el suelo, resetea el contador
          }
          else
          {
              coyoteTimeCounter -= Time.deltaTime;  // Decrementa el tiempo cuando no está en el suelo
          }

          // Gestiona el Jump Buffer
          if (jumpBufferCounter > 0)
          {
              jumpBufferCounter -= Time.deltaTime;  // Decrementa el tiempo del buffer
          }

          // Si el contador de buffer y el de coyote son válidos, realiza el salto
          if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !isDashing && !felt)
          {
              Jump();
              jumpBufferCounter = 0;  // Resetea el buffer después del salto
          }


      }*/
}
