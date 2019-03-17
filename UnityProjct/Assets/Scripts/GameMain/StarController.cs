using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{

    //---------Unityコンポーネント宣言--------------
    GameObject playerObj;

    //------------クラスの宣言----------------------
    PlayerMove playerMove;
    //------------数値変数の宣言--------------------
    float moveSpeed = 2.0f;

    [SerializeField]
    int starPoint = 50;
    //------------フラグ変数の宣言------------------

    // Start is called before the first frame update
    public void Init(GameObject player, PlayerMove playermove)
    {
        playerObj = player;
        playerMove = playermove;
    }

    // Update is called once per frame
    //void Update()
    //{

    //    //var playerPos = playerObj.position;
    //    // 自分の座標とプレイヤーのベクトル作成(プレイヤー座標からEnemy座標を引く)
    //    Vector3 moveVec = playerObj.transform.position - gameObject.transform.localPosition; // (オペレータ機能を利用している)

    //    // 単位ベクトルの作成（上記のベクトル）
    //    Vector3 V2 = moveVec.normalized;


    //    // 近づくスピード
    //    // -1をかけると遠ざかる

    //    moveVec = V2 * moveSpeed * Time.deltaTime;

    //    gameObject.transform.localPosition += moveVec;

    //}

    void AdditionStarPoint(int point)
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            //Debug.Log("chargePoint" + Singleton.Instance.gameSceneController.ChargePoint);
            //Debug.Log("StarChildCount" + Singleton.Instance.gameSceneController.StarChildCount);
            if (starPoint == 1)
            {
                if (!playerMove.DestroyModeFlag && Singleton.Instance.gameSceneController.chargePointManager.ChargePoint < Singleton.Instance.gameSceneController.chargePointManager.ChargePointMax)
                {
                    Singleton.Instance.gameSceneController.chargePointManager.ChargePoint += starPoint;
                    Singleton.Instance.gameSceneController.chargePointManager.StarChildCount += starPoint;
                    playerMove.GetStar = true;
                }
            }
            else
            {
                Singleton.Instance.gameSceneController.chargePointManager.StarChildCountSkip += starPoint;
            }
            Destroy(this.gameObject);
        }
    }
}
