using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using StarProject.Result;

public class ScoreRank : MonoBehaviour
{

    //
    public Image image_1;
    public Image image_2;
    public Image image_3;

    public int damageimage = 0;

    private Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        Score.all_damageText = damageimage;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(damageimage >= 7700)
        {
            sprite = Resources.Load<Sprite>("Result_Rank01");
        }
        if(damageimage >= 3600)
        {
            sprite = Resources.Load<Sprite>("Result_Rank02");

        }
        if (damageimage >= 0)
        {
            sprite = Resources.Load<Sprite>("Result_Rank03");

        }

    }
}
