using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScene : MonoBehaviour
{
    public Text DamageText;
    public Text HealthText;
    public Text SpeedText;
    public Image DamageUpgrade;
    public Image HealthUpgrade;
    public Image SpeedUpgrade;
    // Start is called before the first frame update
    void Start()
    {
        this.DamageUpgrade.gameObject.SetActive(false);
        this.HealthUpgrade.gameObject.SetActive(false);
        this.SpeedUpgrade.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        this.DamageUpgrade.gameObject.SetActive(Game.Player.SkillPoints >= 10);
        this.HealthUpgrade.gameObject.SetActive(Game.Player.SkillPoints >= 10);
        this.SpeedUpgrade.gameObject.SetActive(Game.Player.SkillPoints >= 10);
        this.DamageText.text = (Game.Player.SkillDamage).ToString();
        this.HealthText.text = (Game.Player.SkillHealth).ToString();
        this.SpeedText.text = (Game.Player.SkillSpeed).ToString();
    }
}
