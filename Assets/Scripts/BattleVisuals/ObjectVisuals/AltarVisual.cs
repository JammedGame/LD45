using UnityEngine;

public class AltarVisual : BattleObject3D
{
    public GameObject PW;
    public GameObject PWO;
    public GameObject Eyes;
    public GameObject Staff;
    public GameObject Heart;
    public GameObject Liver;
    public GameObject Lungs;
    public override void Sync(float dT)
    {
        base.Sync(dT);

        string Type = ((Altar)Data).AltarSettings.Item;
        
        bool Complete = ((Altar)Data).Complete;
        PW.SetActive(!Complete);
        PWO.SetActive(Complete);
        Eyes.SetActive(!Complete && Type == "Eyes");
        Staff.SetActive(!Complete && Type == "Staff");
        Heart.SetActive(!Complete && Type == "Heart");
        Liver.SetActive(!Complete && Type == "Liver");
        Lungs.SetActive(!Complete && Type == "Lungs");
    }
}