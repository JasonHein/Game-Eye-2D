/* Follow Rail Y v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// Creates a rail system when attached to an object that follows another focus. The focus point of the rail is the nearest position on the rail to the Y coordinate of that focus.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Follow Rail Y")]
    public class FollowRailY : FollowRail
    {
        /// <summary>
        /// Gets the point along the rail where the focus is.
        /// </summary>
        public override Vector2 GetFocusPoint()
        {
            if (followedFocus != null)
            {
				return ClampToCurveY(followedFocus.GetFocusPoint().y);
            }
            return position2D;
        }

		/// <summary>
		/// Gets the nearest position along the curve to the given y coordinate.
		/// </summary>
		public Vector2 ClampToCurveY(float position)
		{
			position = transform.InverseTransformPoint(new Vector2(0f, position)).y;

			//Get the nearest point
			int index = 0;
			float currentDistance = Mathf.Abs(position - m_Points[0].y);
			float distance = currentDistance;
			for (int i = 1; i < length; i++)
			{
				currentDistance = Mathf.Abs(position - m_Points[i].y);
				if (currentDistance < distance)
				{
					distance = currentDistance;
					index = i;
				}
			}

            //Clamp index to nearest curve
            if (index != 0)
            {
                if (index % 3 == 0)
                {
                    if (length < index + 3 || Mathf.Abs(m_Points[index - 3].x - position) < Mathf.Abs(m_Points[index + 3].x - position))
                    {
                        index -= 3;
                    }
                }
                //Set the index to the start of the curve
                else
                {
                    index = ((int)Mathf.Floor(index / 3)) * 3;
                }
            }

			//Get how far along the curve the the given position is
            float startDistance = position - m_Points[index].y;
            float endDistance = position - m_Points[index + 3].y;
            float totalDistance = Mathf.Abs(m_Points[index + 3].y - m_Points[index].y);
            float curveInterpolation;
            if (totalDistance != 0f)
            {
                curveInterpolation = Mathf.Clamp01(0.5f + (startDistance + endDistance) / (totalDistance * 2f));
            }
            else
            {
                curveInterpolation = 0.5f;
            }

			//Return the point along the curve
			return GetPoint(curveInterpolation, index);
		}
    }
}
