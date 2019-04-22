using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChargePointManager
{
    //小さい☆の獲得状況
    public int starChildCount
    {
        get;set;
    }
    float starChildCountMax = 50;
    public float StarChildCountMax
    {
        get { return starChildCountMax; }
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
        starChildCount = 0;
        starChildCountSkip = 0;
        isSkipStar = false;
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        //一気に大量の☆を獲得したとき☆獲得数が現在の獲得数と足したときに最大獲得数を超えないか確認
        //越えなければ、現在の獲得数にプラスする
        if (starChildCount < starChildCountMax)
        {
            if (isSkipStar)
            {
                isSkipStar = false;
                for (int i = starChildCountSkip; i > 0; i--)
                {
                    starChildCount++;
                    starChildCountSkip--;
                    Singleton.Instance.gameSceneController.starChargeController.UpdateDisplayAcquisitionSpriteStar(starChildCount);
                    if (starChildCount >= starChildCountMax)
                    {
                        starChildCountSkip = 0;
                        break;
                    }
                }
            }
            else
            {
                Singleton.Instance.gameSceneController.starChargeController.UpdateDisplayAcquisitionSpriteStar(starChildCount);
            }
        }
    }

}
