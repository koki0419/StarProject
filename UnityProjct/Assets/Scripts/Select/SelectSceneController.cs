using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StarProject.Select
{
    public class SelectSceneController : MonoBehaviour
    {

        [SerializeField] FadeLayer fadeLayer = null;

        //フェード時表示用TEXT
        public GameObject fadeText;

        //フェード時表示用マスコットキャラ
        public GameObject fadeChara;

        bool isPlaying = false;

        //ステージボタン
        [Header("ステージボタン")]
        [SerializeField] GameObject[] stageButton = null;

        int stageNum;

        static public int updownNum = 0;
        static public int leftrightNum = 0;


        int[,] buttonNum = new int[2, 3];


        IEnumerator Start()
        {
            isPlaying = false;
            yield return null;
            fadeText.SetActive(false);
            fadeChara.SetActive(false);
            yield return null;
            //selectSceneUIManager.OnInit();

            buttonNum[0, 0] = 0;
            buttonNum[0, 1] = 1;
            buttonNum[0, 2] = -1;
            buttonNum[1, 0] = 2;
            buttonNum[1, 1] = 3;
            buttonNum[1, 2] = 4;

            SelectStageButton(buttonNum[updownNum, leftrightNum]);

            yield return fadeLayer.FadeInEnumerator(2);
            isPlaying = true;
        }

        //Start()より早く処理する
        private void Awake()
        {
            //初期化
            fadeLayer.ForceColor(Color.black);
        }

        // Update is called once per frame
        void Update()
        {
            if (isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    SceneManager.LoadScene("PrototypeScene");
                }

            }

            //上
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (updownNum > 0) updownNum--;
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
    }
}