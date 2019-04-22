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
    [SerializeField]int starPoint;
    //------------フラグ変数の宣言------------------

    // Start is called before the first frame update
    public void Init(GameObject player, PlayerMove playermove)
    {
        playerObj = player;
        playerMove = playermove;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            Singleton.Instance.gameSceneController.isGetStar = true;
            if (starPoint == 0 && starPoint == 1)
            {
                playerMove.IsAcquisitionStar = true;
                Singleton.Instance.gameSceneController.ChargePointManager.starChildCount += starPoint;
            }
            else
            {
                Singleton.Instance.gameSceneController.ChargePointManager.starChildCountSkip += starPoint;
                Singleton.Instance.gameSceneController.ChargePointManager.isSkipStar = true;
            }
            Destroy(this.gameObject);
        }
    }
}
