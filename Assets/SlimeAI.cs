using Unity.VisualScripting;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class SlimeAI : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public float MaxHP = 30.0f;
        public float CurHP = 30.0f;
        public float AttackCD = 5.0f;
        public float JumpTimer;
        public float DamageTimer;
        public bool JumpWait = false;
        public bool Grounded = true;
        public bool Dead = false;

        internal Animator _animator;
        internal Collider2D _collider;
        internal Rigidbody2D _body;
        internal GameObject _player;
        internal SpriteRenderer _sprite;
        internal GameObject door;
        private float playerPos;

        void Start()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<CapsuleCollider2D>();
            _body = GetComponent<Rigidbody2D>();
            _player = GameObject.Find("HeroKnight");
            _body.constraints = RigidbodyConstraints2D.FreezeRotation;
            _sprite = GetComponent<SpriteRenderer>();
            door = GameObject.Find("Door");

            MaxHP = 30.0f;
            CurHP = MaxHP;
        }

        // Update is called once per frame
        void Update()
        {
            playerPos = _player.transform.position.x;
            _animator.ResetTrigger("Die");

            if (!Dead && CurHP <= 0 )
            {
                Dead = true;
                _animator.SetTrigger("Die");
                _animator.SetBool("Dead", true);
                door.SendMessage("Open");
                Destroy(gameObject, 2f); // delay so death animation can play
            }

            if (!Dead && playerPos < transform.position.x && Grounded)
            {
                _sprite.flipX = true;
            }
            else if (!Dead && Grounded)
            {
                _sprite.flipX = false;
            }

            if(JumpWait && !Dead)
            {
                JumpTimer -= Time.deltaTime;
                if(JumpTimer < 0)
                {
                    float jumpX;
                    if (playerPos < transform.position.x) { 
                        jumpX = Random.Range(-2, -5); 
                    }
                    else { 
                        jumpX = Random.Range(2, 5); 
                    }

                    float jumpY = 12f;
                    _body.linearVelocity = new Vector2(jumpX, jumpY);
                    JumpWait = false;
                }
            }

            if(AttackCD < 0 && !Dead)
            {
                JumpWait = true;
                JumpAttack();
                AttackCD = 5.0f;
            }
            else if(!Dead)
            {
                AttackCD -= Time.deltaTime;
            }

            if(DamageTimer <= 0)
            {
                _sprite.color = Color.white;
                _animator.ResetTrigger("Hurt");
            }
            else
            {
                DamageTimer -= Time.deltaTime;
            }
        }

        void JumpAttack()
        {
            _animator.SetTrigger("Jump");
            JumpTimer = 1.5f;
        }

        void TakeDamage(float amount)
        {
            if (DamageTimer > 0 || Dead) return;
            CurHP -= amount;
            if (!Grounded && CurHP <= 0)
            {
                CurHP = 1;
            }
            if (Grounded && !JumpWait)
            {
                _animator.SetTrigger("Hurt");
            }
            _sprite.color = Color.red;
            DamageTimer = 0.15f;
        }

        void OnCollisionEnter2D(UnityEngine.Collision2D collision)
        {
            Debug.Log("Entered");
            if (collision.gameObject.CompareTag("Floor"))
            {
                Grounded = true;
                _animator.SetBool("Grounded", true);
            }
        }

        void OnCollisionExit2D(UnityEngine.Collision2D collision)
        {
            Debug.Log("Exited");
            if (collision.gameObject.CompareTag("Floor"))
            {
                Grounded = false;
                _animator.SetBool("Grounded", false);
            }
        }
    }

}
