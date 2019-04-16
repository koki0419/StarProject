using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    //-------------Unityコンポーネント関係-------------------
    new Rigidbody[] rigidbody;


    //エフェクト
    [SerializeField] GameObject breakEffect = null;

    GameObject[] childrenOBJ;
    //-------------クラス関係--------------------------------

    //『PlayerMove』を取得します
    PlayerMove playerMove;
    //-------------数値用変数--------------------------------
    //生成する星の数
    [SerializeField] int starNum = 0;

    //ポイントを獲得した回数
    int acquisitionPoint = 0;

    [SerializeField] float deleteTime = 2.0f;

    //Hp
    [SerializeField] float foundationHP;
    //HpMax
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
        childrenOBJ = new GameObject[transform.childCount];
        //子供オブジェクト取得
        for (int i = 0; transform.childCount > i; i++)
        {
            childrenOBJ[i] = transform.GetChild(i).gameObject;
        }

        var hp = 1.0;
        hp -= foundationHP / foundationHPMax;

        breakEffect.SetActive(false);
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

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.name == "middle_01_r" && acquisitionPoint == 0 && playerMove.CanAttackFlag)
        if (collision.gameObject.name == "Player" && acquisitionPoint == 0 && playerMove.CanAttackFlag)
        {
            //Hpをへらす
            foundationHP -= OnDamage(playerMove.AttackPower, playerMove.AttackSpeed);
            //ダメージテキストにアクセスして生成します
            Singleton.Instance.OnDamage(OnDamage(playerMove.AttackPower, playerMove.AttackSpeed), this.gameObject.transform);
            //substanceにアクセスします
            //var hp = 1.0;
            //hp -= (foundationHP / foundationHPMax);
            //OnSliderUpdate((float)hp);
            //ObjHｐがOになった時
            if (foundationHP <= 0)
            {
                if (starNum != 0)
                {
                    Singleton.Instance.starGenerator.OnCreateStar(this.transform.position, starNum);
                }
                //壊れたときにキャラクターと当たり判定を持たなくします
                //レイヤーの変更
                //レイヤーはやりすぎか？コライダー消去の方がよけれは修正要
                gameObject.layer = LayerMask.NameToLayer("BreakObstacls");
                acquisitionPoint++;
                breakEffect.SetActive(true);
                onRemoveObjFlag = true;
                //gameObject.GetComponent<Renderer>().enabled = false;

            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.name == "middle_01_r" && acquisitionPoint == 0 && playerMove.CanAttackFlag)
    //    {
    //        //Hpをへらす
    //        foundationHP -= OnDamage(playerMove.AttackPower, playerMove.AttackSpeed);
    //        //ダメージテキストにアクセスして生成します
    //        Singleton.Instance.OnDamage(OnDamage(playerMove.AttackPower, playerMove.AttackSpeed), this.gameObject.transform);
    //        //substanceにアクセスします
    //        //var hp = 1.0;
    //        //hp -= (foundationHP / foundationHPMax);
    //        //OnSliderUpdate((float)hp);
    //        //ObjHｐがOになった時
    //        if (foundationHP <= 0)
    //        {
    //            if (starNum != 0)
    //            {
    //                Singleton.Instance.starGenerator.OnCreateStar(this.transform.position, starNum);
    //            }
    //            acquisitionPoint++;
    //            breakEffect.SetActive(true);
    //            onRemoveObjFlag = true;
    //            //gameObject.GetComponent<Renderer>().enabled = false;

    //        }
    //    }
    //}



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
}
