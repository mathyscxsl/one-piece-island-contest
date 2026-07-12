using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float speed = 2f;

    [Header("Combat")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float contactDistance = 1f;

    [Header("Sounds")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    private int _currentHealth;
    private float _attackTimer;
    private bool _isDead = false;
    private bool _isInContact = false;
    private bool _gameOver = false;

    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _sr;
    private AudioSource _audioSource;
    private Transform _player;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _currentHealth = maxHealth;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.excludeLayers = LayerMask.GetMask("Player");
    }


    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        if (state != GameManager.GameState.GameOver) return;

        _gameOver = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _anim.speed = 0f;
    }


    private void FixedUpdate()
    {
        if (_isDead || _gameOver || _player == null) return;

        _isInContact = Vector2.Distance(transform.position, _player.position) <= contactDistance;

        if (!_isInContact)
        {
            MoveTowardsPlayer();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _anim.SetFloat("Velocity", 0f);
            TryAttack();
        }

        _sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10);
    }


    private void MoveTowardsPlayer()
    {
        if (_player == null) return;

        Vector2 toPlayer = (_player.position - transform.position).normalized;
        Vector2 separation = GetSeparation();

        // Keep only the lateral component to avoid oscillation
        separation -= Vector2.Dot(separation, toPlayer) * toPlayer;
        separation = Vector2.ClampMagnitude(separation, 1f);

        Vector2 direction = (toPlayer + separation).normalized;

        _rb.linearVelocity = direction * speed;

        _sr.flipX = direction.x > 0;
        _anim.SetFloat("Velocity", 1f);
    }


    private Vector2 GetSeparation()
    {
        Vector2 result = Vector2.zero;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);
        foreach (Collider2D hit in hits)
        {
            EnemyController other = hit.GetComponentInParent<EnemyController>();
            if (other == null || other == this) continue;
            Vector2 away = (Vector2)(transform.position - hit.transform.position);
            if (away.sqrMagnitude > 0f)
                result += away.normalized;
        }
        return result;
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


    public void PlayAttackSound()
    {
        if (_isDead) return;
        _audioSource.PlayOneShot(attackSound);
    }


    public void DealDamage()
    {
        if (_isDead || _gameOver || !_isInContact) return;

        _player.GetComponent<PlayerController>().TakeDamage(attackDamage);
    }


    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth <= 0)
            Die();
        else
            _audioSource.PlayOneShot(hurtSound);
    }


    public void Configure(float healthMult, float damageMult)
    {
        maxHealth = Mathf.RoundToInt(maxHealth * healthMult);
        attackDamage = Mathf.RoundToInt(attackDamage * damageMult);
        _currentHealth = maxHealth;
    }


    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        _anim.ResetTrigger("Attack");
        _anim.SetTrigger("Die");
        _audioSource.PlayOneShot(deathSound);
        OnDeath?.Invoke();

        Destroy(gameObject, 1f);
    }


    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => maxHealth;
}