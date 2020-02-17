using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpTesting : MonoBehaviour
{
    [SerializeField] private Transform DamagePopUp;

    private void Start()
    {
        Transform damagePopUpTransform = Instantiate(DamagePopUp, Vector3.zero, Quaternion.identity);
        DamagePopUpScript damagePopUp = damagePopUpTransform.GetComponent<DamagePopUpScript>();
        damagePopUp.Setup(-10);
    }
}
