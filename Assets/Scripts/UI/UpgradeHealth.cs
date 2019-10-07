using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeHealth : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Game.Player.Health+=3;
        Game.Player.HealthBonus++;
        Game.Player.SkillPoints -= 8;
    }
}
