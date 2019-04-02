using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarChargeController : MonoBehaviour
{
    //大きい☆使用状態
    public enum Star
    {
        None,
        DontUse = 1001,
        Use = 1010,
        Used = 1011,
        Chage = 1100,//チャージ中（大きい☆用）
        NotAvailable = 9999,//使用禁止（大きい☆用）
    }

    public Star star = Star.None;

    [Header("チャージ用☆UI")]
    //大きい☆UIを取得します →5個
    [SerializeField] StarState[] starChargeUI = null;
    //小さい☆UIを取得します →9個
    [SerializeField] StarState[] starChildrenUI = null;


    //☆獲得ポイント
    [SerializeField]private Image chargeFill;
    //小さい☆UIが10個溜まったフラグ
    bool starChargeMaxFlag = false;
    //現在の大きい☆の数
    [SerializeField]int starCount = 0;
    public int StarCount
    {
        set { starCount = value; }
        get { return starCount; }
    }

    public void Init()
    {
        starChargeMaxFlag = false;
        starCount = 0;

        for (int i = 0; i < starChargeUI.Length; i++)
        {
            starChargeUI[i].UpdateStarSprite((int)Star.DontUse);
        }
        for (int i = 0; i < starChildrenUI.Length; i++)
        {
            starChildrenUI[i].UpdateStarSprite((int)Star.DontUse);
        }
    }

    //大きい☆UIの更新
    public void UpdateStarUI(int starNum)
    {
        for (int i = 0; i < starNum; i++)
        {
            starChargeUI[i].UpdateStarSprite((int)Star.Use);
        }
    }
    //大きい☆UIの更新
    public void ReMoveStarUI(int starNum)
    {
            starChargeUI[starNum-1].UpdateStarSprite((int)Star.DontUse);

    }

    //小さい☆UIの更新
    public void UpdateChildrenUI(int starNum)
    {

        if (starNum == 10) starChargeMaxFlag = true;
        if (!starChargeMaxFlag)
        {
            for (int i = 0; i < starNum % 10; i++)
            {
                starChildrenUI[i].UpdateStarSprite((int)Star.Use);
            }
        }
        else
        {
            for (int i = 0; i < starNum - 1; i++)
            {
                starChildrenUI[i].UpdateStarSprite((int)Star.DontUse);
            }
            starChargeMaxFlag = false;

            UpdateStarUI(++starCount);
        }

    }


    //チャージ時
    public void ChargeStar(int starNum)
    {
        for (int i = 0; i < starNum; i++)
        {
            starChargeUI[i].UpdateStarSprite((int)Star.Chage);
        }
    }

    //☆使用禁止
    public void BanStar(int starNum)
    {
        Debug.Log("starNum" + starNum);
        starChargeUI[starNum-1].UpdateStarSprite((int)Star.NotAvailable);
    }

    //チャージポイントのupdete
    public void UpdateChargePoint(float percentage)
    {
        chargeFill.fillAmount = percentage;
    }
}
