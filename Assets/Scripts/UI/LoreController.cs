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

            if (Input.GetKeyDown(KeyCode.E))
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
