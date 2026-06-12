using UnityEngine;
using UnityEngine.InputSystem;

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
    private int attackDamage = 10;

    private int _currentHealth;

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
        if (!_canMove) return;

        _movement = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!_canMove)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        // Mouvement horizontal uniquement
        _rb.linearVelocity = new Vector2(_movement.x, 0f) * speed;

        // Animation
        _anim.SetFloat("Horizontal", _movement.x);
        _anim.SetFloat("Velocity", Mathf.Abs(_movement.x));

        if (Mathf.Abs(_movement.x) > 0.01f)
        {
            _anim.SetFloat("LastHorizontal", _movement.x);
        }

        // Flip sprite
        if (Mathf.Abs(_movement.x) > 0.01f)
        {
            _sr.flipX = _movement.x < 0;
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        _canMove = false;
        _rb.linearVelocity = Vector2.zero;

        Debug.Log($"{gameObject.name} est mort.");
    }

    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetAttackDamage() => attackDamage;
    public float GetSpeed() => speed;
}