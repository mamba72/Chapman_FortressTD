using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to click on UI even if there are objects behind it
using UnityEngine.EventSystems;
using System;

public class MouseManagerScript : MonoBehaviour
{
	public GameObject gmObject;
	private GameManagerScript gm;

	private GameObject BuildingToShowRange = null;

	// Start is called before the first frame update
	void Start()
	{
		gm = gmObject.GetComponent<GameManagerScript>();
	}

	// Update is called once per frame
	void Update()
	{
		//if the pointer is over something other than a game object (such as UI) dont do anything below
		if(EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hitInfo;

		if(Physics.Raycast(ray, out hitInfo))
		{
			try
			{
				GameObject ourHitObject = hitInfo.collider.transform.parent.gameObject;

				if(ourHitObject == null)
				{
					//Debug.Log("The hit object was null");
					ourHitObject = hitInfo.collider.transform.gameObject;
				}

				string hitTag = ourHitObject.tag;

				switch(hitTag)
				{
					case "Ground":
						MouseOver_GroundTile(ourHitObject);
						break;
					case "Enemy":
						MouseOver_EnemyUnit(ourHitObject);
						break;
					case "Building":
						MouseOver_Building(ourHitObject);
						break;
					default:
						//Debug.Log("We hovered over something with the following tag: " + hitTag);
						break;
				}

				//output the name of the object's parent hit
				//Debug.Log("Raycast hit: " + ourHitObject.name);

				//from here we can determine what kind of object we are over

				//if we are over a ground tile, then run the ground tile stuff
				//if (ourHitObject.GetComponentInChildren<GroundTileScript>() != null)
				//{
				//	MouseOver_GroundTile(ourHitObject);
				//}
				//else if (ourHitObject.GetComponent<EnemyUnitScript>() != null)
				//{
				//	//we are hovering over a unit
				//	MouseOver_EnemyUnit(ourHitObject);
				//}
				//else if(ourHitObject.GetComponent<Building>() != null)
				//{
				//	MouseOver_Building(ourHitObject);
				//}
			}
			catch(NullReferenceException e)
			{
				Debug.LogWarning("Mouse Manager threw a null reference exception. not sure what it was. but i think the parent doesnt exist on what we hovered over");
			}
		}


		//if player right clicks
		if (Input.GetMouseButtonDown(1))
		{
			//if there is a building currently selected
			if (BuildingToShowRange != null)
			{
				RemoveRangeDisplay();
			}

		}



	}

    void MouseOver_GroundTile(GameObject ourHitObject)
	{

        //now we know what we are mousing over
        //lets click on something

        //MouseManagerScript button 0 is left button
        if (Input.GetMouseButtonDown(0))
		{
			RemoveRangeDisplay();
			if(ourHitObject.CompareTag("Ground"))
			{
				if (gm.SelectedBuilding != null)
				{
					if(ourHitObject.GetComponentInChildren<GroundTileScript>().currBuilding == null)
					{
						GameObject testTower = Building.LocalPlaceBuilding(gm.SelectedBuilding, ourHitObject.transform, gm);
						//gm.netGm.InstantiateOverNetwork();

						if (testTower == null)
						{
							return;
						}

						ourHitObject.GetComponentInChildren<GroundTileScript>().currBuilding = testTower;
					}
				}
			}
		}
	}

	void MouseOver_EnemyUnit(GameObject ourHitObject)
	{
		//now we know what we are mousing over
		//lets click on something

		//MouseManagerScript button 0 is left button
		if (Input.GetMouseButtonDown(0))
		{
			RemoveRangeDisplay();
			//MeshRenderer mr = ourHitObject.GetComponentInChildren<MeshRenderer>();

			//Debug.Log("Clicked on enemy unit");

		}
	}

	void MouseOver_Building(GameObject ourHitObject)
	{
		//Debug.Log("Hovered over a building");
		if(Input.GetMouseButtonDown(0))
		{
			RemoveRangeDisplay();

			//Debug.Log("We clicked on a building");
			BuildingToShowRange = ourHitObject;

			Building buildingScript = ourHitObject.GetComponent<Building>();

			buildingScript.ShowRangeCircle();
			//show the ui
			gm.UIManager.ShowBuildingUpgradeUI(ourHitObject.GetComponent<Building>());

		}
	}


	void RemoveRangeDisplay()
	{
		if(BuildingToShowRange != null)
		{
			Building script = BuildingToShowRange.GetComponent<Building>();
			script.HideRangeCircle();
			BuildingToShowRange = null;

			gm.UIManager.HideBuildingUpgradeUI();
		}
		
	}

}
