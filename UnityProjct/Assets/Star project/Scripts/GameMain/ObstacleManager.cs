using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    //-------------Unityコンポーネント関係-------------------
    //エフェクト
    [SerializeField] private GameObject breakEffect = null;
    [SerializeField] private Renderer moaiRenderer = null;
    //-------------クラス関係--------------------------------

    //『PlayerMove』を取得します
    private PlayerMove playerMove = null;
    //-------------数値用変数--------------------------------
    //生成する星の数
    [SerializeField] private int starNum = 0;

    //ポイントを獲得した回数
    private int acquisitionPoint = 0;

    [SerializeField] private float deleteTime = 2.0f;

    //Hp
    [SerializeField] private float foundationHP;
    //HpMax
    private float foundationHPMax;

    private int breakSeNum = 7;

    [SerializeField] private GameObject obstaclesHeadObj = null;

    //-------------フラグ用変数------------------------------
    private bool onRemoveObjFlag = false;

    public bool isDestroyed
    {
        get; private set;
    }

    public void Init()
    {
        foundationHPMax = foundationHP;
        //『PlayerMove』を取得します
        playerMove = Singleton.Instance.gameSceneController.PlayerMove;
        //オブジェクトを削除するかどうか
        onRemoveObjFlag = false;
        //ポイントを獲得した回数
        acquisitionPoint = 0;

        var hp = 1.0;
        hp -= foundationHP / foundationHPMax;
        isDestroyed = false;
        breakEffect.SetActive(false);
    }




    // Update is called once per frame
    public void ObstacleControllerUpdate()
    {
        //オブジェクトを消去します
        if (onRemoveObjFlag)
        {
            deleteTime -= Time.deltaTime;
            if (deleteTime <= 0)
            {

                foreach (Transform child in gameObject.transform)
                {
                    Destroy(child.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.layer)
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Player" && acquisitionPoint == 0 && playerMove.canDamage)
        {
            //Hpをへらす
            foundationHP -= playerMove.attackPower;
            ////ダメージテキストにアクセスして生成します
            //Singleton.Instance.OnDamage((int)playerMove.attackPower, this.gameObject.transform.localPosition);
                        //TextMeshProを表示
            var textPos = gameObject.transform;
            var textSponPos = textPos.position;
            textSponPos.z -= 5.0f;

            Singleton.Instance.OnDamage((int)playerMove.attackPower, textSponPos);
            //ObjHｐがOになった時
            if (foundationHP <= 0)
            {
                Destroy(obstaclesHeadObj);
                playerMove.IsGround = false;
                isDestroyed = true;
                Singleton.Instance.soundManager.StopObstaclesSe();
                Singleton.Instance.soundManager.PlayObstaclesSe(breakSeNum);
                if (starNum != 0)
                {
                    Singleton.Instance.starGenerator.ObstaclesToStarSpon(this.transform.position, starNum);
                }
                //壊れたときにキャラクターと当たり判定を持たなくします
                //レイヤーの変更
                //レイヤーはやりすぎか？コライダー消去の方がよけれは修正要
                gameObject.layer = LayerMask.NameToLayer("BreakObstacls");
                acquisitionPoint++;
                breakEffect.SetActive(true);
                onRemoveObjFlag = true;
                moaiRenderer.enabled = false;

            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
}
