using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public GameObject Panel;
    public Text DialogText;

    public Image Portrait;

    public void Update()
    {
        if (Game.Player.DialogToTell != null)
        {
            Panel.SetActive(true);
            DialogText.text = Game.Player.DialogToTell;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Panel.SetActive(false);
                Game.Player.DialogToTell = null;
            }
        }

        if (Game.Player.MonologToTell != null)
        {
            Portrait.gameObject.SetActive(true);
            Panel.SetActive(true);
            DialogText.text = Game.Player.MonologToTell;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Portrait.gameObject.SetActive(false);
                Panel.SetActive(false);
                Game.Player.MonologToTell = null;
            }
        }
    }

}
