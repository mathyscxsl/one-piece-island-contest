using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float speed = 2f;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1.5f;

    private int _currentHealth;
    private float _attackTimer;
    private bool _isDead = false;

    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _sr;
    private Transform _player;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();

        _currentHealth = maxHealth;
    }


    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }


    private void FixedUpdate()
    {
        if (_isDead || _player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _anim.SetFloat("Velocity", 0f);
            TryAttack();
        }

        // Tri de profondeur
        _sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10);
    }


    private void MoveTowardsPlayer()
    {
        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * speed;

        _sr.flipX = direction.x > 0;
        _anim.SetFloat("Velocity", 1f);
    }


    private void TryAttack()
    {
        _attackTimer -= Time.fixedDeltaTime;

        if (_attackTimer <= 0f)
        {
            _attackTimer = attackCooldown;
            _anim.SetTrigger("Attack");
        }
    }


    // Appelé par Animation Event sur le frame d'impact
    public void DealDamage()
    {
        if (_isDead || _player == null) return;

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance <= attackRange)
        {
            _player.GetComponent<PlayerController>().TakeDamage(attackDamage);
        }
    }


    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }


    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;
        _anim.SetTrigger("Die");

        Destroy(gameObject, 1f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    public int GetCurrentHealth() => _currentHealth;
}