using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        None,
        Search,//索敵
        Discovery,//プレイヤー発見
        ReMovePosition,//元の位置に戻る
        StunAttack,//スタン攻撃
        Stun,//スタン攻撃後動かなくなる
    }
    public EnemyState enemyState
    {
        get; private set;
    }

    public enum EnemyTyp
    {
        None,
        NotMoveEnemy,//
        MoveEnemy,
        AirMoveEnemy,
    }
    [SerializeField] EnemyTyp enemyTyp = EnemyTyp.None;

    //プレイヤーポジション
    GameObject playerObj;
    Vector3 startPos;
    Vector3 endPos;
    // 
    [SerializeField] Vector3 amountOfMovement = Vector3.zero;
    //移動スピード
    [Header("各状態の移動速度")]
    [SerializeField] float searchMoveSpeed;
    [SerializeField] float lockOnMoveSpeed;
    [SerializeField] float removeMoveSpeed;
    //[Header("各状態の移動速度")]
    //[SerializeField] float difference;
    //移動方向
    private Vector3 moveForce;
    //進む戻る
    bool isReturn;
    Rigidbody enemyRigidbody;
    //戻るポジション
    Vector3 removePosition;
    //戻る移動方向
    Vector3 removeForce;
    [SerializeField] float defaultAttackTime = 2.0f;
    float attackTime = 0;
    [SerializeField] GameObject sandEffect;

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
    }

    public void EnemyControllerUpdate()
    {
        //if (GameSceneController.isPlaying)
        // {
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
                        break;
                }
                break;
        }
        //  }
        // Debug.Log("EnemyState : " + enemyState);
    }
    /// <summary>
    /// モアイの間合いに入った時、一定時間後にスタン攻撃します
    /// </summary>
    void DiscoveryUpdate()
    {
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
        if (attackTime >= defaultAttackTime || Input.GetKeyDown(KeyCode.X))
        {
            FreezePositionSet();
            enemyRigidbody.AddForce(enemyVecE * lockOnMoveSpeed, ForceMode.Impulse);
            attackTime = 0;
            SandEffectPlay(true);
            enemyState = EnemyState.StunAttack;
        }
    }
    /// <summary>
    /// モアイの間合いに入った時、一定時間後にスタン攻撃します
    /// </summary>
    void MoveEnemy_DiscoveryUpdate()
    {
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
        if (attackTime >= defaultAttackTime || Input.GetKeyDown(KeyCode.X))
        {
            FreezePositionSet();
            enemyRigidbody.AddForce(enemyVecE * lockOnMoveSpeed, ForceMode.Impulse);
            attackTime = 0;
            SandEffectPlay(true);
            enemyState = EnemyState.StunAttack;
        }
    }
    void StunAttackUpdate()
    {
        return;
    }
    /// <summary>
    /// 索敵状態
    /// スタート位置と終了位置を反復移動します
    /// </summary>
    void SearchUpdate()
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
    void FreezePositionOll()
    {
        enemyRigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }
    /// <summary>
    /// Rigidbodyのフリーズポジション、ローテーションの固定
    /// スタン攻撃時の設定です
    /// </summary>
    void FreezePositionSet()
    {
        enemyRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }
    void FreezePositionAir()
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
    IEnumerator SandEffectEnumerator()
    {
        enemyState = EnemyState.Stun;
        FreezePositionOll();
        Destroy(enemyRigidbody);
        yield return new WaitForSeconds(1.0f);
        SandEffectPlay(false);
    }
    void OnTriggerStay(Collider collision)
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

    void OnTriggerExit(Collider collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" && enemyState == EnemyState.Discovery)
        {
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
            enemyState = EnemyState.Search;
        }
    }

    void SandEffectPlay(bool isPlay)
    {
        sandEffect.SetActive(isPlay);
    }
}

