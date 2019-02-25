using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBoxController : MonoBehaviour
{
    //-------------Unityコンポーネント関係-------------------
    new Rigidbody[] rigidbody;


    //子供オブジェクト取得用
    GameObject[] childrenOBJ;// = new GameObject[62];
    //-------------クラス関係--------------------------------

    //『PlayerMove』を取得します
    PlayerMove playerMove;


    public Substance.Game.SubstanceGraph substanceGraph;
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
    float defensePower;

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
    //力の働く方向
    [SerializeField]
    Vector3 gravityDirection;
    public Vector3 GravityDirection
    {
        get { return gravityDirection; }
        set { gravityDirection = GravityDirection; }
    }

    [SerializeField]
    bool onMove;


    public void Init()
    {
        foundationHPMax = foundationHP;
        //『PlayerMove』を取得します
        playerMove = Singleton.Instance.gameSceneController.PlayerMove;
        //破壊したときの動き
        onMove = false;
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
        hp -= (foundationHP / foundationHPMax);
        OnSliderUpdate((float)hp);
    }




    // Update is called once per frame
    void Update()
    {
        if (onMove)
        {
            for (int i = 0; i < childrenOBJ.Length; i++)
            {
                childrenOBJ[i].transform.localPosition += OnBreak(gravityDirection, lnitialVelocity, i, childrenOBJ[i]);
            }
            lnitialVelocity *= gravityMove;
        }


        //オブジェクトを消去します
        if (onRemoveObjFlag)
        {
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

    private Vector3 OnBreak(Vector3 direction, float lnitialVelocity, float gravityMove, GameObject chilld)
    {
        var randx = Random.Range(-1, 2);
        var randy = Random.Range(-1, 2);
        var randz = Random.Range(-1, 2);
        var obj = chilld.transform;
        var x = lnitialVelocity * gravityMove * direction.x * randx;
        var y = lnitialVelocity * gravityMove * direction.y * randy;
        var z = lnitialVelocity * gravityMove * direction.z * randz;

        return new Vector3(x, y, z);
    }
    private Vector3 OnBreak(Vector3 direction, float lnitialVelocity, int objNo, GameObject chilld)
    {
        var randx = 0;
        var randy = 0;
        var randz = 0;
        if (objNo % 2 == 0)
        {
            randx = 1;
            randy = 1;
            randz = 1;
        }
        else if (objNo % 3 == 0)
        {
            randx = -1;
            randy = 1;
            randz = -1;
        }
        else
        {
            randx = -1;
            randy = -1;
            randz = -1;
        }

        var obj = chilld.transform;
        var x = lnitialVelocity * lnitialVelocity * direction.x * randx;
        var y = lnitialVelocity * lnitialVelocity * direction.y * randy;
        var z = lnitialVelocity * lnitialVelocity * direction.z * randz;

        return new Vector3(x, y, z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player_Hand" && acquisitionPoint == 0 && playerMove.AttackFlag)
        {
            foundationHP -= OnDamage(playerMove.OffensivePower, playerMove.SpeedForce);
            Singleton.Instance.OnDamage(OnDamage(playerMove.OffensivePower, playerMove.SpeedForce), this.gameObject.transform);
            var hp = 1.0;
            hp -= (foundationHP / foundationHPMax);
            OnSliderUpdate((float)hp);
            if (foundationHP <= 0)
            {
                for (int i = 0; transform.childCount > i; i++)
                {
                    childrenOBJ[i].GetComponent<Rigidbody>().isKinematic = false;
                }
                Singleton.Instance.starGenerator.OnCreateStar(this.transform.position, starNum);
                acquisitionPoint++;
                onRemoveObjFlag = true;
            }
        }
    }

    //オブジェクトを小さくして消します
    void OnRemoveObj()
    {
        for (int i = 0; i < childrenOBJ.Length; i++)
        {
            childrenOBJ[i].transform.localScale -= new Vector3(0.01f, 0.0f, 0.01f);

        }
    }


    //ダメージ量
    int OnDamage(float damage, float speed)
    {
        float nowHp = damage * speed / defensePower;

        return (int)nowHp;
    }
}
