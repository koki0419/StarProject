using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarEffect : MonoBehaviour
{
    private RectTransform targetObj;
    private Transform playerObj;
    private float moveSpeed = 10.0f;
    private Vector3 force;

    public void Initialize(RectTransform target,Transform player)
    {
        targetObj = target;
        playerObj = player;
        force = ForceAngle();
    }

    private void Update()
    {
        var moveforce = force * moveSpeed;
        transform.localPosition += moveforce;
        if(transform.localPosition == targetObj.transform.localPosition)
        {
            Debug.Log("来たよー");
            gameObject.SetActive(false);
        }
    }

    //移動方向を計算
    Vector3 ForceAngle()
    {
        //プレイヤーポジション取得
        var targetObjPos = targetObj.transform.position;
        //自分の座標をプレイヤーの座標からベクトル作成
        Vector3 vec = targetObjPos - playerObj.transform.position;
        //単位ベクトル作成（上記のベクトル）
        Vector3 vecE = vec.normalized;
        vecE.z = 0;

       // Debug.Log("enemyVecE : " + vecE);
        return vecE;
    }
}
