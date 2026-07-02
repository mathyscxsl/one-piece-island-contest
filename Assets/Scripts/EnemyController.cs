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
    [SerializeField] private float attackCooldown = 1.5f;

    private int _currentHealth;
    private float _attackTimer;
    private bool _isDead = false;
    private bool _isInContact = false;

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

        if (!_isInContact)
        {
            MoveTowardsPlayer();
        }
        else
        {
            _anim.SetFloat("Velocity", 0f);
            TryAttack();
        }

        _sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _isInContact = true;
            _rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }
    }


    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _isInContact = false;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }


    private void MoveTowardsPlayer()
    {
        if (_player == null) return;

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


    public void DealDamage()
    {
        if (_isDead || !_isInContact) return;

        _player.GetComponent<PlayerController>().TakeDamage(attackDamage);
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
        _rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        _anim.SetTrigger("Die");

        Destroy(gameObject, 1f);
    }


    public int GetCurrentHealth() => _currentHealth;
}