using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarState : MonoBehaviour
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

    [Header("☆の状態画像")]
    [SerializeField] Sprite dontUseStarSprite = null;//使っていない
    [SerializeField] Sprite useStarSprite = null;//使っている
    [SerializeField] Sprite usedStarSprite = null;//使い終わった
    [SerializeField] Sprite chageStarSprite = null;//チャージ中（大きい☆用）
    [SerializeField] Sprite notAvailableStarSprite = null;//使用禁止（大きい☆用）

    [SerializeField] Image starImage = null;


    public void UpdateStarSprite(int starState)
    {
        switch (starState)
        {
            case (int)Star.DontUse:
                starImage.sprite = dontUseStarSprite;
                break;
            case (int)Star.Use:
                starImage.sprite = useStarSprite;
                break;
            case (int)Star.Used:
                starImage.sprite = usedStarSprite;
                break;
            case (int)Star.Chage:
                starImage.sprite = chageStarSprite;
                break;
            case (int)Star.NotAvailable:
                starImage.sprite = notAvailableStarSprite;
                break;
        }
    }
}
