using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverLineController : MonoBehaviour
{


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            Singleton.Instance.gameSceneController.GameOver = true;
        }
    }
}
