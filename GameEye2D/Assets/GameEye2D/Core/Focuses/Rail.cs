/* Rail v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;
using System.Collections;
using System;

namespace GameEye2D.Focus
{
    /// <summary>
    /// Creates a rail system when attached to an object. The class is abstract, and can be inherited from to create rail-based focus’s.
    /// </summary>
    public abstract class Rail : Focus2D
	{
        //The points that make the rail.
		[SerializeField] protected Vector2[] m_Points = new Vector2[] { new Vector2(-15f, 0f), new Vector2(-5f, 0f), new Vector2(5f, 0f), new Vector2(15f, 0f) };


        /// <summary>
        /// The points that are used to calculate the rail’s bezier curve.
        /// Be careful with setting this value, as if the number of points is not 4, 7, 11, 14, etc... it is possible to make incomplete curves which can cause errors calculating the focus point.
        /// </summary>
        public Vector2[] points
		{
			get { return m_Points; }
			set { m_Points = value; }
		}

        /// <summary>
        /// Returns the number of curves in the rail.
        /// </summary>
        public int curveCount
        {
            get { return (m_Points.Length - 1) / 3; }
        }

        /// <summary>
        /// Returns the number of points in the rail.
        /// </summary>
        public int length
        {
            get { return m_Points.Length; }
        }

        /// <summary>
        /// Gets a point along a curve in the rail.
        /// </summary>
        public Vector2 GetPoint (float interpolation, int curve)
		{
			float oneMinusInterpolation = 1f - interpolation;
            return transform.TransformPoint (   oneMinusInterpolation * oneMinusInterpolation * oneMinusInterpolation * m_Points[curve] +
				                                3f * oneMinusInterpolation * oneMinusInterpolation * interpolation * m_Points[curve + 1] +
				                                3f * oneMinusInterpolation * interpolation * interpolation * m_Points[curve + 2] +
                                                interpolation * interpolation * interpolation * m_Points[curve + 3]);
		}


#if UNITY_EDITOR

        /// <summary>
        /// If there are not 4, 7, 10, etc... points on the rail this function will add enough points to complete a curve.
        /// Only works in the editor.
        /// </summary>
		protected void ValidatePoints ()
		{
            if (m_Points.Length == 0)
            {
				m_Points = new Vector2[] { new Vector2(-15f, 0f), new Vector2(-5f, 0f), new Vector2(5f, 0f), new Vector2(15f, 0f) };
            }
            else
            {
                int pointsToAdd;
                if (m_Points.Length > 4)
                {
                    pointsToAdd = 2 - (m_Points.Length + 1) % 3;
                }
                else
                {
                    pointsToAdd = 4 - m_Points.Length;
                }

                if (pointsToAdd != 0)
                {
                    int index = m_Points.Length - 1;
                    Vector2 point = m_Points[index];
                    Array.Resize(ref m_Points, m_Points.Length + pointsToAdd);
                    for (int i = index; i < m_Points.Length; i++)
                    {
                        point.x += 10f;
                        m_Points[i] = point;
                    }
                }
            }
		}
		void OnValidate()
		{
			ValidatePoints();
		}

#endif
    }
}