using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudios.MountainGeneration
{
    /// <summary>
    /// Generates control points for use in generating Bezier surfaces.
    /// Points are generated around the origin of the GO.
    /// </summary>
    public class BezierControlPointGenerator
    {
        #region fields

        private BezierControlPoints controlPoints;

        #endregion

        #region properties
        public BezierControlPoints ControlPoints { get => controlPoints; }

        #endregion

        #region constructors

        public BezierControlPointGenerator()
        {
            controlPoints = new BezierControlPoints();
        }

        #endregion

        #region control point generation

        public Vector3[] GenerateControlPoints(Vector3 origin, float maxX, float maxY, float maxZ, int numSurfaces)
        {
            // TODO: add support for multiple surfaces
            controlPoints.ControlPoints[0] = origin - new Vector3(maxX, 0f, maxZ);
            controlPoints.ControlPoints[1] = origin - new Vector3(maxX, 0f, maxZ/2);
            controlPoints.ControlPoints[2] = origin - new Vector3(maxX, 0f, -maxZ/2);
            controlPoints.ControlPoints[3] = origin - new Vector3(maxX, 0f, -maxZ);
            controlPoints.ControlPoints[4] = origin - new Vector3(maxX/2, 0f, maxZ);
            controlPoints.ControlPoints[5] = origin - new Vector3(maxX/2, -maxY/4, maxZ/2);
            controlPoints.ControlPoints[6] = origin - new Vector3(maxX/2, -maxY/4, -maxZ/2);
            controlPoints.ControlPoints[7] = origin - new Vector3(maxX/2, 0f, -maxZ);
            controlPoints.ControlPoints[8] = origin - new Vector3(-maxX/2, 0f, maxZ);
            controlPoints.ControlPoints[9] = origin - new Vector3(-maxX/2, -maxY, maxZ/2);
            controlPoints.ControlPoints[10] = origin - new Vector3(-maxX/2, -maxY/2, -maxZ/2);
            controlPoints.ControlPoints[11] = origin - new Vector3(-maxX/2, 0f, -maxZ);
            controlPoints.ControlPoints[12] = origin - new Vector3(-maxX, 0f, maxZ);
            controlPoints.ControlPoints[13] = origin - new Vector3(-maxX, 0f, maxZ/2);
            controlPoints.ControlPoints[14] = origin - new Vector3(-maxX, 0f, -maxZ/2);
            controlPoints.ControlPoints[15] = origin - new Vector3(-maxX, 0f, -maxZ);

            return controlPoints.ControlPoints;
        }

        #region helpers

        #endregion

        #endregion
    }
}