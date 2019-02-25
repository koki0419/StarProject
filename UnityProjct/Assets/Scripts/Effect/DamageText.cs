using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TextMeshPro text;

    float startLife = 2.0f;

    public void SetText(int damage)
    {
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
