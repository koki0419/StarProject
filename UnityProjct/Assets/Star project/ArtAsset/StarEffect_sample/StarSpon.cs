using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class StarSpon : MonoBehaviour
{
    //プレイヤーオブジェクト
    [SerializeField] private RectTransform target = null;
    [Header("☆プール生成数")]
    [SerializeField] private int sponMax;
    [Header("☆プレハブ")]
    [SerializeField] private GameObject starEffectPrefab;
    private ObjectPool pool;

    public void Init()
    {
        pool = GetComponent<ObjectPool>();
        pool.CreatePool(starEffectPrefab, sponMax);
    }

    public void CreatStarEffect(Vector3 sponPos)
    {
        //sponPosはワールド座標で取得するのでスクリーン座標に変換
        var screenPos = RectTransformUtility.WorldToScreenPoint(null,sponPos);
        Debug.Log("sponPos : " + sponPos);
        Debug.Log("screenPos : " + screenPos);
        var starEffect = pool.GetObject();
        if (starEffect != null)
        {
            //プレイヤーの位置座標をスクリーン座標に変換
            starEffect.transform.localPosition = screenPos;
            starEffect.GetComponent<StarEffect>().Init(target);
        }
    }
}
