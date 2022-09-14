using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public InventoryObject inventory;

    public float moveSpeed;
    public float jumpForce = 16f;
    public Transform ceilingcheck;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundobject;
    public float checkRadius;
    public int maxjumpCount;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float wallHopForce;
    public float wallJumpForce;
    public int amountOfJumps = 1;
    public float variableJumpHeightMultiplier = 0.5f;
    public float baseSpeed;
    public float dashPower;
    public float dashTime;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public int maxStamina = 100;
    public int currentStamina;
    public Stamina staminaBar;
    

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    private Rigidbody2D rb;
    private bool facingRight = true;
    private float moveDirection;
    private bool isGround;
    private int jumpCount;
    private bool isWarp;
    private bool canFlip;
    private bool canJump;
    private bool isTouchingWall;
    private bool isWallSliding;
    private int facingDirection = 1;
    private int amountOfJumpsLeft;
    private bool isCrouch;
    private GameObject currentTeleporter;

    bool isDashing = false;

    //Animator
    private Animator anim;
    private bool Walking;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        //==============================
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpCount = maxjumpCount;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        CheckMovement();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(!isDashing)
            {
                //StartCoroutine(Dash());
                if(currentStamina > 0)
                {
                    StartCoroutine(Dash());
                    UseStamina(20);
                }
                
            }

        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(15);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            Heal(10);
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(currentTeleporter != null)
            {
                transform.position = currentTeleporter.GetComponent<Teleport>().GetDestination().position;
            }
        }

    }

    

    //test damage & Heal======================
    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
    
    void Heal(int damage)
    {
        currentHealth += damage;

        healthBar.SetHealth(currentHealth);
    }

    void UseStamina(int st)
    {
        currentStamina -= st;

        staminaBar.SetStamina(currentStamina);
    }
    //test damage & Heal======================

    private void FixedUpdate()
    {
        CheckSurrounding();
        Move();
        
    }

    private void CheckSurrounding()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundobject);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundobject);
        
    }

    private void UpdateAnimations()
    {
        anim.SetBool("Walking", Walking);
        anim.SetBool("isGround", isGround);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("Dash",isDashing);
        anim.SetBool("isCrouch", isCrouch);
    }

    private void CheckMovement()
    {
        if(moveDirection > 0 && !facingRight)
        {
            Flip();
        }
        else if(moveDirection < 0 && facingRight)
        {
            Flip();
        }

        if(Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            Walking = true;
        }
        else
        {
            Walking = false;
        }
    }

    private void CheckIfWallSliding()
    {
            if(isTouchingWall && !isGround && rb.velocity.y < 0)
            {
                isWallSliding = true;
            }
            else
            {
                isWallSliding = false;
            }
    }

    private void CheckIfCanJump()
    {
        if((isGround && rb.velocity.y <= 0) || isWallSliding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if(amountOfJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
      
    }


    private void Inputs()
    {
        moveDirection = Input.GetAxis("Horizontal");
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        /*if(Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }*/

        if(Input.GetButtonDown("Crouch"))
        {
            isCrouch = true;
        }
        else if(Input.GetButtonUp("Crouch"))
        {
            isCrouch = false; 
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
       
    }

    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
        else if (isWallSliding && moveDirection == 0 && canJump) //Wall hop
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if((isWallSliding || isTouchingWall) && moveDirection != 0 && canJump)
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * moveDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        moveSpeed *= dashPower;

        yield return new WaitForSeconds(dashTime);

        moveSpeed = baseSpeed;
        isDashing = false;
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    private void Flip()
    {
        if(!isWallSliding)
        {
            facingDirection *= -1;
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Teleporter"))
        {
            currentTeleporter = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Teleporter"))
        {
            if(collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
