using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    //ゲームオーバー時のタイプ
    public enum GameOverTyp
    {
        None,
        Normal,//一回目のセレクト
        Retry,//リトライボタンを押したときのタイプ
        Exit,//Exitボタンを押したときのタイプ
    }
    GameOverTyp gameOverTyp = GameOverTyp.None;
    //ポーズ時のタイプ
    public enum PauseTyp
    {
        None,
        Normal,//一回目のセレクト
        Retry,//リトライボタンを押したときのタイプ
        Exit,//Exitボタンを押したときのタイプ
    }
    PauseTyp pauseTyp = PauseTyp.None;

    //FadeLayerクラスを取得
    [SerializeField] FadeLayer fadeLayer;
    //ゲームオーバー時表示UI
    [SerializeField] GameObject gameOvreUI = null;

    //ゲームクリア時表示用UI
    [SerializeField] GameObject gameClearUI = null;



    //フェード時表示用TEXT
    [SerializeField] GameObject fadeText;

    //フェード時表示用マスコットキャラ
    [SerializeField] GameObject fadeChara;

    //ポーズ時ボタン
    //ポーズ時表示用UI
    [SerializeField] GameObject pauseDiaLog = null;
    [SerializeField] Image pauseRetryButton;
    [SerializeField] Image pauseTitleButton;
    //ポーズボタン
    [Header("ポーズ時のダイアログ画像")]
    [SerializeField] Sprite pauseNormalRetrySprite;
    [SerializeField] Sprite pauseSelectRetrySprite;
    [SerializeField] Sprite pauseNormalTitleSprite;
    [SerializeField] Sprite pauseSelectTitleSprite;

    int countNum;

    //star関係canvas
    public GameObject starUICanvas;

    int pauseButtonSelectNum = 0;
    int pauseButtonSelectNumMax = 2;
    int gameOverButtonSelectNum = 0;
    int gameOverButtonSelectNumMax = 2;
    int exitButtonSelectNum = 0;
    int exitButtonSelectNumMax = 2;
    int retryButtonSelectNum = 0;
    int retryButtonSelectNumMax = 2;

    //ゲームオーバーダイアログ
    [SerializeField] GameObject gameOverDiaLog = null;
    [SerializeField] Image gameOverRetryButton = null;
    [SerializeField] Image gameOverExitTitleButton = null;
    [Header("ゲームオーバー時のダイアログ画像")]
    [SerializeField] Sprite gameOverRetryNormalSprite;
    [SerializeField] Sprite gameOverRetrySelectSprite;
    [SerializeField] Sprite gameOverExitNormalSprite;
    [SerializeField] Sprite gameOverExitSelectSprite;
    //2重確認ダイアログ用画像
    [SerializeField] GameObject exitDoubleCheckDialog;
    [SerializeField] GameObject retryDoubleCheckDialog;
    [Header("2重確認用ボタン")]
    [SerializeField] Image exitDoubleCheckDialogYesButton = null;
    [SerializeField] Image exitDoubleCheckDialogNoButton = null;
    [SerializeField] Image retryDoubleCheckDialogYesButton = null;
    [SerializeField] Image retryDoubleCheckDialogNoButton = null;
    [Header("2重確認用画像")]
    [SerializeField] Sprite doubleCheckDialogYesNormalSprite;
    [SerializeField] Sprite doubleCheckDialogYesSelectSprite;
    [SerializeField] Sprite doubleCheckDialogNoNormalSprite;
    [SerializeField] Sprite doubleCheckDialogNoSelectSprite;

    // Start is called before the first frame update
    public void Init()
    {
        //初期化
        ForceColor(Color.black);
        StarUICanvasDisplay(true);
        GameOvreUIDisplay(false);
        GameOverDiaLogDisplay(false);
        RetryDiaLogDisplay(false);
        GameClearUIDisplay(false);
        PauseDiaLogDisplay(false);
        ExitDiaLogDisplay(false);
        pauseButtonSelectNum = 0;
        exitButtonSelectNum = 0;
        retryButtonSelectNum = 0;
        gameOverButtonSelectNum = 0;
        countNum = 0;
        PauseButtonSelect(pauseButtonSelectNum);
        GemaOverButtonSelect(gameOverButtonSelectNum);
        RetryButtonSelect(retryButtonSelectNum);
        pauseTyp = PauseTyp.Normal;
        gameOverTyp = GameOverTyp.Normal;
    }
    //スタート
    IEnumerator OnTitle()
    {
        yield return FadeOutEnumerator(Color.black, 3.0f);
        SceneManager.LoadScene("TitleScene");
    }
    //リトライ
    IEnumerator OnRetry()
    {
        yield return FadeOutEnumerator(Color.black,3.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// フェード時フェードキャラクターを表示非表示します
    /// </summary>
    /// <param name="isFade">表示するかどうか</param>
    void FadeImageDisplay(bool isFade)
    {
        fadeText.SetActive(isFade);
        fadeChara.SetActive(isFade);
    }
    /// <summary>
    /// フェードインの処理
    /// コルーチンの戻り値で使用します
    /// </summary>
    /// <param name="fadeTime">フェードインの時間を設定します</param>
    /// <returns></returns>
    public IEnumerator FadeInEnumerator(float fadeTime)
    {
        FadeImageDisplay(true);
        yield return fadeLayer.FadeInEnumerator(fadeTime);
        FadeImageDisplay(false);
    }
    /// <summary>
    /// フェードアウトの処理
    /// コルーチンの戻り値で使用します
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOutEnumerator(Color targetColor,float period)
    {
        ForceColor(Color.clear);
        FadeImageDisplay(true);
        yield return fadeLayer.FadeOutEnumerator(targetColor, period);
        FadeImageDisplay(false);
    }
    public void ForceColor(Color fadeColor)
    {
        fadeLayer.ForceColor(fadeColor);
    }

    /// <summary>
    /// ポーズ時に操作できる
    /// ボーズボタンの選択を行います
    /// </summary>
    public void PauseButtonSelectUpdate()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        switch (pauseTyp)
        {
            case PauseTyp.Normal:
                //右
                if (dx > 0 && countNum == 0)
                {
                    countNum++;
                    if (pauseButtonSelectNum < pauseButtonSelectNumMax) pauseButtonSelectNum++;
                    PauseButtonSelect(pauseButtonSelectNum);
                }//左
                else if (dx < 0 && countNum == 0)
                {
                    countNum++;
                    if (pauseButtonSelectNum > 0) pauseButtonSelectNum--;
                    PauseButtonSelect(pauseButtonSelectNum);
                }
                else if (dx == 0 && countNum != 0)
                {
                    countNum = 0;
                }
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    switch (pauseButtonSelectNum)
                    {
                        case 0:
                            RetryDiaLogDisplay(true);
                            retryButtonSelectNum = 0;
                            pauseTyp = PauseTyp.Retry;
                            RetryButtonSelect(retryButtonSelectNum);
                            break;
                        case 1:
                            ExitDiaLogDisplay(true);
                            exitButtonSelectNum = 0;
                            pauseTyp = PauseTyp.Exit;
                            ExitButtonSelect(exitButtonSelectNum);
                            break;
                    }
                }
                break;
            case PauseTyp.Retry:
                RetrySelect(dx);
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    switch (retryButtonSelectNum)
                    {
                        case 0://いいえ
                            RetryDiaLogDisplay(false);
                            pauseTyp = PauseTyp.Normal;
                            break;
                        case 1://はい
                            StartCoroutine(OnRetry());
                            pauseTyp = PauseTyp.None;
                            break;
                    }
                }
                break;
            case PauseTyp.Exit:
                ExitSelect(dx);
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    switch (exitButtonSelectNum)
                    {
                        case 0://いいえ
                            ExitDiaLogDisplay(false);
                            pauseTyp = PauseTyp.Normal;
                            break;
                        case 1://はい
                            StartCoroutine(OnTitle());
                            pauseTyp = PauseTyp.None;
                            break;
                    }
                }
                break;
        }
    }


    /// <summary>
    /// ポーズ時のボタン選択
    /// 選択したボタンの色を変更します
    /// </summary>
    /// <param name="buttonSelectNum">選択したボタンの番号</param>
    private void PauseButtonSelect(int buttonSelectNum)
    {
        switch (buttonSelectNum)
        {
            case 0://リトライボタン
                pauseRetryButton.sprite = pauseSelectRetrySprite;
                pauseTitleButton.sprite = pauseNormalTitleSprite;
                return;
            case 1://終了ボタン
                pauseRetryButton.sprite = pauseNormalRetrySprite;
                pauseTitleButton.sprite = pauseSelectTitleSprite;
                return;
        }
    }


    /// <summary>
    /// ゲームオーバー時に操作できる
    /// ゲームオーバーボタンの選択を行います
    /// </summary>
    public void GameOverButtonSelectUpdate()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        switch (gameOverTyp)
        {
            //ゲームオーバー時最初にいじれる
            case GameOverTyp.Normal:
                //右
                if (dx > 0 && countNum == 0)
                {
                    countNum++;
                    if (gameOverButtonSelectNum < gameOverButtonSelectNumMax) gameOverButtonSelectNum++;
                    GemaOverButtonSelect(gameOverButtonSelectNum);
                }//左
                else if (dx < 0 && countNum == 0)
                {
                    countNum++;
                    if (gameOverButtonSelectNum > 0) gameOverButtonSelectNum--;
                    GemaOverButtonSelect(gameOverButtonSelectNum);
                }
                else if (dx == 0 && countNum != 0)
                {
                    countNum = 0;
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    switch (gameOverButtonSelectNum)
                    {
                        case 0:
                            RetryDiaLogDisplay(true);
                            retryButtonSelectNum = 0;
                            RetryButtonSelect(retryButtonSelectNum);
                            gameOverTyp = GameOverTyp.Retry;
                            break;
                        case 1:
                            ExitDiaLogDisplay(true);
                            exitButtonSelectNum = 0;
                            ExitButtonSelect(exitButtonSelectNum);
                            gameOverTyp = GameOverTyp.Exit;
                            break;
                    }
                }
                break;
            //リトライボタンを押したときの操作
            case GameOverTyp.Retry:
                RetrySelect(dx);
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    switch (retryButtonSelectNum)
                    {
                        case 0:
                            RetryDiaLogDisplay(false);
                            gameOverButtonSelectNum = 0;
                            GemaOverButtonSelect(gameOverButtonSelectNum);
                            gameOverTyp = GameOverTyp.Normal;
                            break;
                        case 1:
                            StartCoroutine(OnRetry());
                            gameOverTyp = GameOverTyp.None;
                            break;
                    }
                }
                break;
            case GameOverTyp.Exit:
                ExitSelect(dx);
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    switch (exitButtonSelectNum)
                    {
                        case 0://いいえ
                            ExitDiaLogDisplay(false);
                            gameOverTyp = GameOverTyp.Normal;
                            break;
                        case 1://はい
                            StartCoroutine(OnTitle());
                            gameOverTyp = GameOverTyp.None;
                            break;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// ゲームオーバー時のボタン選択
    /// 選択したボタンの色を変更します
    /// </summary>
    /// <param name="buttonSelectNum">選択したボタンの番号</param>
    private void GemaOverButtonSelect(int buttonSelectNum)
    {
        switch (buttonSelectNum)
        {
            case 0:
                gameOverRetryButton.sprite = gameOverRetrySelectSprite;
                gameOverExitTitleButton.sprite = gameOverExitNormalSprite;
                return;
            case 1:
                gameOverRetryButton.sprite = gameOverRetryNormalSprite;
                gameOverExitTitleButton.sprite = gameOverExitSelectSprite;
                return;
        }
    }
    /// <summary>
    /// リトライ選択時のYesNoボタンセレクト
    /// </summary>
    /// <param name="dx">左右選択</param>
    private void RetrySelect(float dx)
    {
        //右
        if (dx > 0 && countNum == 0)
        {
            countNum++;
            if (retryButtonSelectNum < retryButtonSelectNumMax) retryButtonSelectNum++;
            RetryButtonSelect(retryButtonSelectNum);
        }//左
        else if (dx < 0 && countNum == 0)
        {
            countNum++;
            if (retryButtonSelectNum > 0) retryButtonSelectNum--;
            RetryButtonSelect(retryButtonSelectNum);
        }
        else if (dx == 0 && countNum != 0)
        {
            countNum = 0;
        }
    }
    /// <summary>
    /// 終了選択時のYesNoボタンセレクト
    /// </summary>
    /// <param name="dx"></param>
    private void ExitSelect(float dx)
    {
        //右
        if (dx > 0 && countNum == 0)
        {
            countNum++;
            if (exitButtonSelectNum < exitButtonSelectNumMax) exitButtonSelectNum++;
            ExitButtonSelect(exitButtonSelectNum);
        }//左
        else if (dx < 0 && countNum == 0)
        {
            countNum++;
            if (exitButtonSelectNum > 0) exitButtonSelectNum--;
            ExitButtonSelect(exitButtonSelectNum);
        }
        else if (dx == 0 && countNum != 0)
        {
            countNum = 0;
        }
    }
    /// <summary>
    /// Exitを選択した後のYesNoボタン選択
    /// 選択したボタンの色を変更します
    /// </summary>
    /// <param name="buttonSelectNum">選択したボタンの番号</param>
    private void ExitButtonSelect(int buttonSelectNum)
    {
        switch (buttonSelectNum)
        {
            case 0:
                exitDoubleCheckDialogYesButton.sprite = doubleCheckDialogYesNormalSprite;
                exitDoubleCheckDialogNoButton.sprite = doubleCheckDialogNoSelectSprite;
                return;
            case 1:
                exitDoubleCheckDialogYesButton.sprite = doubleCheckDialogYesSelectSprite;
                exitDoubleCheckDialogNoButton.sprite = doubleCheckDialogNoNormalSprite;
                return;
        }
    }
    /// <summary>
    /// リトライ時YesNoのボタン選択
    /// 選択したボタンの色を変更します
    /// </summary>
    /// <param name="buttonSelectNum">選択したボタンの番号</param>
    private void RetryButtonSelect(int buttonSelectNum)
    {
        switch (buttonSelectNum)
        {
            case 0:
                retryDoubleCheckDialogYesButton.sprite = doubleCheckDialogYesNormalSprite;
                retryDoubleCheckDialogNoButton.sprite = doubleCheckDialogNoSelectSprite;
                return;
            case 1:
                retryDoubleCheckDialogYesButton.sprite = doubleCheckDialogYesSelectSprite;
                retryDoubleCheckDialogNoButton.sprite = doubleCheckDialogNoNormalSprite;
                return;
        }
    }
    /// <summary>
    /// ゲームクリアUIを表示非表示します
    /// </summary>
    /// <param name="isDisplay">表示するかどうか</param>
    public void GameClearUIDisplay(bool isDisplay)
    {
        gameClearUI.SetActive(isDisplay);
    }
    /// <summary>
    /// ゲームオーバーUIを表示非表示します
    /// </summary>
    /// <param name="isDisplay">表示するかどうか</param>
    public void GameOvreUIDisplay(bool isDisplay)
    {
        gameOvreUI.SetActive(isDisplay);
    }
    /// <summary>
    /// ゲームオーバーダイアログの表示非表示
    /// </summary>
    /// <param name="isDisplay">表示するかどうか</param>
    public void GameOverDiaLogDisplay(bool isDisplay)
    {
        gameOverDiaLog.SetActive(isDisplay);
    }
    /// <summary>
    /// ゲームオーバー時のリトライダイアログ表示非表示
    /// </summary>
    /// <param name="isDisplay">表示するかどうか</param>
    public void RetryDiaLogDisplay(bool isDisplay)
    {
        retryDoubleCheckDialog.SetActive(isDisplay);
    }
    /// <summary>
    /// ポーズダイアログを表示非表示します
    /// </summary>
    /// <param name="isDisplay">表示するかどうか</param>
    public void ExitDiaLogDisplay(bool isDisplay)
    {
        exitDoubleCheckDialog.SetActive(isDisplay);
    }
    /// <summary>
    /// starUICanvasを表示非表示します
    /// </summary>
    /// <param name="isDisplay">表示するかどうか</param>
    public void StarUICanvasDisplay(bool isDisplay)
    {
        starUICanvas.SetActive(isDisplay);
    }
    /// <summary>
    /// ポーズダイアログを表示非表示します
    /// </summary>
    /// <param name="isDisplay">表示するかどうか</param>
    public void PauseDiaLogDisplay(bool isDisplay)
    {
        pauseDiaLog.SetActive(isDisplay);
        if (isDisplay)
        {
            //Time.timeScale = 0.0f;
            pauseButtonSelectNum = 0;
            PauseButtonSelect(pauseButtonSelectNum);
            GameSceneController.isPlaying = false;
        }
        else
        {
            //Time.timeScale = 1.0f;
            GameSceneController.isPlaying = true;
            ExitDiaLogDisplay(false);
            pauseTyp = PauseTyp.Normal;
        }
    }


}
