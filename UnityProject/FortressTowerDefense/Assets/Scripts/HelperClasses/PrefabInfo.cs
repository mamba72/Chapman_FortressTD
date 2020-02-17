using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInfo : MonoBehaviour
{
	public string Category;
	public string PrefabName = string.Empty;

	//public PrefabInfo(string Category)
	//{
	//	this.Category = Category;
		
	//}

	//public PrefabInfo(string Category, string Name)
	//{
	//	this.Category = Category;
	//	this.PrefabName = Name;
	//}

	public void Awake()
	{

		if(PrefabName == string.Empty || PrefabName == "")
		{
			string objName = this.name;

			PrefabName = objName.Replace("(Clone)", "");
		}
		
	}
}
