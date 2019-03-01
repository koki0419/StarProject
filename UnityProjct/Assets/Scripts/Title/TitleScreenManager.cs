using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{

    //ボタン選択用ナンバー
    [Header("ステージ数")]
    [SerializeField] int stageMax;
    [SerializeField] GameObject selectButton;
    [SerializeField] GameObject exitButton;

    int buttonNum;

    //EXITダイアログUI
    [Header("EXIT用テクスチャー")]
    [SerializeField] GameObject exitDialogUI;
    [SerializeField] GameObject yesButton;
    [SerializeField] GameObject noButton;

    //EXITボタン選択用ナンバー
    int exitYNNum;

    bool exitFlag;

    int countNum;

    // Start is called before the first frame update
    void Start()
    {
        exitDialogUI.SetActive(false);
        buttonNum = 0;
        exitFlag = false;
        exitYNNum = 1;
        OnSelect(buttonNum);
        countNum = 0;
    }

    // Update is called once per frame
    void Update()
    {

        float dy = Input.GetAxis("Vertical");
        //下
        if (dy > 0 && countNum ==0)
        {
            countNum++;
            if (!exitFlag)
            {
                if (buttonNum > 0) buttonNum--;
                OnSelect(buttonNum);
            }
            else
            {
                if (exitYNNum < 1) exitYNNum++;
                OnSelect(exitYNNum);
            }
        }//上
        else if(dy < 0 && countNum == 0)
        {
            countNum++;
            if (!exitFlag)
            {
                if (buttonNum < stageMax) buttonNum++;
                OnSelect(buttonNum);
            }
            else
            {
                if (exitYNNum > 0) exitYNNum--;
                OnSelect(exitYNNum);
            }
        }else if(dy == 0 && countNum != 0)
        {
            countNum = 0;
        }

        //buttonNumのUp、Downを行う
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!exitFlag)
            {
                if (buttonNum > 0) buttonNum--;
                OnSelect(buttonNum);
            }
            else
            {
                if (exitYNNum < 1) exitYNNum++;
                OnSelect(exitYNNum);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!exitFlag)
            {
                if (buttonNum < stageMax) buttonNum++;
                OnSelect(buttonNum);
            }
            else
            {
                if (exitYNNum > 0) exitYNNum--;
                OnSelect(exitYNNum);
            }
        }
        //
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            if (!exitFlag)
            {
                switch (buttonNum)
                {
                    case 0:
                        SceneManager.LoadScene("SelectScene");
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
                    selectButton.GetComponent<Image>().color = new Color(0, 100, 100);
                    exitButton.GetComponent<Image>().color = new Color(255, 255, 255);
                    return;
                case 1:
                    selectButton.GetComponent<Image>().color = new Color(255, 255, 255);
                    exitButton.GetComponent<Image>().color = new Color(0, 100, 100);
                    return;
            }
        }
        else
        {
            switch (num)
            {
                case 0:
                    yesButton.GetComponent<Image>().color = new Color(0, 100, 100);
                    noButton.GetComponent<Image>().color = new Color(255, 255, 255);
                    return;
                case 1:
                    yesButton.GetComponent<Image>().color = new Color(255, 255, 255);
                    noButton.GetComponent<Image>().color = new Color(0, 100, 100);
                    return;
            }
        }

    }
}
