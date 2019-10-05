using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBooter : MonoBehaviour
{
    public GameWorldData LevelData;

    void Awake()
    {
        SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);

        Application.targetFrameRate = 60;
        GameTicker.StartTicking(LevelData);
    }
}
