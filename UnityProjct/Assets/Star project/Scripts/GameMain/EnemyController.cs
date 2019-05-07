using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// エネミーの現状ステータス
    /// </summary>
    public enum EnemyState
    {
        None,
        Search,//索敵
        Discovery,//プレイヤー発見
        ReMovePosition,//元の位置に戻る
        StunAttack,//スタン攻撃
        Stun,//スタン攻撃後動かなくなる
        Died,//死んだ（壊れた）とき）
    }
    public EnemyState enemyState
    {
        get; private set;
    }
    /// <summary>
    /// エネミーのタイプ
    /// </summary>
    public enum EnemyTyp
    {
        None,
        NotMoveEnemy,//
        MoveEnemy,
        AirMoveEnemy,
    }
    [SerializeField] EnemyTyp enemyTyp = EnemyTyp.None;

    private ObstacleManager obstacleManager;

    //プレイヤーポジション
    private GameObject playerObj = null;
    private Vector3 startPos = Vector3.zero;
    private Vector3 endPos = Vector3.zero;
    // 
    [SerializeField] private Vector3 amountOfMovement = Vector3.zero;
    //移動スピード
    [Header("各状態の移動速度")]
    [SerializeField] private float searchMoveSpeed;
    [SerializeField] private float lockOnMoveSpeed;
    [SerializeField] private float attackUpOnMoveSpeed;
    [SerializeField] private float removeMoveSpeed;
    //移動方向
    private Vector3 moveForce = Vector3.zero;
    //進む戻る
    private bool isReturn;
    private Rigidbody enemyRigidbody = null;
    //戻るポジション
    private Vector3 removePosition = Vector3.zero;
    //戻る移動方向
    private Vector3 removeForce = Vector3.zero;
    [SerializeField] private float defaultAttackTime = 2.0f;
    private float attackTime = 0;
    [SerializeField] private GameObject sandEffect = null;

    private bool attack;

    [SerializeField] private Animator enemyAnimator;

    public void Init(GameObject player)
    {
        enemyState = EnemyState.None;
        playerObj = player;
        enemyRigidbody = GetComponent<Rigidbody>();
        // StartPosをオブジェクトに初期位置に設定
        startPos = transform.localPosition;
        var pos = transform.localPosition;
        endPos = pos += amountOfMovement;
        // 1秒当たりの移動量を算出
        if (searchMoveSpeed == 0) searchMoveSpeed = 1;
        //スタート位置と移動終点の差を確認
        //スタート位置が終点よりも大きいときスタート位置と終点を入れ替える
        if (startPos.x > endPos.x)
        {
            isReturn = true;
            var temp = startPos;
            startPos = endPos;
            endPos = temp;
        }
        else
            isReturn = false;
        removePosition = transform.localPosition;
        if (enemyTyp == EnemyTyp.NotMoveEnemy)
        {
            FreezePositionOll(); Destroy(enemyRigidbody);
        }
        else FreezePositionAir();
        SandEffectPlay(false);
        enemyState = EnemyState.Search;
        obstacleManager = GetComponent<ObstacleManager>();
        attack = false;
    }
    /// <summary>
    /// エネミーアップデート
    /// </summary>
    public void EnemyControllerUpdate()
    {
        switch (enemyTyp)
        {
            case EnemyTyp.AirMoveEnemy:
                switch (enemyState)
                {
                    case EnemyState.Search:
                        SearchUpdate();
                        break;
                    case EnemyState.Discovery:
                        DiscoveryUpdate();
                        break;
                    case EnemyState.StunAttack:
                        StunAttackUpdate();
                        break;
                    //case EnemyState.ReMovePosition:
                    //    ReMovePositionUpdate();
                    //    break;
                    case EnemyState.Stun:
                        StanUpdate();
                        break;
                    case EnemyState.Died:
                        break;
                }
                break;
            case EnemyTyp.MoveEnemy:
                switch (enemyState)
                {
                    case EnemyState.Search:
                        SearchUpdate();
                        break;
                    case EnemyState.Discovery:
                        MoveEnemy_DiscoveryUpdate();
                        break;
                    case EnemyState.StunAttack:
                        StunAttackUpdate();
                        break;
                    //case EnemyState.ReMovePosition:
                    //    ReMovePositionUpdate();
                    //    break;
                    case EnemyState.Stun:
                        StanUpdate();
                        break;
                    case EnemyState.Died:
                        break;
                }
                break;
        }
    }
    /// <summary>
    /// モアイの間合いに入った時、一定時間後にスタン攻撃します
    /// </summary>
    private void DiscoveryUpdate()
    {
        enemyAnimator.SetBool("IsAttackPreparation", true);
        FreezePositionOll();
        //プレイヤーポジション取得
        var playerPos = playerObj.transform.position;
        //自分の座標をプレイヤーの座標からベクトル作成
        Vector3 enemyVec = playerPos - gameObject.transform.localPosition;
        //単位ベクトル作成（上記のベクトル）
        Vector3 enemyVecE = enemyVec.normalized;
        //長さを調節
        enemyVecE.z = 0;

        removeForce = enemyVecE;
        //攻撃
        attackTime += Time.deltaTime;
        if (attackTime >= defaultAttackTime)
        {
            enemyAnimator.SetBool("IsAttackPreparation", false);
            attack = true;
            FreezePositionSet();
            enemyRigidbody.AddForce(Vector3.up * attackUpOnMoveSpeed, ForceMode.Impulse);

            enemyState = EnemyState.StunAttack;
        }
        if (obstacleManager.isDestroyed) enemyState = EnemyState.Died;
    }
    /// <summary>
    /// モアイの間合いに入った時、一定時間後にスタン攻撃します
    /// </summary>
    private void MoveEnemy_DiscoveryUpdate()
    {
        enemyAnimator.SetBool("IsAttackPreparation", true);
        FreezePositionOll();
        //プレイヤーポジション取得
        var playerPos = playerObj.transform.position;
        //自分の座標をプレイヤーの座標からベクトル作成
        Vector3 enemyVec = playerPos - gameObject.transform.localPosition;
        //単位ベクトル作成（上記のベクトル）
        Vector3 enemyVecE = enemyVec.normalized;
        //長さを調節
        enemyVecE.z = 0;
        enemyVecE.y = 0;

        removeForce = enemyVecE;
        //攻撃
        attackTime += Time.deltaTime;
        if (attackTime >= defaultAttackTime)
        {
            enemyAnimator.SetBool("IsAttackPreparation", false);
            FreezePositionSet();
            enemyRigidbody.AddForce(enemyVecE * lockOnMoveSpeed, ForceMode.Impulse);
            attackTime = 0;
            SandEffectPlay(true);
            enemyState = EnemyState.StunAttack;
        }

        if (obstacleManager.isDestroyed) enemyState = EnemyState.Died;
    }
    private void StunAttackUpdate()
    {
        var velocity = enemyRigidbody.velocity;
        // 下降中
        if (velocity.y < 0 && attack)
        {
            Debug.Log("降下中");
            enemyRigidbody.AddForce(removeForce * lockOnMoveSpeed, ForceMode.Impulse);
            attackTime = 0;
            SandEffectPlay(true);
            attack = false;
        }
        if (obstacleManager.isDestroyed) enemyState = EnemyState.Died;
    }

    private void StanUpdate()
    {
        if (obstacleManager.isDestroyed) enemyState = EnemyState.Died;
        return;
    }
    /// <summary>
    /// 索敵状態
    /// スタート位置と終了位置を反復移動します
    /// </summary>
    private void SearchUpdate()
    {
        var velocity = enemyRigidbody.velocity;
        //進む
        if (!isReturn)
        {
            if (transform.localPosition.x > endPos.x)
            {
                isReturn = true;
                var rot = -90;
                transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            }
            moveForce.x = searchMoveSpeed;
        }
        //戻る
        else
        {
            if (transform.localPosition.x < startPos.x)
            {
                isReturn = false;
                var rot = 90;
                transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            }
            moveForce.x = -searchMoveSpeed;
        }

        velocity.x = moveForce.x;
        enemyRigidbody.velocity = velocity;

        if (obstacleManager.isDestroyed) enemyState = EnemyState.Died;
    }
    /// <summary>
    /// スタン攻撃後元の位置に戻ります
    /// ※現状使いません
    /// </summary>
    //void ReMovePositionUpdate()
    //{
    //    var velocity = enemyRigidbody.velocity;
    //    velocity = -removeForce * removeMoveSpeed;
    //    enemyRigidbody.velocity = velocity;
    //    Debug.Log("removePosition : " + removePosition);
    //    Debug.Log("transform.localPosition : " + transform.localPosition);
    //    if (transform.localPosition.y <= removePosition.y + difference && transform.localPosition.y >= removePosition.y - difference)
    //    {
    //        Debug.Log("帰りました");
    //        FreezePositionAir();
    //        enemyState = EnemyState.Search;
    //    }
    //}
    /// <summary>
    /// Rigidbodyのフリーズポジション、ローテーションの固定
    /// 通常時の設定です
    /// </summary>
    private void FreezePositionOll()
    {
        enemyRigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }
    /// <summary>
    /// Rigidbodyのフリーズポジション、ローテーションの固定
    /// スタン攻撃時の設定です
    /// </summary>
    private void FreezePositionSet()
    {
        enemyRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }
    /// <summary>
    /// Rigidbodyのフリーズポジション、ローテーションの固定
    /// 空中浮遊状態のの設定です
    /// </summary>
    private void FreezePositionAir()
    {
        enemyRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground" && enemyState == EnemyState.StunAttack)
        {
            StartCoroutine(SandEffectEnumerator());
        }
    }
    /// <summary>
    /// スタン状態で地面に着くときにエフェクト非表示＆リジットボディを消去します
    /// </summary>
    /// <returns></returns>
    private IEnumerator SandEffectEnumerator()
    {
        enemyState = EnemyState.Stun;
        FreezePositionOll();
        Destroy(enemyRigidbody);
        yield return new WaitForSeconds(1.0f);
        SandEffectPlay(false);
    }
    private void OnTriggerStay(Collider collision)
    {
        //プレイキャラクターを発見
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" && enemyState == EnemyState.Search)
        {
            //プレイヤーが手前に居るとき
            if (playerObj.transform.position.x < transform.localPosition.x)
            {
                var rot = -90;
                transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            }
            else
            {
                var rot = 90;
                transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            }
            enemyState = EnemyState.Discovery;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" && enemyState == EnemyState.Discovery && !attack)
        {
            enemyAnimator.SetBool("IsAttackPreparation", false);
            FreezePositionAir();
            if (isReturn)
            {
                var rot = -90;
                transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            }
            else
            {
                var rot = 90;
                transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 1, 0));
            }
            attackTime = 0;
            enemyState = EnemyState.Search;
        }
    }

    private void SandEffectPlay(bool isPlay)
    {
        sandEffect.SetActive(isPlay);
    }
}

