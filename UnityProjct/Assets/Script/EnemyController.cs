using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        None,
        //索敵
        Search,
        //プレイヤー発見
        Discovery,
    }

    public EnemyState enemyState = EnemyState.None;

    void OnTriggerStay(Collider col)
    {
        //プレイキャラクターを発見
        if (col.tag == "Player")
        {
            enemyState = EnemyState.Discovery;
        }
    }

    public Vector3 StartPos;
    public Vector3 EndPos;
    public float time;
    private Vector3 deltaPos;
    private float elapsedTime;
    private bool bStartToEnd = true;

    public GameObject target;
    private NavMeshAgent agent;

    public Vector3 Position;
    public Vector3 PlayerPos;

    public float EnemySpeed;

    public EnemyController(Vector3 pos,Vector3 playerPos, float speed)
    {
        Position = pos;
        PlayerPos = playerPos;
        EnemySpeed = speed;
    }

    void Start()
    {
        // StartPosをオブジェクトに初期位置に設定
        transform.position = StartPos;
        // 1秒当たりの移動量を算出
        deltaPos = (EndPos - StartPos) / time;
        elapsedTime = 0;
        enemyState = EnemyState.Search;

        agent = GetComponent<NavMeshAgent>();

    }

    void Update()
    {

        switch (enemyState)
        {
            case EnemyState.Search:
                {
                    // 1秒当たりの移動量にTime.deltaTimeを掛けると1フレーム当たりの移動量となる
                    // Time.deltaTimeは前回Updateが呼ばれてからの経過時間
                    transform.position += deltaPos * Time.deltaTime;
                    // 往路復路反転用経過時間
                    elapsedTime += Time.deltaTime;
                    // 移動開始してからの経過時間がtimeを超えると往路復路反転
                    if (elapsedTime > time)
                    {
                        if (bStartToEnd)
                        {
                            // StartPos→EndPosだったので反転してEndPos→StartPosにする
                            // 現在の位置がEndPosなので StartPos - EndPosでEndPos→StartPosの移動量になる
                            deltaPos = (StartPos - EndPos) / time;
                            // 誤差があるとずれる可能性があるので念のためオブジェクトの位置をEndPosに設定
                            transform.position = EndPos;
                        }
                        else
                        {
                            // EndPos→StartPosだったので反転してにStartPos→EndPosする
                            // 現在の位置がStartPosなので EndPos - StartPosでStartPos→EndPosの移動量になる
                            deltaPos = (EndPos - StartPos) / time;
                            // 誤差があるとずれる可能性があるので念のためオブジェクトの位置をSrartPosに設定
                            transform.position = StartPos;
                        }
                        // 往路復路のフラグ反転
                        bStartToEnd = !bStartToEnd;
                        // 往路復路反転用経過時間クリア
                        elapsedTime = 0;
                    }
                }
                break;
            case EnemyState.Discovery:
                {
                    Debug.Log("プレイヤー発見");

                    // プレイヤーへの向きを獲得
                    Vector3 direction = PlayerPos - Position;

                    // ベクトルを、正規化し、向きだけを保存させる
                    direction.Normalize();

                    // 敵の移動
                    Position += direction * EnemySpeed;

                }
                break;
        }

    }
}
