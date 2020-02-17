using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUpScript : MonoBehaviour
{

    //public static DamagePopUpScript Create()
    //{
    //    Transform damagePopUpTransform = Instantiate(DamagePopUp, Vector3.zero, Quaternion.identity);
    //    DamagePopUpScript damagePopUp = damagePopUpTransform.GetComponent<DamagePopUpScript>();
    //    damagePopUp.Setup(-10);
    //}
    private TextMeshPro TextMesh;

    private void Awake()
    {
        TextMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int DamageAmount)
    {
        TextMesh.SetText(DamageAmount.ToString());
    }

}
