using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using StarProject.Result;
public class Score : MonoBehaviour
{

    //総ダメージ数
    public int all_damageText;

    //総ダメージ表示用のテキストの取得
    public Text ScoreComPonent;

    //スコアランクへの参照用
    static public int scorerank;

    // Start is called before the first frame update
    void Start()
    {
        //総ダメージ表示用
        all_damageText += ResultScreenController.all_damage;

        //スコアランク参照
        scorerank = all_damageText;
    }

    // Update is called once per frame
    void Update()
    {

        //画面上にスコアを表示
        ScoreComPonent.text = "" + all_damageText;

    }
}
