using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : SingletonMonoBehaviour<Singleton>
{
    //『StarGenerator』を取得します
    public StarGenerator starGenerator;
    public GameSceneController gameSceneController;
}
