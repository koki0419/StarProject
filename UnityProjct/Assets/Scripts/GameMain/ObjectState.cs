using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectState
{
    //オブジェクトステータス
    public enum ObjState
    {
        None,
        Normal,
        Attack,
        Wait,

    }

    public ObjState objState = ObjState.None;


}
