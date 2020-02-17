
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class NetworkEnemySyncer : MonoBehaviour, IPunObservable
{
	public EnemyUnitScript enemyScript;

	private Transform childTransform;
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			stream.SendNext(enemyScript.Health);
			//stream.SendNext(this.transform.position);
			//stream.SendNext(childTransform.rotation);
		}
		else
		{
			enemyScript.Health = (int)stream.ReceiveNext();
			//transform.position = (Vector3)stream.ReceiveNext();
			//transform.rotation = (Quaternion)stream.ReceiveNext();
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		//childTransform = GetTransformOfChildNotOfTargetPoint();

		//Debug.Log(childTransform.name);
	}

	
}
