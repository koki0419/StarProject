using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAttack : MonoBehaviour
{

    GameObject Player;

    ChargePointManager script;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        script = Player.GetComponent<ChargePointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        int starCount = script.StarChildCount;

    }



    private void OnTriggerExit(Collider collision)
    {


        if (collision.tag == "Player")
        {


/*            starCount = 0;
            if (starCount == 0)
            {
                SceneManager.LoadScene("ResultScene");
            }*/
        }
    }

}
