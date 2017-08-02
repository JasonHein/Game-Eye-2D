/* Rigid Body v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When attached to a transform with a rigidbody, creates a focus point that is at the position of the transform. If the rigidbody has a velocity, the focus point leads ahead of the transform in the direction of the velocity. The conversion from velocity to offset can be set through the inspector.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Rigid Body")]
    [RequireComponent(typeof(Rigidbody))]
	public class F_RigidBody : F_Transform {

		//The object's rigidbody2D is used to determine the point of interest
		Rigidbody m_RigidBody;

		//How much the camera looks ahead of where a rigidbody is going
		[SerializeField] Vector2 m_VelocityToOffset = new Vector2 (0.6f, 0.1f);


		/// <summary>
		/// Gets the rigidBody of the focus.
		/// </summary>
		public Rigidbody body
		{
			get
			{   if (m_RigidBody == null)
				{
					m_RigidBody = GetComponent<Rigidbody>();
				}
				return m_RigidBody;
			}
		}

		/// <summary>
		/// Gets where the point of interest of this rigidbody is.
		/// For a rigidbody focus the point of interest is where the object is moving towards, plus the offset provided.
		/// </summary>
		public override Vector2 GetFocusPoint ()
        {
#if GAMEEYE2D_XZ
            return base.GetFocusPoint() + new Vector2(body.velocity.x * m_VelocityToOffset.x, body.velocity.z * m_VelocityToOffset.y);
#else
            return base.GetFocusPoint() + new Vector2(body.velocity.x * m_VelocityToOffset.x, body.velocity.y * m_VelocityToOffset.y);
#endif
		}
	}
}