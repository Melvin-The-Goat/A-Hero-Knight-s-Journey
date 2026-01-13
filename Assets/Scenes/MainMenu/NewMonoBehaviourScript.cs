using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Animator animator;

    [Header("Button Type")]
    public ButtonType buttonType; // Choose in Inspector
    public GameObject optionsMenu; // Assign in Inspector if using Options

    public enum ButtonType
    {
        Start,
        Options,
        Exit
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator != null)
            animator.SetBool("Hover", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator != null)
            animator.SetBool("Hover", false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Start:
                // Load next scene (make sure it's added in Build Settings)
                SceneManager.LoadScene("TestMap");
                break;

            case ButtonType.Options:
                if (optionsMenu != null)
                    optionsMenu.SetActive(true);
                break;

            case ButtonType.Exit:
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in editor
#endif
                break;
        }
    }
}
