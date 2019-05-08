using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class StarGenerator : MonoBehaviour
{

    [SerializeField] private GameObject playerObj = null;

    [SerializeField] private PlayerMove playerMove = null;


    [SerializeField] private int sponMax;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private ObjectPool pool;
    [SerializeField] private Vector3[] starSponPosition;
    [SerializeField] private int[] starPoint;
    [SerializeField] private int starDysplayCount;

    public int activeCount
    {
        set;get;
    }

    private int sponIndex = 0;

    public void Init()
    {
       // pool = GetComponent<ObjectPool>();
        pool.CreatePool(starPrefab, sponMax);
        CreatStar();
    }

    public void CreatStar()
    {
        if (sponIndex < starSponPosition.Length-1)
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
                    sponIndex++;
                    activeCount++;
                }
                Debug.Log("sponIndex" + sponIndex);
            }
            Debug.Log("処理されていない");
        }

    }

    public void StarSponUpdate()
    {
        Debug.Log("activeCount" + activeCount);
        CreatStar();
    }

    //星を生成します
    void OnCreate(Vector3 createPos)
    {
        // 実体化
        GameObject starObj =
            Instantiate(starPrefab, createPos, transform.rotation);
        //親オブジェクトにくっ付けます
        starObj.transform.parent = this.gameObject.transform;
        //『startObj』の初期化
        starObj.GetComponent<StarController>().Init(playerMove, 1);
    }

    //他のスクリプトから呼び出します
    public void OnCreateStar(Vector3 starPos, int createNum)
    {
        var createCount = 0;
        var randX = Random.Range(-1, 1);
        var randY = Random.Range(1, 2);
        do
        {
            OnCreate(new Vector3(starPos.x += randX, starPos.y += randY, starPos.z));
            createCount++;
        } while (createCount != createNum);
    }
}
