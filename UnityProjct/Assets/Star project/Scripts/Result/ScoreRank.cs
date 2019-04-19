using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using StarProject.Result;

public class ScoreRank : MonoBehaviour
{

    //スコアランクの画像
    public GameObject image_1;
    public GameObject image_2;
    public GameObject image_3;

    //ランクを判断する変数
    public int damageimage = 0;

    // Start is called before the first frame update
    void Start()
    {
        //スコアをランク判断用の変数に代入
        Score.scorerank = damageimage;
    }

    // Update is called once per frame
    void Update()
    {
        
        //高ランク
        if(damageimage >= 7700)
        {
            image_1.SetActive(true);
            image_2.SetActive(false);
            image_3.SetActive(false);
        }

        //中ランク
        if(damageimage >= 3600)
        {
            image_1.SetActive(false);
            image_2.SetActive(true);
            image_3.SetActive(false);
        }
        
        //低ランク
        if (damageimage >= 0)
        {
            image_1.SetActive(false);
            image_2.SetActive(false);
            image_3.SetActive(true);
        }

    }
}
