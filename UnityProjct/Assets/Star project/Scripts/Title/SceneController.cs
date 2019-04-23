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
        [SerializeField]
        Color normalColor;
        [SerializeField]
        Color selectedColor;


        public enum TitleTyp
        {
            None,
            OnTitle,
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
        int exitYNNum;

        bool exitFlag;

        int countNum;

        float titleGameTime = 0;

        // Start is called before the first frame update
        void Init()
        {
            exitDialogUI.SetActive(false);
            buttonNum = 0;
            exitFlag = false;
            exitYNNum = 1;
            OnSelect(buttonNum);
            countNum = 0;
        }

        //スタート
        IEnumerator Start()
        {
            Init();
            yield return null;
            titleTyp = TitleTyp.OnTitle;
        }

        // Update is called once per frame
        void Update()
        {
            float dx = Input.GetAxis("Horizontal");
            switch (titleTyp)
            {
                case TitleTyp.OnTitle:
                    //左
                    if (dx < 0 && countNum == 0)
                    {
                        countNum++;
                        if (!exitFlag)
                        {
                            if (buttonNum > 0) buttonNum--;
                            OnSelect(buttonNum);
                        }
                        else
                        {
                            if (exitYNNum > 0) exitYNNum--;
                            OnSelect(exitYNNum);
                        }
                    }//右
                    else if (dx > 0 && countNum == 0)
                    {
                        countNum++;
                        if (!exitFlag)
                        {
                            if (buttonNum < stageMax) buttonNum++;
                            OnSelect(buttonNum);
                        }
                        else
                        {
                            if (exitYNNum < 1) exitYNNum++;
                            OnSelect(exitYNNum);
                        }
                    }
                    else if (dx == 0 && countNum != 0)
                    {
                        countNum = 0;
                    }

                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                    {
                        if (!exitFlag)
                        {
                            switch (buttonNum)
                            {
                                case 0:
                                    GameSceneController.stageNum = 1;
                                    SceneManager.LoadScene("PrototypeScene");
                                    return;
                                case 1:
                                    exitDialogUI.SetActive(true);
                                    exitFlag = true;
                                    exitYNNum = 1;
                                    OnSelect(exitYNNum);
                                    return;
                            }
                        }
                        else
                        {
                            switch (exitYNNum)
                            {
                                case 0:
                                    OnExit();
                                    return;
                                case 1:
                                    exitDialogUI.SetActive(false);
                                    exitFlag = false;
                                    return;
                            }
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

        private void OnSelect(int num)
        {
            if (!exitFlag)
            {
                switch (num)
                {
                    case 0:
                        selectButton.sprite = selectButtonNormalSprite;
                        exitButton.sprite = exitButtonSelectSprite;
                        return;
                    case 1:
                        selectButton.sprite = selectButtonSelectSprite;
                        exitButton.sprite = exitButtonNormalSprite;
                        return;
                }
            }
            else
            {
                switch (num)
                {
                    case 0:
                        yesButton.sprite = exitButtonYesNormalSprite;
                        noButton.sprite = exitButtonNoSelectSprite;
                        return;
                    case 1:
                        yesButton.sprite = exitButtonYesSelectSprite;
                        noButton.sprite = exitButtonNoNormalSprite;
                        return;
                }
            }

        }
    }
}