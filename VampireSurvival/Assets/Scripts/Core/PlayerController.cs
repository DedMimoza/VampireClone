using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private Rigidbody2D rb;

    private Vector2 moveInput;
    private float moveSpeed => DataManager.Instance.playerData.moveSpeed;
    private float acceleration => DataManager.Instance.playerData.acceleration;

    [Header("Health")]
    [SerializeField] public Slider healthSlider;

    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    public static System.Action<Vector2> OnPlayerMoved;

    public Vector2 LastDirection { get; private set; } = Vector2.right;

    public static System.Action<Vector2> OnPlayerVelocityChanged;

    public static System.Action<int> OnHealthChanged;
    public static System.Action OnPlayerDied;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.drag = 10;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        if (moveInput.magnitude > 0.1f)
            LastDirection = moveInput.normalized;
        else if (rb.velocity.magnitude > 0.1f)
            LastDirection = rb.velocity.normalized;

        OnPlayerVelocityChanged?.Invoke(LastDirection);
        OnPlayerMoved?.Invoke(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("ÈÃÐÎÊ ÓÌÅÐ!");
        OnPlayerDied?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthUI();
        OnHealthChanged?.Invoke(currentHealth);
    }
}