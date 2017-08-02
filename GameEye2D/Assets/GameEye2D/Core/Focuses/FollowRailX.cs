/* Follow Rail X v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// Creates a rail system when attached to an object that follows another focus. The focus point of the rail is the nearest position on the rail to the X coordinate of that focus.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Follow Rail X")]
    public class FollowRailX : FollowRail
    {
        /// <summary>
        /// Gets the point along the rail where the focus is.
        /// </summary>
        public override Vector2 GetFocusPoint()
        {
            if (followedFocus != null)
            {
				return ClampToCurveX(followedFocus.GetFocusPoint().x);
            }
            return position2D;
        }

		/// <summary>
		/// Gets the nearest position along the curve to the given x coordinate.
		/// </summary>
		public Vector2 ClampToCurveX(float position)
		{
			position = transform.InverseTransformPoint(new Vector2(position, 0f)).x;

			//Get the nearest point
			int index = 0;
			float distance = Mathf.Abs(position - m_Points[index].x);
			float closestDistance = distance;
			for (int i = 1; i < length; i++)
			{
				distance = Mathf.Abs(position - m_Points[i].x);
				if (distance < closestDistance)
				{
					closestDistance = distance;
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
			float startDistance = position - m_Points[index].x;
            float endDistance = position - m_Points[index + 3].x;
            float totalDistance = Mathf.Abs(m_Points[index + 3].x - m_Points[index].x);
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
