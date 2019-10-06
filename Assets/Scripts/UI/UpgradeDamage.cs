using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDamage : MonoBehaviour
{
    public Image image;
    
    public void ShowImage(){
        image.gameObject.SetActive(true);
    }

        public void HideImage(){
        image.gameObject.SetActive(false);
    }
}
