using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_STANDALONE
public class Character : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private CapsuleCollider2D _capsuleCollider2D;
    private PolygonCollider2D _polygonCollider2D;
    private int health;
    //движение персонажа
    private float _inputX;
    private float _inputY;
    private float _speedX;
    private float _speedY;
    //бег
    public float maxSpeed = 10f;
    private bool _flipRight = true;
    //прыжок
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public int jumpForce = 5000;
    private bool _isGrounded = false;
    private float _groundRadius = 0.3f;
    //dash
    public float dashForce = 15f;
    public float dashAcceleration = 100f;
    private bool _isDashing = false;
    private float currentDashForce = 0f;
    //приседание
    public float crouchingTriggerForce = -0.1F;
    public float slidingStoppingForce = 0.01F;
    private bool _isCrouching = false;
    //атака
    public static int currentPlayerDamage = 1;
    public float attackDuration = 0.55F;
    private bool _isAttack;
    //получение урона при падении
    public float hitTriggerSpeed = -30.0F;
    public float hitAnimationDuration = 0.3F;
    public float deathAnimationDuration = 1.0F;
    //получение урона
    public int damageFromFall = 1;
    public float knockbackForce = 30f;
    private bool _isHitted = false;

    public static int GetDamage()
    {
        return currentPlayerDamage;
    }
    private void CheckStates()
    {
        _speedX = Mathf.Abs(_rigidbody2D.velocity.x / maxSpeed);
        _speedY = _rigidbody2D.velocity.y;
        _animator.SetFloat("speedX", _speedX);
        _animator.SetFloat("speedY", _speedY);
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, _groundRadius, whatIsGround);
        _animator.SetBool("isGrounded", _isGrounded);
    }
    private void Flip()
    {
        _flipRight = !_flipRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    private void Run()
    {
        _rigidbody2D.velocity = new Vector2(_inputX * maxSpeed, _speedY);
    }
    private void Jump()
    {
        _animator.SetBool("isGrounded", false);
        _rigidbody2D.AddForce(new Vector2(0, jumpForce));
    }
    private void Crouch()
    {
        _isCrouching = true;
        _capsuleCollider2D.direction = CapsuleDirection2D.Horizontal;
        _capsuleCollider2D.offset = new Vector2(0.0F, -0.8F);
        _capsuleCollider2D.size = new Vector2(1.4F, 0.9F);
        _animator.SetBool("isCrouching", _isCrouching);
    }
    private void UnCrouch()
    {
        _isCrouching = false;
        _capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
        _capsuleCollider2D.offset = new Vector2(-0.1F, -0.26F);
        _capsuleCollider2D.size = new Vector2(1.05F, 2F);
        _animator.SetBool("isCrouching", _isCrouching);
    }
    private void Slide()
    {
        int flipX = 1;
        if (!_flipRight)
            flipX = -1;
        _rigidbody2D.velocity =
            new Vector2(flipX * (_speedX - slidingStoppingForce) * maxSpeed, _speedY);
    }
    private void Dash()
    {
        int flipX = 1;
        if (!_flipRight)
            flipX = -1;
        currentDashForce += dashAcceleration * Time.fixedDeltaTime;
        currentDashForce = Mathf.Clamp(currentDashForce, 0f, dashForce);
        _rigidbody2D.AddForce(flipX * transform.right * currentDashForce * maxSpeed
                              , ForceMode2D.Impulse);
        if (currentDashForce >= dashForce)
        {
            _isDashing = false;
            _animator.SetBool("isDashing", _isDashing);
            currentDashForce = 0f;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            Monster monster = collision.gameObject.GetComponent<Monster>();
            if (monster != null)
            {
                ReceiveDamage(monster);
            }
        }
    }
    IEnumerator ResetHittedState()
    {
        yield return new WaitForSeconds(hitAnimationDuration);
        _isHitted = false;
        _animator.SetBool("isHitted", _isHitted);
    }
    IEnumerator Die()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _animator.SetBool("death", true);
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }
    public void ReceiveDamage(Monster monster)
    {
        _isDashing = false;
        _animator.SetBool("isDashing", _isDashing);
        currentDashForce = 0f;
        PlayerStats.Instance.TakeDamage(monster.GetDamage());
        health = (int)PlayerStats.Instance.Health;
        _isHitted = true;
        _animator.SetBool("isHitted", _isHitted);
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            Vector2 direction = transform.position - monster.transform.position;
            direction = direction.normalized;
            direction += Vector2.up * 1.5F;
            _rigidbody2D.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(ResetHittedState());
        }
    }
    private void ReceiveDamageFromFall()
    {
        PlayerStats.Instance.TakeDamage(damageFromFall);
        health = (int)PlayerStats.Instance.Health;
        _isHitted = true;
        _animator.SetBool("isHitted", _isHitted);
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(ResetHittedState());
        }
    }
    IEnumerator ResetAtackState()
    {
        yield return new WaitForSeconds(attackDuration / 3);
        _polygonCollider2D.enabled = _isAttack;
        yield return new WaitForSeconds(attackDuration / 3);
        _isAttack = false;
        _polygonCollider2D.enabled = _isAttack;
        yield return new WaitForSeconds(attackDuration / 3);
        _animator.SetBool("isAttack", _isAttack);
    }
    
    private void Attack()
    {
        _isAttack = true;
        _animator.SetBool("isAttack", _isAttack);
        StartCoroutine(ResetAtackState());
    }
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _polygonCollider2D = transform.GetChild(1).GetComponent<PolygonCollider2D>();
        whatIsGround = LayerMask.GetMask("Ground");
        groundCheck = transform.GetChild(0);
    }
    void FixedUpdate()
    {
        //обновление состояние персонажа
        CheckStates();
        if (_isHitted)
        {
            return;
        }
        //hurt
        if (!_isHitted && _isGrounded && _speedY < hitTriggerSpeed)
        {
            ReceiveDamageFromFall();
        }
        //бег
        if (!_isCrouching)
        {
            Run();
        }
        //подкат
        else if (_speedX > slidingStoppingForce)
        {
            Slide();
        }
        //поворот персонажа
        if ((_inputX > 0 && !_flipRight) || (_inputX < 0 && _flipRight))
        {
            Flip();
        }
        //dash
        if (_isDashing)
        {
            Dash();
        }
    }
    private void Update()
    {
        //ввод движения
        _inputX = Input.GetAxis("Horizontal");
        _inputY = Input.GetAxis("Vertical");
        if (_isHitted)
        {
            return;
        }
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (!_isDashing && _isGrounded && !_isCrouching && Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isDashing = true;
            _animator.SetBool("isDashing", _isDashing);
        }
        if (!_isCrouching && _isGrounded && !_isDashing &&
              _speedX < 1 && _inputY < crouchingTriggerForce)
        {
            Crouch();
        }
        else if (_isCrouching && _inputY > crouchingTriggerForce)
        {
            UnCrouch();
        }
        if (!_isAttack && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }
}
#endif
