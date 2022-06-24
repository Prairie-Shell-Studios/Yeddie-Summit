using UnityEngine;

namespace PrairieShellStudios.MountainGeneration
{
    public class BezierControlPoints
    {
        #region fields

        private Vector3[] controlPoints;

        public enum Coord { X, Y, Z};

        public static readonly int CP_SIZE = 16;
        private static readonly int[] FRONT_INDICES = { 3, 7, 11, 15 };
        private static readonly int[] BACK_INDICES = { 0, 4, 8, 12 };
        private static readonly int[] LEFT_INDICES = { 0, 1, 2, 3 };
        private static readonly int[] RIGHT_INDICES = { 12, 13, 14, 15 };
        private static readonly int[] MID_INDICES = { 5, 6, 9, 10 };

        #endregion

        #region properties

        public Vector3[] ControlPoints { get => controlPoints; set => controlPoints = value; }
        public Vector3[] LeftPoints { get => GetPoints(LEFT_INDICES); set { SetPointValues(LEFT_INDICES, value); } }
        public Vector3[] RightPoints { get => GetPoints(RIGHT_INDICES); set { SetPointValues(RIGHT_INDICES, value); } }
        public Vector3[] FrontPoints { get => GetPoints(FRONT_INDICES); set { SetPointValues(FRONT_INDICES, value); } }
        public Vector3[] BackPoints { get => GetPoints(BACK_INDICES); set { SetPointValues(BACK_INDICES, value); } }

        #endregion

        #region constructors

        public BezierControlPoints()
        {
            controlPoints = new Vector3[CP_SIZE];
        }

        public BezierControlPoints(Vector3[] points)
        {
            if (points.Length == CP_SIZE)
            {
                controlPoints = points;
            }
            else
            {
                controlPoints = new Vector3[CP_SIZE];
                Debug.LogWarning("BezierControlPoints was initiated with invalid array length. Instance set to empty.");
            }
        }

        #endregion

        #region points

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftPoints"></param>
        /// <param name="rightPoints"></param>
        /// <param name="frontPoints"></param>
        /// <param name="backPoints"></param>
        public void ComputePoints(Vector3[] leftPoints, Vector3[] rightPoints, Vector3[] frontPoints, Vector3[] backPoints)
        {
            LeftPoints = leftPoints;
            RightPoints = rightPoints;
            FrontPoints = frontPoints;
            BackPoints = backPoints;
            InterpolateMidPoints();
        }

        /// <summary>
        /// 
        /// </summary>
        public void InterpolateMidPoints()
        {
            controlPoints[MID_INDICES[0]] = (controlPoints[0] + controlPoints[1] + controlPoints[4]) / 3;
            controlPoints[MID_INDICES[1]] = (controlPoints[2] + controlPoints[3] + controlPoints[7]) / 3;
            controlPoints[MID_INDICES[2]] = (controlPoints[8] + controlPoints[12] + controlPoints[13]) / 3;
            controlPoints[MID_INDICES[3]] = (controlPoints[11] + controlPoints[14] + controlPoints[15]) / 3;
        }

        #endregion

        #region edges

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        public void SetEdgeValues(Vector3 value)
        {
            SetPointValues(FRONT_INDICES, value);
            SetPointValues(BACK_INDICES, value);
            SetPointValues(LEFT_INDICES, value);
            SetPointValues(RIGHT_INDICES, value);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        public void SetEdgeValues(Coord coord, float value)
        {
            SetPointValues(FRONT_INDICES, coord, value);
            SetPointValues(BACK_INDICES, coord, value);
            SetPointValues(LEFT_INDICES, coord, value);
            SetPointValues(RIGHT_INDICES, coord, value);
        }

        /// <summary>
        /// Retrieves the points stored at the specified indices inside the controlPoints field.
        /// </summary>
        /// <param name="indices">The indices of the points to retrieve.</param>
        /// <returns></returns>
        private Vector3[] GetPoints(int[] indices)
        {
            Vector3[] points = new Vector3[4];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = controlPoints[indices[i]];
            }

            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="values"></param>
        private void SetPointValues(int[] indices, Vector3[] values)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                controlPoints[i] = values[i];
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="values"></param>
        private void SetPointValues(int[] indices, Vector3 value)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                controlPoints[i] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="coord"></param>
        /// <param name="value"></param>
        private void SetPointValues(int[] indices, Coord coord, float value)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                controlPoints[i][(int)coord] = value;
            }
        }

        #endregion
    }
}