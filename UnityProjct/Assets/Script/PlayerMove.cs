using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //Attack消費ポイント
    public enum PlayerAttackState
    {
        None,
        Attack1 = 20,
        Attack2 = 40,
        Attack3 = 60,
        Attack4 = 80,
        Attack5 = 100,

    }

    public PlayerAttackState playerAttackState = PlayerAttackState.None;

    //-------------Unityコンポーネント関係-------------------
    // 自分のアニメーションコンポーネント
    private Animator animatorComponent;

    new Rigidbody rigidbody;

    //[SerializeField]
    //GameSceneController gameSceneController;

    //-------------クラス関係--------------------------------
    Attack attack = new Attack();
    //-------------数値用変数--------------------------------
    //移動速度を設定します
    [SerializeField]
    private float moveSpeed;
    //回転速度を設定します
    [SerializeField]
    private float rotSpeed;

    //ジャンプ力
    [SerializeField]
    float jumpSpeed;

    private float moveTime;


    private float rot = 90;

    //チャージ量
    float chargeNow = 0.0f;

    [SerializeField]
    float shotSpeed = 0.5f;

    //チャージポイント使用時のユーザーゲージ上昇量
    [SerializeField]
    float userChargePonitTime = 0.001f;

    //-------------フラグ用変数------------------------------
    [SerializeField]
    bool jumpFlag;

    bool attackFlag;

    bool chargeUp = true;


    //初期化
    public void Init()
    {
        // アニメーションコンポーネント取得
        animatorComponent = gameObject.GetComponentInChildren<Animator>();
        //右向きに指定
        transform.rotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
        rigidbody = gameObject.GetComponent<Rigidbody>();
        Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(OnCharge((float)Singleton.Instance.gameSceneController.ChargePoint / 100));

        //----初期化-----
        jumpFlag = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            jumpFlag = true;
        }
        else
        {
            jumpFlag = false;
        }

        //移動
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        //移動
        Move(dx, dx, jumpFlag);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("攻撃方向 = " + attack.OnAttack(new Vector2(dx, dy),this.gameObject));
            
        }

        if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
        {
            attackFlag = true;

            Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(OnCharge((float)Singleton.Instance.gameSceneController.ChargePoint / 100));
        }
        else if (Input.GetKeyUp(KeyCode.T) && Singleton.Instance.gameSceneController.ChargePoint!=0 || Input.GetKeyUp(KeyCode.Joystick1Button2))
        {

            OnAttack(OnCharge((float)Singleton.Instance.gameSceneController.ChargePoint / 100),new Vector2(dx,dy));
            Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(0);
            chargeNow = 0.0f;
            attackFlag = false;
        }


    }

    //--------------関数-----------------------------
    //移動
    void Move(float x, float horizontal, bool jumpFlag)
    {
        var position = transform.position;
        position.x += x * moveSpeed;// * Time.deltaTime;
        transform.position = position;

        //キャラクターの向き
        if (horizontal > 0)
        {
            transform.rotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
        }
        else if (horizontal < 0)
        {
            transform.rotation = Quaternion.AngleAxis(-rot, new Vector3(0, 1, 0));
        }
        if (jumpFlag)
        {
            var velocity = rigidbody.velocity;
            velocity.y = jumpSpeed; // * Time.deltaTime;
            rigidbody.velocity = velocity;
        }
    }

    //攻撃
    void OnAttack(float charge, Vector2 direction)
    {

        charge *= 100.0f;
        float usedPoint = charge;
        string attackName;

        if (charge >= 80)
        {
            usedPoint = (float)PlayerAttackState.Attack5;
            attackName = "BigBang";//ビックバン
        }
        else if (charge >= 60)
        {
            usedPoint = (float)PlayerAttackState.Attack4;
            attackName = "LastSupper";//最後の晩餐
        }
        else if (charge >= 40)
        {
            usedPoint = (float)PlayerAttackState.Attack3;
            attackName = "Explosion";// 爆発
        }
        else if (charge >= 20)
        {
            usedPoint = (float)PlayerAttackState.Attack2;
            attackName = "Dynamite";// ダイナマイト
        }
        else
        {
            usedPoint = (float)PlayerAttackState.Attack1;
            attackName = "Bullet01";// 通常攻撃
        }
        
        //Singleton.Instance.gameSceneController.ChargePoint -= usedPoint;

        attack.OnAttackBullet(attackName, this.gameObject, shotSpeed,new Vector2(direction.x, direction.y));
    }

    //狙い(チャージ)
    float OnCharge(float charge)
    {
        //マックスのチャージ量
        var chargeMax = charge;


        //チャージ量の+-量
        float chargeProportion = userChargePonitTime;

        if (charge != 0)
        {
            if (0 >= chargeNow)
            {
                chargeUp = true;
            }
            else if (chargeMax <= chargeNow)
            {
                chargeUp = false;
            }


            if (chargeUp)
            {
                chargeNow += chargeProportion;
            }

        }
        return chargeNow;
    }

    

}
