﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using StarProject.Result;

namespace StarProject.Gamemain
{
    public class GameSceneController : MonoBehaviour
    {
        /// <summary>
        /// ゲームのシーン状態
        /// </summary>
        private enum GameMainState
        {
            None,
            Opening,
            Play,
            Pause,
            GameClear,
            GameOver,
        }
        private GameMainState gameMainState = GameMainState.None;
        //---------Unityコンポーネント宣言--------------
        [SerializeField] private GameObject playerObj = null;

        [SerializeField] private GameObject safeHitGigMoaiObj = null;
        //エネミーのスクリプト取得用
        [SerializeField] private GameObject enemysObj = null;
        private EnemyController[] enemyController = null;
        private GameObject[] enemyChilledObj = null;
        private ObstacleManager[] obstacleManager = null;
        //☆子供オブジェクト取得用
        [SerializeField] private GameObject starObj = null;
        private StarController[] starControllers = null;
        private GameObject[] starChildrenOBJ = null;
        [SerializeField] private GameObject mainCamera = null;
        [SerializeField] private GameObject openingCamera = null;
        //------------クラスの宣言----------------------
        [SerializeField] private PlayerMove playerMove = null;
        public PlayerMove PlayerMove
        {
            get { return playerMove; }
        }

        [SerializeField] private Boss[] boss = null;
        private CameraController cameraController = null;

        [SerializeField] private StarChargeController starChargeController = null;
        public StarChargeController StarChargeController
        {
            get { return starChargeController; }
        }
        // 変数を直接参照させないでプロパティ文法でアクセサを経由させる
        public ChargePointManager ChargePointManager
        {
            get { return chargePointManager; }
        }
        // 変数本体でInspectorにはこちらが表示される
        [SerializeField] private ChargePointManager chargePointManager = null;
        [SerializeField] private UiManager uiManager = null;
        [SerializeField] private GameOverLineController gameOverLineController = null;

        //------------数値変数の宣言--------------------
        //現在のステージ番号 // リザルトでリトライやNextステージで使用します
        //タイトルでstageNumを1に設定します。その後はリザルトシーンのみでしか使用しません
        static public int stageNum;
        //エネミーを何体倒したか
        //ステージ１のカメラ起動用に使用します
        private int destroyCount = 0;
        //------------フラグ変数の宣言------------------
        //ゲームクリア
        public bool isGameClear
        {
            set; private get;
        }
        //ゲームオーバー
        public bool isGameOver
        {
            set; get;
        }
        //☆獲得したかどうか
        public bool isGetStar
        {
            get; set;
        }
        //カメラを振動出せるかどうか
        private bool canCameraShake;
        //カメラを動かすかどうか
        private bool isMoveCamera;
        //ゲームオーバー時にボタンを操作できるかどうか
        private bool isOperation;
        //登場演出が終了したかどうか
        private bool exitOpning;
        //debug状態かどうか
        [SerializeField] private bool debug;
        //初期化
        public void Init()
        {
            //gaugeDroportion = (float)PlayerMove.PlayerBeastModeState.StarCost / 100;//StarCostを『0.01』にする
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

            //エネミー子供オブジェクトを取得
            enemyChilledObj = new GameObject[enemysObj.transform.childCount];
            enemyController = new EnemyController[enemysObj.transform.childCount];
            obstacleManager = new ObstacleManager[enemysObj.transform.childCount];
            //☆エネミー子供オブジェクト初期化
            for (int i = 0; enemysObj.transform.childCount > i; i++)
            {
                enemyChilledObj[i] = enemysObj.transform.GetChild(i).gameObject;
                enemyController[i] = enemyChilledObj[i].GetComponent<EnemyController>();
                enemyController[i].Init(playerObj);
                obstacleManager[i] = enemyChilledObj[i].GetComponent<ObstacleManager>();
                obstacleManager[i].Init();
            }

            isGetStar = false;
            isGameClear = false;
            isGameOver = false;
            isOperation = false;
            canCameraShake = false;

            ResultScreenController.all_damage = 0;
        }

        //Start()より早く処理する
        private void Awake()
        {
            uiManager.Init();
            CameraSelect(false, true);
        }

