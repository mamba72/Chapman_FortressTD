using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace Com.MyCompany.FortressTD.Exceptions
{
    public class GeneralExceptions : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class BuildingExceptions 
    {
        public class TargetNotFoundException : Exception
        {
            public TargetNotFoundException()
            {

            }

            public TargetNotFoundException(string message) : base(message)
            {

            }
        }
    }
}

