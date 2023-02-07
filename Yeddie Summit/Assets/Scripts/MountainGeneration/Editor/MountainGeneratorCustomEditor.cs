using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace PrairieShellStudios.MountainGeneration
{
    /// <summary>
    /// A custom editor for the MountainGenerator class
    /// </summary>
    [CustomEditor(typeof(MountainGenerator))]
    public class MountainGeneratorCustomEditor : Editor
    {
        #region fields



        #endregion

        #region editor

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MountainGenerator gen = (MountainGenerator)target;

            GenerateTerrainButton(gen);
        }

        #endregion

        #region element generation

        private void GenerateTerrainButton(MountainGenerator gen)
        {
            if (GUILayout.Button("Generate New Terrain"))
            {
                gen.GenerateMountain();
                Debug.Log("Generatin new mountain.");
            }
        }

        #endregion

    }
}