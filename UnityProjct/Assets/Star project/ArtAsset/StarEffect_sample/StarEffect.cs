using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarEffect : MonoBehaviour
{

    [SerializeField] private RectTransform target;
    private float moveSpeed = 30;
    private float errorPosition = 5.0f;

    public void Init(RectTransform t)
    {
        target = t;
    }
    
    private Vector3 Direction()
    {
        //自分の座標をプレイヤーの座標からベクトル作成
        Vector3 targetVec = target.localPosition - gameObject.transform.localPosition;
        //単位ベクトル作成（上記のベクトル）
        Vector3 targetVecE = targetVec.normalized;
        //長さを調節
        targetVecE.z = 0;

        var moveForce = targetVecE * moveSpeed;

        return moveForce;
    }

    private void Update()
    {
        gameObject.transform.localPosition += Direction();
        Debug.Log(Direction());
        if(target.localPosition.x- errorPosition <= gameObject.transform.localPosition.x && target.localPosition.y- errorPosition <= gameObject.transform.localPosition.y)
        {
            gameObject.SetActive(false);
        }
    }
}
