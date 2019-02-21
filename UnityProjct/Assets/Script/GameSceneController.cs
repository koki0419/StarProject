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
    float chargePoint = 0;
    public float ChargePoint
    {
        get { return chargePoint; }
        set { chargePoint = value; }
    }


    //プレイヤーHP
    [SerializeField]
    int playerHp =100;

    int playerHpMax;

    //HPの減少時間
    [SerializeField]
    float hpDownTime = 1;
    //HPの回復量
    float hpRecovery = 2;

    //Hpゲージの数
    int hpGageNum = 5;

    //float[] updateHPs = new float[5];

    float updateHP;

    //ゲージダウン割合
    [SerializeField]
    float gaugeDroportion = 3;

    //------------フラグ変数の宣言------------------
    bool isPlaying = false;

    //初期化
    public void Init()
    {
        //プレー中かどうか
        isPlaying = false;
        //チャージポイント
        chargePoint = 0;

        playerHpMax = playerHp;
        updateHP = playerHp;
        //for (int i = 0;i< updateHPs.Length; i++)
        //{
        //    updateHPs[i] = hp;
        //}

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
        chargeUIController.UpdateChargePoint(chargePoint / 100.0f);

        //ダメージを受ける
        HpDamage(hpDownTime);

        if (Input.GetKey(KeyCode.H))
        {
            if (chargePoint > 0)
            {
                //HPを回復します
                HpRecovery(hpRecovery);
                chargePoint -= gaugeDroportion;
                if (chargePoint <= 0)
                {
                    chargePoint = 0;
                }
            }
        }
        
    }

    void HpDamage( float damage)
    {
        //int hpNum = -1;
        //if (updateHPs[0] > 0)
        //{
        //    if (updateHPs[4] > 0)
        //    {
        //        updateHPs[4] -= damage;
        //        hpNum = 4;
        //    }
        //    else if (updateHPs[3] > 0)
        //    {
        //        updateHPs[3] -= damage;
        //        hpNum = 3;
        //    }
        //    else if (updateHPs[2] > 0)
        //    {
        //        updateHPs[2] -= damage;
        //        hpNum = 2;
        //    }
        //    else if (updateHPs[1] > 0)
        //    {
        //        updateHPs[1] -= damage;
        //        hpNum = 1;
        //    }
        //    else if (updateHPs[0] > 0)
        //    {
        //        updateHPs[0] -= damage;
        //        hpNum = 0;
        //    }
        //}
        //else
        //{
        //    updateHPs[0] = 0; hpNum = 0;
        //}

        if (updateHP > 0)
        {
            updateHP -= damage;
        }
        else
        {
            updateHP = 0;
        }

            //chargeUIController.UpdateHppoint(updateHPs[hpNum] / 100, hpNum);
            chargeUIController.UpdateHppoint(updateHP / 100);
    }

    //HPを回復します
    void HpRecovery(float recovery)
    {
        //int hpNum = -1;
        //if (updateHPs[4] < playerHpMax)
        //{
        //    if (updateHPs[0] < playerHpMax)
        //    {
        //        updateHPs[0] += recovery;
        //        hpNum = 0;
        //    }
        //    else if (updateHPs[1] < playerHpMax)
        //    {
        //        updateHPs[1] += recovery;
        //        hpNum = 1;
        //    }
        //    else if (updateHPs[2] < playerHpMax)
        //    {
        //        updateHPs[2] += recovery;
        //        hpNum = 2;
        //    }
        //    else if (updateHPs[3] < playerHpMax)
        //    {
        //        updateHPs[3] += recovery;
        //        hpNum = 3;
        //    }
        //    else if (updateHPs[4] < playerHpMax)
        //    {
        //        updateHPs[4] += recovery;
        //        hpNum = 4;
        //    }
        //}
        //else
        //{
        //    updateHPs[4] = playerHpMax; hpNum = 4;
        //}

        if (updateHP < playerHpMax)
        {
            updateHP += recovery;
        }
        else
        {
            updateHP = playerHpMax;
        }
        //chargeUIController.UpdateHppoint(updateHPs[hpNum] / 100, hpNum);
        chargeUIController.UpdateHppoint(updateHP / 100);
    }
}
