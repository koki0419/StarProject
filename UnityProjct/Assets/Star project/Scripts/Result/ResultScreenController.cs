using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace StarProject.Result
{
    public class ResultScreenController : MonoBehaviour
    {
        private enum ResultState
        {
            None,
            ResultAnimation,
            ResultSerect,
        }
        ResultState resultState = ResultState.None;

        private enum ResultRetryState
        {
            None,
            ResultSelect,
            Retry,
            Exit,
        }
        ResultRetryState resultRetryState = ResultRetryState.None;

        [SerializeField] Animator resultAnimator;

        //ネクストステージダイアログ
        [SerializeField] private GameObject nextStageDiaLog = null;
        [SerializeField] private Image nextStageButton = null;
        [SerializeField] private Image exitTitleButton = null;
        [SerializeField] private Image retryButton = null;

        [Header("次のステージ選択ダイアログ用画像")]
        [SerializeField] private Sprite nextStageNormalSprite = null;
        [SerializeField] private Sprite nextStageSelectSprite = null;
        [SerializeField] private Sprite exitTitleNormalSprite = null;
        [SerializeField] private Sprite exitTitleSelectSprite = null;
        [SerializeField] private Sprite retryNormalSprite = null;
        [SerializeField] private Sprite retrySelectSprite = null;

        //2重確認ダイアログ用画像
        [SerializeField] GameObject exitDoubleCheckDialog;
        [SerializeField] GameObject retryDoubleCheckDialog;
        [Header("2重確認用ボタン")]
        [SerializeField] Image exitDoubleCheckDialogYesButton = null;
        [SerializeField] Image exitDoubleCheckDialogNoButton = null;
        [SerializeField] Image retryDoubleCheckDialogYesButton = null;
        [SerializeField] Image retryDoubleCheckDialogNoButton = null;
        [Header("2重確認用画像")]
        [SerializeField] Sprite doubleCheckDialogYesNormalSprite;
        [SerializeField] Sprite doubleCheckDialogYesSelectSprite;
        [SerializeField] Sprite doubleCheckDialogNoNormalSprite;
        [SerializeField] Sprite doubleCheckDialogNoSelectSprite;
        //ボタン選択時にGetAxisを使用するので回数制限に使用します
        int countNum = 0;
        //ボタン選択番号
        int nextStageButtonSelectNum = 0;
        int nextStageButtonSelectNumMax = 3;
        int retryButtonSelectNum = 0;
        int retryButtonSelectNumMax = 2;
        int exitButtonSelectNum = 0;
        int exitButtonSelectNumMax = 2;
        //選択ステージ番号を格納
        int stageNum;

        float resultAnimationTime;
        float resultAnimationTimeMax = 10.0f;

        //総ダメージの表示
        static public int all_damage = 0;
        static public int allStar = 0;
        //ダメージ表記スコアUIを取得
        [SerializeField] Image[] scoreUI;
        //ダメージ表示用の数値画像0～9
        [SerializeField] Sprite[] numSprite;
        //ステージ数の表記UI
        [SerializeField] Image stageNumUI;
        //ステージ数表示画像
        [SerializeField] Sprite[] stageNumSprite;
        //クリアランク表示用UI
        [SerializeField] Image rankUI;
        //クリアランク表示画像
        [SerializeField] Sprite[] rankSprite;
        [Header("ランク振り分けスコア")]
        [SerializeField] int rankAScore;
        [SerializeField] int rankBScore;
        [SerializeField] int rankCScore;


        // Start is called before the first frame update
        void Start()
        {
            stageNum = GameSceneController.stageNum;
            NextStageDiaLogDisplay(false);
            RetryDiaLogDysplay(false);
            ExitDiaLogDysplay(false);
            nextStageButtonSelectNum = 0;
            NextStageButtonSelect(nextStageButtonSelectNum);
            StageNumDisplay(GameSceneController.stageNum);
            ResultScoreDisplay(all_damage);
            ClearRankDisplay(all_damage);
            resultState = ResultState.ResultAnimation;
            resultRetryState = ResultRetryState.ResultSelect;
            if (allStar / 10 != 0) resultAnimator.SetInteger("ResultStars", allStar / 10);

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
        /// <summary>
        /// 評価演出
        /// </summary>
        void ResultAnimation()
        {
            AnimatorStateInfo animInfo = resultAnimator.GetCurrentAnimatorStateInfo(0);
            if (animInfo.normalizedTime < 1.0f)
            {
                //アニメーション早送り
                if (Input.GetKey(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    Time.timeScale = 2.5f;
                }
                else
                {
                    Time.timeScale = 1.0f;
                }
            }
            else
            {   //アニメーション終了後クリックしてダイアログ表示
                if (Input.GetKey(KeyCode.Return) || Input.GetButtonDown("SelectOk"))
                {
                    NextStageDiaLogDisplay(true);
                    resultState = ResultState.ResultSerect;
                }

            }

        }

        /// <summary>
        /// 次のステージに挑戦するかどうかを選択する関数
        /// </summary>
        void NextStageSelect()
        {
            float dx = Input.GetAxis("Horizontal");
            float dy = Input.GetAxis("Vertical");
            switch (resultRetryState)
            {
                case ResultRetryState.ResultSelect:
                    //左
                    if (dx < 0 && countNum == 0)
                    {
                        countNum++;
                        if (nextStageButtonSelectNum > 0) nextStageButtonSelectNum--;
                        NextStageButtonSelect(nextStageButtonSelectNum);
                    }//右
                    else if (dx > 0 && countNum == 0)
                    {
                        countNum++;
                        if (nextStageButtonSelectNum < nextStageButtonSelectNumMax) nextStageButtonSelectNum++;
                        NextStageButtonSelect(nextStageButtonSelectNum);
                    }
                    else if (dx == 0 && countNum != 0)
                    {
                        countNum = 0;
                    }

                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                    {
                        switch (nextStageButtonSelectNum)
                        {
                            case 0://次のステージ
                                GameSceneController.stageNum++;
                                stageNum = GameSceneController.stageNum;
                                SceneManager.LoadScene(string.Format("Main0{0}", stageNum));
                                resultRetryState = ResultRetryState.None;
                                break;
                            case 1://リトライ
                                retryButtonSelectNum = 0;
                                RetryButtonSelect(retryButtonSelectNum);
                                RetryDiaLogDysplay(true);
                                resultRetryState = ResultRetryState.Retry;
                                break;
                            case 2://やめる
                                exitButtonSelectNum = 0;
                                ExitButtonSelect(exitButtonSelectNum);
                                ExitDiaLogDysplay(true);
                                resultRetryState = ResultRetryState.Exit;
                                break;
                        }
                    }
                    break;
                case ResultRetryState.Retry:
                    RetrySelect(dx);
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                    {
                        switch (retryButtonSelectNum)
                        {
                            case 0://いいえ
                                RetryDiaLogDysplay(false);
                                resultRetryState = ResultRetryState.ResultSelect;
                                break;
                            case 1://はい
                                SceneManager.LoadScene(string.Format("Main{0}", stageNum));
                                resultRetryState = ResultRetryState.None;
                                break;
                        }
                    }
                    break;
                case ResultRetryState.Exit:
                    ExitSelect(dx);
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                    {
                        switch (exitButtonSelectNum)
                        {
                            case 0://いいえ
                                ExitDiaLogDysplay(false);
                                resultRetryState = ResultRetryState.ResultSelect;
                                break;
                            case 1://はい
                                SceneManager.LoadScene("TitleScene");
                                resultRetryState = ResultRetryState.None;
                                break;
                        }
                    }
                    break;
            }

        }

        /// <summary>
        /// クリア時のボタン選択
        /// 選択したボタンの色を変更します
        /// </summary>
        /// <param name="buttonSelectNum"><選択したボタンの番号/param>
        private void NextStageButtonSelect(int buttonSelectNum)
        {
            switch (buttonSelectNum)
            {
                case 0://次のステージ
                    nextStageButton.sprite = nextStageSelectSprite;
                    exitTitleButton.sprite = exitTitleNormalSprite;
                    retryButton.sprite = retryNormalSprite;
                    return;
                case 1://リトライ
                    nextStageButton.sprite = nextStageNormalSprite;
                    exitTitleButton.sprite = exitTitleNormalSprite;
                    retryButton.sprite = retrySelectSprite;
                    return;
                case 2://やめる
                    nextStageButton.sprite = nextStageNormalSprite;
                    exitTitleButton.sprite = exitTitleSelectSprite;
                    retryButton.sprite = retryNormalSprite;
                    break;
            }
        }
        /// <summary>
        /// リトライ選択時のYesNoボタンセレクト
        /// </summary>
        /// <param name="dx">左右選択</param>
        private void RetrySelect(float dx)
        {
            //右
            if (dx > 0 && countNum == 0)
            {
                countNum++;
                if (retryButtonSelectNum < retryButtonSelectNumMax) retryButtonSelectNum++;
                RetryButtonSelect(retryButtonSelectNum);
            }//左
            else if (dx < 0 && countNum == 0)
            {
                countNum++;
                if (retryButtonSelectNum > 0) retryButtonSelectNum--;
                RetryButtonSelect(retryButtonSelectNum);
            }
            else if (dx == 0 && countNum != 0)
            {
                countNum = 0;
            }
        }
        /// <summary>
        /// 終了選択時のYesNoボタンセレクト
        /// </summary>
        /// <param name="dx"></param>
        private void ExitSelect(float dx)
        {
            //右
            if (dx > 0 && countNum == 0)
            {
                countNum++;
                if (exitButtonSelectNum < exitButtonSelectNumMax) exitButtonSelectNum++;
                ExitButtonSelect(exitButtonSelectNum);
            }//左
            else if (dx < 0 && countNum == 0)
            {
                countNum++;
                if (exitButtonSelectNum > 0) exitButtonSelectNum--;
                ExitButtonSelect(exitButtonSelectNum);
            }
            else if (dx == 0 && countNum != 0)
            {
                countNum = 0;
            }
        }
        /// <summary>
        /// Exitを選択した後のYesNoボタン選択
        /// 選択したボタンの色を変更します
        /// </summary>
        /// <param name="buttonSelectNum">選択したボタンの番号</param>
        private void ExitButtonSelect(int buttonSelectNum)
        {
            switch (buttonSelectNum)
            {
                case 0:
                    exitDoubleCheckDialogYesButton.sprite = doubleCheckDialogYesNormalSprite;
                    exitDoubleCheckDialogNoButton.sprite = doubleCheckDialogNoSelectSprite;
                    return;
                case 1:
                    exitDoubleCheckDialogYesButton.sprite = doubleCheckDialogYesSelectSprite;
                    exitDoubleCheckDialogNoButton.sprite = doubleCheckDialogNoNormalSprite;
                    return;
            }
        }
        /// <summary>
        /// リトライ時YesNoのボタン選択
        /// 選択したボタンの色を変更します
        /// </summary>
        /// <param name="buttonSelectNum">選択したボタンの番号</param>
        private void RetryButtonSelect(int buttonSelectNum)
        {
            switch (buttonSelectNum)
            {
                case 0:
                    retryDoubleCheckDialogYesButton.sprite = doubleCheckDialogYesNormalSprite;
                    retryDoubleCheckDialogNoButton.sprite = doubleCheckDialogNoSelectSprite;
                    return;
                case 1:
                    retryDoubleCheckDialogYesButton.sprite = doubleCheckDialogYesSelectSprite;
                    retryDoubleCheckDialogNoButton.sprite = doubleCheckDialogNoNormalSprite;
                    return;
            }
        }
        /// <summary>
        /// 次のステージに挑戦するかダイアログを表示非表示
        /// </summary>
        /// <param name="isDisplay">表示非表示</param>
        void NextStageDiaLogDisplay(bool isDisplay)
        {
            nextStageDiaLog.SetActive(isDisplay);
        }
        /// <summary>
        /// リトライボタンを押したときのYesNo確認ダイアログ
        /// </summary>
        /// <param name="isDysplay">表示非表示</param>
        void RetryDiaLogDysplay(bool isDysplay)
        {
            retryDoubleCheckDialog.SetActive(isDysplay);
        }
        /// <summary>
        /// 終了ボタンを押したときのYesNo確認ダイアログ
        /// </summary>
        /// <param name="isDysplay">表示非表示</param>
        void ExitDiaLogDysplay(bool isDysplay)
        {
            exitDoubleCheckDialog.SetActive(isDysplay);
        }
        /// <summary>
        /// クリアステージ数を表示します
        /// </summary>
        /// <param name="stageNum">クリアステージ数</param>
        void StageNumDisplay(int stageNum)
        {
            stageNumUI.sprite = stageNumSprite[stageNum];
        }
        /// <summary>
        /// ダメージを表示します
        /// 変数の作りすぎかもなので修正できるといいです
        /// </summary>
        /// <param name="ollDamage">総ダメージ値</param>
        void ResultScoreDisplay(int allDamage)
        {
            var damage = allDamage;
            //1の桁
            var score1 = damage % 10;
            //10の桁
            var score10 = damage / 10 % 10;
            //100の桁
            var score100 = damage / 100 % 10;
            //1000の桁
            var score1000 = damage / 1000 % 10;
            //10000の桁
            var score10000 = damage / 10000 % 10;
            //100000の桁
            var score100000 = damage / 100000 % 10;
            //1000000の桁
            var score1000000 = damage / 1000000 % 10;
            //10000000の桁
            var score10000000 = damage / 10000000 % 10;
            scoreUI[0].sprite = numSprite[score1];
            scoreUI[1].sprite = numSprite[score10];
            scoreUI[2].sprite = numSprite[score100];
            scoreUI[3].sprite = numSprite[score1000];
            scoreUI[4].sprite = numSprite[score10000];
            scoreUI[5].sprite = numSprite[score100000];
            scoreUI[6].sprite = numSprite[score1000000];
            scoreUI[7].sprite = numSprite[score10000000];
        }
        /// <summary>
        /// クリアランクを表示します
        /// </summary>
        /// <param name="allDamage">総ダメージ値</param>
        void ClearRankDisplay(int allDamage)
        {
            //Aランク
            if (allDamage > rankAScore) rankUI.sprite = rankSprite[0];
            //Bランク
            else if (allDamage > rankBScore) rankUI.sprite = rankSprite[1];
            //Cランク
            else
                rankUI.sprite = rankSprite[2];
        }
    }
}