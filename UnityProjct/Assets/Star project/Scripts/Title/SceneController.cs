using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using StarProject.Gamemain;

namespace StarProject.Title
{
    public class SceneController : MonoBehaviour
    {
        private enum TitleTyp
        {
            None,
            TitleSelect,
            ExitSelect
        }
        private TitleTyp titleTyp = TitleTyp.None;

        //フェード関係
        [Header("フェード関係")]
        [SerializeField] private GameObject fadeImageObj = null;
        [SerializeField] private GameObject fadeText = null;
        [SerializeField] private GameObject fadeChara = null;
        private Image fadeImage;
        [SerializeField] private Color fadeOutColor;
        [SerializeField] private float fadeOutTime;

        //ボタン選択用ナンバー
        [Header("ボタン")]
        [SerializeField] private int stageMax = 0;
        [SerializeField] private Image selectButton = null;
        [SerializeField] private Image exitButton = null;
        [SerializeField] private Sprite selectButtonNormalSprite = null;
        [SerializeField] private Sprite selectButtonSelectSprite = null;
        [SerializeField] private Sprite exitButtonNormalSprite = null;
        [SerializeField] private Sprite exitButtonSelectSprite = null;

        //タイトル終了ボタン選択ナンバー
        private int buttonNum;

        //EXITダイアログUI
        [Header("EXIT用テクスチャー")]
        [SerializeField] private GameObject exitDialogUI = null;

        [SerializeField] private Image yesButton = null;
        [SerializeField] private Image noButton = null;
        [SerializeField] private Sprite exitButtonYesNormalSprite = null;
        [SerializeField] private Sprite exitButtonYesSelectSprite = null;
        [SerializeField] private Sprite exitButtonNoNormalSprite = null;
        [SerializeField] private Sprite exitButtonNoSelectSprite = null;

        //EXITボタン選択用ナンバー
        private int exitSelectNum = 0;
        //ゲームパッドjoyコン制御用
        private int countNum;

        // Start is called before the first frame update
        void Init()
        {
            buttonNum = 0;
            countNum = 0;
            exitSelectNum = 0;
            exitDialogUI.SetActive(false);
            fadeImage = fadeImageObj.GetComponent<Image>();
            TitleSelectButton(buttonNum);
        }

        //スタート
        IEnumerator Start()
        {
            Init();
            yield return null;
            titleTyp = TitleTyp.TitleSelect;
        }
        private void Awake()
        {
            FadeImageDysplay(false);
        }
        // Update is called once per frame
        void Update()
        {
            float dx = Input.GetAxis("Horizontal");
            switch (titleTyp)
            {
                case TitleTyp.TitleSelect:
                    //左
                    if (dx < 0 && countNum == 0)
                    {
                        countNum++;
                        if (buttonNum > 0) buttonNum--;
                        TitleSelectButton(buttonNum);
                    }//右
                    else if (dx > 0 && countNum == 0)
                    {
                        countNum++;
                        if (buttonNum < stageMax) buttonNum++;
                        TitleSelectButton(buttonNum);
                    }
                    else if (dx == 0 && countNum != 0)
                    {
                        countNum = 0;
                    }

                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                    {
                        switch (buttonNum)
                        {
                            case 0:
                                StartCoroutine(GameStartEnumerator());
                                break;
                            case 1:
                                exitDialogUI.SetActive(true);
                                exitSelectNum = 0;
                                ExitSelectButton(exitSelectNum);
                                titleTyp = TitleTyp.ExitSelect;
                                break;
                        }
                    }
                    break;
                case TitleTyp.ExitSelect:
                    //左
                    if (dx < 0 && countNum == 0)
                    {
                        countNum++;
                        if (exitSelectNum > 0) exitSelectNum--;
                        ExitSelectButton(exitSelectNum);
                    }//右
                    else if (dx > 0 && countNum == 0)
                    {
                        countNum++;
                        if (exitSelectNum < 1) exitSelectNum++;
                        ExitSelectButton(exitSelectNum);
                    }
                    else if (dx == 0 && countNum != 0)
                    {
                        countNum = 0;
                    }

                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                    {
                        switch (exitSelectNum)
                        {
                            case 0:
                                exitDialogUI.SetActive(false);
                                titleTyp = TitleTyp.TitleSelect;
                                break;
                            case 1:
                                OnExit();
                                titleTyp = TitleTyp.None;
                                break;
                        }
                    }
                    break;
            }
        }


