using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    public bool open;
    private SpriteRenderer sprite;

    void Start()
    {
        open = false;
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Input.GetAxis("Vertical") < 0 && open)
            {
                SceneManager.LoadScene("Lobby");
            }
        }
    }

    private void Open()
    {
        open = true;
        sprite.enabled = false;
    }
}
