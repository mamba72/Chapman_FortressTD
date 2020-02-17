using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyUnitScript : MonoBehaviour
{
	public PrefabInfo PrefabInfo;

	public GameObject gmObject;
	public GameManagerScript gm;

	public string UnitName;

	public Vector3 destination;
	public float speed = 2;

	public bool enablePathing = false;
	public int nextTileNumber = 0;
	public GameObject nextTile;

	public int KillReward;
	public int Health;
	public bool Collided = false;

	protected Animator animator = null;

	protected Photon.Pun.PhotonView PhotonView;
	
	//this is essentially the Start function, but children scripts dont call the parent's Start function
	protected void Starter()
	{
		PhotonView = Photon.Pun.PhotonView.Get(this);


		if (gmObject == null)
		{
			gmObject = GameObject.FindGameObjectWithTag("GameManager");
		}
		gm = gmObject.GetComponent<GameManagerScript>();
		destination = transform.position;

		animator = GetComponentInChildren<Animator>();
        if(animator != null)
        {
            StartWalking();
        }

		//if this player is not master client and is still connected to the server, then add the enemy on its own
		if(gm.netGm.isMasterClient == false && gm.netGm.isConnected == true)
			gm.enemySpawner.CurrentSpawnedEnemies.Add(this.gameObject);
	}

	// Start is called before the first frame update
	void Start()
	{
		if (gmObject == null)
		{
			gmObject = GameObject.FindGameObjectWithTag("GameManager");
		}
		gm = gmObject.GetComponent<GameManagerScript>();
		destination = transform.position;

		animator = GetComponentInChildren<Animator>();
	}

	// Update is called once per frame
	//void Update()
	//{
	//	//move towards destination
	//	Vector3 dir;
	//	if(enablePathing)
	//		dir = GetNextTile() - transform.position;
	//	else
	//		dir = destination - transform.position;
		
	//	Vector3 velocity = dir.normalized * speed * Time.deltaTime;

	//	//this is to prevent a unit from wiggling back and forth when it gets to the destination or overshooting the destination
	//	velocity = Vector3.ClampMagnitude(velocity, dir.magnitude);
	//	//actually do the movement
	//	transform.Translate(velocity);
	//}


	private Transform GetMyChildCharacter()
	{
		for(int i = 0; i < transform.childCount; ++i)
		{
			Transform child = transform.GetChild(i);

			if(!child.gameObject.name.Contains("TargetPoint"))
			{
				return child;
			}

		}

		return null;
	}

	//this is the pathing function that needs to be called in update of the children
	protected void Pathing()
	{
		//move towards destination
		Vector3 dir;
		if (enablePathing)
		{
			dir = GetNextTile() - transform.position;
			//try
			//{
				
			//}
			//catch(UnassignedReferenceException)
			//{
			//	dir = new Vector3() - transform.position;
			//}
			
		}
		else
			dir = destination - transform.position;

		Vector3 velocity = dir.normalized * speed * Time.deltaTime;

		//this is to prevent a unit from wiggling back and forth when it gets to the destination or overshooting the destination
		velocity = Vector3.ClampMagnitude(velocity, dir.magnitude);

		GetMyChildCharacter().rotation = Quaternion.LookRotation(dir);

		//actually do the movement
		transform.Translate(velocity);
	}

	[PunRPC]
	public void StartPathingRPC()
	{
		StartPathing();
	}


	Vector3 GetNextTile()
	{
		//if the path tile order is still empty, then just return the current position
		if (gm.PathTileOrder.Count == 0)
		{
			return transform.position;
		}
		//round all the coordinates to hundredths
		float myPosXRounded = Mathf.Round(transform.position.x * 100f) / 100f;
		float myPosZRounded = Mathf.Round(transform.position.z * 100f) / 100f;
		float tilePosXRounded = Mathf.Round(nextTile.transform.position.x * 100f) / 100f;
		float tilePosZRounded = Mathf.Round(nextTile.transform.position.z * 100f) / 100f;

		//if we have already made it to the next tile, grab a new one
		if (myPosXRounded == tilePosXRounded && myPosZRounded == tilePosZRounded)
		{
			//if (animator != null)
			//	animator.SetBool("Walk Forward", true);
			nextTileNumber++;
			//if the next tile is out of range of the pathing order list, then stop pathing and sit still
			if(nextTileNumber >= gm.PathTileOrder.Count)
			{
				//if (animator != null)
				//	animator.SetBool("Walk Forward", false);
				if (animator != null)
					StopPathing();
				return transform.position;
			}

			nextTile = gm.PathTileOrder[nextTileNumber];
			Turn();
		}

		//remove the y coordinate because we dont want our guys changing height
		Vector3 dest = new Vector3(nextTile.transform.position.x, 0, nextTile.transform.position.z);
		return dest;
	}


	//subscribe to all the events
	private void OnEnable()
	{
		MapCreationScript.OnFinishedMap += StartPathing;
	}

	//unsubscribe from all the events
	private void OnDisable()
	{
		MapCreationScript.OnFinishedMap -= StartPathing;
	}


	//this will enable the units to start moving between tiles
	public void StartPathing()
	{
		enablePathing = true;
		nextTile = gm.PathTileOrder[nextTileNumber];
        //Debug.Log("Start Pathing");
        if (animator != null)
        {
            StartWalking();
        }
    }

	//tell the unit to sit still
	public void StopPathing()
	{
		enablePathing = false;
		destination = transform.position;
		//Debug.Log("Stop Pathing");
		if(animator != null)
			StopWalking();
	}

	//this will determine whether the current object needs to be turned or not and will call the appropriate movement animation
	public void Turn()
	{
		Transform childTrans = GetMyChildCharacter();
		//to go left
		if (transform.position.z <= nextTile.transform.position.z && childTrans.rotation.y !=-90)
		{
			childTrans.rotation = new Quaternion (0,-90,0, 0);
			if (animator != null)
				TurnLeft();
			//Debug.Log("turn left");
		}
		//turn right
		if (transform.position.z >= nextTile.transform.position.z && childTrans.rotation.y != 90)
		{
			childTrans.rotation = new Quaternion(0, 90, 0, 0);
			if (animator != null)
				TurnRight();
			//Debug.Log("turn right");
		}
	}

	public void OnTriggerEnter(Collider collision)
	{
		GameObject collidedObject = collision.gameObject;

		//Debug.Log(gameObject.name + " Collided with " + collidedObject.name + " with tag: " + collidedObject.tag);

		switch (collidedObject.tag)
		{
			case "Projectile":
				//set collided to true so it does not trigger twice
				if(Collided == true)
				{
					Invoke("ResetCollision", 0.01f);
					return;
				}
				Collided = true;
				Invoke("ResetCollision", 0.01f);
				Health = Health - collidedObject.GetComponent<Projectile>().damage;
				//Debug.Log("Health = " + Health);
				
				//destroy the projectile
				Destroy(collidedObject);
				if (Health <= 0)
				{
					EnemyKilled();
				}
				break;
		}
	}

	public void ResetCollision()
	{
		Collided = false;
	}

	public void EnemyKilled()
	{
		if(Health <= 0)
		{
			//Write down that you killed an enemy
			Settings.AddToKilledEnemies();

			//stop the enemy from moving (maybe)
			if (animator != null)
				StopPathing();
			//call the animation
			Die();
			//increase coins
			gm.EconomyManager.IncreaseCoins(KillReward);
			//remove from current spawned enemies list
			gm.enemySpawner.CurrentSpawnedEnemies.Remove(gameObject);
			//delete dead enemy
			gm.netGm.DestroyOverNetwork(gameObject);//, 2.0f);
		}
	}

	//this function is used when the enemy makes it across the map to the end of the board
	//this will delete the unit without dying
	public void ReachedGoal()
	{
		//stop the enemy from moving (maybe)
		if (animator != null)
			StopPathing();
		//remove from current spawned enemies list
		gm.enemySpawner.CurrentSpawnedEnemies.Remove(gameObject);
		//delete the enemy that survived
		gm.netGm.DestroyOverNetwork(gameObject);//, 2.0f);
	}


	private void OnDestroy()
	{
		gm.enemySpawner.CurrentSpawnedEnemies.Remove(this.gameObject);
	}


	protected abstract void StartWalking();

	protected abstract void TurnRight();
	protected abstract void TurnLeft();
	protected abstract void StopWalking();
	protected abstract void Die();

	public delegate void EnemyDied(object sender, EventArgs e, int killReward);
	public static event EnemyDied OnEnemyDied;


}
