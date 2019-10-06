using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBooter : MonoBehaviour
{
    void Awake()
    {
        var state = Game.GameState;
        var levelData = Resources.Load<GameWorldData>($"LevelDesign/Level{state.Level}");

        Application.targetFrameRate = 60;
        GameTicker.StartTicking(levelData);
    }
}
