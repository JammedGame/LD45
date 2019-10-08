using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLogic : MonoBehaviour
{
    private int CLevel = 1;
    public AudioSource Source;
    public AudioClip Level1;
    public AudioClip Level2;
    public AudioClip Level3;
    void Update()
    {
        int Level = Game.GameState.Level;
        if(Level != CLevel)
        {
            CLevel = Level;
            if(Level == 1) Source.clip = Level1;
            else if(Level == 2) Source.clip = Level2;
            else if(Level == 3) Source.clip = Level3;
            Source.Play();
        }
    }
}
