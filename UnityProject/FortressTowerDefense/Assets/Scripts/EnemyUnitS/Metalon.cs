using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metalon : EnemyUnitScript
{
    
    
    
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

    protected override void StartWalking()
    {
        animator.SetBool("Walk Forward", true);
    }

    protected override void TurnRight()
    {
        animator.SetBool("Turn Right", true);
    }
    protected override void TurnLeft()
    {
        animator.SetBool("Turn Left", true);
    }
    protected override void StopWalking()
    {
        animator.SetBool("Walk Forward", false);
    }
    protected override void Die()
    {
        animator.SetBool("Die", true);

    }


}
