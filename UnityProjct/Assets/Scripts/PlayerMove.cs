using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //攻撃時設定数値
    public enum PlayerAttackIndex
    {
        None,
        AttackNormal = 1001,
        AttackUp = 1010,
        AttackDown = 1011,

    }

    public PlayerAttackIndex payerAttackIndex = PlayerAttackIndex.None;

    //オブジェクトステータス
    public enum ObjState
    {
        None,
        Normal,
        Attack,
        DestroyMode,
        OnCharge,

    }
    public ObjState objState = ObjState.None;



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
    [Header("エフェクト関係")]
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
   /* [SerializeField] GameObject beastModeEffect;
    public GameObject BeastModeEffect
    {
        get { return beastModeEffect; }
        set { beastModeEffect = value; }
    }*/

    //-------------クラス関係--------------------------------
    //『Attack』をインスタンス
    Attack attack = new Attack();
    //『ObjectState』をインスタンス
    //ObjectState objectState = new ObjectState();

    //-------------数値用変数--------------------------------
    [Header("プレイヤー情報")]
    //移動速度を設定します
    [SerializeField] private float moveSpeed;

    //ジャンプ力
    [SerializeField] float jumpSpeed;
    //ジャンプ重力
    [SerializeField] float jumpgravity;

    //チャージポイント使用時のユーザーゲージ上昇量
    [SerializeField] float userChargePonitTime = 0.001f;

    //デストロイモード使用時のユーザーゲージ減少量
    [SerializeField] float destroyModeChargePonitTime = 0.001f;

    //回転速度を設定します
    [SerializeField] private float rotSpeed;


    [Header("プレイヤー攻撃初期情報")]
    //初期攻撃力
    [SerializeField] float foundationoffensivePower;
    //初期移動量
    [SerializeField] float foundationSpeedForce;

    [Header("チャージ回数に掛け算される力")]
    //攻撃力
    [SerializeField] float offensivePower;

    //移動量
    [SerializeField] float speedForce;


    //現在のチャージ量
    float chargeNow = 0.0f;
    //何回チャージしたか
    int chargeCount = 0;
    //チャージ上限
    int chargeCountMax = 0;
    //回転
    private float rot = 90;

    //ジャンプボタンを押している時間
    float jumpPushKeyTime = 0;

    //private float moveTime;

    //[Range(0.0f, 100.0f)]
    //[SerializeField] float jumpTimeLimit;

    //弾の発射スピード
    //[SerializeField] float shotSpeed = 0.5f;



    //ビーストモード攻撃力
    float beastAttackPower;

    //攻撃時Speed
    [SerializeField] float attackSpeed;
    public float AttackSpeed
    {
        get { return attackSpeed; }
    }

    //攻撃時パワー
    [SerializeField] float attackPower;
    public float AttackPower
    {
        get { return attackPower; }
    }

    //-------------フラグ用変数------------------------------
    [Header("各種フラグ")]
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
    [SerializeField] bool destroyModeFlag = false;
    public bool DestroyModeFlag
    {
        get { return destroyModeFlag; }
        set { destroyModeFlag = value; }
    }

    //チャージ中かどうか
    bool Oncharge = false;

    //初期化
    public void Init()
    {
        //プレイヤーの状態を通常状態に設定します
        objState = ObjState.Normal;
        //右向きに指定
        transform.rotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
        //Rigidbodyを取得します
        rigidbody = gameObject.GetComponent<Rigidbody>();
        //Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.ChargePoint / Singleton.Instance.gameSceneController.ChargePointMax));
        //チャージゲージをリセットします
        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(0);
        //----初期化-----
        attackFlag = false;
        jumpFlag = false;
        isGroundFlag = false;
        getStar = false;
        hpRecoveryFlag = false;
        destroyModeFlag = false;

        starEffect.SetActive(false);
        hpRecoveryEffect.SetActive(false);


        //デバック用//エフェクト
        chargeEffectFlag1 = false;
        chargeEffectFlag2 = false;
        chargeEffect1.SetActive(chargeEffectFlag1);
        chargeEffect2.SetActive(chargeEffectFlag2);
        //beastModeEffect.SetActive(destroyModeFlag);
    }

    // Update is called once per frame
    public void OnUpdate(float deltaTime)
    {
        float dy = Input.GetAxis("Vertical");

        //移動
        float dx = Input.GetAxis("Horizontal");

        if (dx != 0 && !attack)
        {
            animatorComponent.SetBool("walkFlag", true);
        }else
        {
            animatorComponent.SetBool("walkFlag", false);
        }
        switch (objState)
        {
            //通常時
            case ObjState.Normal:
                {
                    if (!attackFlag)
                    {
                        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                        {
                            jumpFlag = true;
                            animatorComponent.SetBool("walkFlag", false);
                        }
                        //else if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Joystick1Button0))
                        //{
                        //    jumpPushKeyTime += Time.deltaTime;
                        //    if(jumpPushKeyTime > jumpTimeLimit && isGroundFlag)
                        //    {
                        //        isGroundFlag = false;
                        //        jumpFlag = true;
                        //    }
                        //}
                        //else if (Input.GetKeyUp(KeyCode.J) || Input.GetKeyUp(KeyCode.Joystick1Button0))
                        //{
                        //    if (jumpPushKeyTime < jumpTimeLimit && isGroundFlag)
                        //    {
                        //        isGroundFlag = false;
                        //        jumpFlag = true;
                        //    }
                        //    jumpPushKeyTime = 0.0f;
                        //    //jumpFlag = true;

                        //}
                        //移動
                        Move(dx, jumpPushKeyTime, deltaTime);
                        //Debug.Log("jumpPushKeyTime" + jumpPushKeyTime);
                    }
                    else
                    {
                        DirectionMove(dx);
                        rigidbody.isKinematic = true;
                    }

                    //チャージ
                    if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
                    {
                        animatorComponent.SetBool("walkFlag", false);
                        attackFlag = true;

                        //チャージ中
                        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.chargePointManager.ChargePoint / 10));

                        //チャージエフェクトデバック---------------------------
                        if (chargeCount < 3)
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

                    }
                    else if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Joystick1Button2))
                    {
                        //チャージ終了（チャージゲージを0に戻す）
                        attackPower = chargeCount * offensivePower + foundationoffensivePower;
                        attackSpeed = chargeCount * speedForce + foundationSpeedForce;

                        //チャージゲージをリセットします
                        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(0);
                        //チャージ中☆を戻します
                        Singleton.Instance.gameSceneController.starChargeController.UpdateStarUI(chargeCount);
                        chargeCount = 0;
                        chargeNow = 0.0f;

                        OnAttackMotion(attack.OnAttack(new Vector2(dx, dy), this.gameObject));
                        chargeEffect1.SetActive(false);
                        chargeEffect2.SetActive(false);

                        rigidbody.isKinematic = false;


                        objState = ObjState.Attack;
                    }
                }
                break;
                //デストロイモード
            case ObjState.DestroyMode:
                {
                    if (!attackFlag)
                    {
                        //移動
                        Move(dx * (int)PlayerBeastModeState.SpeedPower, jumpPushKeyTime, deltaTime);
                    }
                    else
                    {
                        DirectionMove(dx);
                        rigidbody.isKinematic = true;
                    }
                    //チャージ
                    if (Input.GetKey(KeyCode.T) || Input.GetKeyDown(KeyCode.Joystick1Button2))
                    {
                        animatorComponent.SetBool("walkFlag", false);
                        attackFlag = true;
                        //チャージエフェクトデバック---------------------------
                        chargeEffectFlag1 = false;
                        chargeEffectFlag2 = true;

                        chargeEffect1.SetActive(chargeEffectFlag1);
                        chargeEffect2.SetActive(chargeEffectFlag2);
                    }
                    if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Joystick1Button2))
                    {

                        //チャージ終了（チャージゲージを0に戻す）
                        attackPower = chargeCountMax * offensivePower + foundationoffensivePower;
                        attackSpeed = chargeCountMax * speedForce + foundationSpeedForce;

                        //チャージゲージをリセットします
                        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(0);
                        //チャージ中☆を戻します
                        Singleton.Instance.gameSceneController.starChargeController.UpdateStarUI(chargeCount);
                        chargeCount = 0;
                        chargeNow = 0.0f;


                        OnAttackMotion(attack.OnAttack(new Vector2(dx, dy), this.gameObject));
                        chargeEffect1.SetActive(false);
                        chargeEffect2.SetActive(false);

                        rigidbody.isKinematic = false;

                        objState = ObjState.Attack;

                    }

                }
                break;

            case ObjState.Attack:
                {
                    MoveAttack(attackSpeed / 10, dy);
                    StartCoroutine(OnAttack(0));
                }
                break;

                //case ObjState.OnCharge:
                //{
                //DirectionMove(dx);
                //rigidbody.isKinematic = true;
                ////通常時
                //if (!destroyModeFlag)
                //{
                //    //チャージ
                //    if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
                //    {
                //        //チャージ中
                //        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.ChargePoint / 10));

                //        //チャージエフェクトデバック---------------------------
                //        if (chargeCount < 3)
                //        {
                //            chargeEffectFlag1 = true;
                //            chargeEffectFlag2 = false;
                //        }
                //        else
                //        {
                //            chargeEffectFlag1 = false;
                //            chargeEffectFlag2 = true;
                //        }

                //        chargeEffect1.SetActive(chargeEffectFlag1);
                //        chargeEffect2.SetActive(chargeEffectFlag2);

                //    }
                //    if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Joystick1Button2))
                //    {

                //        //チャージ終了（チャージゲージを0に戻す）
                //        attackPower = chargeCount * offensivePower + foundationoffensivePower;
                //        attackSpeed = chargeCount * speedForce + foundationSpeedForce;

                //        //Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(0);
                //        //チャージゲージをリセットします
                //        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(0);
                //        //チャージ中☆を戻します
                //        Singleton.Instance.gameSceneController.starChargeController.UpdateStarUI(chargeCount);
                //        chargeCount = 0;
                //        chargeNow = 0.0f;

                //        attackFlag = true;
                //        OnAttackMotion(attack.OnAttack(new Vector2(dx, dy), this.gameObject));
                //        chargeEffect1.SetActive(false);
                //        chargeEffect2.SetActive(false);

                //        rigidbody.isKinematic = false;

                //        objState = ObjState.Attack;

                //    }
                //}
                //else if (destroyModeFlag)
                //{

                //    //チャージ
                //    if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
                //    {
                //        //チャージ中
                //        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.ChargePoint / 10));
                //    }
                //    else if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Joystick1Button2))
                //    {

                //        //チャージ終了（チャージゲージを0に戻す）
                //        attackPower = chargeCount * offensivePower + foundationoffensivePower;

                //        attackSpeed = chargeCount * speedForce + foundationSpeedForce;

                //        //Singleton.Instance.gameSceneController.chargeUIController.UseUpdateChargePoint(0);
                //        //チャージゲージをリセットします
                //        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(0);
                //        //チャージ中☆を戻します
                //        Singleton.Instance.gameSceneController.starChargeController.UpdateStarUI(chargeCount);
                //        chargeCount = 0;
                //        chargeNow = 0.0f;

                //        attackFlag = true;
                //        OnAttackMotion(attack.OnAttack(new Vector2(dx, dy), this.gameObject));
                //        chargeEffect1.SetActive(false);
                //        chargeEffect2.SetActive(false);

                //        rigidbody.isKinematic = false;

                //        objState = ObjState.Attack;
                //    }
        }

        //}
        //break;


        // }

        if (getStar)
        {
            StartCoroutine(OnGetStar());
        }
        //if (hpRecoveryFlag)
        //{
        //    hpRecoveryEffect.SetActive(true);
        //}
        //else
        //{
        //    hpRecoveryEffect.SetActive(false);
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (!chargeEffectFlag1)
        //    {
        //        chargeEffectFlag1 = true;
        //        chargeEffectFlag2 = false;
        //    }
        //    else
        //    {
        //        chargeEffectFlag1 = false;
        //        chargeEffectFlag2 = true;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick1Button3))
        //{
        //    //Debug.Log("ChargePoint" + Singleton.Instance.gameSceneController.ChargePoint);
        //    //Debug.Log("ChargePointMax" + Singleton.Instance.gameSceneController.ChargePointMax);

        //    if (!destroyModeFlag && Singleton.Instance.gameSceneController.ChargePoint == Singleton.Instance.gameSceneController.ChargePointMax && Singleton.Instance.gameSceneController.ChargePoint != 0)
        //    {
        //        Singleton.Instance.gameSceneController.DestroyCount = 10;
        //        destroyModeFlag = true;
        //        beastAttackPower = Singleton.Instance.gameSceneController.ChargePointMax;
        //        //Debug.Log("chargePoint" + Singleton.Instance.gameSceneController.ChargePoint);
        //        //Debug.Log("chargePointMax" + Singleton.Instance.gameSceneController.ChargePointMax);
        //        //Debug.Log("StarChildCount" + Singleton.Instance.gameSceneController.StarChildCount);
        //    }
        //    else
        //    {
        //        destroyModeFlag = false;
        //        if (Singleton.Instance.gameSceneController.ChargePoint < 10)
        //        {
        //            Singleton.Instance.gameSceneController.ChargePoint = 0;
        //        }
        //        else if (Singleton.Instance.gameSceneController.ChargePoint < 20)
        //        {
        //            Singleton.Instance.gameSceneController.ChargePoint = 10;
        //        }
        //        else if (Singleton.Instance.gameSceneController.ChargePoint < 30)
        //        {
        //            Singleton.Instance.gameSceneController.ChargePoint = 20;
        //        }
        //        else if (Singleton.Instance.gameSceneController.ChargePoint < 40)
        //        {
        //            Singleton.Instance.gameSceneController.ChargePoint = 30;
        //        }
        //        else if (Singleton.Instance.gameSceneController.ChargePoint < 50)
        //        {
        //            Singleton.Instance.gameSceneController.ChargePoint = 40;
        //        }




        //        Singleton.Instance.gameSceneController.starChargeController.BanStar(Singleton.Instance.gameSceneController.BanStarCheck(Singleton.Instance.gameSceneController.ChargePointMax));
        //        //Singleton.Instance.gameSceneController.ChargePointMax -= 10;
        //    }
        //    beastModeEffect.SetActive(destroyModeFlag);

        //}
    }


    //--------------関数-----------------------------
    //地面との当たり判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animatorComponent.SetBool("walkFlag", true);
            isGroundFlag = true;
            jumpFlag = false;
        }
    }

    //移動
    void Move(float horizontal, float jTime, float deltaTime)
    {
        var position = transform.position;
        position.x += horizontal * moveSpeed * deltaTime;
        transform.position = position;

        var velocity = rigidbody.velocity;
        //キャラクターの向き
        if (horizontal > 0)
        {
            transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            rightDirection = true;
            leftDirection = false;
        }
        else if (horizontal < 0)
        {
            transform.localRotation = Quaternion.AngleAxis(-rot, new Vector3(0, 1, 0));
            rightDirection = false;
            leftDirection = true;
        }
        if (jumpFlag)
        {
            jumpFlag = false;
            isGroundFlag = false;

            velocity.y += jumpSpeed;

            animatorComponent.SetBool("walkFlag", true);
        }

        velocity.y += Physics.gravity.y * jumpgravity * deltaTime;

        rigidbody.velocity = velocity;
    }

    void DirectionMove(float horizontal)
    {
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
        }
        else if (horizontal >= 1)
        {
            var velocity = rigidbody.velocity;
            velocity.y = speedForce; // * Time.deltaTime;
            rigidbody.velocity = velocity;
        }

    }
    //攻撃
    //void OnAttack(float charge, Vector2 direction)
    //{

    //    charge *= 100.0f;
    //    float usedPoint = charge;
    //    string attackName;

    //    if (charge >= 80)
    //    {
    //        usedPoint = (float)PlayerAttackState.Attack5;
    //        attackName = "BigBang";//ビックバン
    //    }
    //    else if (charge >= 60)
    //    {
    //        usedPoint = (float)PlayerAttackState.Attack4;
    //        attackName = "LastSupper";//最後の晩餐
    //    }
    //    else if (charge >= 40)
    //    {
    //        usedPoint = (float)PlayerAttackState.Attack3;
    //        attackName = "Explosion";// 爆発
    //    }
    //    else if (charge >= 20)
    //    {
    //        usedPoint = (float)PlayerAttackState.Attack2;
    //        attackName = "Dynamite";// ダイナマイト
    //    }
    //    else
    //    {
    //        usedPoint = (float)PlayerAttackState.Attack1;
    //        attackName = "Bullet01";// 通常攻撃
    //    }

    //    //Singleton.Instance.gameSceneController.ChargePoint -= usedPoint;

    //    attack.OnAttackBullet(attackName, this.gameObject, shotSpeed, new Vector2(direction.x, direction.y));
    //}

    //狙い(チャージ)
    float OnCharge(float charge)
    {
        //マックスのチャージ量
        var chargeMax = charge;
        //チャージ量の+-量
        float chargeProportion = userChargePonitTime;


        if (charge >= 1 && charge < 2)
        {
            charge = 1;
        }
        else if (charge >= 2 && charge < 3)
        {
            charge = 2;
        }
        else if (charge >= 3 && charge < 4)
        {
            charge = 3;
        }
        else if (charge >= 4 && charge < 5)
        {
            charge = 4;
        }
        else if (charge >= 5)
        {
            charge = 5;
        }
        else
        {
            charge = 0;
            chargeNow = 0.0f;
        }

        if (charge != 0 && chargeNow <= chargeMax && chargeCount < charge)
        {

            chargeNow += chargeProportion;
            if (chargeNow >= 1)
            {
                chargeCount++;
                if (chargeCount < charge)
                {
                    chargeNow = 0.0f;
                }
            }
        }

        Singleton.Instance.gameSceneController.starChargeController.ChargeStar(chargeCount);
        return chargeNow;
    }


    float OnChargeEffect()
    {
        var chargeMax = Singleton.Instance.gameSceneController.chargePointManager.ChargePointMax;
        var chargeNow = Singleton.Instance.gameSceneController.chargePointManager.ChargePoint;

        var charge = chargeNow / chargeMax;

        return charge;
    }

    //アタック
    void OnAttackMotion(int attackNum)
    {
        //animatorComponent.SetInteger("AttackNum", attackNum);
        switch (attackNum)
        {
            case (int)PlayerAttackIndex.AttackNormal:
                animatorComponent.SetBool("punchFlag", true);
                break;
            case (int)PlayerAttackIndex.AttackDown:
                animatorComponent.SetBool("punchFlag", true);
                break;
            case (int)PlayerAttackIndex.AttackUp:
                animatorComponent.SetBool("punchFlag", true);
                break;
        }
    }

    //アタック時
    public IEnumerator OnAttack(int attackResetNum)
    {
        //yield return new WaitForSeconds(0.5f);
        yield return null;
        attackFlag = false;
        //animatorComponent.SetInteger("AttackNum", attackResetNum);
        //animatorComponent.SetBool("Ground", isGroundFlag);

        objState = ObjState.Normal;
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
