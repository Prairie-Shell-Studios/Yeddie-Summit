using System;
using UnityEngine;

namespace PrairieShellStudios.MountainGeneration
{
    /// <summary>
    /// Generates Bezier Spline Patches.
    /// The control points must be 16 in size.
    /// Generates points in the v axis first and then the u axis.
    /// </summary>
    public class BezierSurfaceGenerator
    {
        #region fields

        private Matrix4x4 bezierMatrix;
        private const int MATRIX_SIZE = 4;

        #endregion

        #region constructors

        public BezierSurfaceGenerator()
        {
            Init();
        }

        #endregion

        #region patch generation

        /// <summary>
        /// Generates the vertices and triangles needed to input into a mesh to create a bezier surface.
        /// </summary>
        /// <param name="controlPoints">User defined points that will define the shape of the bezier surface. Must be 16 in length.</param>
        /// <param name="uResolution">The number of line segments to create along the u direction.</param>
        /// <param name="vResolution">The number of line segments to create along the v direction.</param>
        /// <returns>A tuple that contains the vertices in Item1 and triangles in Item2.</returns>
        public Tuple<Vector3[], int[]> GenerateSurface(Vector3[] controlPoints, int uResolution, int vResolution)
        {
            if (controlPoints.Length == BezierControlPoints.CP_SIZE)
            {
                Vector3[] vertices = GenerateVertices(controlPoints, uResolution, vResolution);
                int[] triangles = GenerateTriangles(uResolution, vResolution);

                return new Tuple<Vector3[], int[]>(vertices, triangles);
            }
            else
            {
                Debug.LogWarning("The control points array must be 16 in size to generate a Bezier Surface.");
                return null;
            }
        }

        #region helper methods

        /// <summary>
        /// Generates the triangles for the Bezier surface.
        /// </summary>
        /// <param name="uResolution">The amount of line segments to generate along the u direction.</param>
        /// <param name="vResolution">The amount of line segments to generate along the v direction.</param>
        /// <returns></returns>
        private int[] GenerateTriangles(int uResolution, int vResolution)
        {
            int[] triangles = new int[uResolution * vResolution * 6];

            for (int u = 0, vert = 0, tri = 0; u < uResolution; u++, vert++)
            {
                for (int v = 0; v < vResolution; vert++, v++, tri += 6)
                {
                    // creates a single quad
                    triangles[tri + 0] = vert;
                    triangles[tri + 1] = vert + 1;
                    triangles[tri + 2] = vert + vResolution + 1;
                    triangles[tri + 3] = vert + vResolution + 1;
                    triangles[tri + 4] = vert + 1;
                    triangles[tri + 5] = vert + vResolution + 2;
                }
            }

            return triangles;
        }

        /// <summary>
        /// Generates the vertices along the Bezier surface.
        /// </summary>
        /// <param name="controlPoints">The control points that will help define the shape of the Bezier surface.</param>
        /// <param name="uResolution">The amount of line segments to generate along the u direction.</param>
        /// <param name="vResolution">The amount of line segments to generate along the v direction.</param>
        /// <returns></returns>
        private Vector3[] GenerateVertices(Vector3[] controlPoints, int uResolution, int vResolution)
        {
            Vector3[] vertexBuffer = new Vector3[(uResolution + 1) * (vResolution + 1)];

            float t = 0.0f;
            float tInc = 1.0f / uResolution;
            int vertexCount = 0;

            for (int u = 0; u <= uResolution; u++)
            {
                // calculate the control points along the curve in the u direction at the current t value
                Vector4 ctrlPoint1 = ComputePatchVertex(
                    ComputeCoefficients(controlPoints[0], controlPoints[4], controlPoints[8], controlPoints[12]), t);
                Vector4 ctrlPoint2 = ComputePatchVertex(
                    ComputeCoefficients(controlPoints[1], controlPoints[5], controlPoints[9], controlPoints[13]), t);
                Vector4 ctrlPoint3 = ComputePatchVertex(
                    ComputeCoefficients(controlPoints[2], controlPoints[6], controlPoints[10], controlPoints[14]), t);
                Vector4 ctrlPoint4 = ComputePatchVertex(
                    ComputeCoefficients(controlPoints[3], controlPoints[7], controlPoints[11], controlPoints[15]), t);

                // uses the control points to calculate the vertices in the v direction
                ComputeBezierSegment(ref vertexBuffer, ref vertexCount, vResolution,
                    ComputeCoefficients(ctrlPoint1, ctrlPoint2, ctrlPoint3, ctrlPoint4));

                t += tInc;
            }

            return vertexBuffer;
        }

