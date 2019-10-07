using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSpeed : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Game.Player.MovementSpeedBonus += 15;
        Game.Player.SkillPoints -= 10;
    }
}
