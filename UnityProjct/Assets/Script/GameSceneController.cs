using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    //---------Unityコンポーネント宣言--------------
    //------------クラスの宣言----------------------
    [SerializeField]
    PlayerMove playerMove;
    [SerializeField]
    BreakBoxController[] breakBoxController;

    public ChargeUIController chargeUIController;


    //------------数値変数の宣言--------------------
    [SerializeField]
    int chargePoint = 0;
    public int ChargePoint
    {
        get { return chargePoint; }
        set { chargePoint = value; }
    }

    //------------フラグ変数の宣言------------------
    bool isPlaying = false;

    //初期化
    public void Init()
    {
        //プレー中かどうか
        isPlaying = false;
        //チャージポイント
        chargePoint = 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        Init();
        playerMove.Init();
        for (int i = 0; i < breakBoxController.Length; i++)
        {
            breakBoxController[i].Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (chargePoint <= 0)
        {
            chargePoint = 0;
        }else if (chargePoint >= 100)
        {
            chargePoint = 100;
        }
        chargeUIController.UpdateChargePoint((float)chargePoint/100.0f);
    }

    
}
