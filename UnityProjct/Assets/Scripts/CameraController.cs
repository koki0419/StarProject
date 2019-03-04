using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    //プレイヤーのTransformを取得
    [SerializeField] Transform player;
    [SerializeField] float camaraPos = 1.5f;
    [Header("カメラ移動速度")]
    [SerializeField] float camaraMove = 1.5f;

    public void Init()
    {
        var position = transform.position;
        position.x = player.position.x;

    }

    // Update is called once per frame
    public void OnUpdate()
    {
            var position = transform.position;
        //position.x = player.position.x;
        position.x += camaraMove;
            //position.y = player.position.y + camaraPos;
            position.y =  camaraPos;
            transform.position = position;

    }
}
