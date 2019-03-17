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
    public GameObject gameOvreUI;

    //ゲームクリア時表示用UI
    public GameObject gameClearUI;

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

    int buttonNum=0;
    int buttonNumMax = 2;

    // Start is called before the first frame update
    public void Init()
    {
        //初期化
        fadeLayer.ForceColor(Color.black);
        fadeText.SetActive(true);
        fadeChara.SetActive(true);

        starUICanvas.SetActive(true);
        gameOvreUI.SetActive(false);
        gameClearUI.SetActive(false);
        resultUIBG.SetActive(false);
        OnSelect(0);
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        //buttonNumのUp、Downを行う
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (buttonNum < buttonNumMax) buttonNum++;
            OnSelect(buttonNum);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (buttonNum > 0) buttonNum--;
            OnSelect(buttonNum);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (buttonNum)
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



    private void OnSelect(int num)
    {

        switch (num)
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
        yield return fadeLayer.FadeOutEnumerator(Color.black, 2);

        SceneManager.LoadScene("SelectScene");
    }
    //スタート
    IEnumerator OnRetry()
    {
        yield return fadeLayer.FadeOutEnumerator(Color.black, 2);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
