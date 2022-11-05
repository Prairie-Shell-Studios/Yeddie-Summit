using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudios.ScriptableObjects
{ 
    /// <summary>
    /// A ScriptableObject that contains a single float value.
    /// Intended purpose is have the float value globally available to other
    /// scripts for reference.
    /// </summary>
    [CreateAssetMenu(fileName ="Float", menuName ="ScriptableObject/Float")]
    public class FloatScriptableObject : ScriptableObject
    {
        #region fields

        [SerializeField] private float value = 0f;

        #endregion

        #region properties

        public float Value { get => value; set => this.value = value; }

        #endregion
    }
}

