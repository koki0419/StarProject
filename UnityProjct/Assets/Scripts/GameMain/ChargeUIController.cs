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
    //ビーストモード
    public Image beastModeFill;

    //public Image[] hpFils;
    public Image hpFil;

    //チャージポイントのupdete
    //public void UpdateChargePoint(float percentage)
    //{
    //    chargeFill.fillAmount = percentage;
    //}
    ////ユーザー用チャージポイントのupdete
    //public void UseUpdateChargePoint(float percentage)
    //{
    //    useChargeFill.fillAmount = percentage;
    //}
    ////ユーザー用チャージポイントのupdete
    //public void beastModeUpdateChargePoint(float percentage)
    //{
    //    beastModeFill.fillAmount = percentage;
    //}

    //public void UpdateHppoint(float hp,int hpFillNum)
    //{
    //    //Debug.Log(hp);
    //    hpFils[hpFillNum].fillAmount = hp;
    //}
    public void HppointUpdate(float hp)
    {

        hpFil.fillAmount = hp;
    }

}
