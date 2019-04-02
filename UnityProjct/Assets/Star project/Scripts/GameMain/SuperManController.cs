using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperManController : MonoBehaviour
{



    public float movespeed = 1.0f;



    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.gameObject.transform.position;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.gameObject.transform.position = new Vector3(pos.x, pos.y + 0.05f, pos.z);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.gameObject.transform.position = new Vector3(pos.x, pos.y - 0.05f, pos.z);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.gameObject.transform.position = new Vector3(pos.x - 0.05f, pos.y, pos.z);
            if (Input.GetKey(KeyCode.DownArrow))
            {
                this.gameObject.transform.position = new Vector3(pos.x - 0.05f, pos.y - 0.05f, pos.z);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                this.gameObject.transform.position = new Vector3(pos.x - 0.05f, pos.y + 0.05f, pos.z);
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.gameObject.transform.position = new Vector3(pos.x + 0.05f, pos.y, pos.z);
            if (Input.GetKey(KeyCode.DownArrow))
            {
                this.gameObject.transform.position = new Vector3(pos.x + 0.05f, pos.y - 0.05f, pos.z);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                this.gameObject.transform.position = new Vector3(pos.x + 0.05f, pos.y + 0.05f, pos.z);
            }
        }




    }
}
