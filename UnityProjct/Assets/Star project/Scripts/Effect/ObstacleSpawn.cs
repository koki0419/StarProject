using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class ObstacleSpawn : MonoBehaviour
{
    //プレイヤースクリプト
    [SerializeField] private PlayerMove playerMove = null;
    [SerializeField] private GameObject playerObj = null;

    [Header("障害物プール生成数")]
    [SerializeField] private int spawnMax;
    public int SpawnMax
    {
        get { return spawnMax; }
    }

    [Header("画面内に最大何個障害物を表示するか")]
    [SerializeField] private int obstaclesDysplayCount;
    [Header("障害物プレハブ")]
    [SerializeField] private GameObject obstaclesPrefab;
    private ObjectPool pool;
    [Header("障害物生成場所と各障害物HPと壊した際の☆生成数")]
    [SerializeField] private Vector3[] obataclesSponPosition;
    [SerializeField] private int[] obataclesHp;
    [SerializeField] private int[] spawnStarNum;

    /// <summary>
    /// エネミーのタイプ
    /// </summary>
    private enum EnemyTyp
    {
        None,
        NotMoveEnemy,//
        MoveEnemy,
        AirMoveEnemy,
    }
    [SerializeField] private EnemyTyp[] enemyTyp;

    [SerializeField] private Vector3[] amountOfMovement;
    //移動スピード
    [Header("各状態の移動速度")]
    [SerializeField] private float[] searchMoveSpeed;
    private float lockOnMoveSpeed = 5000;
    private float attackUpOnMoveSpeed = 1000;
    //[SerializeField] private float[] removeMoveSpeed;
    [SerializeField] private float[] defaultAttackTime;

    //☆現在の表示数
    [HideInInspector]
    public int activeCount
    {
        set; get;
    }
    //☆生成数（経過）→次生成する☆のインデックス
    private int spawnIndex = 0;

    public void Init()
    {
        pool = GetComponent<ObjectPool>();
        pool.CreatePool(obstaclesPrefab, spawnMax);
        CreatObstacle();
    }

    public void CreatObstacle()
    {
        if (spawnIndex < obataclesSponPosition.Length - 1)
        {
            while (activeCount < obstaclesDysplayCount)
            {
                var obstacle = pool.GetObject();
                if (obstacle != null)
                {
                    if (obstacle.GetComponent<Rigidbody>() == null) { obstacle.AddComponent<Rigidbody>(); Debug.Log("追加した"); }
                    //プレイヤーの位置座標をスクリーン座標に変換
                    obstacle.transform.localPosition = obataclesSponPosition[spawnIndex];
                    obstacle.GetComponent<ObstacleManager>().playerMove = this.playerMove;
                    obstacle.GetComponent<ObstacleManager>().foundationHP = this.obataclesHp[spawnIndex];
                    obstacle.GetComponent<ObstacleManager>().spawnStarNum = this.spawnStarNum[spawnIndex];
                    obstacle.GetComponent<ObstacleManager>().obstacleSpawn = this;
                    obstacle.GetComponent<ObstacleManager>().Init();
                    switch (this.enemyTyp[spawnIndex])
                    {
                        case EnemyTyp.NotMoveEnemy:
                            obstacle.GetComponent<EnemyController>().enemyTyp = EnemyController.EnemyTyp.NotMoveEnemy;
                            break;
                        case EnemyTyp.MoveEnemy:
                            obstacle.GetComponent<EnemyController>().enemyTyp = EnemyController.EnemyTyp.MoveEnemy;
                            break;
                        case EnemyTyp.AirMoveEnemy:
                            obstacle.GetComponent<EnemyController>().enemyTyp = EnemyController.EnemyTyp.AirMoveEnemy;
                            break;
                    }

                    obstacle.GetComponent<EnemyController>().amountOfMovement = this.amountOfMovement[spawnIndex];
                    obstacle.GetComponent<EnemyController>().searchMoveSpeed = this.searchMoveSpeed[spawnIndex];
                    obstacle.GetComponent<EnemyController>().lockOnMoveSpeed = this.lockOnMoveSpeed;
                    obstacle.GetComponent<EnemyController>().attackUpOnMoveSpeed = this.attackUpOnMoveSpeed;
                    //obstacle.GetComponent<EnemyController>().removeMoveSpeed = this.removeMoveSpeed[spawnIndex];
                    obstacle.GetComponent<EnemyController>().defaultAttackTime = this.defaultAttackTime[spawnIndex];
                    obstacle.GetComponent<EnemyController>().Init(this.playerObj);


                    spawnIndex++;
                    activeCount++;
                }
            }
        }
        else return;
    }

    public void ObstaclesSponUpdate()
    {
        CreatObstacle();
    }
}
