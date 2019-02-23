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
    public Animator animatorComponent;

    new Rigidbody rigidbody;

    //スター獲得エフェクト
    [SerializeField] GameObject starEffect;
    //Hp回復エフェクト
    [SerializeField] GameObject hpRecoveryEffect;
    //-------------クラス関係--------------------------------
    //『Attack』をインスタンス
    Attack attack = new Attack();
    //『ObjectState』をインスタンス
    ObjectState objectState = new ObjectState();

    //-------------数値用変数--------------------------------
    //移動速度を設定します
    [SerializeField] private float moveSpeed;
    //回転速度を設定します
    [SerializeField] private float rotSpeed;

    //ジャンプ力
    [SerializeField] float jumpSpeed;

    private float moveTime;


    private float rot = 90;

    //チャージ量
    float chargeNow = 0.0f;

    [SerializeField] float shotSpeed = 0.5f;

    //チャージポイント使用時のユーザーゲージ上昇量
    [SerializeField] float userChargePonitTime = 0.001f;


    //初期攻撃力
    [SerializeField]
    float foundationoffensivePower;

    //攻撃力
    [SerializeField] float offensivePower;
    public float OffensivePower
    {
        get { return offensivePower; }
    }
    //移動量
    [SerializeField] float speedForce;
    public float SpeedForce
    {
        get { return speedForce; }
    }

    //-------------フラグ用変数------------------------------
    //ジャンプフラグ
    [SerializeField] bool jumpFlag;
    //アタックフラグ
    [SerializeField] bool attackFlag;
    public bool AttackFlag
    {
        get { return attackFlag; }
    }
    //チャージUp
    bool chargeUp = true;

    //☆獲得時フラグ
    bool getStar = false;
    public bool GetStar
    {
        set { getStar = value; }
    }
    //Hp回復フラグ
    bool hpRecoveryFlag = false;
    public bool HpRecoveryFlag
    {
        set { hpRecoveryFlag = value; }
    }

    //初期化
    public void Init()
    {
        //プレイヤーの状態を通常状態に設定します
        objectState.objState = ObjectState.ObjState.Normal;
        //右向きに指定
        transform.rotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
        rigidbody = gameObject.GetComponent<Rigidbody>();
        Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.ChargePoint / Singleton.Instance.gameSceneController.ChargePointMax));

        //----初期化-----
        attackFlag = false;
        jumpFlag = false;
        getStar = false;
        hpRecoveryFlag = false;

        starEffect.SetActive(false);
        hpRecoveryEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float dx = Input.GetAxis("Horizontal");
        switch (objectState.objState)
        {
            case ObjectState.ObjState.Normal:
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

                    float dy = Input.GetAxis("Vertical");
                    //移動
                    Move(dx, dx, jumpFlag);

                    if (Input.GetKeyDown(KeyCode.Y))
                    {

                    }

                    //チャージ
                    if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
                    {
                        //チャージ中
                        Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.ChargePoint / Singleton.Instance.gameSceneController.ChargePointMax));

                    }
                    else if (Input.GetKeyUp(KeyCode.T) && Singleton.Instance.gameSceneController.ChargePoint != 0 || Input.GetKeyUp(KeyCode.Joystick1Button2))
                    {
                        //チャージ終了（チャージゲージを0に戻す）
                        //OnAttack(OnCharge((float)Singleton.Instance.gameSceneController.ChargePoint / 100), new Vector2(dx, dy));

                        offensivePower = OnCharge(Singleton.Instance.gameSceneController.ChargePoint) + foundationoffensivePower;
                        speedForce = OnCharge(Singleton.Instance.gameSceneController.ChargePoint) * 100;

                        Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(0);
                        chargeNow = 0.0f;

                        attackFlag = true;
                        OnAttackMotion(attack.OnAttack(new Vector2(dx, dy), this.gameObject));
                        objectState.objState = ObjectState.ObjState.Attack;
                    }
                }
                break;

            case ObjectState.ObjState.Attack:
                {
                    StartCoroutine(OnAttack(0));
                }
                break;
        }

        if (getStar)
        {
            StartCoroutine(OnGetStar());
        }
        if (hpRecoveryFlag)
        {
            hpRecoveryEffect.SetActive(true);
        }
        else
        {
            hpRecoveryEffect.SetActive(false);
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

        attack.OnAttackBullet(attackName, this.gameObject, shotSpeed, new Vector2(direction.x, direction.y));
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

    //アタック
    void OnAttackMotion(int attackNum)
    {
        animatorComponent.SetInteger("AttackNum", attackNum);
    }

    //アタック時
    public IEnumerator OnAttack(int attackResetNum)
    {
        yield return new WaitForSeconds(1.0f);
        attackFlag = false;
        animatorComponent.SetInteger("AttackNum", attackResetNum);
        objectState.objState = ObjectState.ObjState.Normal;
    }
    //☆獲得時
    public IEnumerator OnGetStar()
    {
        starEffect.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        getStar = false;
        starEffect.SetActive(false);
    }

}
