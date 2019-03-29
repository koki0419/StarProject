using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    //---------Unityコンポーネント宣言--------------
    [SerializeField]
    GameObject playerObj = null;
    //------------クラスの宣言----------------------
    [SerializeField]
    PlayerMove playerMove = null;
    //------------数値変数の宣言--------------------
    //------------フラグ変数の宣言------------------
    public void Init()
    {

    }

    //星を生成します
    void OnCreate(Vector3 createPos)
    {
        // プレハブデータ取得
        GameObject prefab = (GameObject)Resources.Load("Star");

        // 実体化
        GameObject starObj =
            Instantiate(prefab, createPos, transform.rotation);
        //親オブジェクトにくっ付けます
        starObj.transform.parent = this.gameObject.transform;
        //『startObj』の初期化
        starObj.GetComponent<StarController>().Init(playerObj, playerMove);
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
