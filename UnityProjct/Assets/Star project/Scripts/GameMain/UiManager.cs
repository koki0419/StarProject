using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
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

    //リザルト時表示用UI
    public GameObject resultUIBG;
    //リザルト時ボタン
    public GameObject retryButton;
    public GameObject titleButton;


    //star関係canvas
    public GameObject starUICanvas;

    int buttonSelectNum = 0;
    int buttonSelectNumMax = 2;

    // Start is called before the first frame update
    public void Init()
    {
        //初期化
        ForceColor(Color.black);
        FadeImageDisplay(true);
        StarUICanvasDisplay(true);
        GameOvreUIDisplay(false);
        GameClearUIDisplay(false);
        ResultUIBGUIDisplay(false);
        ButtonSelect(0);
    }

    // Update is called once per frame
    public void ButtonSelectUpdate()
    {
        //buttonNumのUp、Downを行う
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (buttonSelectNum < buttonSelectNumMax) buttonSelectNum++;
            ButtonSelect(buttonSelectNum);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (buttonSelectNum > 0) buttonSelectNum--;
            ButtonSelect(buttonSelectNum);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (buttonSelectNum)
            {
                case 0:
                    StartCoroutine(OnRetry());
                    break;
                case 1:
                    StartCoroutine(OnSelect());
                    break;
            }
        }
    }


    /// <summary>
    /// クリア時のボタン選択
    /// 選択したボタンの色を変更します
    /// </summary>
    /// <param name="buttonSelectNum"><選択したボタンの番号/param>
    private void ButtonSelect(int buttonSelectNum)
    {

        switch (buttonSelectNum)
        {
            case 0:
                retryButton.GetComponent<Image>().color = new Color(0, 100, 100);
                titleButton.GetComponent<Image>().color = new Color(255, 255, 255);
                return;
            case 1:
                retryButton.GetComponent<Image>().color = new Color(255, 255, 255);
                titleButton.GetComponent<Image>().color = new Color(0, 100, 100);
                return;
        }
    }

    //スタート
    IEnumerator OnSelect()
    {
        yield return FadeOutEnumerator();

        SceneManager.LoadScene("SelectScene");
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
    /// <param name="isFade"></表示するかどうかparam>
    public void FadeImageDisplay(bool isFade)
    {
        fadeText.SetActive(isFade);
        fadeChara.SetActive(isFade);
    }
    /// <summary>
    /// フェードインの処理
    /// コルーチンの戻り値で使用します
    /// </summary>
    /// <param name="fadeTime"><フェードインの時間を設定します/param>
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
    /// ゲームクリアUIを表示非表示します
    /// </summary>
    /// <param name="isDisplay"><表示するかどうか/param>
    public void GameClearUIDisplay(bool isDisplay)
    {
        gameClearUI.SetActive(isDisplay);
    }
    /// <summary>
    /// ゲームオーバーUIを表示非表示します
    /// </summary>
    /// <param name="isDisplay"><表示するかどうか/param>
    public void GameOvreUIDisplay(bool isDisplay)
    {
        gameOvreUI.SetActive(isDisplay);
    }
    /// <summary>
    /// リザルトUIを表示非表示します
    /// </summary>
    /// <param name="isDisplay"><表示するかどうか/param>
    public void ResultUIBGUIDisplay(bool isDisplay)
    {
        resultUIBG.SetActive(isDisplay);
    }
    /// <summary>
    /// starUICanvasを表示非表示します
    /// </summary>
    /// <param name="isDisplay"><表示するかどうか/param>
    public void StarUICanvasDisplay(bool isDisplay)
    {
        starUICanvas.SetActive(isDisplay);
    }

}
