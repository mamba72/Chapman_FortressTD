using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private float moveSpeed = 0.5f;
    private float scrollSpeed = 0.1f;
    public GameObject board;
    GameManagerScript boardScript;
    // Start is called before the first frame update
    void Start()
    {
        boardScript = board.GetComponent<GameManagerScript>();

    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            transform.position += moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }*/

        if (transform.position.x < boardScript.BoardBounds.x)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                transform.position += moveSpeed * new Vector3(1, 0, 0);
            }
        }

        if (transform.position.x > 0)
        {
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                transform.position -= moveSpeed * new Vector3(1, 0, 0);
            }
        }

        if(transform.position.z < boardScript.BoardBounds.y - 5)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                transform.position += moveSpeed * new Vector3(0, 0, 1);
            }
        }

        if (transform.position.z > -2)
        {
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                transform.position -= moveSpeed * new Vector3(0, 0, 1);
            }
        }



        if (transform.position.y < 20)
        { 
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                transform.position += scrollSpeed * new Vector3(0, transform.position.y, 0);
            }
        }
        if (transform.position.y > 3)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                transform.position -= scrollSpeed * new Vector3(0, transform.position.y, 0);
            }
        }
    }
}
