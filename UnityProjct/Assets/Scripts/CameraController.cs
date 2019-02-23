using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    //プレイヤーのTransformを取得
    [SerializeField]
    Transform player;
    [SerializeField]
    float camaraPos = 1.5f;

    void Init()
    {


    }

    // Update is called once per frame
    void Update()
    {
            var position = transform.position;
            position.x = player.position.x;
            position.y = player.position.y + camaraPos;
            transform.position = position;

    }
}
