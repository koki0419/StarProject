using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class StarGenerator : MonoBehaviour
{
    //プレイヤーオブジェクト
    [SerializeField] private GameObject playerObj = null;
    //プレイヤースクリプト
    [SerializeField] private PlayerMove playerMove = null;

    [Header("☆プール生成数")]
    [SerializeField] private int sponMax;
    [Header("画面内に最大何個☆を表示するか")]
    [SerializeField] private int starDysplayCount;
    [Header("☆プレハブ")]
    [SerializeField] private GameObject starPrefab;
    private ObjectPool pool;
    [Header("☆生成場所と各☆獲得ポイント")]
    [SerializeField] private Vector3[] starSponPosition;
    [SerializeField] private int[] starPoint;

    //☆現在の表示数
    [HideInInspector]
    public int activeCount
    {
        set; get;
    }
    //☆生成数（経過）→次生成する☆のインデックス
    private int sponIndex = 0;

    public void Init()
    {
        pool = GetComponent<ObjectPool>();
        pool.CreatePool(starPrefab, sponMax);
        CreatStar();
    }

    public void CreatStar()
    {
        if (sponIndex < starSponPosition.Length - 1)
        {
            while (activeCount < starDysplayCount)
            {
                var star = pool.GetObject();
                if (star != null)
                {
                    //プレイヤーの位置座標をスクリーン座標に変換
                    star.transform.localPosition = starSponPosition[sponIndex];
                    star.GetComponent<StarController>().Init(playerMove, starPoint[sponIndex]);
                    star.GetComponent<StarController>().starGenerator = this;
                    star.GetComponent<StarController>().starSponType = StarController.StarSponType.SpecifiedSpon;
                    sponIndex++;
                    activeCount++;
                }
            }
        }
        else return;
    }

    public void StarSponUpdate()
    {
        CreatStar();
    }

    /// <summary>
    /// 障害物を壊した際に☆生成時に使用します
    /// </summary>
    /// <param name="targetPos">☆生成するときのポジション</param>
    /// <param name="sponIndex">☆生成数</param>
    public void ObstaclesToStarSpon(Vector3 targetPos, int sponIndex)
    {
        var spon = 0;
        var plusPosition = 0;
        var randPoint = Random.RandomRange(1, 4);
        while (true)
        {
            var star = pool.GetObject();
            if (star != null)
            {
                var randX = Random.Range(-1, 1);
                var randY = Random.Range(1, 2);

                //プレイヤーの位置座標をスクリーン座標に変換
                star.transform.localPosition = new Vector3(targetPos.x + randX, targetPos.y + randY + targetPos.z);
                star.GetComponent<StarController>().Init(playerMove, randPoint);
                star.GetComponent<StarController>().starGenerator = this;
                star.GetComponent<StarController>().starSponType = StarController.StarSponType.ObstacleSpon;
                spon++;
            }
            if (spon >= sponIndex) break;
        }
    }

}
