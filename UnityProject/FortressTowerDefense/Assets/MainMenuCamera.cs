using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	public Transform target;
	int speed;
    // Start is called before the first frame update
    void Start()
    {
		speed = 10;
    }

    // Update is called once per frame
    void Update()
    {
		transform.LookAt(target);
		transform.Translate(Vector3.right * Time.deltaTime * speed);
	}
}
