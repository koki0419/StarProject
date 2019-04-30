using UnityEngine;
using TMPro;
using UnityEngine.UI;

using StarProject.Result;

public class DamageText : MonoBehaviour
{
    public TextMeshPro text;

    private float startLife = 2.0f;

    static public int a = 0;


    /// <summary>
    /// ダメージ表示
    /// </summary>
    /// <param name="damage"></param>
    public void SetText(int damage)
    {
        ResultScreenController.all_damage += damage;
        text.SetText(damage.ToString());
        text.ForceMeshUpdate();
    }

    public void Update()
    {
        startLife -= Time.deltaTime;
        if (startLife <= 0)
        {
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
