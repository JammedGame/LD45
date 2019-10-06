﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image image;

    public void Update(){
        image.fillAmount = Game.Player.Health/Game.Player.PlayerSettings.Health;
    }
}