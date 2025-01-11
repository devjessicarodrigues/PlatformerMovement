using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    private SpriteRenderer sprite;
    public Animator animator;
    public Rigidbody2D rb;
    private bool canJump = true;
    public ParticleSystem dust;

    [Space]
    [Header("Audio")]
    public AudioSource audioSource;  
    public AudioClip walkSound;     
    public AudioClip jumpSound;      
    public AudioClip dashSound;      
    public AudioClip climbSound;
    public AudioClip slideSound;
    public AudioClip landSound;      

    [Space]
    [Header("Dash")]
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [Space]
    [Header("Double Jump")]
    public int extraJumpsValue = 1;
    private int extraJumps;
    public float jumpForce = 5f;

    [Space]
    [Header("Wall")]
    private bool wallGrab;
    private bool wallSlide;
    private bool wallJumped;
    public Collision collisionScript;
    public ParticleSystem dustWall;
    public ParticleSystem dustSlide;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        extraJumps = extraJumpsValue;
        audioSource = GetComponent<AudioSource>();

        animator.SetBool("isWalking", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isDoubleJump", false);
        animator.SetBool("isDashing", false);
        animator.SetBool("isGrabbing", false);
        animator.SetBool("isClimbing", false);
        animator.SetBool("isSliding", false);
    }

    private void Update()
    {
        if (isDashing) return;
        HandleMovement();

        if (collisionScript.onWall && Input.GetButtonDown("Jump") && wallGrab)
        {
            WallJump();
            return;
        }

        HandleWallInteraction();
        HandleJumpAction();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        bool isWalking = moveInput != 0 && collisionScript.onGround;
        animator.SetBool("isWalking", isWalking);

        if (moveInput != 0)
        {
            sprite.flipX = moveInput < 0;
            CreateDust();
            PlayWalkSound();
        }
        else
        {
            dust.Stop();
        }

        canJump = Mathf.Abs(rb.velocity.y) <= 0.001f && collisionScript.onGround;
        if (canJump) extraJumps = extraJumpsValue; 
        animator.SetBool("isJumping", !canJump);
    }


    private void HandleWallInteraction()
    {
        if (collisionScript.onWall && Input.GetButton("Grab"))
        {
            wallGrab = true;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, Input.GetAxisRaw("Vertical") * speed);

            bool isClimbing = Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0;
            animator.SetBool("isClimbing", isClimbing);

            if (isClimbing)  
            {
                PlayClimbSound();
            }

            animator.SetBool("isGrabbingWall", true);

            if (!dustWall.isPlaying) dustWall.Play();
        }
        else
        {
            ResetWallGrabState();
        }

        if (collisionScript.onWall && !collisionScript.onGround && !wallGrab)
        {
            wallSlide = true;
            WallSlide();
        }
        else
        {
            wallSlide = false;
            animator.SetBool("isSliding", false);
            if (dustSlide.isPlaying) dustSlide.Stop();
        }
    }
    private void HandleJumpAction()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (canJump) 
            {
                Jump();
                animator.SetBool("isDoubleJump", false);
            }
            else if (extraJumps > 0) 
            {
                Jump();
                extraJumps--;
                animator.SetBool("isDoubleJump", true);
            }
            else if (wallGrab) 
            {
                WallJump();
            }
        }
    }

    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce; 
        CreateDust();
        animator.SetBool("isJumping", true);
        PlayJumpSound();
    }

    private void WallSlide()
    {
        float slideSpeed = 5f;
        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
        animator.SetBool("isSliding", true);
        PlaySlideSound();
    }

    private void WallJump()
    {
        Vector2 jumpDirection = collisionScript.onWall ? Vector2.left : Vector2.right;

        rb.velocity = jumpDirection * jumpForce + Vector2.up * jumpForce;

        sprite.flipX = jumpDirection.x > 0;

        animator.SetBool("isJumping", true);

        wallJumped = true;
        Invoke(nameof(ResetWallJumped), 0.2f);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        animator.SetBool("isDashing", true);
        sprite.color = Color.cyan; 
        PlayDashSound();

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = sprite.flipX ? -1f : 1f;
        rb.velocity = new Vector2(dashDirection * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        rb.velocity = Vector2.zero;
        sprite.color = Color.white;
        animator.SetBool("isDashing", false);

        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }


    private void CreateDust()
    {
        if (!dust.isPlaying) dust.Play();
    }

    private void ResetWallGrabState()
    {
        wallGrab = false;
        animator.SetBool("isGrabbingWall", false);
        animator.SetBool("isClimbing", false);
        rb.gravityScale = 3;
        if (dustWall.isPlaying) dustWall.Stop();
    }
    private void ResetWallJumped()
    {
        wallJumped = false;
    }

    private void PlayWalkSound()
    {
        if (!audioSource.isPlaying && collisionScript.onGround)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
        }
    }

    private void PlayJumpSound()
    {
        audioSource.clip = jumpSound;
        audioSource.Play();
    }

    private void PlayDashSound()
    {
        audioSource.clip = dashSound;
        audioSource.Play();
    }

    private void PlayClimbSound()
    {
        if (!audioSource.isPlaying && wallGrab)
        {
            audioSource.clip = climbSound;
            audioSource.Play();
        }
    }
    private void PlaySlideSound()
    {
        audioSource.clip = slideSound;
        audioSource.Play();
    }
}
