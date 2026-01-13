using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            collision.SendMessage("Damage");
        }
    }
}
