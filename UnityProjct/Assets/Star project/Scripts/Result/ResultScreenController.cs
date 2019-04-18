using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace StarProject.Result
{
    public class ResultScreenController : MonoBehaviour
    {
        public enum ResultState
        {
            None,
            ResultAnimation,
            ResultSerect,
        }
        public ResultState resultState = ResultState.None;


        //ボタン選択時選択していない状態
        [SerializeField] Color normalColor;
        //ボタン選択時選択されている状態
        [SerializeField] Color selectColor;
        //ネクストステージダイアログ
        [SerializeField] private GameObject nextStageDiaLog = null;
        [SerializeField] private GameObject nextStageButton = null;
        [SerializeField] private GameObject exitTitleButton = null;
        //ボタン選択時にGetAxisを使用するので回数制限に使用します
        int countNum = 0;
        //ボタン選択番号
        int buttonSelectNum;
        //選択ステージ番号を格納
        int stageNum;

        float resultAnimationTime;
        float resultAnimationTimeMax = 10.0f;

        static public int all_damage;

        // Start is called before the first frame update
        void Start()
        {
            stageNum = GameSceneController.stageNum;
            NextStageDiaLogDisplay(false);
            buttonSelectNum = 0;
            NextStageButtonSelectColor(buttonSelectNum);
            resultState = ResultState.ResultAnimation;
        }

        // Update is called once per frame
        void Update()
        {
            switch (resultState)
            {
                case ResultState.ResultAnimation:
                    ResultAnimation();
                    break;
                case ResultState.ResultSerect:
                    NextStageSelect();
                    break;
            }
        }

        void ResultAnimation()
        {
            //resultAnimationTime += Time.deltaTime;
            //if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Joystick1Button0))
            //{
            //    Time.timeScale = 10.5f;
            //}
            //else
            //{
            //    Time.timeScale = 1.0f;
            //}

            //if (resultAnimationTime >= resultAnimationTimeMax)
            //{
            //    NextStageDiaLogDisplay(true);
            //    resultState = ResultState.ResultSerect;
            //}
        }
        //次のステージに挑戦するかダイアログを表示非表示
        void NextStageDiaLogDisplay(bool isDisplay)
        {
            nextStageDiaLog.SetActive(isDisplay);
        }

        /// <summary>
        /// 次のステージに挑戦するかどうかを選択する関数
        /// </summary>
        void NextStageSelect()
        {
            float dx = Input.GetAxis("Horizontal");
            float dy = Input.GetAxis("Vertical");
            //buttonNumのUp、Downを行う
            //上
            //左
            if (dx < 0 && countNum == 0)
            {
                countNum++;
                if (buttonSelectNum > 0) buttonSelectNum--;
                NextStageButtonSelectColor(buttonSelectNum);
            }//右
            else if (dx > 0 && countNum == 0)
            {
                countNum++;
                if (buttonSelectNum < 2) buttonSelectNum++;
                NextStageButtonSelectColor(buttonSelectNum);
            }
            else if (dx == 0 && countNum != 0)
            {
                countNum = 0;
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                NextStageDiaLogDisplay(false);
                switch (buttonSelectNum)
                {
                    case 0://次のステージ
                        GameSceneController.stageNum++;
                        stageNum = GameSceneController.stageNum;
                        SceneManager.LoadScene(string.Format("Main0{0}", stageNum));
                        Debug.Log("次のステージ");
                        break;
                    case 1://リトライ
                        SceneManager.LoadScene(string.Format("Main{0}", stageNum));
                        Debug.Log("リトライ");
                        break;
                }
            }
        }

        /// <summary>
        /// クリア時のボタン選択
        /// 選択したボタンの色を変更します
        /// </summary>
        /// <param name="buttonSelectNum"><選択したボタンの番号/param>
        private void NextStageButtonSelectColor(int buttonSelectNum)
        {

            switch (buttonSelectNum)
            {
                case 0:
                    nextStageButton.GetComponent<Image>().color = selectColor;
                    exitTitleButton.GetComponent<Image>().color = normalColor;
                    return;
                case 1:
                    nextStageButton.GetComponent<Image>().color = normalColor;
                    exitTitleButton.GetComponent<Image>().color = selectColor;
                    return;
            }
        }
    }
}