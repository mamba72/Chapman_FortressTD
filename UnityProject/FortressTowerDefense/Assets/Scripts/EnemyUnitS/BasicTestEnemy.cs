using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTestEnemy : EnemyUnitScript
{


    protected override void Die()
    {
        //throw new System.NotImplementedException();
    }

    protected override void StartWalking()
    {
        //throw new System.NotImplementedException();
    }

    protected override void StopWalking()
    {
        //throw new System.NotImplementedException();
    }

    protected override void TurnLeft()
    {
        //throw new System.NotImplementedException();
    }

    protected override void TurnRight()
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        //call the parent's start script
        Starter();
    }

    // Update is called once per frame
    void Update()
    {
        //call the parent's pathing Script
        Pathing();
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    //Health = Health - collision.gameObject.GetComponent<Projectile>().damage;
    //    GameObject collidedObject = collision.gameObject;
    //    Debug.Log("Collided Objects Name: " + collidedObject.name);
    //    Debug.Log("Health = " + Health);
    //}
}
