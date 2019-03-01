using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{
    //---------Unityコンポーネント宣言--------------
    [SerializeField] GameObject gameOvreUI;
    [SerializeField] GameObject gameClearUI;
    [SerializeField] GameObject playerObj;

    [SerializeField] GameObject starObj;

    [SerializeField] GameObject enemyObj;

    //☆子供オブジェクト取得用
    GameObject[] starChildrenOBJ;
    GameObject[] enemyChildrenOBJ;
    //------------クラスの宣言----------------------
    [SerializeField] PlayerMove playerMove;
    public PlayerMove PlayerMove
    {
        get { return playerMove; }
    }
    [SerializeField] BreakBoxController[] breakBoxController;

    [SerializeField] Boss[] boss;

    [SerializeField] StarController[] starControllers;


    [SerializeField] EnemyController[] enemyController;

    public ChargeUIController chargeUIController;

    [SerializeField] FadeLayer fadeLayer;
    [SerializeField] GameObject fadeText;
    [SerializeField] GameObject fadeChara;

    //------------数値変数の宣言--------------------
    [SerializeField]
    float chargePoint = 0;
    public float ChargePoint
    {
        get { return chargePoint; }
        set { chargePoint = value; }
    }

    [SerializeField] float chargePointMax = 300;
    public float ChargePointMax
    {
        get { return chargePointMax; }
        set { chargePointMax = value; }
    }

    //プレイヤーHP
    [SerializeField] float playerHp;

    float playerHpMax;

    //HPの減少時間
    [SerializeField] float hpDownTime = 1;
    //HPの回復量
    float hpRecovery = 2;

    //Hpゲージの数
    int hpGageNum = 5;

    //*****************************
    //デバックポイント
    [SerializeField] int debugPoint;

    //ゲージダウン割合
    float gaugeDroportion;
    //------------フラグ変数の宣言------------------
    bool isPlaying = false;

    [SerializeField]
    bool gameClear = false;
    public bool GameClear
    {
        get { return gameClear; }
        set { gameClear = value; }
    }

    //初期化
    public void Init()
    {
        //プレー中かどうか
        isPlaying = false;
        //チャージポイント
        chargePoint = 0;
        chargePoint = debugPoint;

        gaugeDroportion = (float)PlayerMove.PlayerBeastModeState.StarCost/100;//StarCostを『0.01』にする

        playerHpMax = playerHp;
        for (int i = 0; i < breakBoxController.Length; i++)
        {
            breakBoxController[i].Init();
        }

        for (int i = 0; i < boss.Length; i++)
        {
            boss[i].Init();
        }

        //☆子供オブジェクトを取得
        //starControllers
        starChildrenOBJ = new GameObject[starObj.transform.childCount];
        starControllers = new StarController[starObj.transform.childCount];
        //☆子供オブジェクト取得
        for (int i = 0; starObj.transform.childCount > i; i++)
        {
            starChildrenOBJ[i] = starObj.transform.GetChild(i).gameObject;
            starControllers[i] = starChildrenOBJ[i].GetComponent<StarController>();
            starControllers[i].Init(playerObj, playerMove);
        }

        //エネミー子供オブジェクト取得
        enemyChildrenOBJ = new GameObject[enemyObj.transform.childCount];
        enemyController = new EnemyController[enemyObj.transform.childCount];
        //エネミー子供オブジェクト取得
        for (int i = 0; enemyObj.transform.childCount > i; i++)
        {
            enemyChildrenOBJ[i] = enemyObj.transform.GetChild(i).gameObject;
            enemyController[i] = enemyChildrenOBJ[i].GetComponent<EnemyController>();
            enemyController[i].Init(playerObj);
        }
        chargeUIController.UpdateChargePoint(chargePoint / chargePointMax);

        //初期化
        gameOvreUI.SetActive(false);
        gameClearUI.SetActive(false);
        gameClear = false;

    }

    private void Awake()
    {
        fadeLayer.ForceColor(Color.black);
        fadeText.SetActive(true);
        fadeChara.SetActive(true);
    }

    IEnumerator Start()
    {
        isPlaying = false;

        yield return null;
        Init();
        playerMove.Init();
        yield return null;
        fadeText.SetActive(false);
        fadeChara.SetActive(false);
        yield return null;

        yield return fadeLayer.FadeInEnumerator(2);
        isPlaying = true;

    }

    // Start is called before the first frame update
    //void Start()
    //{


    //}

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            playerMove.OnUpdate();

            if (chargePoint <= 0)
            {
                chargePoint = 0;
            }
            else if (chargePoint >= chargePointMax)
            {
                chargePoint = chargePointMax;
            }
            chargeUIController.UpdateChargePoint(chargePoint / chargePointMax);

            //ダメージを受ける
            if (playerMove.BeastModeFlag){
                HpDamage(hpDownTime * (int)PlayerMove.PlayerBeastModeState.PhysicalFitnessCost);
            }else{
                HpDamage(hpDownTime);
            }
            //HPを回復します
            if (playerMove.BeastModeFlag)
            {
                if (chargePoint > 0)
                {
                    //HPを回復します
                    //HpRecovery(hpRecovery);
                    chargePoint -= gaugeDroportion;
                    if (chargePoint <= 0)
                    {
                        chargePoint = 0;
                        playerMove.BeastModeFlag = false;
                        playerMove.BeastModeEffect.SetActive(playerMove.BeastModeFlag);
                    }
                    //playerMove.HpRecoveryFlag = true;
                }
            }

            //ゲームオーバー
            if (playerHp <= 0)
            {
                isPlaying = false;
                gameOvreUI.SetActive(true);
            }

            //ゲームクリア
            if (gameClear)
            {
                isPlaying = false;
                gameClearUI.SetActive(true);
            }
        }
    }

    //ダメージを受けます
    void HpDamage(float damage)
    {

        if (playerHp > 0)
        {
            playerHp -= damage;
        }
        else
        {
            playerHp = 0;
        }

        //chargeUIController.UpdateHppoint(updateHPs[hpNum] / 100, hpNum);
        chargeUIController.UpdateHppoint(playerHp/playerHpMax);
    }

    //HPを回復します
    void HpRecovery(float recovery)
    {
        if (playerHp < playerHpMax)
        {
            playerHp += recovery;
        }
        else
        {
            playerHp = playerHpMax;
        }
        chargeUIController.UpdateHppoint(playerHp / playerHpMax);
    }


    //private void OnPawnTakeDamage(Pawn pawn, float damage)
    //{
    //    DamageText dmgText = GameObject.Instantiate<DamageText>(damageText);
    //    dmgText.SetText((int)damage);
    //    dmgText.transform.position = pawn.transform.position;
    //    GameObject.Destroy(dmgText.gameObject, damageTextLifeTime);

    //    if (pawn == Singleton.instance.playerPawn)
    //    {
    //        cameraEffect.ShakeCamera(shakeCameraStrength, shakeCameraPeriod);
    //    }
    //    else
    //    {
    //        cameraEffect.ShakeCamera(shakeCameraStrength / 2, shakeCameraPeriod / 2);
    //    }
    //}
}
