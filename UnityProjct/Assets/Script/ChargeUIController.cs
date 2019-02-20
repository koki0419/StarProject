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

    //チャージポイントのupdete
    public void UpdateChargePoint(float percentage)
    {
        chargeFill.fillAmount = percentage;
    }

    public void UseUpdateChargePoint(float percentage)
    {
        useChargeFill.fillAmount = percentage;
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
