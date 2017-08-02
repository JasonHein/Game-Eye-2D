/* Rigid Body 2D v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When attached to a transform with a rigidbody2D, creates a focus point that is at the position of the transform. If the rigidbody2D has a velocity, the focus point leads ahead of the transform in the direction of the velocity. The conversion from velocity to offset can be set through the inspector.
    /// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Rigid Body 2D")]
    [RequireComponent(typeof(Rigidbody2D))]
	public class F_RigidBody2D : F_Transform {

		//The object's rigidbody2D is used to determine the point of interest
		Rigidbody2D m_RigidBody2D;

		//How much the camera looks ahead of where the rigidbody is going
		[SerializeField] Vector2 m_VelocityToOffset = new Vector2 (0.6f, 0.1f);


		/// <summary>
		/// Gets the rigidBody of the focus.
		/// </summary>
		public Rigidbody2D body2D
		{
			get
			{   if (m_RigidBody2D == null)
				{
					m_RigidBody2D = GetComponent<Rigidbody2D>();
				}
				return m_RigidBody2D;
			}
		}

		/// <summary>
		/// Gets where the point of interest of this rigidbody is.
	    /// For a rigidbody2D focus the point of interest is where the object is moving towards, plus the offset provided.
		/// </summary>
		public override Vector2 GetFocusPoint ()
		{
            return base.GetFocusPoint() + new Vector2(body2D.velocity.x * m_VelocityToOffset.x, body2D.velocity.y * m_VelocityToOffset.y);
		}
	}
}