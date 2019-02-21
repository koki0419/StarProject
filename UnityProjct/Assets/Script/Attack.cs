using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public void OnAttackBullet(string objName, GameObject obj, float shotSpeed, Vector2 direction)
    {
        // プレハブデータ取得
        GameObject prefab = (GameObject)Resources.Load(objName);


        //単位ベクトル計算
        Vector2 firing = direction.normalized;


        //// ラジアン
        //float radian = Mathf.Atan2(firing.y, firing.x);

        //// 角度
        //float degree = radian * Mathf.Rad2Deg;

        //Debug.Log("degree = " + degree);

        // 座標設定用変数
        Vector3 pos;
        float x;
        float z;
        float y = 1;

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


        // ショットの移動ベクトル（加速度）
        Vector3 shotMov;
        if (firing.x != 0 || firing.y != 0)
        {
            shotMov =
            new Vector3(shotSpeed * firing.x,
                        y * shotSpeed * firing.y,
                        0);
        }
        else
        {
            firing.x = x;
            shotMov =
            new Vector3(shotSpeed * firing.x,
                        y * shotSpeed * firing.y,
                        0);
        }




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

    public string OnAttack(Vector2 direction, GameObject obj)
    {
        string animationName;

        // 座標設定用変数
        float x;

        // キャラクタ管理のデータ取得
        // 回転量
        Vector3 objRot = obj.transform.eulerAngles;
        // 座標
        Vector3 objPos = obj.transform.position;

        // キャラクタの向いている方向ベクトル計算
        x = Mathf.Sin(objRot.y * Mathf.Deg2Rad);

        //単位ベクトル計算
        Vector2 firing = direction.normalized;
        // ラジアン
        float radian = Mathf.Atan2(firing.y, firing.x);

        // 角度
        float degree = radian * Mathf.Rad2Deg;

        Debug.Log("firing = " + firing);
        Debug.Log("degree = " + degree);
        if (firing.x == 0 && firing.y == 0)
        {
            animationName = null;
        }
        else
        {
            if (degree < 30 && degree > -30)
            {
                animationName = "右";
            }
            else if (degree > 150 && degree <= 180 || degree < -150 && degree >= -180)
            {
                animationName = "左";
            }
            else if (degree > 30 && degree < 150)
            {
                animationName = "上";
            }
            else if (degree < -30 && degree > -150)
            {
                animationName = "下";
            }
            else
            {
                animationName = null;
            }
        }
        return animationName;
    }
}
