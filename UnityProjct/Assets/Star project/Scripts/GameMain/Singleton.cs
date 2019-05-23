using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarProject.Gamemain;

public class Singleton : SingletonMonoBehaviour<Singleton>
{
    //『StarGenerator』を取得します
    public StarGenerator starGenerator;
    public GameSceneController gameSceneController;
    public CameraController cameraController;
    public SoundManager soundManager;
    public StarSpawn starSpawn;

    //public GameObject damageTextUI;

    public void OnDamage(int damage,Vector3 obj)
    {
        // プレハブデータ取得
        GameObject prefab = (GameObject)Resources.Load("DamageText");

        // 実体化
        GameObject damageText =
            Instantiate(prefab, obj, transform.rotation);

        damageText.GetComponent<DamageText>().SetText(damage);
    } 
}
