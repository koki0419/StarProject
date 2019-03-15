﻿using System.Collections;
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
    public PlayerMove playerMove;
    
    [SerializeField] BreakBoxController[] breakBoxController;

    [SerializeField] Boss[] boss;

    [SerializeField] StarController[] starControllers;

    CameraController cameraController;

    [SerializeField] EnemyController[] enemyController;

    public ChargeUIController chargeUIController;

    public StarChargeController starChargeController;

    public ChargePointManager chargePointManager;

    [SerializeField] FadeLayer fadeLayer;
    [SerializeField] GameObject fadeText;
    [SerializeField] GameObject fadeChara;

    //------------数値変数の宣言--------------------


    //*****************************
    //デバックポイント
    //[SerializeField] int debugPoint;


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
        chargePointManager.Init();
        yield return null;
        fadeText.SetActive(false);
        fadeChara.SetActive(false);
        yield return null;

        yield return fadeLayer.FadeInEnumerator(2);
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

    private void LateUpdate()
    {
        cameraController.OnUpdate();
        chargePointManager.OnUpdate();
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