        //スタート
        IEnumerator Start()
        {
            exitOpning = false;
            playerMove.Init();
            yield return null;
            Init();
            starChargeController.Init();
            StarsObjDysplay(false);
            cameraController.Init();
            chargePointManager.Init();
            gameOverLineController.Init();
            yield return null;
            yield return uiManager.FadeInEnumerator();
            if (!debug)
            {
                if (stageNum == 1)
                {
                    isMoveCamera = false;
                }
                else
                {
                    isMoveCamera = true;
                    Destroy(safeHitGigMoaiObj);
                    gameOverLineController.gameOverLineState = GameOverLineController.GameOverLineState.Awakening;
                    gameOverLineController.GameOverLineAnimation();
                }
            }
            else
            {
                isMoveCamera = false;
            }
            gameMainState = GameMainState.Opening;
            if (SoundManager.audioVolume != 0) Singleton.Instance.soundManager.AudioVolume();
            else Singleton.Instance.soundManager.AllAudioVolume();
            Singleton.Instance.soundManager.PlayBgm("NormalBGM");
        }

        // Update is called once per frame
        void Update()
        {
            switch (gameMainState)
            {
                case GameMainState.Opening:
                    OpeningUpdate();
                    break;
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
                if (isMoveCamera) cameraController.MoveUpdate();

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
            gameOverLineController.PlayMoaiAwakeningSE();
            gameOverLineController.gameOverLineState = GameOverLineController.GameOverLineState.Awakening;
            gameOverLineController.GameOverLineAnimation();
            yield return new WaitForSeconds(1.0f);
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
            ResultScreenController.allStar = chargePointManager.starChildCount;
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
        /// <summary>
        /// ゲーム開始時登場演出
        /// </summary>
        void OpeningUpdate()
        {
            if (!exitOpning)
            {
                AnimatorStateInfo animInfo = playerMove.playerAnimator.GetCurrentAnimatorStateInfo(0);
                if (animInfo.normalizedTime < 1.0f)
                {
                    return;
                }
                else
                {
                    exitOpning = true;
                    StartCoroutine(OpeningEnumerator());
                }
            }
        }
        IEnumerator OpeningEnumerator()
        {
            yield return uiManager.FadeOutEnumerator();
            CameraChange();
            StarsObjDysplay(true);
            playerMove.CharacterAnimation("gameStart");
            yield return uiManager.FadeInEnumerator();
            gameMainState = GameMainState.Play;
        }
        void GamePlay()
        {
            float deltaTime = Time.deltaTime;

            playerMove.OnUpdate(deltaTime);//PlayerのUpdate
                                           //☆エネミー子供オブジェクト初期化
            if (enemysObj.transform.childCount != 0)
            {
                for (int i = 0; enemysObj.transform.childCount > i; i++)
                {
                    enemyChilledObj[i] = enemysObj.transform.GetChild(i).gameObject;
                    enemyController[i] = enemyChilledObj[i].GetComponent<EnemyController>();
                    enemyController[i].EnemyControllerUpdate();
                    obstacleManager[i] = enemyChilledObj[i].GetComponent<ObstacleManager>();
                    obstacleManager[i].ObstacleControllerUpdate();
                }
            }
            if (!debug)
            {
                if (obstacleManager[0] != null && obstacleManager[0].isDestroyed && stageNum == 1 && destroyCount == 0)
                {
                    destroyCount++;
                    StartCoroutine(BigMoaiMoveStart());
                }
            }
            else
            {
                if (obstacleManager[0] != null && obstacleManager[0].isDestroyed && destroyCount == 0)
                {
                    destroyCount++;
                    StartCoroutine(BigMoaiMoveStart());
                }
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
        void StarsObjDysplay(bool isDysplay)
        {
            starObj.SetActive(isDysplay);
        }
        void CameraSelect(bool mainCameraActive, bool openingCameraActive)
        {
            mainCamera.SetActive(mainCameraActive);
            openingCamera.SetActive(openingCameraActive);
        }

        void CameraChange()
        {
            if (mainCamera.activeSelf)
            {
                mainCamera.SetActive(false);
                openingCamera.SetActive(true);
            }
            else
            {
                mainCamera.SetActive(true);
                openingCamera.SetActive(false);
            }
        }
    }
}
