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
    float chargePointMax = 50;
    public float ChargePointMax
    {
        get { return chargePointMax; }
    }


    //小さい☆の獲得状況
    int starChildCount = 0;
    public int StarChildCount
    {
        get { return starChildCount; }
        set { starChildCount = value; }
    }
    //小さい☆の獲得状況スキップ
    public int starChildCountSkip
    {
        set; get;
    }

    [SerializeField] float destroyCount = 0;
    public float DestroyCount
    {
        get { return destroyCount; }
        set { destroyCount = (int)value; }
    }

    //ゲージダウン割合
    float gaugeDroportion;

    //一気に沢山の星を獲得したかどうか
    public bool isSkipStar
    {
        set; private get;
    }

    // Start is called before the first frame update
    public void Init()
    {
        //チャージポイント
        chargePoint = 0;
        starChildCountSkip = 0;
        isSkipStar = false;
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
        //一気に大量の☆を獲得したとき☆獲得数が現在の獲得数と足したときに最大獲得数を超えないか確認
        //越えなければ、現在の獲得数にプラスする
        if (chargePoint < ChargePointMax)
        {
            if (isSkipStar)
            {
                isSkipStar = false;
                chargePoint += starChildCountSkip;
                if (chargePoint >= ChargePointMax)
                {
                    chargePoint = ChargePointMax;
                }

                for (int i = starChildCountSkip; i > 0; i--)
                {
                    starChildCount++;
                    starChildCountSkip--;
                    Singleton.Instance.gameSceneController.starChargeController.UpdateDisplayAcquisitionSpriteStar(starChildCount);

                }
            }
            else
            {
                Singleton.Instance.gameSceneController.starChargeController.UpdateDisplayAcquisitionSpriteStar(starChildCount);
            }
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
