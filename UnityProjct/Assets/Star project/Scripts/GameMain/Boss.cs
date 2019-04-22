using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    bool gameClear;
    //-------------Unityコンポーネント関係-------------------
    new Rigidbody[] rigidbody;
    [SerializeField] Animator animator;

    //子供オブジェクト取得用
    //GameObject[] childrenOBJ;// = new GameObject[62];
    //-------------クラス関係--------------------------------

    //『PlayerMove』を取得します
    PlayerMove playerMove;
    //-------------数値用変数--------------------------------
    //生成する星の数
    [SerializeField]
    int starNum;

    //ポイントを獲得した回数
    int acquisitionPoint = 0;

    [SerializeField]
    float deleteTime = 2.0f;

    //防御力
    [SerializeField]
    float defensePower = 0;

    //Hp
    [SerializeField]
    float foundationHP;
    //Hp
    [SerializeField]
    float foundationHPMax;

    //-------------フラグ用変数------------------------------
    bool onRemoveObjFlag = false;

    //----------------Rigidbodyを使用せずに破壊の動きを演出するときに使用----------------
    //初速
    [SerializeField]
    float lnitialVelocity;
    //重力加速度
    [SerializeField]
    float gravityMove;

    public void Init()
    {
        foundationHPMax = foundationHP;
        //『PlayerMove』を取得します
        playerMove = Singleton.Instance.gameSceneController.playerMove;

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

        var hp = 1.0;
        hp -= (foundationHP / foundationHPMax);
        //OnSliderUpdate((float)hp);

        animator = gameObject.GetComponent<Animator>();
    }




    // Update is called once per frame
    void Update()
    {
        //オブジェクトを消去します
        if (onRemoveObjFlag)
        {
            deleteTime -= Time.deltaTime;
            animator.SetBool("Break", true);
            //OnRemoveObj();
            if (deleteTime <= 0)
            {
                //foreach (Transform child in gameObject.transform)
                //{
                //    Destroy(child.gameObject);
                //}
                Destroy(this.gameObject);
            }
        }

        if (foundationHP <= 0)
        {
            Singleton.Instance.gameSceneController.isGameClear = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "middle_01_r" && acquisitionPoint == 0 && playerMove.canAttackFlag)
        {
            foundationHP -= OnDamage(playerMove.AttackPower, playerMove.AttackSpeed);
            Singleton.Instance.OnDamage(OnDamage(playerMove.AttackPower, playerMove.AttackSpeed), this.gameObject.transform);
            var hp = 1.0;
            hp -= (foundationHP / foundationHPMax);
            if (foundationHP <= 0)
            {
                acquisitionPoint++;
                onRemoveObjFlag = true;
            }
        }
    }


    //ダメージ量
    int OnDamage(float damage, float speed)
    {
        float nowHp = damage * speed / defensePower;

        return (int)nowHp;
    }
}
