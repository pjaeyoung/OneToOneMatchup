using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    int sensibilityX = 5;
    int playerSpeed = 2;
    Rigidbody playerRigidBody;
    Ray ray;
    RaycastHit hit;
    public GameObject point;
    GameObject ball;
    Rigidbody ballBody;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
        if(Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag =="object")
                {
                    ball = hit.transform.gameObject;
                    ballBody = ball.GetComponent<Rigidbody>();
                    ballBody.useGravity = false;
                    hit.transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
                }
            }
        }
        if(Input.GetMouseButton(1))
        {
            point.SetActive(true);
            point.transform.localPosition += new Vector3(Input.GetAxisRaw("Mouse X") * 2, 0, Input.GetAxisRaw("Mouse Y") * 2);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            Vector3 dir = point.transform.position - ball.transform.position;
            ballBody.velocity = ball.transform.TransformDirection(new Vector3(dir.x, 0, dir.z));
            ballBody.useGravity = true;
            point.SetActive(false);
        }
        else
        {
            point.transform.position = transform.position;
        }
        if (Input.GetMouseButton(2)) //회전
        {
            transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensibilityX, 0);
        }
       if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
        { //움직임, 움직이는 애니
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * playerSpeed / 20);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * playerSpeed / 20);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * playerSpeed / 20);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * playerSpeed / 20);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y <= 0.6f) //점프
        {
            playerRigidBody.AddForce(new Vector3(0, 300, 0));
        }

    }
}
