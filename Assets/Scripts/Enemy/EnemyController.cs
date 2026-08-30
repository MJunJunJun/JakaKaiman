using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRay = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Health Bar")]
    [SerializeField] private Transform healthBarTransform;  // bar isi (yang scale.x menyusut)
    [SerializeField] private Transform healthBarVisual;     // bar container (yang ikut arah)
    private Vector3 initialHealthBarScale;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isAttacking = false;
    private bool playerInAttackRange = false;
    private float lastAttackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (healthBarTransform != null)
            initialHealthBarScale = healthBarTransform.localScale;
    }

    private void Update()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.deltaTime;
        }

        if (player == null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            animator.SetBool("Walking", false);
            return;
        }

        if (playerInAttackRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            animator.SetBool("Walking", false);
            TriggerAttack();
        }
        else
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
            FlipSprite(dir.x);
            animator.SetBool("Walking", Mathf.Abs(dir.x) > 0.1f);
        }
    }

    private void LateUpdate()
    {
        AlignToSlope();
    }

    private void AlignToSlope()
    {
        if (groundCheck == null) return;

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRay, groundLayer);
        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void FlipSprite(float directionX)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX > 0 ? 1 : -1);
        transform.localScale = scale;

        // Ikut arah flip untuk bar visual
        if (healthBarVisual != null)
        {
            Vector3 visualScale = healthBarVisual.localScale;
            visualScale.x = Mathf.Abs(visualScale.x) * (directionX > 0 ? 1 : -1);
            healthBarVisual.localScale = visualScale;
        }
    }

    // ---------- ZONA ----------
    public void SetPlayerDetected(Transform target)
    {
        player = target;
    }

    public void SetPlayerLost()
    {
        player = null;
    }

    public void SetInAttackZone(bool status)
    {
        playerInAttackRange = status;
    }

    // ---------- ATTACK ----------
    public void TriggerAttack()
    {
        if (isAttacking || Time.time < lastAttackTime + attackCooldown) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        int index = Random.Range(1, 3);
        animator.SetInteger("AttackIndex", index);
        animator.SetTrigger("Attack");
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetInteger("AttackIndex", 0);
    }

    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, 0.5f, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            Debug.Log("PLAYER terkena serangan!");
            var player = hit.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }

    // ---------- DAMAGE ----------
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy terkena damage {damage}. Sisa HP: {currentHealth}");

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
        Debug.Log("Enemy mati!");
        animator.SetTrigger("Die");

        GetComponent<Collider2D>().enabled = false;
        rb.simulated = false;

        Destroy(gameObject, 2f);
    }

    // ---------- DEBUG ----------
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, 0.5f);
        }
    }
}
