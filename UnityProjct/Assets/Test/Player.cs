using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] float moveSpeed;
    [SerializeField] float airUpMoveSpeed;
    [SerializeField] float airDownMoveSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float dragPower;

    [SerializeField] float inputMoveKey;
    bool isGround;
    new Rigidbody rigidbody;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        dragPower = rigidbody.drag;
        isGround = false;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        var force = new Vector3(horizontal * moveSpeed, 0.0f, 0.0f);

        if (Input.GetButtonDown("Jump") && isGround)
        {
            // Debug.Log("Jump");
            rigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
        if (!isGround)
        {
            force = new Vector3(horizontal * airUpMoveSpeed, 0.0f, 0.0f);
            rigidbody.AddForce(force, ForceMode.Force);
            var velocity = rigidbody.velocity;
            // 下降中
            if (velocity.y < 0)
            {
                rigidbody.drag = 0;
                force = new Vector3(horizontal * airDownMoveSpeed, 0.0f, 0.0f);
                rigidbody.AddForce(force, ForceMode.Force);
            }
        }
        Debug.Log("force" + force);
        Debug.Log("horizontal" + horizontal);
        rigidbody.AddForce(force, ForceMode.Force);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Box001")
        {
            Debug.Log("ジャンプ");
            isGround = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Box001")
        {
            Debug.Log("着地");
            isGround = true;
            rigidbody.drag = dragPower;
        }
    }

}
