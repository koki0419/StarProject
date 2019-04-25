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
            ResultSerect,
            RetrySerect,
        }
        ResultRetryState resultRetryState = ResultRetryState.None;

        [SerializeField] Animator resultAnimator;

        //ネクストステージダイアログ
        [SerializeField] private GameObject nextStageDiaLog = null;
        [SerializeField] private Image nextStageButton = null;
        [SerializeField] private Image exitTitleButton = null;
        [SerializeField] private Image retryButton = null;
        [SerializeField] private GameObject resultRetryDiaLog = null;
        [SerializeField] private Image resultRetryYesButton = null;
        [SerializeField] private Image resultRetryNoButton = null;
        [Header("次のステージ選択ダイアログ用画像")]
        [SerializeField] private Sprite nextStageNormalSprite = null;
        [SerializeField] private Sprite nextStageSelectSprite = null;
        [SerializeField] private Sprite exitTitleNormalSprite = null;
        [SerializeField] private Sprite exitTitleSelectSprite = null;
        [SerializeField] private Sprite retryNormalSprite = null;
        [SerializeField] private Sprite retrySelectSprite = null;
        //リトライダイアログ
        [SerializeField] private Sprite retryYesNormalSprite = null;
        [SerializeField] private Sprite retryYesSelectSprite = null;
        [SerializeField] private Sprite retryNoNormalSprite = null;
        [SerializeField] private Sprite retryNoSelectSprite = null;
        //ボタン選択時にGetAxisを使用するので回数制限に使用します
        int countNum = 0;
        //ボタン選択番号
        int nextStageButtonSelectNum = 0;
        int nextStageButtonSelectNumMax = 3;
        int retryButtonSelectNum = 0;
        int retryButtonSelectNumMax = 2;
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
            ResultRetryDiaLogDysplay(false);
            nextStageButtonSelectNum = 0;
            NextStageButtonSelect(nextStageButtonSelectNum);
            StageNumDisplay(GameSceneController.stageNum);
            ResultScoreDisplay(all_damage);
            ClearRankDisplay(all_damage);
            resultState = ResultState.ResultAnimation;
            resultRetryState = ResultRetryState.ResultSerect;
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

        void ResultAnimation()
        {
            AnimatorStateInfo animInfo = resultAnimator.GetCurrentAnimatorStateInfo(0);
            if (animInfo.normalizedTime < 1.0f)
            {
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
            {
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
                case ResultRetryState.ResultSerect:
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
                                ResultRetryButtonSelect(retryButtonSelectNum);
                                ResultRetryDiaLogDysplay(true);
                                resultRetryState = ResultRetryState.RetrySerect;
                                break;
                            case 2://やめる
                                SceneManager.LoadScene("TitleScene");
                                resultRetryState = ResultRetryState.None;
                                break;
                        }
                    }
                    break;
                case ResultRetryState.RetrySerect:
                    //左
                    if (dx < 0 && countNum == 0)
                    {
                        countNum++;
                        if (retryButtonSelectNum > 0) retryButtonSelectNum--;
                        ResultRetryButtonSelect(retryButtonSelectNum);
                    }//右
                    else if (dx > 0 && countNum == 0)
                    {
                        countNum++;
                        if (retryButtonSelectNum < retryButtonSelectNumMax) retryButtonSelectNum++;
                        ResultRetryButtonSelect(retryButtonSelectNum);
                    }
                    else if (dx == 0 && countNum != 0)
                    {
                        countNum = 0;
                    }

                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                    {
                        switch (retryButtonSelectNum)
                        {
                            case 0:
                                ResultRetryDiaLogDysplay(false);
                                resultRetryState = ResultRetryState.ResultSerect;
                                break;
                            case 1://リトライ
                                SceneManager.LoadScene(string.Format("Main{0}", stageNum));
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
        /// リトライ選択YesNoボタンの選択
        /// </summary>
        /// <param name="buttonSelectNum">選択したボタンに種類</param>
        private void ResultRetryButtonSelect(int buttonSelectNum)
        {

            switch (buttonSelectNum)
            {
                case 0://No
                    resultRetryYesButton.sprite = retryYesNormalSprite;
                    resultRetryNoButton.sprite = retryNoSelectSprite;
                    return;
                case 1://Yes
                    resultRetryYesButton.sprite = retryYesSelectSprite;
                    resultRetryNoButton.sprite = retryNoNormalSprite;
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
        void ResultRetryDiaLogDysplay(bool isDysplay)
        {
            resultRetryDiaLog.SetActive(isDysplay);
        }
        /// <summary>
        /// クリアステージ数を表示します
        /// </summary>
        /// <param name="stageNum">クリアステージ数</param>
        void StageNumDisplay(int stageNum)
        {
            stageNumUI.sprite = stageNumSprite[stageNum-1];
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