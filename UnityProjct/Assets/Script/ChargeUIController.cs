using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUIController : MonoBehaviour
{
    //☆獲得ポイント
    public Image chargeFill;
    //攻撃使用ポイント
    public Image useChargeFill;

    //public Image[] hpFils;
    public Image hpFil;

    //チャージポイントのupdete
    public void UpdateChargePoint(float percentage)
    {
        chargeFill.fillAmount = percentage;
    }
    //ユーザー用チャージポイントのupdete
    public void UseUpdateChargePoint(float percentage)
    {
        useChargeFill.fillAmount = percentage;
    }

    //public void UpdateHppoint(float hp,int hpFillNum)
    //{
    //    //Debug.Log(hp);
    //    hpFils[hpFillNum].fillAmount = hp;
    //}
    public void UpdateHppoint(float hp)
    {

        hpFil.fillAmount = hp;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
