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
    }
    GameOverTyp gameOverTyp = GameOverTyp.None;
    //ポーズ時のタイプ
    public enum PauseTyp
    {
        None,
        Normal,//一回目のセレクト
        Exit,//Exitボタンを押したときのタイプ
    }
    PauseTyp pauseTyp = PauseTyp.None;

    //FadeLayerクラスを取得
    public FadeLayer fadeLayer;
    //ゲームオーバー時表示UI
    [SerializeField] GameObject gameOvreUI = null;

    //ゲームクリア時表示用UI
    [SerializeField] GameObject gameClearUI = null;



    //フェード時表示用TEXT
    public GameObject fadeText;

    //フェード時表示用マスコットキャラ
    public GameObject fadeChara;

    //ポーズ時ボタン
    //ポーズ時表示用UI
    [SerializeField] GameObject pauseDiaLog = null;
    [SerializeField] Image pauseRetryButton;
    [SerializeField] Image pauseTitleButton;
    [SerializeField] GameObject pauseExitDiaLog = null;
    [SerializeField] Image pauseExitYesButton;
    [SerializeField] Image pauseExitNoButton;
    //ポーズボタン
    [Header("ポーズ時のダイアログ画像")]
    [SerializeField] Sprite pauseNormalRetrySprite;
    [SerializeField] Sprite pauseSelectRetrySprite;
    [SerializeField] Sprite pauseNormalTitleSprite;
    [SerializeField] Sprite pauseSelectTitleSprite;
    [Header("ポーズ時Exitダイアログ画像")]
    [SerializeField] Sprite pauseExitYesNormalRetrySprite;
    [SerializeField] Sprite pauseExitYesSelectRetrySprite;
    [SerializeField] Sprite pauseExitNoNormalRetrySprite;
    [SerializeField] Sprite pauseExitNoSelectRetrySprite;

    int countNum;

    //star関係canvas
    public GameObject starUICanvas;

    int pauseButtonSelectNum = 0;
    int pauseButtonSelectNumMax = 2;
    int pauseExitButtonSelectNum = 0;
    int pauseExitButtonSelectNumMax = 2;
    int gameOverButtonSelectNum = 0;
    int gameOverButtonSelectNumMax = 2;
    int gameOverRetryButtonSelectNum = 0;
    int gameOverRetryButtonSelectNumMax = 2;

    //ゲームオーバーダイアログ
    [SerializeField] GameObject gameOverDiaLog = null;
    [SerializeField] Image gameOverRetryButton = null;
    [SerializeField] Image gameOverExitTitleButton = null;
    [SerializeField] GameObject GameOverRetryDiaLog = null;
    [SerializeField] Image gameOverRetryYesButton = null;
    [SerializeField] Image gameOverRetryNoButton = null;
    [Header("ゲームオーバー時のダイアログ画像")]
    [SerializeField] Sprite gameOverRetryNormalSprite;
    [SerializeField] Sprite gameOverRetrySelectSprite;
    [SerializeField] Sprite gameOverExitNormalSprite;
    [SerializeField] Sprite gameOverExitSelectSprite;
    [Header("ゲームオーバー時のリトライダイアログ用画像")]
    [SerializeField] Sprite retryYesNormalSprite;
    [SerializeField] Sprite retryYesSelectSprite;
    [SerializeField] Sprite retryNoNormalSprite;
    [SerializeField] Sprite retryNoSelectSprite;
    // Start is called before the first frame update
    public void Init()
    {
        //初期化
        ForceColor(Color.black);
        FadeImageDisplay(true);
        StarUICanvasDisplay(true);
        GameOvreUIDisplay(false);
        GameOverDiaLogDisplay(false);
        GameOverRetryDiaLogDisplay(false);
        GameClearUIDisplay(false);
        PauseDiaLogDisplay(false);
        PauseExitDiaLogDisplay(false);
        pauseButtonSelectNum = 0;
        pauseExitButtonSelectNum = 0;
        gameOverRetryButtonSelectNum = 0;
        gameOverButtonSelectNum = 0;
        countNum = 0;
        PauseButtonSelect(pauseButtonSelectNum);
        GemaOverButtonSelect(gameOverButtonSelectNum);
        GemaOverRetryButtonSelect(gameOverRetryButtonSelectNum);
        pauseTyp = PauseTyp.Normal;
        gameOverTyp = GameOverTyp.Normal;
    }
    //スタート
    IEnumerator OnTitle()
    {
        yield return FadeOutEnumerator();

        SceneManager.LoadScene("TitleScene");
    }
    //リトライ
    IEnumerator OnRetry()
    {
        yield return FadeOutEnumerator();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// フェード時フェードキャラクターを表示非表示します
    /// </summary>
    /// <param name="isFade">表示するかどうか</param>
    public void FadeImageDisplay(bool isFade)
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
        yield return fadeLayer.FadeInEnumerator(fadeTime);
    }
    /// <summary>
    /// フェードアウトの処理
    /// コルーチンの戻り値で使用します
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOutEnumerator()
    {
        yield return fadeLayer.FadeOutEnumerator(Color.black, 2);
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
                            StartCoroutine(OnRetry());
                            pauseTyp = PauseTyp.None;
                            break;
                        case 1:
                            PauseExitDiaLogDisplay(true);
                            pauseExitButtonSelectNum = 0;
                            pauseTyp = PauseTyp.Exit;
                            PauseExitButtonSelect(pauseExitButtonSelectNum);
                            break;
                    }
                }
                break;
            case PauseTyp.Exit:
                //右
                if (dx > 0 && countNum == 0)
                {
                    countNum++;
                    if (pauseExitButtonSelectNum < pauseExitButtonSelectNumMax) pauseExitButtonSelectNum++;
                    PauseExitButtonSelect(pauseExitButtonSelectNum);
                }//左
                else if (dx < 0 && countNum == 0)
                {
                    countNum++;
                    if (pauseExitButtonSelectNum > 0) pauseExitButtonSelectNum--;
                    PauseExitButtonSelect(pauseExitButtonSelectNum);
                }
                else if (dx == 0 && countNum != 0)
                {
                    countNum = 0;
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    switch (pauseExitButtonSelectNum)
                    {
                        case 0:
                            PauseExitDiaLogDisplay(false);
                            pauseTyp = PauseTyp.Normal;
                            break;
                        case 1:
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
            case 0:
                pauseRetryButton.sprite = pauseSelectRetrySprite;
                pauseTitleButton.sprite = pauseNormalTitleSprite;
                return;
            case 1:
                pauseRetryButton.sprite = pauseNormalRetrySprite;
                pauseTitleButton.sprite = pauseSelectTitleSprite;
                return;
        }
    }
    /// <summary>
    /// ポーズ時Exitを選択した後のYesNoボタン選択
    /// 選択したボタンの色を変更します
    /// </summary>
    /// <param name="buttonSelectNum">選択したボタンの番号</param>
    private void PauseExitButtonSelect(int buttonSelectNum)
    {
        switch (buttonSelectNum)
        {
            case 0:
                pauseExitYesButton.sprite = pauseExitYesNormalRetrySprite;
                pauseExitNoButton.sprite = pauseExitNoSelectRetrySprite;
                return;
            case 1:
                pauseExitYesButton.sprite = pauseExitYesSelectRetrySprite;
                pauseExitNoButton.sprite = pauseExitNoNormalRetrySprite;
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
    public void GameOverRetryDiaLogDisplay(bool isDisplay)
    {
        GameOverRetryDiaLog.SetActive(isDisplay);
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
                            GameOverRetryDiaLogDisplay(true);
                            gameOverRetryButtonSelectNum = 0;
                            GemaOverRetryButtonSelect(gameOverRetryButtonSelectNum);
                            gameOverTyp = GameOverTyp.Retry;
                            break;
                        case 1:
                            StartCoroutine(OnTitle());
                            gameOverTyp = GameOverTyp.None;
                            break;
                    }
                }
                break;
            //リトライボタンを押したときの操作
            case GameOverTyp.Retry:
                //右
                if (dx > 0 && countNum == 0)
                {
                    countNum++;
                    if (gameOverRetryButtonSelectNum < gameOverRetryButtonSelectNumMax) gameOverRetryButtonSelectNum++;
                    GemaOverRetryButtonSelect(gameOverRetryButtonSelectNum);
                }//左
                else if (dx < 0 && countNum == 0)
                {
                    countNum++;
                    if (gameOverRetryButtonSelectNum > 0) gameOverRetryButtonSelectNum--;
                    GemaOverRetryButtonSelect(gameOverRetryButtonSelectNum);
                }
                else if (dx == 0 && countNum != 0)
                {
                    countNum = 0;
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    switch (gameOverRetryButtonSelectNum)
                    {
                        case 0:
                            GameOverRetryDiaLogDisplay(false);
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
    /// リトライ時のボタン選択
    /// 選択したボタンの色を変更します
    /// </summary>
    /// <param name="buttonSelectNum">選択したボタンの番号</param>
    private void GemaOverRetryButtonSelect(int buttonSelectNum)
    {
        switch (buttonSelectNum)
        {
            case 0:
                gameOverRetryYesButton.sprite = retryYesNormalSprite;
                gameOverRetryNoButton.sprite = retryNoSelectSprite;
                return;
            case 1:
                gameOverRetryYesButton.sprite = retryYesSelectSprite;
                gameOverRetryNoButton.sprite = retryNoNormalSprite;
                return;
        }
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
            PauseExitDiaLogDisplay(false);
            pauseTyp = PauseTyp.Normal;
        }
    }
    /// <summary>
    /// ポーズダイアログを表示非表示します
    /// </summary>
    /// <param name="isDisplay">表示するかどうか</param>
    public void PauseExitDiaLogDisplay(bool isDisplay)
    {
        pauseExitDiaLog.SetActive(isDisplay);
    }

}
