using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    CameraController cameraController;

    [SerializeField] EnemyController[] enemyController;

    public ChargeUIController chargeUIController;

    public StarChargeController starChargeController;

    [SerializeField] FadeLayer fadeLayer;
    [SerializeField] GameObject fadeText;
    [SerializeField] GameObject fadeChara;

    //------------数値変数の宣言--------------------
    //☆チャージポイント
    [SerializeField] float chargePoint = 0;
    public float ChargePoint
    {
        get { return chargePoint; }
        set { chargePoint = value; }
    }
    //☆チャージポイント最大値+1で入力
    [SerializeField] float chargePointMax = 51;
    public float ChargePointMax
    {
        get { return chargePointMax; }
        set { chargePointMax = value; }
    }


    //小さい☆の獲得状況
    [SerializeField] int starChildCount = 0;
    public float StarChildCount
    {
        get { return starChildCount; }
        set { starChildCount = (int)value; }
    }
    //小さい☆の獲得状況スキップ
    [SerializeField] int starChildCountSkip = 0;
    public float StarChildCountSkip
    {
        get { return starChildCountSkip; }
        set { starChildCountSkip = (int)value; }
    }

    [SerializeField] float destroyCount = 0;
    public float DestroyCount
    {
        get { return destroyCount; }
        set { destroyCount = (int)value; }
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
    //[SerializeField] int debugPoint;

    //ゲージダウン割合
    float gaugeDroportion;
    //------------フラグ変数の宣言------------------
    bool isPlaying = false;

    [SerializeField] bool gameClear = false;
    public bool GameClear
    {
        get { return gameClear; }
        set { gameClear = value; }
    }

    [SerializeField] bool gameOver = false;
    public bool GameOver
    {
        get { return gameOver; }
        set { gameOver = value; }
    }

    //初期化
    public void Init()
    {
        //プレー中かどうか
        isPlaying = false;
        //チャージポイント
        chargePoint = 0;

        gaugeDroportion = (float)PlayerMove.PlayerBeastModeState.StarCost / 100;//StarCostを『0.01』にする

        playerHpMax = playerHp;
        for (int i = 0; i < breakBoxController.Length; i++)
        {
            breakBoxController[i].Init();
        }

        for (int i = 0; i < boss.Length; i++)
        {
            boss[i].Init();
        }

        cameraController = Singleton.Instance.cameraController;

        //☆子供オブジェクトを取得
        starChildrenOBJ = new GameObject[starObj.transform.childCount];
        starControllers = new StarController[starObj.transform.childCount];
        //☆子供オブジェクト初期化
        for (int i = 0; starObj.transform.childCount > i; i++)
        {
            starChildrenOBJ[i] = starObj.transform.GetChild(i).gameObject;
            starControllers[i] = starChildrenOBJ[i].GetComponent<StarController>();
            starControllers[i].Init(playerObj, playerMove);
        }

        //エネミー子供オブジェクト取得
        enemyChildrenOBJ = new GameObject[enemyObj.transform.childCount];
        enemyController = new EnemyController[enemyObj.transform.childCount];
        //エネミー子供オブジェクト初期化
        for (int i = 0; enemyObj.transform.childCount > i; i++)
        {
            enemyChildrenOBJ[i] = enemyObj.transform.GetChild(i).gameObject;
            enemyController[i] = enemyChildrenOBJ[i].GetComponent<EnemyController>();
            enemyController[i].Init(playerObj);
        }

        //chargeUIController.UpdateChargePoint(chargePoint / chargePointMax);
        //☆画像の初期化
        starChargeController.UpdateChildrenUI(starChildCount);
        starChargeController.UpdateDestroyPoint(0);

        //初期化
        gameOvreUI.SetActive(false);
        gameClearUI.SetActive(false);
        gameClear = false;
        gameOver = false;

    }

    private void Awake()
    {
        fadeLayer.ForceColor(Color.black);
        fadeText.SetActive(true);
        fadeChara.SetActive(true);
    }

    //スタート
    IEnumerator Start()
    {
        isPlaying = false;

        yield return null;
        Init();
        starChargeController.Init();
        playerMove.Init();
        cameraController.Init();
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
        if (isPlaying)//ゲームスタート
        {
            float deltaTime = Time.deltaTime;

            playerMove.OnUpdate(deltaTime);//PlayerのUpdate

            cameraController.OnUpdate();
            //チャージポイント
            if (chargePoint <= 0)
            {
                chargePoint = 0;
            }
            else if (chargePoint >= chargePointMax)
            {
                chargePoint = chargePointMax;
            }
            if ((starChildCountSkip + chargePoint) >= chargePointMax)
            {
                starChildCountSkip = (int)chargePointMax - (int)chargePoint;
                chargePoint += starChildCountSkip;
            }

            if (starChildCountSkip != 0)
            {
                for (int i = 0; i < starChildCountSkip; i++)
                {
                    starChildCount++;
                    starChargeController.UpdateChildrenUI(starChildCount);
                    //☆獲得10個になったら初期化
                    if (starChildCount == 10)
                    {
                        starChildCount = 0;
                    }
                }
            }
            //chargeUIController.UpdateChargePoint(chargePoint / chargePointMax);
            if (chargePoint <= chargePointMax)
            {
                starChargeController.UpdateChildrenUI(starChildCount);
                //☆獲得10個になったら初期化
                if (starChildCount == 10)
                {
                    starChildCount = 0;
                }
            }
            //ダメージを受ける//デストロイモード
            if (playerMove.DestroyModeFlag)
            {
                 HpDamage(hpDownTime * (int)PlayerMove.PlayerBeastModeState.PhysicalFitnessCost);
            }
            else
            {
                  HpDamage(hpDownTime);
            }

            if (playerMove.DestroyModeFlag)
            {
                if (chargePoint > 0)
                {
                    //HPを回復します
                    //HpRecovery(hpRecovery);
                    chargePoint -= gaugeDroportion;

                    if (destroyCount >= 0)
                    {
                        destroyCount -= gaugeDroportion;
                    }
                    else
                    {
                        
                        destroyCount = 10;
                    }
                    starChargeController.ReMoveStarUI(DestroyMode(chargePoint));
                    starChargeController.UpdateDestroyPoint(destroyCount / 10);


                    if (chargePoint <= 0)
                    {
                        chargePoint = 0;
                        playerMove.DestroyModeFlag = false;
                        if (chargePointMax >= 10)
                        {
                            chargePointMax -= 10;
                        }
                        playerMove.BeastModeEffect.SetActive(playerMove.DestroyModeFlag);
                        starChargeController.BanStar(BanStarCheck(chargePointMax));
                    }
                    //playerMove.HpRecoveryFlag = true;
                }
            }

            //ゲームオーバー
            if (gameOver)
            {
                StartCoroutine(OnGameOver());
            }

            //ゲームクリア
            if (gameClear)
            {
               StartCoroutine(OnClear());
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
        chargeUIController.UpdateHppoint(playerHp / playerHpMax);
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

    //☆使用後状態を確認します
    public int DestroyMode(float charge)
    {
        if (charge < 10)
        {
            charge = 1;
            starChargeController.StarCount = 0;
        }
        else if (charge < 20)
        {
            charge = 2;
            starChargeController.StarCount = 1;
        }
        else if (charge < 30)
        {
            charge = 3;
            starChargeController.StarCount = 2;
        }
        else if (charge < 40)
        {
            charge = 4;
            starChargeController.StarCount = 3;
        }
        else if (charge < 50)
        {
            charge = 5;
            starChargeController.StarCount = 4;
        }

        return (int)charge;
    }
    public int BanStarCheck(float charge)
    {
        if (charge <= 10)
        {
            charge = 1;
            starChargeController.StarCount = 0;
        }
        else if (charge <= 20)
        {
            charge = 2;
            starChargeController.StarCount = 1;
        }
        else if (charge <= 30)
        {
            charge = 3;
            starChargeController.StarCount = 2;
        }
        else if (charge <= 40)
        {
            charge = 4;
            starChargeController.StarCount = 3;
        }
        else if (charge <= 50)
        {
            charge = 5;
            starChargeController.StarCount = 4;
        }

        return (int)charge;
    }



    //スタート
    IEnumerator OnClear()
    {
        isPlaying = false;

        yield return new WaitForSeconds(0.5f);
        gameClearUI.SetActive(true);
        yield return null;

        yield return fadeLayer.FadeOutEnumerator(Color.black, 2);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("TitleScene");
    }
    
    //スタート
    IEnumerator OnGameOver()
    {
        isPlaying = false;

        yield return new WaitForSeconds(0.5f);
        gameOvreUI.SetActive(true);
        yield return null;

        yield return fadeLayer.FadeOutEnumerator(Color.black, 2);

        SceneManager.LoadScene("TitleScene");
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
