using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoalScript : MonoBehaviour
{
	public PrefabInfo PrefabInfo;

	public GameObject gmObject;
	public GameManagerScript gm;

	// Start is called before the first frame update
	void Start()
	{
		if(gmObject == null)
			gmObject = GameObject.FindGameObjectWithTag("GameManager");
		if(gm == null)
			gm = gmObject.GetComponent<GameManagerScript>();

		//if this player is not master client and is still connected to the server, then add the enemyscript on its own
		if (gm.netGm.isMasterClient == false && gm.netGm.isConnected == true)
		{
			gm.enemyGoalObj = this.gameObject;
			gm.enemyGoal = this;
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnTriggerEnter(Collider collider)
	{

		GameObject collidedObj = collider.gameObject;

		if(collidedObj.tag.Equals("Enemy"))
		{
			EnemyUnitScript enemyScript = collidedObj.GetComponent<EnemyUnitScript>();
			enemyScript.ReachedGoal();
			gm.LostLife();
		}
	}

	//private void OnCollisionEnter(Collision collider)
	//{
	//	Debug.Log("Collided with enemy. OnCollision was activated");
	//	GameObject collidedObj = collider.gameObject;

	//	if (collidedObj.CompareTag("Enemy"))
	//	{
	//		EnemyUnitScript enemyScript = collidedObj.GetComponent<EnemyUnitScript>();
	//		enemyScript.ReachedGoal();
	//		gm.LostLife();

	//		if(gm.NumberOfLives == 0)
	//		{
	//			gm.GameOver();
	//		}
	//	}
	//}
}
