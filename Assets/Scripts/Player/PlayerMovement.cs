using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int maxJump = 2;
    [SerializeField] private float jumpCooldown = 0.2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Health Bar")]
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private Transform healthBarVisual;
    private Vector3 initialHealthBarScale;

    [Header("UI Control")]
    public bool moveLeft;
    public bool moveRight;
    private bool jumpPressed;
    private bool attackPressed;

    private int jumpCount = 0;
    private float lastJumpTime;
    private bool isSprinting = false;
    private bool wasGroundedLastFrame = false;
    private bool isAttacking = false;

    private bool isJump = false;
    private float jumpTriggerTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        if (healthBarTransform != null)
            initialHealthBarScale = healthBarTransform.localScale;
    }

    private void FixedUpdate()
    {
        if (isAttacking || isDead) return;

        // ⌨️ Keyboard + 🕹️ UI input
        float move = Input.GetAxisRaw("Horizontal");
        if (moveLeft) move = -1f;
        else if (moveRight) move = 1f;

        float currentSpeed = isSprinting ? baseSpeed * sprintMultiplier : baseSpeed;
        rb.velocity = new Vector2(move * currentSpeed, rb.velocity.y);

        if (move > 0 && transform.localScale.x < 0)
            FlipSprite(1);
        else if (move < 0 && transform.localScale.x > 0)
            FlipSprite(-1);

        animator.SetBool("Walking", IsGrounded() && Mathf.Abs(move) > 0.01f);
    }

    private void Update()
    {
        if (isDead) return;

        bool isGrounded = IsGrounded();

        if (isGrounded && !wasGroundedLastFrame)
        {
            jumpCount = 0;
            animator.SetBool("DoubleJumping", false);
        }

        // ⌨️ Spacebar + 🕹️ UI
        if (!isAttacking && (Input.GetKeyDown(KeyCode.Space) || jumpPressed) && Time.time - lastJumpTime >= jumpCooldown)
        {
            if (jumpCount < maxJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
                lastJumpTime = Time.time;
                jumpPressed = false;

                if (jumpCount == 2)
                    animator.SetBool("DoubleJumping", true);

                isJump = true;
                jumpTriggerTimer = jumpCooldown;
                animator.SetBool("isJump", true);
            }
        }

        if (isJump)
        {
            jumpTriggerTimer -= Time.deltaTime;
            if (jumpTriggerTimer <= 0f)
            {
                isJump = false;
                animator.SetBool("isJump", false);
            }
        }

        // ⌨️ Shift
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // ⌨️ X + 🕹️ UI
        if (!isAttacking && (Input.GetKeyDown(KeyCode.X) || attackPressed))
        {
            TriggerRandomAttack();
            attackPressed = false;
        }

        animator.SetBool("Jumping", !isGrounded && rb.velocity.y > 0.1f);
        animator.SetBool("Sprinting", isSprinting);

        wasGroundedLastFrame = isGrounded;

        AlignToSlope();
    }

    private void FlipSprite(int direction)
    {
        float newX = Mathf.Abs(transform.localScale.x) * direction;
        transform.localScale = new Vector3(newX, transform.localScale.y, transform.localScale.z);

        if (healthBarVisual != null)
        {
            Vector3 barScale = healthBarVisual.localScale;
            barScale.x = Mathf.Abs(barScale.x) * direction;
            healthBarVisual.localScale = barScale;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void AlignToSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.5f, groundLayer);
        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            Debug.DrawRay(hit.point, normal, Color.green);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void TriggerRandomAttack()
    {
        if (isAttacking) return;
        isAttacking = true;
        int attackIndex = Random.Range(1, 3);
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetTrigger("Attack");
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetInteger("AttackIndex", 0);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player terkena damage {damage}. Sisa HP: {currentHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarTransform != null)
        {
            float healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);
            Vector3 newScale = initialHealthBarScale;
            newScale.x *= healthPercent;
            healthBarTransform.localScale = newScale;

            Vector3 newPos = healthBarTransform.localPosition;
            newPos.x = initialHealthBarScale.x * (healthPercent - 1) * 0.5f;
            healthBarTransform.localPosition = newPos;
        }
    }

    private void Die()
    {
        Debug.Log("PLAYER MATI!");
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        if (animator != null)
            animator.SetTrigger("Die");
        this.enabled = false;
    }

    // 🎮 Dipanggil dari tombol UI
    public void OnMoveLeftDown() => moveLeft = true;
    public void OnMoveLeftUp() => moveLeft = false;

    public void OnMoveRightDown() => moveRight = true;
    public void OnMoveRightUp() => moveRight = false;

    public void OnJumpButton() => jumpPressed = true;
    public void OnAttackButton() => attackPressed = true;
}
