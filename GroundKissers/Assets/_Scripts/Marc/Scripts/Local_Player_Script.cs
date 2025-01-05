using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class Local_Player_Script : MonoBehaviour
{
    // Inputs
    private PlayerInput playerInput;

    [Header("movement")]
    // Variables de movimiento
    public float speed = 5f;
    public float jumpForce = 7f;
    private Vector2 moveInput;

    public float gravityScale;
    public float fallGravityScale;

    private Rigidbody2D rb;

    // GroundCheck
    [Header("GroundCheck")]
    public Transform groundCheck;  // Punto desde donde se dispara el Raycast
    public float groundCheckDistance = 0.2f;  // Distancia del Raycast para verificar el suelo
    public LayerMask groundLayer;  // Capa que representa el suelo
    [SerializeField]private bool isGrounded;

    // Animator
    private Animator animator;

    // Tropiezo
    private bool felt;

    // Dash
    [Header("Dash")]

    public float tackleForce;
    public bool isDashing;
    public float dashTime;
    [SerializeField] private GameObject dashCollider;
    private bool facingRight;

    [Header("Trip")]
    [SerializeField] private GameObject zancadillaCollider;

    [Header("Better Jump")]
    // Coyote Time y Jump Buffering
    public float coyoteTime = 0.2f;   // Tiempo permitido para Coyote Time
    private float coyoteTimeCounter;  // Contador de Coyote Time

    public float jumpBufferTime = 0.2f;   // Tiempo permitido para Jump Buffering
    private float jumpBufferCounter;  // Contador de Jump Buffer

    //Knockback
    [Header("Knockback")]

    public float knockbackX;
    public float knockbackY;
    [SerializeField]private float waitTime;

    [Header("Utility")]

    [SerializeField] private float walkCounter;
    [SerializeField] private float maxWalkCounter;
    public int utilityCount;


    //States

    public enum States { dashing,idleing,walking,falling, jumping, triping,damage,zancadilla}
    public States mystate;

    //Inputs
    private bool jumpPressed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        SetState(States.idleing);
    }
    private void FixedUpdate()
    {
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
            case States.damage:
                Knockback();
                break;
            case States.zancadilla:
                Zancadilla();
                break;
        }
        if (mystate == States.idleing)
        {
            rb.drag = 10;
        }
        else
        {
            rb.drag = 1;
        }
    }
    private void Update()
    {
        
    }

     #region Estados
    public void SetState(States s)
    {
        mystate = s;
        ActivarColliderZancadilla(false);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    public void Idle()
    {
        animator.Play("idle");
        if (moveInput.x != 0)
        { SetState(States.walking); }
        rb.drag = 100;
        if (rb.velocity.y < 0) SetState(States.falling);

    }
    public void Walk()
    {
        animator.Play("walk");
        UpdateMovement();

        if (rb.velocity.y < 0) SetState(States.falling);
        if (moveInput.x == 0) SetState(States.idleing);
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
        if (rb.velocity.y < 0) SetState(States.falling);
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

    public void Zancadilla()
    {
        animator.Play("zancadilla");
        ActivarColliderZancadilla(true);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }
    public void Knockback()
    {
        animator.Play("damage");
        waitTime-=Time.deltaTime;
        if(groundCheck==true && waitTime < 0)
        {
            waitTime = 0.4f;
            Debug.Log("ya");
            SetState(States.idleing);
        }
    }
    #endregion 
    //Metodos fuera de estados
    void UpdateMovement()
    {
        if(moveInput.x >0.3f || moveInput.x <-0.3f)
        {
            // Movimiento horizontal
            rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
            if (moveInput.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                facingRight = true;
            }
            else if (moveInput.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                facingRight = false;
            }
        }
        UtilityCharge();
    }
    void CheckGround()
    {
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
        // Reinicia el contador si est� en el suelo
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            // Disminuye el contador si no est� en el suelo
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void ManageJumpBuffering()
    {
        // Disminuye el contador si se presion� el bot�n de salto
        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            // Reduce el tiempo de buffer si no est� presionado
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    private void UtilityCharge()
    {
        if (walkCounter < maxWalkCounter)
        {
            if(utilityCount < 3)
            {
                walkCounter += Time.deltaTime;
            }
        }
        else
        {
            utilityCount++;
            walkCounter = 0;
        }
    }
    //Corroutines
    IEnumerator TackleCorroutine()
    {
        isDashing = true;
        ActivarColliderDash(true);
        float dashDuration = dashTime;  // Duraci�n total del dash
        float elapsedTime = 0f;  // Tiempo transcurrido
        float startSpeed = 2f;  // Velocidad inicial del dash
        float maxSpeed = tackleForce;  // Velocidad m�xima que quieres alcanzar
        Vector2 dashDirection = new Vector2(transform.right.x, 0f);  // Direcci�n del dash (derecha o izquierda)

        while (elapsedTime < dashDuration)
        {
            // Calcula la velocidad del dash usando una interpolaci�n suave
            float speed = Mathf.Lerp(startSpeed, maxSpeed, elapsedTime / dashDuration);

            // Aplica la velocidad gradualmente al jugador
            rb.velocity = new Vector2(speed * dashDirection.x, rb.velocity.y);

            elapsedTime += Time.deltaTime;  // Incrementa el tiempo transcurrido
            yield return null;  // Espera al siguiente frame
        }
        ActivarColliderDash(false); // Notificar al servidor que termina el dash
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

        if(collision.gameObject.tag == "Dashing Player")
        {
           /* Network_Player_Script OtherPlayerScript = collision.gameObject.GetComponent<Network_Player_Script>();

            OtherPlayerScript.isDashing = false;
            OtherPlayerScript.StopCoroutine("TackleCorroutine"); */

            isDashing = false;
            SetState(States.damage);
            StopCoroutine("TackleCorroutine");
            ActivarColliderDash(false);
            Local_Player_Script otherPlayerScript = collision.transform.parent.GetComponent<Local_Player_Script>();
            
            if(otherPlayerScript.facingRight)
            {
                rb.velocity=(new Vector2 (knockbackX,knockbackY));
            }
            else
            {
                rb.velocity = (new Vector2(-knockbackX, knockbackY));

            }
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
        if (callbackContext.performed && isGrounded && utilityCount>0)
        {
            SetState(States.dashing);
            utilityCount--;
        }
    }
    public void Trip(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && isGrounded && utilityCount > 0)
        {
            SetState(States.zancadilla);
            utilityCount--;
        }
        if(callbackContext.canceled && mystate==States.zancadilla)
        {
            SetState(States.idleing);
        }
    }


    // M�todo para gestionar Coyote Time y Jump Buffering
    /*
    
      

      void ManageCoyoteTime()
      {
          // Gestiona el Coyote Time
          if (isGrounded && !isDashing)
          {
              coyoteTimeCounter = coyoteTime;  // Si est� en el suelo, resetea el contador
          }
          else
          {
              coyoteTimeCounter -= Time.deltaTime;  // Decrementa el tiempo cuando no est� en el suelo
          }

          // Gestiona el Jump Buffer
          if (jumpBufferCounter > 0)
          {
              jumpBufferCounter -= Time.deltaTime;  // Decrementa el tiempo del buffer
          }

          // Si el contador de buffer y el de coyote son v�lidos, realiza el salto
          if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !isDashing && !felt)
          {
              Jump();
              jumpBufferCounter = 0;  // Resetea el buffer despu�s del salto
          }


      }*/

    private void ActivarColliderDash(bool isActive)
    {
        dashCollider.SetActive(isActive);
    }
    private void ActivarColliderZancadilla(bool isActive)
    {
        zancadillaCollider.SetActive(isActive);
    }
}
