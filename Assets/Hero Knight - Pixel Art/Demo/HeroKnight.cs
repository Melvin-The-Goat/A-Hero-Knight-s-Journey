using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] float m_speed = 6.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private PolygonCollider2D AttackBox;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;

    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private bool Hurt = false;
    private bool Dead = false;

    private int m_facingDirection = 1;
    private int m_currentAttack = 0;

    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;
    private float Invuln = 0;
    private float Knockback = 0;

    private float MaxHP;
    public float CurHP;
    public float AttackDamage;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        AttackBox = GetComponent<PolygonCollider2D>();
        AttackBox.enabled = false;

        MaxHP = 10f;
        CurHP = MaxHP;
        AttackDamage = 1f;

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    void Update()
    {
        if (Dead) return;

        m_timeSinceAttack += Time.deltaTime;

        if (m_rolling) m_rollCurrentTime += Time.deltaTime;
        if (m_rollCurrentTime > m_rollDuration) m_rolling = false;

        // Ground check
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // Input + movement
        float inputX = Input.GetAxis("Horizontal");

        if (inputX > 0 && m_timeSinceAttack > 0.15f)
        {
            transform.localScale = new Vector3(3, 3, 2);
            m_facingDirection = 1;
        }
        else if (inputX < 0 && m_timeSinceAttack > 0.15f)
        {
            transform.localScale = new Vector3(-3, 3, 2);
            m_facingDirection = -1;
        }

        if (!m_rolling && Knockback <= 0)
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);
        
        /*
        // Wall slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) ||
                          (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);
        */

        // Hurt
        if (Hurt && Invuln <= 0 && !m_rolling)
        {
            m_animator.SetTrigger("Hurt");
            m_body2d.linearVelocityX = 4f * m_facingDirection * -1f;
            m_body2d.linearVelocityY = 2f;
            CurHP--;
            if (CurHP <= 0) { Die(); }
            Hurt = false;
            Knockback = 0.5f;
            Invuln = 1.5f;
        }

        // Attack
        else if (Input.GetKeyDown("f") && m_timeSinceAttack > 0.25f && !m_rolling && Knockback <= 0)
        {
            m_currentAttack++;

            if (m_currentAttack > 3) m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f) m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
        }

        // Jump
        else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        // Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        // Idle
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }

        if (Invuln > 0 || Knockback > 0)
        {
            Invuln -= Time.deltaTime;
            Knockback -= Time.deltaTime;
        }
    }

    // Attack Animation Events
    void StartAttack()
    {
        AttackBox.enabled = true;
    }

    void EndAttack()
    {
        AttackBox.enabled = false;
    }

    // Slide dust event
    void AE_SlideDust()
    {
        Vector3 spawnPosition = (m_facingDirection == 1)
            ? m_wallSensorR2.transform.position
            : m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            GameObject dust = Instantiate(m_slideDust, spawnPosition, transform.localRotation);
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (AttackBox.enabled && collision.CompareTag("Enemy"))
        {
            collision.gameObject.SendMessage("TakeDamage", AttackDamage);
        }
    }

    void Die()
    {
        if (Dead) return;

        Dead = true;
        m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");

        m_body2d.linearVelocity = Vector2.zero;
        StartCoroutine(DisableAfterDelay(0.1f));
    }

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enabled = false;
    }

    void Damage()
    {
        if (Dead) return;

        Hurt = true;
        CurHP--;

        if (CurHP <= 0)
        {
            Die();
        }
    }
}
