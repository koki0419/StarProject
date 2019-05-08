using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class StarEffecSpon : MonoBehaviour
{
    [SerializeField] private GameObject starGetEffect_sPrefab = null;
    private const int effectMax = 10;
    private ObjectPool pool;

    [SerializeField] private RectTransform targetObj = null;
    [SerializeField] private Transform player = null;

    //private void Awake()
    //{
    //    pool = GetComponent<ObjectPool>();
    //    pool.CreatePool(starGetEffect_sPrefab, effectMax);
    //}

    //public void CreatStarEffect()
    //{
    //    var effect = pool.GetObject();
    //    if(effect != null)
    //    {
    //        //プレイヤーの位置座標をスクリーン座標に変換
    //        var pos2D = RectTransformUtility.WorldToScreenPoint(null, player.position);
    //        effect.transform.localPosition = pos2D;
    //        effect.GetComponent<StarEffect>().Initialize(targetObj,player);

    //        Debug.Log("player : " + player.position);
    //        Debug.Log("pos2D : " + pos2D);
    //    }

    //}
}
