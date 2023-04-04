using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Samples
{
    [ExecuteInEditMode]
    internal class RotationScaller : MonoBehaviour
    {
        private void Update()
        {
            transform.localScale = new Vector3(1, 1f / Mathf.Cos(transform.rotation.eulerAngles.x * Mathf.Deg2Rad), 1f);
        }
    }
    
}
