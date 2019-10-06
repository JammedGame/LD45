using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeHealth : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Game.Player.Health += 2;
        Game.Player.HealthBonus += 2;
        Debug.Log(Game.Player.Health);
        Debug.Log(Game.Player.HealthBonus);
        Debug.Log(Game.Player.PlayerSettings.Health);
        Game.Player.SkillPoints -= 10;
    }
}
