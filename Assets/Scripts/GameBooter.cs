using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBooter : MonoBehaviour
{
    public GameWorldData LevelData;

    void Awake()
    {
        Application.targetFrameRate = 60;
        GameTicker.StartTicking(LevelData);
    }
}
