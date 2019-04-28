using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBoxController : MonoBehaviour
{
    //-------------Unityコンポーネント関係-------------------
    new Rigidbody[] rigidbody;


    //子供オブジェクト取得用
    [SerializeField] GameObject[] childrenOBJ = null;// = new GameObject[62];

    GameObject[] prefabObj;
    [SerializeField] int prefabsNum = 10;
    //-------------クラス関係--------------------------------

    //『PlayerMove』を取得します
    PlayerMove playerMove;


    public Substance.Game.SubstanceGraph substanceGraph;
    //-------------数値用変数--------------------------------
    //生成する星の数
    [SerializeField] int starNum = 0;

    //ポイントを獲得した回数
    int acquisitionPoint = 0;

    [SerializeField] float deleteTime = 2.0f;

    //防御力
    //[SerializeField] float defensePower;

    //Hp
    [SerializeField] float foundationHP;
    //Hp
    float foundationHPMax;

    //-------------フラグ用変数------------------------------
    bool onRemoveObjFlag = false;


    [SerializeField] bool onMove;


    public void Init()
    {
        foundationHPMax = foundationHP;
        //『PlayerMove』を取得します
        playerMove = Singleton.Instance.gameSceneController.playerMove;
        //破壊したときの動き
        // bool producedWhenDestroyed = false;
        //オブジェクトを削除するかどうか
        onRemoveObjFlag = false;
        //ポイントを獲得した回数
        acquisitionPoint = 0;
        //子供オブジェクトを取得
        //childrenOBJ = new GameObject[transform.childCount];
        //子供オブジェクト取得
        //for (int i = 0; transform.childCount > i; i++)
        //{
        //    childrenOBJ[i] = transform.GetChild(i).gameObject;
        //}
        prefabObj = new GameObject[prefabsNum];

        var hp = 1.0;
        hp -= foundationHP / foundationHPMax;
         OnSliderUpdate((float)hp);
    }




    // Update is called once per frame
    void Update()
    {
        //オブジェクトを消去します
        if (onRemoveObjFlag)
        {
            //
            //onRemoveObjFlag = false;
            deleteTime -= Time.deltaTime;
            OnRemoveObj();
            if (deleteTime <= 0)
            {
                foreach (Transform child in gameObject.transform)
                {
                    Destroy(child.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
    }
    public void OnSliderUpdate(float value) //******************************
    {
        substanceGraph.SetInputFloat("dust_Level", value);//******************************
        substanceGraph.QueueForRender();    //******************************
        substanceGraph.RenderAsync();       //******************************
    }                                       //******************************
                                            //*******************************************************************************



    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "middle_01_r" && acquisitionPoint == 0 && playerMove.canDamage)
        {
            //Hpをへらす
            foundationHP -= OnDamage(playerMove.attackPower, playerMove.attackSpeed);
            //ダメージテキストにアクセスして生成します
            Singleton.Instance.OnDamage(OnDamage(playerMove.attackPower, playerMove.attackSpeed), this.gameObject.transform);
            //substanceにアクセスします
            //var hp = 1.0;
            //hp -= (foundationHP / foundationHPMax);
            //OnSliderUpdate((float)hp);
            //ObjHｐがOになった時
            if (foundationHP <= 0)
            {
                //for (int i = 0; transform.childCount > i; i++)
                //{
                //    childrenOBJ[i].GetComponent<Rigidbody>().isKinematic = false;
                //}
                if (starNum != 0)
                {
                    Singleton.Instance.starGenerator.OnCreateStar(this.transform.position, starNum);
                }
                acquisitionPoint++;
                ProducedWhenDestroyed();
                onRemoveObjFlag = true;
                gameObject.GetComponent<Renderer>().enabled = false;

            }
        }
    }

    //オブジェクトを小さくして消します
    void OnRemoveObj()
    {
        for (int i = 0; i < childrenOBJ.Length; i++)
        {
            if(prefabObj[i].transform.localScale.x > 0 || prefabObj[i].transform.localScale.z > 0)
            prefabObj[i].transform.localScale -= new Vector3(0.01f, 0.0f, 0.01f);

        }
    }


    /// <summary>
    /// ダメージが与えられたとき
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    int OnDamage(float damage, float speed)
    {
        float nowHp = damage * speed;

        return (int)nowHp;
    }
    //破壊されたときの処理
    void ProducedWhenDestroyed()
    {
        for (int i = 0; i < childrenOBJ.Length; i++)
        {
            Vector3 pos = new Vector3(0,0,0);

            prefabObj[i] = Instantiate(childrenOBJ[i]);
            prefabObj[i].transform.parent = gameObject.transform;
            prefabObj[i].transform.localPosition = pos;
        }
    }
}
