using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSceneUIManager : MonoBehaviour
{


    //ステージボタン
    [Header("ステージボタン")]
    [SerializeField] GameObject[] stageButton;

    int stageNum;

    static public int updownNum = 0;
    static public int leftrightNum = 0;


    int[,] buttonNum = new int[2, 3];




    //[0,0] = stage1
    //[0,1] = stage2
    //[0,2] = Null
    //[0,3] = Null
    //[1,0] = stage3
    //[1,1] = stage4
    //[1,2] = stage5
    //[1,3] = Null

    public void OnInit()
    {
        buttonNum[0, 0] = 0;
        buttonNum[0, 1] = 1;
        buttonNum[0, 2] = -1;
        buttonNum[1, 0] = 2;
        buttonNum[1, 1] = 3;
        buttonNum[1, 2] = 4;

        SelectStageButton(buttonNum[updownNum, leftrightNum]);
    }


    public void OnUpdate()
    {
        //上
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(updownNum > 0) updownNum--;
            if (leftrightNum == 2) leftrightNum = 1;

            SelectStageButton(buttonNum[updownNum, leftrightNum]);
        }
        //下
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (updownNum < 1) updownNum++;

            SelectStageButton(buttonNum[updownNum, leftrightNum]);
        }
        //右
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (leftrightNum < 2) leftrightNum++;
            if (updownNum == 0 && leftrightNum == 2) leftrightNum = 1;

            SelectStageButton(buttonNum[updownNum, leftrightNum]);
        }
        //左
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (leftrightNum > 0) leftrightNum--;

            SelectStageButton(buttonNum[updownNum, leftrightNum]);
        }

    }

    public void SelectStageButton(int stageNum)
    {

        for (int i = 0; i < stageButton.Length; i++)
        {
            if (i == stageNum) stageButton[i].GetComponent<Image>().color = new Color(0, 100, 100);
            else stageButton[i].GetComponent<Image>().color = new Color(255, 255, 255);
        }
    }

    void SelectStageButton(int[][] Array)
    {

    }


}