        /// <summary>
        /// Calculates a vertex along the Bezier surface with given coefficients and t value.
        /// </summary>
        /// <param name="coeffs">The coefficients calculate from the control points.<./param>
        /// <param name="t">The current t value along the curve.</param>
        /// <returns>A vector that contains the coordinates of the calculated vertex along the curve.</returns>
        private Vector4 ComputePatchVertex(Vector4[] coeffs, float t)
        {
            Vector4 tVector = new Vector4(Mathf.Pow(t, 3), Mathf.Pow(t, 2), t, 1.0f);

            return new Vector4(Vector4.Dot(tVector, coeffs[0]), Vector4.Dot(tVector, coeffs[1]), Vector4.Dot(tVector, coeffs[2]), 1.0f);
        }

        /// <summary>
        /// Calculates the vertices along the length of a curve.
        /// </summary>
        /// <param name="vertices">The array of each vertex in the surface.</param>
        /// <param name="vertexCount">The current number of vertices calculated.</param>
        /// <param name="vResolution">The number of line segments along the v direction.</param>
        /// <param name="coeffs">The coefficients to use to compute the vertices along the bezier curve.</param>
        private void ComputeBezierSegment(ref Vector3[] vertices, ref int vertexCount, int vResolution, Vector4[] coeffs)
        {
            float t = 0.0f;
            float tInc = 1.0f / vResolution;

            for (int v = 0; v <= vResolution; v++, vertexCount++, t += tInc)
            {
                vertices[vertexCount] = ComputePatchVertex(coeffs, t);
            }
        }

        /// <summary>
        /// Calculates bezier coefficients for use in vertex computation.
        /// </summary>
        /// <param name="point1">A control point to use to calculate the coefficients.</param>
        /// <param name="point2">A control point to use to calculate the coefficients.</param>
        /// <param name="point3">A control point to use to calculate the coefficients.</param>
        /// <param name="point4">A control point to use to calculate the coefficients.</param>
        /// <returns>A Vector4 that contains the 3 calculated bezier coefficients.</returns>
        private Vector4[] ComputeCoefficients(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
        {
            Vector4 ctrlPointsX = new Vector4(point1.x, point2.x, point3.x, point4.x);
            Vector4 ctrlPointsY = new Vector4(point1.y, point2.y, point3.y, point4.y);
            Vector4 ctrlPointsZ = new Vector4(point1.z, point2.z, point3.z, point4.z);
            Vector4[] coeffs = new Vector4[3];

            coeffs[0] = bezierMatrix * ctrlPointsX;
            coeffs[1] = bezierMatrix * ctrlPointsY;
            coeffs[2] = bezierMatrix * ctrlPointsZ;

            return coeffs;
        }

        #endregion

        #endregion

        #region initialization

        /// <summary>
        /// Initializes required fields.
        /// </summary>
        private void Init()
        {
            bezierMatrix = new Matrix4x4();
            for (int row = 0; row < MATRIX_SIZE; row++)
            {
                bezierMatrix.SetRow(0, new Vector4(-1.0f, 3.0f, -3.0f, 1.0f));
                bezierMatrix.SetRow(1, new Vector4(3.0f, -6.0f, 3.0f, 0.0f));
                bezierMatrix.SetRow(2, new Vector4(-3.0f, 3.0f, 0.0f, 0.0f));
                bezierMatrix.SetRow(3, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
            }
        }

        #endregion

    }
}