using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private Character _player;

    public float moveSpeed = 2f; // —корость движени€ монстра
    public float moveDistance = 5f; // –ассто€ние движени€, задаваемое в редакторе Unity
    private bool _flipRight = true;
    private float speedDirection = 1.0F;
    private float currentDistance;
    private float _speedX;

    public float hitAnimationDuration = 0.2F;
    public float deathAnimationDuration = 0.8F;
    public float knockbackForce = 5f;
    public int maxHealth = 3;
    public int monsterDamage = 1;
    private int currentHealth;
    private bool _isHitted;
    private bool death = false;

    private void Flip()
    {
        _flipRight = !_flipRight;
        speedDirection *= -1;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    //атака
    public int GetDamage()
    {
        return monsterDamage;
    }
    //получение урона
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            int damage = Character.GetDamage();
            ReceiveDamage(damage);
        }
    }
    IEnumerator ResetHittedState()
    {
        _rigidbody2D.velocity = Vector2.zero;
        yield return new WaitForSeconds(hitAnimationDuration);
        _isHitted = false;
        _animator.SetBool("isHitted", _isHitted);
    }
    IEnumerator Die()
    {
        death = true;
        _rigidbody2D.isKinematic = true;
        GetComponent<BoxCollider2D>().enabled = false;
        _animator.SetBool("death", death);
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
    private void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
        _isHitted = true;
        _animator.SetBool("isHitted", _isHitted);
        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            Vector2 direction = transform.position - _player.transform.position;
            direction = direction.normalized;
            direction += Vector2.up * 1.5F;
            GetComponent<Rigidbody2D>().AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(ResetHittedState());
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<Character>();
    }
    void FixedUpdate()
    {
        if (death)
            return;
        if (currentDistance >= moveDistance)
        {
            Flip();
            currentDistance = 0f;
        }

        Vector3 newPosition = transform.position + new Vector3(speedDirection * moveSpeed * Time.fixedDeltaTime, 0f, 0f);
        transform.position = newPosition;
        _speedX = Mathf.Abs(speedDirection * moveSpeed);
        _animator.SetFloat("speedX", _speedX);
        currentDistance += Mathf.Abs(speedDirection * moveSpeed) * Time.fixedDeltaTime;
    }
}
