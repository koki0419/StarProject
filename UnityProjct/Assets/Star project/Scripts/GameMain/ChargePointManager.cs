using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChargePointManager
{

    //☆チャージポイント
    [SerializeField] float chargePoint = 0;
    public float ChargePoint
    {
        get { return chargePoint; }
        set { chargePoint = value; }
    }
    //☆チャージポイント最大値+1で入力
    [SerializeField] float chargePointMax = 51;
    public float ChargePointMax
    {
        get { return chargePointMax; }
    }


    //小さい☆の獲得状況
    [SerializeField] int starChildCount = 0;
    public float StarChildCount
    {
        get { return starChildCount; }
        set { starChildCount = (int)value; }
    }
    //小さい☆の獲得状況スキップ
    [SerializeField] int starChildCountSkip = 0;
    public float StarChildCountSkip
    {
        get { return starChildCountSkip; }
        set { starChildCountSkip = (int)value; }
    }

    [SerializeField] float destroyCount = 0;
    public float DestroyCount
    {
        get { return destroyCount; }
        set { destroyCount = (int)value; }
    }
    //プレイヤーHP
    [SerializeField] float playerHp;

    float playerHpMax;

    //ゲージダウン割合
    float gaugeDroportion;




    // Start is called before the first frame update
    public void Init()
    {
        //チャージポイント
        chargePoint = 0;

        playerHpMax = playerHp;
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        //チャージポイント
        if (chargePoint <= 0)
        {
            chargePoint = 0;
        }
        else if (chargePoint >= chargePointMax)
        {
            chargePoint = chargePointMax;
        }
        if ((starChildCountSkip + chargePoint) >= chargePointMax)
        {
            starChildCountSkip = (int)chargePointMax - (int)chargePoint;
            chargePoint += starChildCountSkip;
        }

        if (starChildCountSkip != 0)
        {
            for (int i = 0; i < starChildCountSkip; i++)
            {
                starChildCount++;
               Singleton.Instance.gameSceneController.starChargeController.UpdateChildrenUI(starChildCount);
                //☆獲得10個になったら初期化
                if (starChildCount == 10)
                {
                    starChildCount = 0;
                }
            }
        }
        //chargeUIController.UpdateChargePoint(chargePoint / chargePointMax);
        if (chargePoint <= chargePointMax)
        {
            Singleton.Instance.gameSceneController.starChargeController.UpdateChildrenUI(starChildCount);
            //☆獲得10個になったら初期化
            if (starChildCount == 10)
            {
                starChildCount = 0;
            }
        }

    }


    //ダメージを受けます
    void HpDamage(float damage)
    {

        if (playerHp > 0)
        {
            playerHp -= damage;
        }
        else
        {
            playerHp = 0;
        }
    }

    //☆使用後状態を確認します
    public int DestroyMode(float charge)
    {
        if (charge < 10)
        {
            charge = 1;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 0;
        }
        else if (charge < 20)
        {
            charge = 2;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 1;
        }
        else if (charge < 30)
        {
            charge = 3;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 2;
        }
        else if (charge < 40)
        {
            charge = 4;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 3;
        }
        else if (charge < 50)
        {
            charge = 5;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 4;
        }

        return (int)charge;
    }
    public int BanStarCheck(float charge)
    {
        if (charge <= 10)
        {
            charge = 1;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 0;
        }
        else if (charge <= 20)
        {
            charge = 2;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 1;
        }
        else if (charge <= 30)
        {
            charge = 3;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 2;
        }
        else if (charge <= 40)
        {
            charge = 4;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 3;
        }
        else if (charge <= 50)
        {
            charge = 5;
            Singleton.Instance.gameSceneController.starChargeController.StarCount = 4;
        }

        return (int)charge;
    }
}
