using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryTowerScript : Building
{

    //public Rigidbody projectile;
    public Transform Spawnpoint;
    public float speed = 100;
    //public int buildingDamage;

    

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    //point and throw the projectile at the target
    public override void ShootTarget()
    {
        if (target != null)
        {
            Vector3 direction;
            GameObject adjGameObject = GetTargetPoint();
            Vector3 adjTarget;// = //adjGameObject.transform;
            adjTarget = new Vector3(adjGameObject.transform.position.x, adjGameObject.transform.position.y, adjGameObject.transform.position.z);
            // float projectileRotation = target.transform.rotation.x - transform.rotation.x
            direction = adjTarget - Spawnpoint.transform.position;
            Vector3 velocity = direction.normalized * speed;
            //spawn projectile
            Rigidbody clone;
            clone = (Rigidbody)Instantiate(Projectiles[upgradeLevel], Spawnpoint.position, Projectiles[upgradeLevel].rotation);
            clone.rotation = Quaternion.LookRotation(adjTarget - Spawnpoint.transform.position);
            //projectile velocity
            clone.velocity = Spawnpoint.TransformDirection(velocity);
            //get the amount of damage the projectile causes
            clone.gameObject.GetComponent<Projectile>().damage = (int)((float)clone.gameObject.GetComponent<Projectile>().damage * damageModifier);
        }
    }

    private GameObject GetTargetPoint()
    {
        for(int i = 0; i < target.transform.childCount; ++i)
        {
            GameObject targetPoint = target.transform.GetChild(i).gameObject;
            if(targetPoint.name == "TargetPoint")
            {
                return targetPoint;
            }
        }
        return null;
    }
}
