using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /*
        //移動開始位置
        public Vector3 StartPos;

        //移動終了位置
        public Vector3 EndPos;

        //移動時間
        public float time;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }*/

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
        if(collision.tag == "Player")
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

    public Vector3 StartPos;
    public Vector3 EndPos;
    public float time;
    private Vector3 deltaPos;
    private float elapsedTime;
    private bool bStartToEnd = true;
    void Start()
    {
        // StartPosをオブジェクトに初期位置に設定
        transform.position = StartPos;
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
                    Debug.Log("プレイヤー発見");



                } break;
        }

    }
}

