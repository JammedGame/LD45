using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoreController : MonoBehaviour
{
    public GameObject Panel;
    public Text LoreText;

    public void Update()
    {
        if (Game.Player.LoreToTell != null)
        {
            Panel.SetActive(true);
            LoreText.text = Game.Player.LoreToTell;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                Game.Player.LoreToTell = null;
            }
        }
        else
        {
            Panel.SetActive(false);
        }
    }

}
