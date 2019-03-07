using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTrans : MonoBehaviour
{

    [SerializeField] Transform transform;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.localPosition;
        pos = new Vector3(0,0,0);
        transform.localPosition = pos;
    }
}
