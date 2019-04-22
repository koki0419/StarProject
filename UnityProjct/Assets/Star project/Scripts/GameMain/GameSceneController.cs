using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    public enum GameMainState
    {
        None,
        Play,
        Pause,
        GameClear,
        GameOver,
    }
    public GameMainState gameMainState = GameMainState.None;
    //---------Unityコンポーネント宣言--------------

    [SerializeField] GameObject playerObj = null;

    [SerializeField] GameObject starObj = null;

    [SerializeField] GameObject safeHitGigMoaiObj;

    //☆子供オブジェクト取得用
    GameObject[] starChildrenOBJ;
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
    }
    // 変数本体でInspectorにはこちらが表示される
    [SerializeField] ChargePointManager chargePointManager = null;
    [SerializeField] UiManager uiManager = null;

    //------------数値変数の宣言--------------------
    //現在のステージ番号 // リザルトでリトライやNextステージで使用します
    //タイトルでstageNumを1に設定します。その後はリザルトシーンのみでしか使用しません
    static public int stageNum;
    //------------フラグ変数の宣言------------------
    public bool isGameClear
    {
        set;private get;
    }

    public bool isGameOver
    {
        set;get;
    }
    //カメラを振動出せるかどうか
    bool canCameraShake;

    public bool isGetStar
    {
        get; set;
    }

    public bool isMoveCamera
    {
        get;set;
    }

    bool isOperation;
    //初期化
    public void Init()
    {
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

        isGetStar = false;
        isGameClear = false;
        isGameOver = false;
        isOperation = false;
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
        isMoveCamera = false;
        gameMainState = GameMainState.Play;
        Singleton.Instance.soundManager.PlayBgm("NormalBGM");

    }

    // Update is called once per frame
    void Update()
    {
        switch (gameMainState)
        {
            case GameMainState.Play:
                GamePlay();
                break;
            case GameMainState.Pause:
                GamePause();
                break;
            case GameMainState.GameClear:
                GameClear();
                break;
            case GameMainState.GameOver:
                GameOver();
                break;
        }
        //カメラShake
        if (canCameraShake)
        {
            cameraController.Shake(0.25f, 0.1f);
        }
    }

    private void LateUpdate()
    {
        if (gameMainState == GameMainState.Play)//ゲームスタート
        {
            if(isMoveCamera) cameraController.MoveUpdate();

            if (isGetStar)
            {
                isGetStar = false;
                chargePointManager.OnUpdate();
            }
        }
    }

    //モアイ動きスタート
    IEnumerator BigMoaiMoveStart()
    {
        canCameraShake = true;
        yield return new WaitForSeconds(2.0f);
        canCameraShake = false;
        Destroy(safeHitGigMoaiObj);
        isMoveCamera = true;
    }

    //クリア
    IEnumerator OnClear()
    {
        canCameraShake = true;
        yield return new WaitForSeconds(0.5f);
        uiManager.GameClearUIDisplay(true);
        yield return new WaitForSeconds(3.0f);
        canCameraShake = false;
        SceneManager.LoadScene("ResultScene");

    }

    //ゲームオーバー
    IEnumerator OnGameOver()
    {
        yield return new WaitForSeconds(0.5f);
        uiManager.GameOvreUIDisplay(true);
        yield return new WaitForSeconds(2.5f);
        uiManager.GameOverDiaLogDisplay(true);
        isOperation = true;
    }

    void GamePlay()
    {
        float deltaTime = Time.deltaTime;

        playerMove.OnUpdate(deltaTime);//PlayerのUpdate

        if (obstacleManager[0] != null && obstacleManager[0].isDestroyed)
        {
            StartCoroutine(BigMoaiMoveStart());
        }
        //ゲームオーバー
        if (isGameOver)
        {
            Singleton.Instance.soundManager.PlayJingle("GameOver");
            gameMainState = GameMainState.GameOver;
        }
        //ゲームクリア
        if (isGameClear)
        {
            Singleton.Instance.soundManager.PlayJingle("GameClear");
            gameMainState = GameMainState.GameClear;
        }
        //ポーズ
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.PauseDiaLogDisplay(true);
            gameMainState = GameMainState.Pause;
        }
    }
    /// <summary>
    /// Pause時の処理
    /// この時にもう一度Pauseボタンを押すとPlayモードに戻る
    /// </summary>
    void GamePause()
    {
        uiManager.PauseButtonSelectUpdate();
        if (Input.GetButtonDown("Pause") || Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.PauseDiaLogDisplay(false);
            gameMainState = GameMainState.Play;
        }
    }
    /// <summary>
    /// /ゲームクリア時の処理
    /// </summary>
    void GameClear()
    {
        StartCoroutine(OnClear());
    }
    /// <summary>
    /// ゲームオーバー時の処理
    /// コルーチンで一定時間後にゲームオーバーダイアログを操作可能にできる
    /// isOperationはゲームオーバー時に適当にボタンを押されるとダイアログ表示前に【リトライorやめる】に遷移してしまう
    /// ため設定しました
    /// </summary>
    void GameOver()
    {
        StartCoroutine(OnGameOver());
        if (isOperation)
        {
            uiManager.GameOverButtonSelectUpdate();
        }
    }
}
