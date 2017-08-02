/* Follow Rail v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// Creates a rail system when attached to an object that follows another focus. The focus point of the rail is the nearest position on the rail to that focus.
    /// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Follow Rail")]
    public class FollowRail : Rail
    {
		[SerializeField] Focus2D m_FollowedFocus;

        /// <summary>
        /// Gets or sets the focus the rail is following.
        /// </summary>
		public Focus2D followedFocus
        {
            get { return m_FollowedFocus; }
            set { m_FollowedFocus = value; }
        }


        /// <summary>
        /// Calculates the focus point along the rail.
        /// </summary>
        public override Vector2 GetFocusPoint()
        {
            if (m_FollowedFocus != null)
            {
				return ClampToCurve(m_FollowedFocus.GetFocusPoint());
            }
            return position2D;
        }

		/// <summary>
		/// Calculates the nearest position along the curve to the given position.
		/// </summary>
		public Vector2 ClampToCurve (Vector2 position)
		{
			position = transform.InverseTransformPoint(position);

			//Get the nearest point
			int index = 0;
			float currentDistance = Vector2.Distance(position, m_Points[0]);
			float distance = currentDistance;
			for (int i = 1; i < length; i++)
			{
				currentDistance = Vector2.Distance(position, m_Points[i]);
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
					if (length < index + 3 || Vector2.Distance(m_Points[index - 3], position) < Vector2.Distance(m_Points[index + 3], position))
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

			//Calculate interpolation on that curve
			Vector2 difference = m_Points[index + 3] - m_Points[index];
			Vector2 toPosition = position - m_Points[index];
            float curveInterpolation;
            if (difference != Vector2.zero)
            {
                curveInterpolation = Mathf.Clamp01(Mathf.Cos(Vector2.Angle(difference, toPosition) * Mathf.Deg2Rad) * toPosition.magnitude / difference.magnitude);
            }
            else
            {
                curveInterpolation = 0.5f;
            }

			//Return the point along the curve
			return GetPoint (curveInterpolation, index);
		}
    }
}
