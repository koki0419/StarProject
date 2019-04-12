using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //攻撃時設定数値
    public enum PlayerAttackIndex
    {
        None,
        AttackNormal = 1000,
        ChargeAttackNormal = 1010,
        ChargeAttackDown = 1001,
        ChargeAttackUp = 1011,
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

    //-------------Unityコンポーネント関係-------------------
    // 自分のアニメーションコンポーネント
    public Animator animatorComponent;

    new Rigidbody rigidbody;
    [Header("エフェクト関係")]
    //スター獲得エフェクト
    [SerializeField] GameObject starAcquisitionEffect = null;
    //チャージエフェクト1
    [SerializeField] GameObject chargeEffect1 = null;
    bool chargeEffectFlag1 = false;
    //チャージエフェクト2
    [SerializeField] GameObject chargeEffect2 = null;
    bool chargeEffectFlag2 = false;


    //-------------クラス関係--------------------------------
    //『Attack』をインスタンス
    Attack attack = new Attack();
    //-------------数値用変数--------------------------------
    [Header("プレイヤー情報")]
    //移動速度を設定します
    [SerializeField] private float moveSpeed = 0;
    //ジャンプ力
    [SerializeField] float jumpSpeed = 0;
    //ジャンプ重力
    [SerializeField] float jumpGravity = 0;
    //チャージポイント使用時のユーザーゲージ上昇量
    [SerializeField] float userChargePonitUp = 0.001f;

    [Header("プレイヤー攻撃初期情報")]
    //初期攻撃力
    [SerializeField] float foundationoffensivePower = 0;
    //初期移動量
    [SerializeField] float foundationSpeedForce = 0;

    [Header("チャージ回数に掛け算される力")]
    //攻撃力
    [SerializeField] float offensivePower = 0;
    //移動量
    [SerializeField] float speedForce = 0;
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
    [SerializeField] bool cnaJumpFlag;
    //アタックフラグ
    [SerializeField] bool canAttackFlag;
    public bool CanAttackFlag
    {
        get { return canAttackFlag; }
    }
    //地面との接触
    bool isGroundFlag;
    //チャージ中かどうか
    bool isChargeFlag;
    //☆獲得時フラグ
    bool isAcquisitionStar = false;
    public bool IsAcquisitionStar
    {
        set { isAcquisitionStar = value; }
    }

    //キャラクターの向き
    bool isRightDirection;
    bool isLeftDirection;

    bool isUpAttack;
    bool isAttack;

    //初期化
    public void Init()
    {
        //プレイヤーの状態を通常状態に設定します
        objState = ObjState.Normal;
        //右向きに指定
        transform.rotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
        //Rigidbodyを取得します
        rigidbody = gameObject.GetComponent<Rigidbody>();
        //チャージゲージをリセットします
        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(0);
        //----初期化-----
        canAttackFlag = false;
        cnaJumpFlag = true;
        isGroundFlag = true;
        isAcquisitionStar = false;

        starAcquisitionEffect.SetActive(false);


        //デバック用//エフェクト
        chargeEffectFlag1 = false;
        chargeEffectFlag2 = false;
        chargeEffect1.SetActive(chargeEffectFlag1);
        chargeEffect2.SetActive(chargeEffectFlag2);

        isUpAttack = false;
        isAttack = false;
    }

    // Update is called once per frame
    public void OnUpdate(float deltaTime)
    {
        float dy = Input.GetAxis("Vertical");

        //移動
        float dx = Input.GetAxis("Horizontal");

        switch (objState)
        {
            //通常時
            case ObjState.Normal:
                {
                    if (!canAttackFlag)
                    {
                        if (Input.GetKeyDown(KeyCode.J) && isGroundFlag || Input.GetKeyDown(KeyCode.Joystick1Button0) && isGroundFlag)
                        {
                            CharacterAnimation("jump");
                            cnaJumpFlag = false;
                            isGroundFlag = false;
                        }
                        //アニメーション
                        if (dx != 0 && !isChargeFlag && isGroundFlag)
                        {
                            CharacterAnimation("dash");
                        }
                        else if (!isChargeFlag && isGroundFlag)
                        {
                            CharacterAnimation("idol");
                        }

                        //移動
                        CharacterMove(dx, jumpPushKeyTime, deltaTime);
                    }
                    else
                    {
                        DirectionMove(dx);
                        rigidbody.isKinematic = true;
                    }

                    //チャージ
                    if (Input.GetKeyDown(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
                    {
                        CharacterAnimation("charge");
                        canAttackFlag = true;
                        isChargeFlag = true;
                        objState = ObjState.OnCharge;
                    }
                }
                break;
            case ObjState.OnCharge:
                {
                    DirectionMove(dx);
                    rigidbody.isKinematic = true;
                    //通常時
                    //チャージ
                    if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.Joystick1Button2))
                    {
                        //チャージ中
                        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.ChargePointManager.ChargePoint / 10));

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
                    if (Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Joystick1Button2))
                    {

                        //チャージ終了（チャージゲージを0に戻す）
                        attackPower = chargeCount * offensivePower + foundationoffensivePower;
                        attackSpeed = chargeCount * speedForce + foundationSpeedForce;
                        //チャージゲージをリセットします
                        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(0);
                        //チャージ中☆を戻します
                        Singleton.Instance.gameSceneController.starChargeController.UpdateBigStarUI(chargeCount);
                        //攻撃アニメーション
                        //チャージ回数が1回までなら通常パンチ
                        //チャージしたなら入力角度を計算して上下左右を判断して攻撃
                        if (chargeCount <= 1)
                        {
                            OnAttackMotion(1000);
                        }
                        else
                        {
                            OnAttackMotion(attack.OnAttack(new Vector2(dx, dy), this.gameObject));
                        }

                        chargeEffect1.SetActive(false);
                        chargeEffect2.SetActive(false);

                        rigidbody.isKinematic = false;

                        chargeCount = 0;
                        chargeNow = 0.0f;
                        isAttack = true;
                        objState = ObjState.Attack;
                    }
                    break;
                }
            case ObjState.Attack:
                {
                    if (isAttack)
                    {
                        MoveAttack(attackSpeed / 10);
                        isUpAttack = false;
                        isAttack = false;
                        StartCoroutine(OnAttack(0));
                    }
                }
                break;
        }

        if (isAcquisitionStar)
        {
            StartCoroutine(OnGetStar());
        }
    }


    //--------------関数-----------------------------
    //地面との当たり判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGroundFlag = true;
            cnaJumpFlag = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //ゲームオーバーの当たり判定
        if (other.gameObject.name == "GameOverLine")
        {
            Singleton.Instance.gameSceneController.IsGameOver = true;
        }
    }

    //移動
    void CharacterMove(float horizontal, float jTime, float deltaTime)
    {
        var position = transform.position;
        position.x += horizontal * moveSpeed * deltaTime;
        transform.position = position;

        var velocity = rigidbody.velocity;
        //キャラクターの向き
        if (horizontal > 0)
        {
            transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            isRightDirection = true;
            isLeftDirection = false;
        }
        else if (horizontal < 0)
        {
            transform.localRotation = Quaternion.AngleAxis(-rot, new Vector3(0, 1, 0));
            isRightDirection = false;
            isLeftDirection = true;
        }
        if (!cnaJumpFlag)
        {
            cnaJumpFlag = true;
            velocity.y += jumpSpeed;
        }
        if (!isGroundFlag)
        {
            velocity.y += Physics.gravity.y * jumpGravity * deltaTime;
        }

        rigidbody.velocity = velocity;
    }
    /// <summary>
    /// キャラクターの向きを変更します
    /// 右向き左向きに変更します
    /// rot(90°）回転させます
    /// </summary>
    /// <param name="horizontal">左右キー入力値</param>
    void DirectionMove(float horizontal)
    {
        //キャラクターの向き
        if (horizontal > 0)
        {
            transform.rotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            isRightDirection = true;
            isLeftDirection = false;
        }
        else if (horizontal < 0)
        {
            transform.rotation = Quaternion.AngleAxis(-rot, new Vector3(0, 1, 0));
            isRightDirection = false;
            isLeftDirection = true;
        }

    }

    //攻撃時の移動
    void MoveAttack(float speedForce)
    {
        if (!isUpAttack)
        {
            var position = transform.position;
            //右向きの時
            if (isRightDirection && !isLeftDirection)
            {
                //position.x += speedForce * Time.deltaTime;
                var rig = rigidbody;
                rig.AddForce(Vector3.right * speedForce, ForceMode.Impulse);
            }
            //左向きの時
            else
            {
                //position.x -= speedForce * Time.deltaTime;
                var rig = rigidbody;
                rig.AddForce(Vector3.left * speedForce, ForceMode.Impulse);
            }
            transform.position = position;
        }
        else
        {
            var rig = rigidbody;
            rig.AddForce(Vector3.up* speedForce, ForceMode.Impulse);
            isGroundFlag = false;
        }
        isChargeFlag = false;
    }

    //チャージ時のチャージ量
    float OnCharge(float charge)
    {
        //マックスのチャージ量
        var chargeMax = charge;
        //チャージ量の+-量
        float chargeProportion = userChargePonitUp;


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

        Singleton.Instance.gameSceneController.starChargeController.ChargeGigStar(chargeCount);
        return chargeNow;
    }


    float OnChargeEffect()
    {
        var chargeMax = Singleton.Instance.gameSceneController.ChargePointManager.ChargePointMax;
        var chargeNow = Singleton.Instance.gameSceneController.ChargePointManager.ChargePoint;

        var charge = chargeNow / chargeMax;

        return charge;
    }

    //アタック
    void OnAttackMotion(int attackNum)
    {
        switch (attackNum)
        {
            case (int)PlayerAttackIndex.AttackNormal:
                CharacterAnimation("punch");
                break;
            case (int)PlayerAttackIndex.ChargeAttackNormal:
                CharacterAnimation("chargepunch");
                break;
            case (int)PlayerAttackIndex.ChargeAttackDown:
                CharacterAnimation("chargepunchDown");
                break;
            case (int)PlayerAttackIndex.ChargeAttackUp:
                CharacterAnimation("chargepunchUp");
                isUpAttack = true;
                break;

        }
    }

    //アタック時
    public IEnumerator OnAttack(int attackResetNum)
    {
        yield return new WaitForSeconds(2.0f);
        canAttackFlag = false;
        //isAttack = false;
        objState = ObjState.Normal;
    }
    /// <summary>
    /// ☆獲得時の獲得エフェクトを表示する為のコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnGetStar()
    {
        starAcquisitionEffect.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        isAcquisitionStar = false;
        starAcquisitionEffect.SetActive(false);
    }
    /// <summary>
    /// キャラクターのアニメーションです
    /// いろんなところでアニメーションをセットするとどこでいじったか
    /// 分からなくなると思うのでここにまとめました。
    /// </summary>
    /// <param name="animationName">使用するアニメーション名</param>
    public void CharacterAnimation(string animationName)
    {
        switch (animationName)
        {
            case "idol":
                animatorComponent.SetBool("isDash", false);
                animatorComponent.SetBool("isJump", false);
                break;
            case "dash":
                animatorComponent.SetBool("isDash", true);
                animatorComponent.SetBool("isJump", false);
                break;
            case "jump":
                animatorComponent.SetBool("isDash", false);
                animatorComponent.SetBool("isJump", true);
                break;
            case "charge":
                animatorComponent.SetBool("isDash", false);
                animatorComponent.SetTrigger("isCharge");
                animatorComponent.SetInteger("setPunchNum", 0);
                break;
            case "punch":
                animatorComponent.SetTrigger("isPunch");
                animatorComponent.SetInteger("setPunchNum", 1000);
                break;
            case "chargepunch":
                animatorComponent.SetTrigger("isPunch");
                animatorComponent.SetInteger("setPunchNum", 1010);
                break;
            case "chargepunchUp":
                animatorComponent.SetTrigger("isPunch");
                animatorComponent.SetInteger("setPunchNum", 1011);
                break;
            case "chargepunchDown":
                animatorComponent.SetTrigger("isPunch");
                animatorComponent.SetInteger("setPunchNum", 1001);
                break;
        }
    }

}
