using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public void OnAttackBullet(int damage, string objName,GameObject obj, float shotSpeed,Vector2 direction)
    {
        // プレハブデータ取得
        GameObject prefab = (GameObject)Resources.Load(objName);


        //単位ベクトル計算
        Vector2 firing = direction.normalized;

        //Debug.Log("firing = " + firing);

        // 座標設定用変数
        Vector3 pos;
        float x;
        float z;
        float y=1;

        // キャラクタ管理のデータ取得
        // 回転量
        Vector3 objRot = obj.transform.eulerAngles;
        // 座標
        Vector3 objPos = obj.transform.position;

        // キャラクタの向いている方向ベクトル計算
        x = Mathf.Sin(objRot.y * Mathf.Deg2Rad); //* shotSpeed;
        z = Mathf.Cos(objRot.y * Mathf.Deg2Rad); //* shotSpeed;

        // ショットポジション計算（ベクトルから座標へ）
        pos = new Vector3(objPos.x + x,
                          objPos.y,
                          objPos.z + z);

        // 実体化
        GameObject attackObj =
            Instantiate(prefab, pos, obj.transform.rotation);

       // Debug.Log("firing = " + firing);

        // ショットの移動ベクトル（加速度）
        
        Vector3 shotMov =
            new Vector3(shotSpeed * firing.x,
                        y * shotSpeed * firing.y,
                        0 );


            // 物理演算システム取得
            Rigidbody bulletRigit =
            attackObj.GetComponent<Rigidbody>();

        // アドレスチェック
        if (bulletRigit != null)
        {
            // 加速度設定
            bulletRigit.velocity = shotMov;
        }
        //親オブジェクトにくっ付けます
        //attackObj.transform.parent = obj.transform;
    }
}
