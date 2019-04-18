using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using StarProject.Result;
public class Score : MonoBehaviour
{


    //総ダメージ数
    static public int all_damageText;

    //総ダメージ表示用のテキストの取得
    public Text ScoreComPonent;


    // Start is called before the first frame update
    void Start()
    {
        all_damageText += ResultScreenController.all_damage;

    }

    // Update is called once per frame
    void Update()
    {

        ScoreComPonent.text = "" + all_damageText;

    }
}
