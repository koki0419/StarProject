using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class DamageTextSpawn : MonoBehaviour
{
    [Header("☆プール生成数")]
    [SerializeField] private int spawnMax;
    [Header("☆プレハブ")]
    [SerializeField] private GameObject damageEffectPrefab;
    private ObjectPool pool;

    [SerializeField] private Sprite[] scoreNumbreSprite = null;

    [SerializeField] private Camera mainCamera = null;

    public void Init()
    {
        pool = GetComponent<ObjectPool>();
        pool.CreatePool(damageEffectPrefab, spawnMax);
    }

    public Vector3 debugVector = Vector3.zero;

    public void CreatDamageEffect(Vector3 sponPos,int damage)
    {
        StarProject.Result.ResultScreenController.all_damage += damage;
        //sponPosはワールド座標で取得するのでスクリーン座標に変換
        var screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, sponPos);

        var damageEffect = pool.GetObject();
        if (damageEffect != null)
        {
            damageEffect.GetComponent<DamageEffect>().Init();
            damageEffect.GetComponent<DamageEffect>().scoreNumbreSprite = new Sprite[10];
            for (int i = 0;i< scoreNumbreSprite.Length; i++)
            {
                damageEffect.GetComponent<DamageEffect>().scoreNumbreSprite[i] = this.scoreNumbreSprite[i];
            }
            damageEffect.GetComponent<DamageEffect>().SetDamage(damage);
            damageEffect.GetComponent<RectTransform>().position = screenPos;
        }
    }
}
