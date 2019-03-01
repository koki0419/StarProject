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

    //ビーストモード消費ポイント
    public enum PlayerBeastModeState
    {
        None,
        AttacPower = 2,     //攻撃力2倍
        SpeedPower = 2,     //スピード2倍
        StarCost = 1,       //☆消費
        Resilience = 0,        //回復力
        PhysicalFitnessCost = 2,//体力消費

    }

    public PlayerBeastModeState playerBeastModeState = PlayerBeastModeState.None;

    //-------------Unityコンポーネント関係-------------------
    // 自分のアニメーションコンポーネント
    public Animator animatorComponent;

    new Rigidbody rigidbody;

    //スター獲得エフェクト
    [SerializeField] GameObject starEffect;
    //Hp回復エフェクト
    [SerializeField] GameObject hpRecoveryEffect;
    //チャージエフェクト1
    [SerializeField] GameObject chargeEffect1;
    bool chargeEffectFlag1 = false;
    //チャージエフェクト2
    [SerializeField] GameObject chargeEffect2;
    bool chargeEffectFlag2 = false;

    //ビーストモードエフェクト
    [SerializeField] GameObject beastModeEffect;
    public GameObject BeastModeEffect
    {
        get { return beastModeEffect; }
        set { beastModeEffect = value; }
    }

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
    [SerializeField] float foundationoffensivePower;
    //初期移動量
    [SerializeField] float foundationSpeedForce;

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


    //ビーストモード攻撃力
    float beastAttackPower;

    //-------------フラグ用変数------------------------------
    //ジャンプフラグ
    [SerializeField] bool jumpFlag;
    //アタックフラグ
    [SerializeField] bool attackFlag;
    public bool AttackFlag
    {
        get { return attackFlag; }
    }

    //地面との接触
    bool isGroundFlag;
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
    //キャラクターの向き
    bool rightDirection;
    bool leftDirection;

    //ビーストモード
    [SerializeField] bool beastModeFlag = false;
    public bool BeastModeFlag
    {
        get { return beastModeFlag; }
        set { beastModeFlag = value; }
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
        isGroundFlag = false;
        getStar = false;
        hpRecoveryFlag = false;
        beastModeFlag = false;

        starEffect.SetActive(false);
        hpRecoveryEffect.SetActive(false);


        //デバック用
        chargeEffectFlag1 = false;
        chargeEffectFlag2 = false;
        chargeEffect1.SetActive(chargeEffectFlag1);
        chargeEffect2.SetActive(chargeEffectFlag2);
        beastModeEffect.SetActive(beastModeFlag);
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        float dy = Input.GetAxis("Vertical");
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
                    float dx = Input.GetAxis("Horizontal");

                    //通常時
                    if (!beastModeFlag)
                    {
                        //移動
                        Move(dx, dx, jumpFlag);

                        //チャージ
                        if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
                        {
                            //チャージ中
                            Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.ChargePoint / Singleton.Instance.gameSceneController.ChargePointMax));


                            //チャージエフェクトデバック---------------------------
                            var charge = OnCharge(Singleton.Instance.gameSceneController.ChargePoint / Singleton.Instance.gameSceneController.ChargePointMax);

                            if (charge < 0.5)
                            {
                                chargeEffectFlag1 = true;
                                chargeEffectFlag2 = false;
                            }
                            else
                            {
                                chargeEffectFlag1 = false;
                                chargeEffectFlag2 = true;
                            }

                            chargeEffect1.SetActive(chargeEffectFlag1);
                            chargeEffect2.SetActive(chargeEffectFlag2);



                            //--------------------------------------------------
                        }
                        //else if (Input.GetKeyUp(KeyCode.T) && Singleton.Instance.gameSceneController.ChargePoint != 0 || Input.GetKeyUp(KeyCode.Joystick1Button2))
                        else if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Joystick1Button2))
                        {
                            //通常時
                            //if (!beastModeFlag)
                            //{
                            //チャージ終了（チャージゲージを0に戻す）
                            offensivePower = OnCharge(Singleton.Instance.gameSceneController.ChargePoint) + foundationoffensivePower;
                            speedForce = OnCharge(Singleton.Instance.gameSceneController.ChargePoint) * 100 + foundationSpeedForce;
                            //}
                            //ビーストモード
                            // else
                            //{
                            //チャージ終了（チャージゲージを0に戻す）
                            //offensivePower = (OnCharge(Singleton.Instance.gameSceneController.ChargePoint) + foundationoffensivePower) * (int)PlayerBeastModeState.AttacPower;
                            //speedForce = (OnCharge(Singleton.Instance.gameSceneController.ChargePoint) * 100) * (int)PlayerBeastModeState.SpeedPower;
                            //  }
                            Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(0);
                            chargeNow = 0.0f;

                            attackFlag = true;
                            OnAttackMotion(attack.OnAttack(new Vector2(dx, dy), this.gameObject));
                            chargeEffect1.SetActive(false);
                            chargeEffect2.SetActive(false);
                            objectState.objState = ObjectState.ObjState.Attack;
                        }
                    }
                    //ビーストモード
                    else
                    {
                        //移動
                        Move(dx * (int)PlayerBeastModeState.SpeedPower, dx, jumpFlag);

                        //チャージ
                        if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
                        {
                            //チャージエフェクトデバック---------------------------
                            chargeEffectFlag1 = false;
                            chargeEffectFlag2 = true;

                            chargeEffect1.SetActive(chargeEffectFlag1);
                            chargeEffect2.SetActive(chargeEffectFlag2);
                        }
                        else if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Joystick1Button2))
                        {
                            //チャージ終了（チャージゲージを0に戻す）
                            offensivePower = beastAttackPower * (float)PlayerBeastModeState.AttacPower + foundationoffensivePower;
                            speedForce = beastAttackPower * (int)PlayerBeastModeState.SpeedPower + foundationSpeedForce;


                            attackFlag = true;
                            OnAttackMotion(attack.OnAttack(new Vector2(dx, dy), this.gameObject));
                            chargeEffect1.SetActive(false);
                            chargeEffect2.SetActive(false);
                            objectState.objState = ObjectState.ObjState.Attack;

                            if (dy >= 1)
                            {
                                isGroundFlag = false;
                                jumpFlag = true;
                            }
                        }
                    }
                }
                break;

            case ObjectState.ObjState.Attack:
                {
                    MoveAttack(speedForce / 10,dy);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!chargeEffectFlag1)
            {
                chargeEffectFlag1 = true;
                chargeEffectFlag2 = false;
            }
            else
            {
                chargeEffectFlag1 = false;
                chargeEffectFlag2 = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!beastModeFlag && Singleton.Instance.gameSceneController.ChargePoint == Singleton.Instance.gameSceneController.ChargePointMax)
            {
                beastModeFlag = true;
                beastAttackPower = Singleton.Instance.gameSceneController.ChargePointMax;
            }
            else
            {
                beastModeFlag = false;
            }
            beastModeEffect.SetActive(beastModeFlag);

        }
    }


    //--------------関数-----------------------------
    //地面との当たり判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGroundFlag = true;
            jumpFlag = false;
        }
    }

    //移動
    void Move(float x, float horizontal, bool jumpFlag)
    {
        var position = transform.position;
        position.x += x * moveSpeed; //Time.deltaTime;
        transform.position = position;

        //キャラクターの向き
        if (horizontal > 0)
        {
            transform.rotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            rightDirection = true;
            leftDirection = false;
        }
        else if (horizontal < 0)
        {
            transform.rotation = Quaternion.AngleAxis(-rot, new Vector3(0, 1, 0));
            rightDirection = false;
            leftDirection = true;
        }
        if (jumpFlag)
        {
            isGroundFlag = false;
            var velocity = rigidbody.velocity;
            velocity.y = jumpSpeed; // * Time.deltaTime;
            rigidbody.velocity = velocity;
        }
    }

    //攻撃時の移動
    void MoveAttack(float speedForce, float horizontal)
    {
        if (horizontal == 0)
        {
            var position = transform.position;
            if (rightDirection && !leftDirection)
            {
                position.x += speedForce * Time.deltaTime;
            }
            else
            {
                position.x -= speedForce * Time.deltaTime;
            }
            transform.position = position;
        }else if (horizontal >= 1)
        {
            var velocity = rigidbody.velocity;
            velocity.y = speedForce; // * Time.deltaTime;
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
        animatorComponent.SetBool("Ground", isGroundFlag);

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
