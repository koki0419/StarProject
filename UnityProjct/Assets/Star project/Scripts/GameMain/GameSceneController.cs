using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    //---------Unityコンポーネント宣言--------------

    [SerializeField] GameObject playerObj = null;

    [SerializeField] GameObject starObj = null;

    [SerializeField] GameObject enemyObj = null;

    //☆子供オブジェクト取得用
    GameObject[] starChildrenOBJ;
    GameObject[] enemyChildrenOBJ;
    //------------クラスの宣言----------------------
    public PlayerMove playerMove;

    [SerializeField] BreakBoxController[] breakBoxController = null;

    [SerializeField] Boss[] boss = null;

    [SerializeField] StarController[] starControllers;

    CameraController cameraController;

    [SerializeField] EnemyController[] enemyController;

    public ChargeUIController chargeUIController;

    public StarChargeController starChargeController;


    // 変数を直接参照させないでプロパティ文法でアクセサを経由させる
    public ChargePointManager ChargePointManager
    {
        get { return chargePointManager; }
        //private set { chargePointManager = value; }
    }
    // 変数本体でInspectorにはこちらが表示される
    [SerializeField] ChargePointManager chargePointManager = null;


    [SerializeField] UiManager uiManager = null;

    //------------数値変数の宣言--------------------


    //*****************************
    //デバックポイント
    //[SerializeField] int debugPoint;


    //------------フラグ変数の宣言------------------
    bool isPlaying = false;

    bool isGameClear = false;
    public bool IsGameClear
    {
        set { isGameClear = value; }
    }

    bool isGameOver = false;
    public bool IsGameOver
    {
        set { isGameOver = value; }
    }

    bool cnaChangeScene;

    bool canCameraShake;

    //初期化
    public void Init()
    {
        //プレー中かどうか
        isPlaying = false;
        //gaugeDroportion = (float)PlayerMove.PlayerBeastModeState.StarCost / 100;//StarCostを『0.01』にする
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
        starChargeController.UpdateChildrenUI(0);


        isGameClear = false;
        isGameOver = false;

        cnaChangeScene = false;

        canCameraShake = false;

    }

    //Start()より早く処理する
    private void Awake()
    {
        uiManager.Init();
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
        chargePointManager.Init();
        yield return null;
        uiManager.FadeImageDisplay(false);
        yield return null;

        yield return uiManager.FadeInEnumerator(2);

        isPlaying = true;

    }


    // Update is called once per frame
    void Update()
    {
        if (isPlaying)//ゲームスタート
        {
            float deltaTime = Time.deltaTime;

            playerMove.OnUpdate(deltaTime);//PlayerのUpdate



            //ゲームオーバー
            if (isGameOver)
            {
                StartCoroutine(OnGameOver());
            }
        }
        //ゲームクリア
        if (isGameClear)
        {
            StartCoroutine(OnClear());
        }
        if (cnaChangeScene)
        {
            uiManager.ButtonSelectUpdate();
        }
        if (canCameraShake)
        {
            cameraController.Shake(0.25f, 0.1f);
        }
    }

    private void LateUpdate()
    {
        if (isPlaying)//ゲームスタート
        {
            cameraController.MoveUpdate();
            chargePointManager.OnUpdate();
        }
    }





    //スタート
    IEnumerator OnClear()
    {
        isPlaying = false;
        canCameraShake = true;
        yield return new WaitForSeconds(0.5f);
        uiManager.GameClearUIDisplay(true);
        yield return new WaitForSeconds(3.0f);
        canCameraShake = false;
        SceneManager.LoadScene("ResultScene");
        /*uiManager.ResultUIBGUIDisplay(true);
        cnaChangeScene = true;
        isGameClear = false;
        */

    }

    //スタート
    IEnumerator OnGameOver()
    {
        isPlaying = false;
        yield return new WaitForSeconds(0.5f);
        uiManager.GameOvreUIDisplay(true);
        yield return null;
        yield return uiManager.FadeOutEnumerator();

        SceneManager.LoadScene("SelectScene");
    }

}
