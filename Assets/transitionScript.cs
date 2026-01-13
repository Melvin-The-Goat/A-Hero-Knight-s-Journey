using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class transitionScript : MonoBehaviour
{
    public string nextScene;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (SceneIndex.scene.index)
            {
                case 0: nextScene = "DungeonSlime"; break;
                case 1: nextScene = "DungeonGolem"; break;
                case 2: nextScene = "FinalBoss"; break;
                default: break;
            }
            SceneIndex.scene.index++;
            SceneManager.LoadScene(nextScene);
        }
    }
}
