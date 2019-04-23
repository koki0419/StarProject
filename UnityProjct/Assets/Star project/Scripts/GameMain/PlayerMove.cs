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
        Normal,//通常状態
        AttackJab,//ジャブ攻撃状態
        AttackUp,//上攻撃状態
        AttackDown,//下攻撃状態
        ChargeAttack,//チャージ攻撃状態
        OnCharge,//チャージ中状態
        CharacterGameOver,//ゲームオーバー状態
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
    //チャージエフェクト2
    [SerializeField] GameObject chargeEffect2 = null;
    //砂煙エフェクト
    [SerializeField] GameObject sandEffect = null;
    //パンチエフェクト
    [SerializeField] GameObject punchEffect = null;

    //-------------クラス関係--------------------------------
    //『Attack』をインスタンス
    Attack attack = new Attack();
    //-------------数値用変数--------------------------------
    [Header("プレイヤー情報")]
    //移動速度を設定します
    [SerializeField] private float moveSpeed = 0;
    //ジャンプ中の移動速度
    [SerializeField] private float airUpMoveSpeed = 0;
    [SerializeField] private float airDownMoveSpeed = 0;
    //空気抵抗
    [SerializeField] private float dragPower = 0;
    //キー入力制御
    [SerializeField] private float inputMoveKey = 0;
    //ジャンプ力
    [SerializeField] float jumpSpeed = 0;

    //チャージポイント使用時のユーザーゲージ上昇量
    [SerializeField] float userChargePonitUp = 0.001f;

    [Header("プレイヤー攻撃初期情報")]
    //初期攻撃力
    [SerializeField] float foundationoffensivePower = 0;
    //初期移動量
    [SerializeField] float foundationSpeedForce = 0;
    [SerializeField] float UpFoundationSpeedForce = 0;

    [Header("チャージ回数に掛け算される力")]
    //攻撃力
    [SerializeField] float offensivePower = 0;
    //移動量
    [SerializeField] float speedForce = 0;
    //現在のチャージ量
    float chargeNow = 0.0f;
    //現在のチャージ量
    float chargeNowHand = 0.0f;
    //何回チャージしたか
    int chargeCount = 0;
    //チャージ上限
    int chargeCountMax = 0;
    //回転
    private float rot = 90;
    //攻撃時Speed
    float attackSpeed;
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

    [Header("攻撃時の攻撃時間")]
    [SerializeField] private float AttackJabTime;
    [SerializeField] private float chargeAttackTime;
    [SerializeField] private float chargeAttackUpTime;
    [SerializeField] private float chargeAttackDownTime;

    [Header("キャラクターSEの種類番号")]
    [SerializeField] private int dashSeNum;
    [SerializeField] private int jumpSeNum;
    [SerializeField] private int chargeSeNum;
    [SerializeField] private int punchSeNum;
    [SerializeField] private int getStarSeNum;
    //-------------フラグ用変数------------------------------
    [Header("各種フラグ")]
    //ジャンプフラグ
    bool isJumpFlag;
    //アタックフラグ
    public bool canAttackFlag
    {
        get; private set;
    }
    //地面との接触
    bool isGround;
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
    bool isDownAttack;
    bool isAttack;

    [Header("当たり判定のあるオブジェクトの名前")]
    [SerializeField] string groundName;
    [SerializeField] string gameOverLineName;

    //初期化
    public void Init()
    {
        //プレイヤーの状態を通常状態に設定します
        objState = ObjState.Normal;
        //右向きに指定
        transform.rotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
        //Rigidbodyを取得します
        rigidbody = GetComponent<Rigidbody>();
        dragPower = rigidbody.drag;
        //チャージゲージをリセットします
        Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(0);
        //----初期化-----
        canAttackFlag = false;
        isJumpFlag = false;
        isGround = false;
        isAcquisitionStar = false;


        SandEffectPlay(false);
        PunchEffectPlay(false);

        starAcquisitionEffect.SetActive(false);
        ChargeEffectPlay(false, false);

        isUpAttack = false;
        isDownAttack = false;
        isAttack = false;

    }

    // Update is called once per frame
    public void OnUpdate(float deltaTime)
    {
        switch (objState)
        {
            //通常時
            case ObjState.Normal:
                NormalModeUpdate(deltaTime);
                break;
            case ObjState.OnCharge:
                ChargeUpdate();
                break;
            case ObjState.AttackJab:
                AttackUpdate(AttackJabTime);
                break;
            case ObjState.ChargeAttack:
                AttackUpdate(chargeAttackTime);
                break;
            case ObjState.AttackUp:
                AttackUpdate(chargeAttackUpTime);
                break;
            case ObjState.AttackDown:
                AttackUpdate(chargeAttackDownTime);
                break;
            case ObjState.CharacterGameOver:
                CharacterGameOver();
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
       
        //ゲームオーバーの当たり判定
        if (collision.gameObject.name == gameOverLineName)
        {
            objState = ObjState.CharacterGameOver;
        }
    }
  

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == groundName)
        {
            isGround = true;
            isJumpFlag = false;
            rigidbody.drag = dragPower;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == groundName)
        {
            isGround = false;
            Singleton.Instance.soundManager.StopPlayerSe();
            //ジャンプ音再生
            Singleton.Instance.soundManager.PlayPlayerSe(jumpSeNum);
        }
    }

    /// <summary>
    /// キャラクターの移動です
    /// </summary>
    /// <param name="horizontal">横移動のキー入力値</param>
    /// <param name="deltaTime">GameSceneManagerから受け取ります</param>
    void CharacterMove(float horizontal, float deltaTime)
    {
        var force = new Vector3(horizontal * moveSpeed, 0.0f, 0.0f);


        if (!isGround)
        {
            force = new Vector3(horizontal * airUpMoveSpeed, 0.0f, 0.0f);
            rigidbody.AddForce(force, ForceMode.Force);
            var velocity = rigidbody.velocity;
            // 下降中
            if (velocity.y < 0)
            {
                rigidbody.drag = 0;
                force = new Vector3(horizontal * airDownMoveSpeed, 0.0f, 0.0f);
                rigidbody.AddForce(force, ForceMode.Force);
            }
        }
        rigidbody.AddForce(force, ForceMode.Force);


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

    }

    /// <summary>
    /// キャラクターの向きを変更します
    /// 右向き左向きに変更します
    /// rot(90°）回転させます
    /// </summary>
    /// <param name="horizontal">左右キー入力値</param>
    void DirectionMove(float horizontal)
    {
        //rigidbody.velocity = Vector3.zero.normalized;
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
    /// <summary>
    /// チャージ攻撃時のキャラクターの移動
    /// 上下左右の動きを管理しています
    /// </summary>
    /// <param name="speedForce">チャージ攻撃時の移動量</param>
    void MoveAttack()
    {
        var rig = rigidbody;
        rig.drag = dragPower;
        attackSpeed = (chargeCount * speedForce + foundationSpeedForce)/10;
        if (!isUpAttack && !isDownAttack)
        {

            //右向きの時
            if (isRightDirection && !isLeftDirection)
            {
                rig.AddForce(Vector3.right * attackSpeed, ForceMode.Impulse);
            }
            //左向きの時
            else
            {
                rig.AddForce(Vector3.left * attackSpeed, ForceMode.Impulse);
            }
        }
        else if (isUpAttack)
        {
            attackSpeed = (chargeCount * speedForce + UpFoundationSpeedForce)/10;
            rig.AddForce(Vector3.up * attackSpeed, ForceMode.Impulse);
        }
        else if (isDownAttack)
        {
            rig.AddForce(Vector3.down * attackSpeed, ForceMode.Impulse);
        }
        isChargeFlag = false;
    }

    /// <summary>
    /// attack時の手の大きさを大きくする
    /// </summary>
    /// <param name="charge"></param>
    /// <returns></returns>
    void ChargeAttackHand(float charge)
    {
        var chargeMax = Singleton.Instance.gameSceneController.ChargePointManager.StarChildCountMax;
        var charaHand = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        //チャージ量の+-量
        float chargeProportion = userChargePonitUp * 10;

        if (chargeNowHand <= charge)
        {
            chargeNowHand += chargeProportion;
        }

        charaHand.SetBlendShapeWeight(0, chargeNowHand / chargeMax * 100);

    }
    //チャージ時のチャージ量
    /// <summary>
    /// チャージ時のチャージ量
    /// 何回チャージできるのか（Fillを何回0～1にできるのか）を返します
    /// </summary>
    /// <param name="charge"></param>
    /// <returns></returns>
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
        return chargeNow;
    }


    /// <summary>
    /// 攻撃の種類を選択します
    /// </summary>
    /// <param name="attackNum"></param>
    void OnAttackMotion(int attackNum)
    {
        switch (attackNum)
        {
            case (int)PlayerAttackIndex.AttackNormal:
                CharacterAnimation("punch");
                objState = ObjState.AttackJab;
                break;
            case (int)PlayerAttackIndex.ChargeAttackNormal:
                CharacterAnimation("chargepunch");
                FreezeUp();
                objState = ObjState.ChargeAttack;
                break;
            case (int)PlayerAttackIndex.ChargeAttackDown:
                CharacterAnimation("chargepunchDown");
                FreezePositionCancel();
                objState = ObjState.AttackDown;
                isDownAttack = true;
                break;
            case (int)PlayerAttackIndex.ChargeAttackUp:
                CharacterAnimation("chargepunchUp");
                objState = ObjState.AttackUp;
                FreezePositionCancel();
                isUpAttack = true;
                break;
        }
    }

    //アタック時
    public IEnumerator OnAttack(int attackResetNum, float animationTime)
    {
        yield return new WaitForSeconds(animationTime);
        canAttackFlag = false;
        //var charaHand = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        //charaHand.SetBlendShapeWeight(0, 0);
        chargeNowHand = 0.0f;
        FreezePositionCancel();
        PunchEffectPlay(false);
        isUpAttack = false;
        isDownAttack = false;
        CharacterAnimation("ExitAnimation");
        objState = ObjState.Normal;
    }
    /// <summary>
    /// ☆獲得時の獲得エフェクトを表示する為のコルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnGetStar()
    {
        starAcquisitionEffect.SetActive(true);
        Singleton.Instance.soundManager.PlayPlayerSe(getStarSeNum);
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
            case "GameOver":
                animatorComponent.SetBool("isDash", false);
                animatorComponent.SetBool("isJump", false);
                animatorComponent.SetTrigger("isGameOver");
                break;
            case "ExitAnimation":
                animatorComponent.SetTrigger("ExitAnimation");
                break;
        }
    }
    /// <summary>
    /// 通常状態でのキャラクターの処理
    /// </summary>
    /// <param name="dx">横移動キー入力値</param>
    /// <param name="deltaTime">deltaTimeをGameSceneManagerからもらう</param>
    void NormalModeUpdate(float deltaTime)
    {
        //移動
        float dx = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && isGround)
        {
            CharacterAnimation("jump");
            rigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        //アニメーション
        if (dx != 0 && !isChargeFlag && isGround)
        {
            CharacterAnimation("dash");
            Singleton.Instance.soundManager.PlayPlayerSe(dashSeNum);
            SandEffectPlay(true);
        }
        else if (!isChargeFlag && isGround)
        {
            CharacterAnimation("idol");
            SandEffectPlay(false);
        }else if (!isGround)
        {
            CharacterAnimation("jump");
            SandEffectPlay(false);
        }

        //移動
        CharacterMove(dx, deltaTime);

        if (Input.GetKeyDown(KeyCode.T) || Input.GetButtonDown("Charge"))
        {
            FreezePositionSet();
            CharacterAnimation("charge");
            isChargeFlag = true;
            //チャージSE再生
            Singleton.Instance.soundManager.StopPlayerSe();
            Singleton.Instance.soundManager.PlayPlayerSe(chargeSeNum);
            objState = ObjState.OnCharge;
        }
    }
    /// <summary>
    /// キャラクターのチャージ時に呼び出します
    /// </summary>
    /// <param name="dx">横移動キー入力値</param>
    /// <param name="dy">上下移動キー入力値</param>
    void ChargeUpdate()
    {
        float dy = Input.GetAxis("Vertical");
        //移動
        float dx = Input.GetAxis("Horizontal");
        DirectionMove(dx);

        //通常時
        //チャージ
        if (Input.GetKey(KeyCode.T) || Input.GetButton("Charge"))
        {
            //チャージ中
            Singleton.Instance.gameSceneController.starChargeController.UpdateChargePoint(OnCharge(Singleton.Instance.gameSceneController.ChargePointManager.starChildCount / 10));
            Singleton.Instance.gameSceneController.starChargeController.ChargeBigStar(chargeCount);
            ChargeAttackHand(Singleton.Instance.gameSceneController.ChargePointManager.starChildCount);
            //チャージエフェクトデバック---------------------------
            if (chargeCount < 3)
            {
                ChargeEffectPlay(true, false);
            }
            else
            {
                ChargeEffectPlay(false, true);
            }

        }
        if (Input.GetKeyUp(KeyCode.T) || Input.GetButtonUp("Charge"))
        {

            //チャージ終了（チャージゲージを0に戻す）
            attackPower = chargeCount * offensivePower + foundationoffensivePower;

            //attackSpeed = chargeCount * speedForce + foundationSpeedForce;
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

            ChargeEffectPlay(false, false);

            PunchEffectPlay(true);

            chargeCount = 0;
            chargeNow = 0.0f;
            canAttackFlag = true;
            isAttack = true;

        }
    }
    /// <summary>
    /// 攻撃時のキャラクター更新
    /// </summary>
    /// <param name="animationTime">アニメーション時間</param>
    void AttackUpdate(float animationTime)
    {
        MoveAttack();
        if (isAttack)
        {
            isAttack = false;
            Singleton.Instance.soundManager.StopPlayerSe();
            Singleton.Instance.soundManager.PlayPlayerSe(punchSeNum);
            StartCoroutine(OnAttack(0, animationTime));
        }
    }

    void CharacterGameOver()
    {
        StartCoroutine(GameOverIEnumerator());
    }

    IEnumerator GameOverIEnumerator()
    {
        ChargeEffectPlay(false, false);
        CharacterAnimation("GameOver");

        yield return new WaitForSeconds(1.5f);
        Singleton.Instance.gameSceneController.isGameOver = true;
    }
    void FreezeUp()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ |RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }
    /// <summary>
    /// 初期状態に戻します
    /// </summary>
    void FreezePositionCancel()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }
    /// <summary>
    /// チャージ時動かないようにする
    /// チャージ攻撃横移動のみこれ使う
    /// 落下しなくなる
    /// </summary>
    void FreezePositionSet()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    void SandEffectPlay(bool isPlay)
    {
        sandEffect.SetActive(isPlay);
    }

    void PunchEffectPlay(bool isPlay)
    {
        punchEffect.SetActive(isPlay);
    }
    /// <summary>
    /// チャージ時のエフェクト表示非表示
    /// チャージエフェクトは2種類あってどちらか使用中の時はどちらか使用しないのでまとめました。
    /// 2種類のエフェクトだが使い方は1通りです。
    /// </summary>
    /// <param name="effect1_isPlay">1段階目のチャージエフェクト表示非表示</param>
    /// <param name="effect2_isPlay">2段階目のチャージエフェクト表示非表示</param>
    void ChargeEffectPlay(bool effect1_isPlay, bool effect2_isPlay)
    {
        chargeEffect1.SetActive(effect1_isPlay);
        chargeEffect2.SetActive(effect2_isPlay);
    }
}
