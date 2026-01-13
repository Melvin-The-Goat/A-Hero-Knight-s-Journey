using UnityEngine;

public class GolemAI : MonoBehaviour
{
    [Header("Stats")]
    public float MaxHP = 30.0f;
    public float CurHP = 30.0f;
    public float speed = 2.0f;
    public float attackRange = 20.0f;
    public float chaseRange = 4.0f;
    public float AttackCD = 2.0f; // attack cooldown in seconds
    public int attackDamage = 1; // how much damage golem does


    [Header("States")]
    public bool Grounded = true;
    public bool Dead = false;
    private bool canAttack = true;

    private Transform player;
    private Vector2 moveDirection;

    internal Animator _animator;
    internal Collider2D _collider;
    internal Rigidbody2D _body;
    internal GameObject door;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _body = GetComponent<Rigidbody2D>();
        door = GameObject.Find("Door");

        // Find player in scene (make sure your player has the tag "Player")
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }


    void Update()
    {
        if (Dead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Add debug logging
        Debug.Log($"Distance to player: {distance}, Attack Range: {attackRange}, Chase Range: {chaseRange}, Can Attack: {canAttack}");

        if (attacking)
        {
            moveDirection = Vector2.zero;
            _animator.SetBool("isWalking", false);
            Debug.Log("Currently attacking - movement locked");
            return;
        }

        // Check for attack first
        if (distance <= attackRange && canAttack)
        {
            Debug.Log("Attacking player");
            moveDirection = Vector2.zero;
            _animator.SetBool("isWalking", false);
            Attack();
        }
        else if (distance <= chaseRange)
        {
            Debug.Log("Chasing player");
            moveDirection = (player.position - transform.position).normalized;
            _animator.SetBool("isWalking", true);

            // Flip sprite
            if (moveDirection.x > 0)
                transform.localScale = new Vector3(3, 3, 1);
            else if (moveDirection.x < 0)
                transform.localScale = new Vector3(-3, 3, 1);
        }
        else
        {
            Debug.Log("Idle - player too far");
            moveDirection = Vector2.zero;
            _animator.SetBool("isWalking", false);
        }
    }
    void FixedUpdate()
    {
        if (!Dead && moveDirection != Vector2.zero)
        {
            // Scripted movement for walking
            _body.MovePosition(_body.position + moveDirection * speed * Time.fixedDeltaTime);
        }
    }

    private bool attacking = false;

    void Attack()
    {
        canAttack = false;
        attacking = true; // stop walking while attacking
        _animator.SetTrigger("isAttacking");

        Invoke(nameof(ResetAttack), AttackCD);
    }

    // Called via Animation Event
    public void DealDamage()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= attackRange) // still close enough
            {
                HeroKnight hero = player.GetComponent<HeroKnight>();
                if (hero != null)
                {
                    hero.SendMessage("Damage"); // calls Damage() on HeroKnight
                    Debug.Log("Golem dealt " + attackDamage + " damage to player!");
                }
            }
        }
    }

    void ResetAttack()
    {
        canAttack = true;
        attacking = false;
    }
    public void TakeDamage(float amount)
    {
        if (Dead) return;

        CurHP -= amount;
        _animator.SetTrigger("Hurt"); // make sure you have a Hurt trigger in Animator
        if (CurHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Dead = true;
        moveDirection = Vector2.zero;
        _animator.SetTrigger("isDead");
        door.SendMessage("Open");
        Destroy(gameObject, 2f); // delay so death animation can play
    }
}
