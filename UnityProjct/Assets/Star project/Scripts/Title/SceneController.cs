using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace StarProject.Title
{
    public class SceneController : MonoBehaviour
    {
        public enum TitleTyp
        {
            None,
            TitleSelect,
            ExitSelect
        }
        public TitleTyp titleTyp = TitleTyp.None;


        //ボタン選択用ナンバー
        [Header("ボタン")]
        [SerializeField] int stageMax = 0;
        [SerializeField] Image selectButton = null;
        [SerializeField] Image exitButton = null;
        [SerializeField] Sprite selectButtonNormalSprite;
        [SerializeField] Sprite selectButtonSelectSprite;
        [SerializeField] Sprite exitButtonNormalSprite;
        [SerializeField] Sprite exitButtonSelectSprite;

        int buttonNum;

        //EXITダイアログUI
        [Header("EXIT用テクスチャー")]
        [SerializeField] GameObject exitDialogUI = null;

        [SerializeField] Image yesButton = null;
        [SerializeField] Image noButton = null;
        [SerializeField] Sprite exitButtonYesNormalSprite;
        [SerializeField] Sprite exitButtonYesSelectSprite;
        [SerializeField] Sprite exitButtonNoNormalSprite;
        [SerializeField] Sprite exitButtonNoSelectSprite;

        //EXITボタン選択用ナンバー
        int exitSelectNum = 0;

        bool exitFlag;

        int countNum;

        float titleGameTime = 0;

        // Start is called before the first frame update
        void Init()
        {
            exitDialogUI.SetActive(false);
            buttonNum = 0;
            exitFlag = false;
            exitSelectNum = 0;
            TitleSelectButton(buttonNum);
            countNum = 0;
        }

        //スタート
        IEnumerator Start()
        {
            Init();
            yield return null;
            titleTyp = TitleTyp.TitleSelect;
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
                                GameSceneController.stageNum = 1;
                                SceneManager.LoadScene("main01");
                                titleTyp = TitleTyp.None;
                                break;
                            case 1:
                                exitDialogUI.SetActive(true);
                                exitFlag = true;
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
                                exitFlag = false;
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
    }
}