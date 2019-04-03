using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarChargeController : MonoBehaviour
{
    //大きい☆使用状態
    public enum Star
    {
        None = 0, //☆取得していないとき
        Normal = 1010,//☆獲得状態
        Chage = 1100,//チャージ中（大きい☆用）
    }
    public Star star = Star.None;

    [Header("チャージ用☆UI")]
    //大きい☆UIを取得します →5個
    [SerializeField] StarState[] starChargeUI = null;
    //小さい☆UIを取得します →9個
    [SerializeField] StarState[] starChildrenUI = null;


    //☆獲得ポイント
    [SerializeField] private Image chargeFill;
    //小さい☆UIが10個溜まったフラグ
    bool starChargeMaxFlag = false;
    //現在の大きい☆の数
    [SerializeField] int bigStarCount = 0;
    public int StarCount
    {
        set { bigStarCount = value; }
        get { return bigStarCount; }
    }

    public void Init()
    {
        starChargeMaxFlag = false;
        bigStarCount = 0;

        for (int i = 0; i < starChargeUI.Length; i++)
        {
            starChargeUI[i].UpdateStarSprite((int)Star.None);
        }
        for (int i = 0; i < starChildrenUI.Length; i++)
        {
            starChildrenUI[i].UpdateStarSprite((int)Star.None);
        }
    }

    //大きい☆UIの更新
    public void UpdateBigStarUI(int starNum)
    {
        for (int i = 0; i < starNum; i++)
        {
            starChargeUI[i].UpdateStarSprite((int)Star.Normal);
        }
    }
    //大きい☆UIの更新
    public void ReMoveBigStarUI(int starNum)
    {
        starChargeUI[starNum - 1].UpdateStarSprite((int)Star.None);
    }

    //小さい☆UIの更新
    /// <summary>
    /// 小さい☆を光らせます
    /// 1～10のstarNumを使用します
    /// </summary>
    /// <param name="starNum"></param>
    public void UpdateChildrenStarUI(int starNum)
    {
        //星が10こかどうか判断して10個なら大きい☆を光らせる
        //10個以下なら小さい星を光らせる
        if (starNum == 10) starChargeMaxFlag = true;
        if (!starChargeMaxFlag)
        {
            for (int i = 0; i < starNum; i++)
            {
                starChildrenUI[i].UpdateStarSprite((int)Star.Normal);
            }
        }
        else
        {
            //9個の小さい☆を獲得していない状態に戻します
            for (int i = 0; i < starNum - 1; i++)
            {
                starChildrenUI[i].UpdateStarSprite((int)Star.None);
            }
            starChargeMaxFlag = false;
            if (bigStarCount < 5)
            {
                UpdateBigStarUI(++bigStarCount);
            }
        }

    }


    //チャージ時
    public void ChargeGigStar(int starNum)
    {
        for (int i = 0; i < starNum; i++)
        {
            starChargeUI[i].UpdateStarSprite((int)Star.Chage);
        }
    }

    //チャージポイントのupdete
    public void UpdateChargePoint(float percentage)
    {
        chargeFill.fillAmount = percentage;
    }
}
