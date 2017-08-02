/* Simple Moving v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When attached to a transform, creates a focus point that is at the position of the transform. If the transform moves, the focus point leads ahead of it. The conversion from velocity to offset can be set through the inspector.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/SimpleMoving")]
    public class SimpleMoving : F_Transform {

		//How much the camera looks ahead of where the transform is going
		[SerializeField] Vector2 m_VelocityToOffset = new Vector2 (0.6f, 0.1f);

		//Position and offset of the focus last frame
		Vector2 m_LastPosition2D = Vector2.zero;
		Vector2 m_Velocity = Vector2.zero;

		//Initialize the last position the transform was at.
		void Start ()
		{
			m_LastPosition2D = position2D;
		}
		
		//Calculate the velocity of the transform
		void Update ()
		{
			m_Velocity = (position2D - m_LastPosition2D) / Time.deltaTime;
			m_LastPosition2D = position2D;
		}

        /// <summary>
        /// Calculates a position that is ahead of the focus’s transform, in the direction the transform it moving.
        /// </summary>
        public override Vector2 GetFocusPoint ()
		{
            return base.GetFocusPoint() + new Vector2(m_Velocity.x * m_VelocityToOffset.x, m_Velocity.y * m_VelocityToOffset.y);
		}
	}
}