using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonEnemyScript : EnemyUnitScript
{
    protected override void Die()
    {
        //throw new System.NotImplementedException();
    }

    protected override void StartWalking()
    {
        animator.SetBool("Walk Forward", true);
    }

    protected override void StopWalking()
    {
        animator.SetBool("Walk Forward", false);
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
}
