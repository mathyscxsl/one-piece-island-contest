using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Range(1f, 20f)]
    private float speed = 5f;

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private int attackDamage = 25;


    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask enemyLayer;

    private float _attackTimer = 0f;
    private bool _isAttacking = false;


    [Header("Death")]
    [SerializeField] private float deathAnimationDuration = 1.5f;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;


    private int _currentHealth;
    private bool _isDead = false;

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _anim;
    private SpriteRenderer _sr;

    private bool _canMove = false;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();

        _currentHealth = maxHealth;
    }


    private void Start()
    {
        StartStartingPose();
    }


    private void StartStartingPose()
    {
        _canMove = false;
        _movement = Vector2.zero;
        _rb.linearVelocity = Vector2.zero;
        _anim.SetBool("IsPosing", true);
    }


    public void EndStartingPose()
    {
        _anim.SetBool("IsPosing", false);
        _movement = Vector2.zero;
        _rb.linearVelocity = Vector2.zero;
        _canMove = true;
    }


    private void OnMove(InputValue value)
    {
        _movement = value.Get<Vector2>();
    }


    private void OnAttack()
    {
        if (!_canMove) return;
        if (_attackTimer > 0f) return;
        if (_isAttacking) return;

        _isAttacking = true;
        _attackTimer = attackCooldown;
        _anim.SetTrigger("Attack");
    }


    private void Update()
    {
        if (_attackTimer > 0f)
            _attackTimer -= Time.deltaTime;
    }


    private void FixedUpdate()
    {
        if (!_canMove)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = _movement.magnitude > 1f ? _movement.normalized : _movement;
        _rb.linearVelocity = dir * speed;
        _sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10);

        _anim.SetFloat("Horizontal", _movement.x);
        _anim.SetFloat("Velocity", _movement.magnitude);

        if (Mathf.Abs(_movement.x) > 0.01f)
        {
            _anim.SetFloat("LastHorizontal", _movement.x);
            _sr.flipX = _movement.x < 0;
        }
    }


    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            EnemyController enemy = hit.GetComponentInParent<EnemyController>();
            enemy?.TakeDamage(attackDamage);
        }
    }


    public void EndAttack()
    {
        _isAttacking = false;
    }


    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth <= 0)
            Die();
    }


    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _canMove = false;
        _rb.linearVelocity = Vector2.zero;
        _anim.SetTrigger("Die");

        StartCoroutine(DieSequence());
    }


    private IEnumerator DieSequence()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        _anim.speed = 0f;
        OnDeath?.Invoke();
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }


    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetAttackDamage() => attackDamage;
    public float GetSpeed() => speed;
}