using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUIController : MonoBehaviour
{
    //攻撃使用ポイント
    [SerializeField] private Image useChargeFill;

    //ユーザー用チャージポイントのupdete
    public void UseUpdateChargePoint(float percentage)
    {
        useChargeFill.fillAmount = percentage;
    }



}
