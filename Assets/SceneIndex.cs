using UnityEngine;

public class SceneIndex : MonoBehaviour
{
    public static SceneIndex scene;
    public int index;

    void Awake()
    {
        if(scene == null)
        {
            DontDestroyOnLoad(gameObject);
            scene = this;
            index = 0;
        }
        else if(scene != this)
        {
            Destroy(gameObject);
        }
    }
}
