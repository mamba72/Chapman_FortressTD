using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public PrefabInfo PrefabInfo;

    public int damage = 10;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        //GameObject collidedObject = collision.gameObject;
        Destroy(gameObject);
        //switch(collidedObject.tag)
        //{
        //    //destroy projectile when it collides with the ground
        //    case "Ground":
        //        Destroy(gameObject);
        //        break;
        //    case "Path"
        //}
        
    }
}
