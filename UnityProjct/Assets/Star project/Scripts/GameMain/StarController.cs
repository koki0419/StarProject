using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{

    //---------Unityコンポーネント宣言--------------
    GameObject playerObj;

    //------------クラスの宣言----------------------
    PlayerMove playerMove;
    public StarGenerator starGenerator;
    //------------数値変数の宣言--------------------
    [SerializeField]private int starPoint;
    //------------フラグ変数の宣言------------------

    private const string gameOverLineLayerName = "GameOverObj";

    // Start is called before the first frame update
    public void Init(PlayerMove playermove,int point)
    {
        playerMove = playermove;
        starPoint = point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            Singleton.Instance.gameSceneController.isGetStar = true;
            if (starPoint == 0 && starPoint == 1)
            {
                playerMove.isAcquisitionStar = true;
                Singleton.Instance.gameSceneController.ChargePointManager.starChildCount += starPoint;
            }
            else
            {
                playerMove.isAcquisitionStar = true;
                Singleton.Instance.gameSceneController.ChargePointManager.starChildCountSkip += starPoint;
                Singleton.Instance.gameSceneController.ChargePointManager.isSkipStar = true;
            }
            //Singleton.Instance.starEffecSpon.CreatStarEffect();
            starGenerator.activeCount--;
            gameObject.SetActive(false);
            //Destroy(this.gameObject);
        }else if(LayerMask.LayerToName(other.gameObject.layer) == gameOverLineLayerName)
        {
            starGenerator.activeCount--;
            gameObject.SetActive(false);
        }
    }
}
