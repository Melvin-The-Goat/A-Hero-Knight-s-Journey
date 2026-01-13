using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Public variables
    public float Accel_Mult = 0.05f;
    public float Slow_Rate = 1.0f;
    public float Max_Speed = 5.0f;
    public float Jump_Speed = 50.0f;
    public bool Grounded = false;

    // Private variables 
    private Rigidbody2D rb; // Reference to the Rigidbody2D component attached to the player
    private Vector2 movement; // Stores the direction of player movement

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        movement = rb.linearVelocity;
        // Get player input from keyboard or controller
        rb.linearVelocityX = Max_Speed * Input.GetAxisRaw("Horizontal");

        if (Grounded && Input.GetAxisRaw("Jump") > 0)
        {
            movement.y += Jump_Speed;
            Grounded = false;
        }
    }
    void FixedUpdate()
    {
        // Apply movement to the player in FixedUpdate for physics consistency
        rb.linearVelocity = movement;
    }
    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.name == "floor")
        {
            Grounded = true;
        }
    }
}

