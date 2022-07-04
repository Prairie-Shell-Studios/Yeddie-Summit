using System;
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

        //private BezierControlPoints controlPoints;
        private static readonly int[] peakRange = new int[] { 5, 6, 9, 10 };
        private const int MIN_DEN = 2;
        private const int MAX_DEN = 7;

        #endregion

        #region constructors

        public BezierControlPointGenerator()
        {
            // Does nothing so far
        }

        #endregion

        #region control point generation

        /// <summary>
        /// Generates all the control points that will be used to define the mountain.
        /// </summary>
        /// <param name="origin">The center position of the mountain.</param>
        /// <param name="maxX">The largest X position (relative to the origin) that the mountain can reach.</param>
        /// <param name="maxY">The largest Y position (relative to the origin) that the mountain can reach.</param>
        /// <param name="maxZ">The largest Z position (relative to the origin) that the mountain can reach.</param>
        /// <param name="resolution">Determines the number of Bezier surfaces that the mountain is made up of.</param>
        /// <returns>A 2D array that contains the control points that define the mountain.</returns>
        public Vector3[][] GenerateControlPoints(Vector3 origin, float maxX, float maxY, float maxZ, int resolution)
        {
            // get control points that determine the generated mountain shape
            BezierControlPoints controlPoints = GenerateMainControlPoints(origin, maxX, maxY, maxZ);

            // generate bezier surface that defines the generated mountain shape
            Vector3[] bezierVertices = GenerateBezierSurface(controlPoints, resolution);

            // parse bezier surface into control points
            Vector3[][] finalControlPoints = ParseSurface(resolution, bezierVertices);

            return finalControlPoints;
        }

        #region helpers

        /// <summary>
        /// Compute the control points that determine the overall mountain shape.
        /// There will always be 16 control points generated.
        /// </summary>
        /// <param name="origin">The center position of the mountain.</param>
        /// <param name="maxX">The largest X position (relative to the origin) that the mountain can reach.</param>
        /// <param name="maxY">The largest Y position (relative to the origin) that the mountain can reach.</param>
        /// <param name="maxZ">The largest Z position (relative to the origin) that the mountain can reach.</param>
        /// <returns>An array of control points that defines the overall mountain shape.</returns>
        private BezierControlPoints GenerateMainControlPoints(Vector3 origin, float maxX, float maxY, float maxZ)
        {
            BezierControlPoints points = new BezierControlPoints();

            // randomly select the peak points
            int peakPoint = peakRange[UnityEngine.Random.Range(0, peakRange.Length)];

            // generate the control points
            points.ControlPoints[0] = origin + new Vector3(-maxX, 0f, -maxZ);
            points.ControlPoints[1] = origin + new Vector3(-maxX, 0f, -maxZ / 2);
            points.ControlPoints[2] = origin + new Vector3(-maxX, 0f, maxZ / 2);
            points.ControlPoints[3] = origin + new Vector3(-maxX, 0f, maxZ);
            points.ControlPoints[4] = origin + new Vector3(-maxX / 2, 0f, -maxZ);

            points.ControlPoints[5] = peakPoint == 5 ? 
                origin + new Vector3(-maxX / 2, maxY, -maxZ / 2) : origin + new Vector3(-maxX / 2, maxY / UnityEngine.Random.Range(MIN_DEN, MAX_DEN), -maxZ / 2);
            points.ControlPoints[6] = peakPoint == 6 ? 
                origin + new Vector3(-maxX / 2, maxY, maxZ / 2) : origin + new Vector3(-maxX / 2, maxY / UnityEngine.Random.Range(MIN_DEN, MAX_DEN), maxZ / 2); 

            points.ControlPoints[7] = origin + new Vector3(-maxX / 2, 0f, maxZ);
            points.ControlPoints[8] = origin + new Vector3(maxX / 2, 0f, -maxZ);

            points.ControlPoints[9] = peakPoint == 9 ?
                origin + new Vector3(maxX / 2, maxY, -maxZ / 2) : origin + new Vector3(maxX / 2, maxY / UnityEngine.Random.Range(MIN_DEN, MAX_DEN), -maxZ / 2);
            points.ControlPoints[10] = peakPoint == 10 ?
                origin + new Vector3(maxX / 2, maxY, maxZ / 2) : origin + new Vector3(maxX / 2, maxY / UnityEngine.Random.Range(MIN_DEN, MAX_DEN), maxZ / 2);

            points.ControlPoints[11] = origin + new Vector3(maxX / 2, 0f, maxZ);
            points.ControlPoints[12] = origin + new Vector3(maxX, 0f, -maxZ);
            points.ControlPoints[13] = origin + new Vector3(maxX, 0f, -maxZ / 2);
            points.ControlPoints[14] = origin + new Vector3(maxX, 0f, maxZ / 2);
            points.ControlPoints[15] = origin + new Vector3(maxX, 0f, maxZ);

            return points;
        }

        /// <summary>
        /// Generates the number of control points needed to define the mountain with the specified
        /// number of Bezier Surfaces.
        /// </summary>
        /// <param name="controlPoints">The control points that define the overall shape of the mountain.</param>
        /// <param name="resolution">Defines the number of Bezier patches to compute.</param>
        /// <returns>An array of all the control points that define each Bezier patch which make up the mountains.</returns>
        private Vector3[] GenerateBezierSurface(BezierControlPoints controlPoints, int resolution)
        {
            BezierSurfaceGenerator gen = new BezierSurfaceGenerator();
            int surfaceResolution = 3 * resolution;

            Tuple<Vector3[], int[]> surfaceInfo = gen.GenerateSurface(controlPoints.ControlPoints, surfaceResolution, surfaceResolution);

            return surfaceInfo.Item1;
        }

        /// <summary>
        /// Divides up the surface into Bezier patches.
        /// </summary>
        /// <param name="resolution">The number of Bezier patches to generate (squared).</param>
        /// <param name="vertices">The vertices of the main control points.</param>
        /// <returns>The control points for each Bezier patch generated.</returns>
        private Vector3[][] ParseSurface(int resolution, Vector3[] vertices)
        {
            int numSurfaces = (int)Mathf.Pow(resolution, 2);
            Vector3[][] controlPoints = new Vector3[numSurfaces][];
            int[] jumps = ComputeJumps(resolution);

            for (int surf = 0, vert = 0, col = 0; surf < numSurfaces; surf++)
            {
                controlPoints[surf] = new Vector3[16];

                controlPoints[surf][0] = vertices[vert + 0];
                controlPoints[surf][1] = vertices[vert + 1];
                controlPoints[surf][2] = vertices[vert + 2];
                controlPoints[surf][3] = vertices[vert + 3];
                controlPoints[surf][4] = vertices[vert + jumps[0] + 0];
                controlPoints[surf][5] = vertices[vert + jumps[0] + 1];
                controlPoints[surf][6] = vertices[vert + jumps[0] + 2];
                controlPoints[surf][7] = vertices[vert + jumps[0] + 3];
                controlPoints[surf][8] = vertices[vert + jumps[1] + 0];
                controlPoints[surf][9] = vertices[vert + jumps[1] + 1];
                controlPoints[surf][10] = vertices[vert + jumps[1] + 2];
                controlPoints[surf][11] = vertices[vert + jumps[1] + 3];
                controlPoints[surf][12] = vertices[vert + jumps[2] + 0];
                controlPoints[surf][13] = vertices[vert + jumps[2] + 1];
                controlPoints[surf][14] = vertices[vert + jumps[2] + 2];
                controlPoints[surf][15] = vertices[vert + jumps[2] + 3];

                if ((surf + 1) % resolution == 0)
                {
                    vert = 3 * jumps[col];
                    col++;
                }
                else
                {
                    vert += 3;
                }
            }

            return controlPoints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public int[] ComputeJumps(int resolution)
        {
            int[] jumps = resolution <= 3 ? new int[3] : new int[resolution];

            for (int j = 0; j < jumps.Length; j++)
            {
                if (j == 0)
                {
                    jumps[j] = 3 * resolution + 1;
                }
                else
                {
                    jumps[j] = (j + 1) * jumps[0];
                }
            }

            return jumps;

        }

        #endregion

        #endregion
    }
}