        //アプリ終了関数
        private void OnExit()
        {
            Application.Quit();
            Debug.Log("アプリ終了");
        }
        /// <summary>
        /// タイトルボタンセレクト時に画像を切り替えます
        /// </summary>
        /// <param name="selectNum"></param>
        private void TitleSelectButton(int selectNum)
        {
            switch (selectNum)
            {
                case 0:
                    selectButton.sprite = selectButtonSelectSprite;
                    exitButton.sprite = exitButtonNormalSprite;
                    break;
                case 1:
                    selectButton.sprite = selectButtonNormalSprite;
                    exitButton.sprite = exitButtonSelectSprite;
                    break;
            }
        }
        /// <summary>
        /// 終了ボタン選択時YesNoボタンセレクトの画像を切り替えます
        /// </summary>
        /// <param name="selectNum"></param>
        private void ExitSelectButton(int selectNum)
        {
            switch (selectNum)
            {
                case 0:
                    yesButton.sprite = exitButtonYesNormalSprite;
                    noButton.sprite = exitButtonNoSelectSprite;
                    break;
                case 1:
                    yesButton.sprite = exitButtonYesSelectSprite;
                    noButton.sprite = exitButtonNoNormalSprite;
                    break;
            }
        }
        /// <summary>
        /// ゲームスタート時に実行します
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameStartEnumerator()
        {
            ForceColor(Color.clear);
            yield return FadeOutEnumerator(fadeOutColor, fadeOutTime);
            FadeImageDisplay(false);
            GameSceneController.stageNum = 1;
            SceneManager.LoadScene("main01");
            titleTyp = TitleTyp.None;
        }
        /// <summary>
        /// フェードアウト
        /// </summary>
        /// <param name="color">最終カラー</param>
        /// <param name="period">フェード時間</param>
        /// <returns></returns>
        private IEnumerator FadeOutEnumerator(Color color, float period)
        {
            fadeImageObj.transform.SetAsLastSibling();
            yield return FadeEnumerator(Color.clear, color, period);
        }
        /// <summary>
        /// フェード実体
        /// </summary>
        /// <param name="startColor">初期カラー</param>
        /// <param name="targetColor">最終カラー</param>
        /// <param name="period">フェード時間</param>
        /// <returns></returns>
        private IEnumerator FadeEnumerator(Color startColor, Color targetColor, float period)
        {
            float t = 0;
            while (t < period)
            {

                t += Time.deltaTime;
                Color color = Color.Lerp(startColor, targetColor, t / period);
                fadeImage.color = color;
                yield return null;
            }

            fadeImage.color = targetColor;
        }
        /// <summary>
        /// フェードカラー初期化用
        /// </summary>
        /// <param name="color">初期化カラー</param>
        private void ForceColor(Color color)
        {
            FadeImageDysplay(true);
            fadeImageObj.transform.SetAsLastSibling();
            fadeImage.color = color;
        }

        /// <summary>
        /// フェード時フェードキャラクターを表示非表示します
        /// </summary>
        /// <param name="isFade">表示するかどうか</param>
        private void FadeImageDisplay(bool isFade)
        {
            fadeText.SetActive(isFade);
            fadeChara.SetActive(isFade);
        }
        /// <summary>
        /// フェード用Imageの表示非表示
        /// </summary>
        /// <param name="isDysplay">表示非表示</param>
        private void FadeImageDysplay(bool isDysplay)
        {
            fadeImageObj.SetActive(isDysplay);
        }
    }
}