using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPointText : MonoBehaviour
{
    public Text SkillPointCount;

    public void Update(){
        SkillPointCount.text = Game.Player.SkillPoints.ToString();
    }
}
