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

    //[SerializeField] GameObject enemyObj = null;

    //☆子供オブジェクト取得用
    GameObject[] starChildrenOBJ;
    //GameObject[] enemyChildrenOBJ;
    //------------クラスの宣言----------------------
    public PlayerMove playerMove;

    [SerializeField] ObstacleManager[] obstacleManager = null;

    [SerializeField] Boss[] boss = null;

    [SerializeField] StarController[] starControllers;

    CameraController cameraController;

    [SerializeField] EnemyController[] enemyController;

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

    bool canCameraShake;

    public bool isGetStar
    {
        get; set;
    }

    bool isPause;

    //初期化
    public void Init()
    {
        //プレー中かどうか
        isPlaying = false;
        //gaugeDroportion = (float)PlayerMove.PlayerBeastModeState.StarCost / 100;//StarCostを『0.01』にする
        for (int i = 0; i < obstacleManager.Length; i++)
        {
            obstacleManager[i].Init();
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
        //enemyChildrenOBJ = new GameObject[enemyObj.transform.childCount];
        //enemyController = new EnemyController[enemyObj.transform.childCount];
        ////エネミー子供オブジェクト初期化
        //for (int i = 0; enemyObj.transform.childCount > i; i++)
        //{
        //    enemyChildrenOBJ[i] = enemyObj.transform.GetChild(i).gameObject;
        //    enemyController[i] = enemyChildrenOBJ[i].GetComponent<EnemyController>();
        //    enemyController[i].Init(playerObj);
        //}


        isGetStar = false;
        isGameClear = false;
        isGameOver = false;
        isPause = false;

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
        //ポーズ
        if(isPlaying && !isPause)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Escape))
            {
                isPause = true;
                isPlaying = false;
                uiManager.PauseDiaLogDisplay(isPause);
            }
        }
        else if(!isPlaying && isPause)
        {
            uiManager.ButtonSelectUpdate();
            if (Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Escape))
            {
                isPause = false;
                isPlaying = true;
                uiManager.PauseDiaLogDisplay(isPause);
            }
        }
        //ゲームクリア
        if (isGameClear)
        {
            StartCoroutine(OnClear());
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
            if (isGetStar)
            {
                isGetStar = false;
                chargePointManager.OnUpdate();
            }
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

    //ゲームオーバー
    IEnumerator OnGameOver()
    {
        isPlaying = false;
        yield return new WaitForSeconds(0.5f);
        uiManager.GameOvreUIDisplay(true);
        yield return null;
        yield return uiManager.FadeOutEnumerator();

        SceneManager.LoadScene("TitleScene");
    }

}
