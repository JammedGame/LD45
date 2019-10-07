using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreController : MonoBehaviour
{
    public void Update(){

        if (Input.GetKeyDown(KeyCode.E)) {
            Game.Player.LoreToTell = null;
        }
    }
            
}
