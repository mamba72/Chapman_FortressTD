using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this importing the namespace I'm using for all my structs and helper classes
using HelperClasses;

public class GroundTileScript : MonoBehaviour
{
	public PrefabInfo PrefabInfo;

	public bool isPath = false;
	public bool isOccupied = false;

	public GameObject currBuilding = null;
	public Coordinates coordinates;
    private Color startcolor;

    /*by default the gound will not be a path
	 * the x and y positions will be the coordinates in the board array
	 */
    public GroundTileScript(int xPos, int yPos, bool isPath)
	{
		coordinates.x = xPos;
		coordinates.y = yPos;

		this.isPath = isPath;

	}


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public GroundTileScript[] GetNeighbors()
	{
		return null;
	}

    
    void OnMouseEnter()
    {
        if (isPath == false)
        {
            startcolor = GetComponent<Renderer>().material.color;
            GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
    void OnMouseExit()
    {
        if (isPath == false)
        {
            GetComponent<Renderer>().material.color = startcolor;
        }
    }
}
