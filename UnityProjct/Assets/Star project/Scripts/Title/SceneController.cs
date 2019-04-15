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
            OnMovie,
            OnTitle,

        }

        public TitleTyp titleTyp = TitleTyp.None;


        //オープニング用
        [SerializeField] VideoPlayer video = null;
        //オープニングムービー再生用OBJ
        [SerializeField] GameObject movieObj = null;
        //skipダイアログ
        [SerializeField] GameObject skipDialog = null;
        [SerializeField] GameObject skipYesButton = null;
        [SerializeField] GameObject skipNoButton = null;
        bool skipFlag;
        int skipButtonNum;

        [SerializeField] GameObject canvas = null;

        //ボタン選択用ナンバー
        [Header("ステージ数")]
        [SerializeField] int stageMax = 0;
        [SerializeField] GameObject selectButton = null;
        [SerializeField] GameObject exitButton = null;

        int buttonNum;

        //EXITダイアログUI
        [Header("EXIT用テクスチャー")]
        [SerializeField] GameObject exitDialogUI = null;
        [SerializeField] GameObject yesButton = null;
        [SerializeField] GameObject noButton = null;

        //EXITボタン選択用ナンバー
        int exitYNNum;

        bool exitFlag;

        int countNum;

        float titleGameTime = 0;

        // Start is called before the first frame update
        void Init()
        {
            canvas.SetActive(false);
            exitDialogUI.SetActive(false);
            buttonNum = 0;
            exitFlag = false;
            exitYNNum = 1;
            OnSelect(buttonNum);
            countNum = 0;
            movieObj.SetActive(true);
            skipDialog.SetActive(false);
            skipFlag = false;
        }

        //スタート
        IEnumerator Start()
        {
            Init();
            yield return null;
            video.Play();
            titleTyp = TitleTyp.OnMovie;
        }

        // Update is called once per frame
        void Update()
        {
            float dx = Input.GetAxis("Horizontal");
            switch (titleTyp)
            {
                case TitleTyp.OnMovie:
                    titleGameTime += Time.deltaTime;
                    if (titleGameTime > 33.0f)
                    {
                        movieObj.SetActive(false);
                        canvas.SetActive(true);
                        skipDialog.SetActive(false);
                        titleTyp = TitleTyp.OnTitle;
                    }

                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                    {
                        if (!skipFlag)
                        {
                            skipFlag = true;
                            canvas.SetActive(true);
                            skipDialog.SetActive(true);
                            video.Pause();
                        }
                        else
                        {
                            switch (skipButtonNum)
                            {
                                case 0:
                                    movieObj.SetActive(false);
                                    skipDialog.SetActive(false);
                                    titleTyp = TitleTyp.OnTitle;
                                    break;
                                case 1:
                                    skipFlag = false;
                                    canvas.SetActive(false);
                                    skipDialog.SetActive(false);
                                    video.Play();
                                    break;
                            }
                        }
                    }

                    if (skipFlag)
                    {
                        //左
                        if (dx < 0 && countNum == 0)
                        {
                            countNum++;
                            if (skipButtonNum > 0) skipButtonNum--;
                            OnSkip(skipButtonNum);
                        }//右
                        else if (dx > 0 && countNum == 0)
                        {
                            countNum++;
                            if (skipButtonNum < 2) skipButtonNum++;
                            OnSkip(skipButtonNum);
                        }
                        else if (dx == 0 && countNum != 0)
                        {
                            countNum = 0;
                        }
                    }
                    break;


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

                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                    {
                        if (!exitFlag)
                        {
                            switch (buttonNum)
                            {
                                case 0:
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
                        selectButton.GetComponent<Image>().color = selectedColor;
                        exitButton.GetComponent<Image>().color = normalColor;
                        return;
                    case 1:
                        selectButton.GetComponent<Image>().color = normalColor;
                        exitButton.GetComponent<Image>().color = selectedColor;
                        return;
                }
            }
            else
            {
                switch (num)
                {
                    case 0:
                        yesButton.GetComponent<Image>().color = selectedColor;
                        noButton.GetComponent<Image>().color = normalColor;
                        return;
                    case 1:
                        yesButton.GetComponent<Image>().color = normalColor;
                        noButton.GetComponent<Image>().color = selectedColor;
                        return;
                }
            }

        }

        private void OnSkip(int num)
        {
            switch (num)
            {
                case 0:
                    skipYesButton.GetComponent<Image>().color = new Color(0, 100, 100);
                    skipNoButton.GetComponent<Image>().color = new Color(255, 255, 255);
                    return;
                case 1:
                    skipYesButton.GetComponent<Image>().color = new Color(255, 255, 255);
                    skipNoButton.GetComponent<Image>().color = new Color(0, 100, 100);
                    return;
            }
        }

    }
}