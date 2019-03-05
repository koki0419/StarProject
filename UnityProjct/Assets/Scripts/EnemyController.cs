using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void OnTriggerStay(Collider collision)
    {
        //プレイキャラクターを発見
        if (collision.tag == "Player")
        {
            enemyState = EnemyState.Discovery;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {
            enemyState = EnemyState.Search;
        }
    }

    //デフォルトスピード
    const float DefSpeed = 0.05f;

    //プレイヤーポジション
    GameObject playerObj;

    //プレイヤーポジション
    Vector3 playerPos;


     Vector3 StartPos;
     Vector3 EndPos;
    [SerializeField] Vector3 amountOfMovement;

    public float time;
    private Vector3 deltaPos;
    private float elapsedTime;
    private bool bStartToEnd = true;

    public void Init(GameObject player)
    {
        playerObj = player;

        // StartPosをオブジェクトに初期位置に設定
        StartPos = transform.position;
        var pos = transform.position;
        EndPos = pos += amountOfMovement;
        // 1秒当たりの移動量を算出
        deltaPos = (EndPos - StartPos) / time;
        elapsedTime = 0;
        enemyState = EnemyState.Search;

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

                    //プレイヤーポジション取得
                    playerPos = playerObj.transform.localPosition;

                    //自分の座標をプレイヤーの座標へ
                    //gameObject.transform.localPosition = playerPos;

                    //自分の座標をプレイヤーの座標からベクトル作成
                    Vector3 enemyVec = playerPos - gameObject.transform.localPosition;

                    //単位ベクトル作成（上記のベクトル）
                    Vector3 enemyVecE = enemyVec.normalized;

                    //長さを調節
                    enemyVec = enemyVecE * DefSpeed;

                    //自分の座標へプラスする
                    gameObject.transform.localPosition += enemyVec;

                    //単位ベクトル作成（上記のベクトル）
                    Vector3 V2 = enemyVec.normalized;

                    //座標設定用変数
                    Vector3 pos;
                    float x;
                    float z;

                    //向きベクトルの作成
                    //回転量
                    Vector3 objRot = transform.eulerAngles;
                    //キャラクタの向いている方向をベクトル計算
                    x = -Mathf.Sin(objRot.y * Mathf.Deg2Rad);
                    z = -Mathf.Cos(objRot.y * Mathf.Deg2Rad);
                    Vector3 V1 = new Vector3(x, 0, z);

                    //内積計算
                    float theta = V1.x * V2.x + V1.y * V2.y + V1.z * V2.z;

                    //計算誤差修正
                    if (theta > 1.0f)
                    {
                        theta = 1.0f;
                    }
                    if (theta < -1.0f)
                    {
                        theta = -1.0f;
                    }

                    //角度を求める acos
                    float rot = Mathf.Acos(theta);

                    //外積計算
                    float crosY = V1.z * V2.x - V1.x * V2.z;

                    //向き修正
                    if (crosY > 0.0f)
                    {
                        rot = -rot;
                    }

                    transform.Rotate(new Vector3(0, rot, 0));


                }
                break;
        }
    }
}

