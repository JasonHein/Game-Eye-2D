/* Transform v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When attached to a transform, creates a focus point that is at the position of the transform. This point can be offset through the inspector.
    /// </summary>
	/// 
	[AddComponentMenu("GameEye2D/Focus2D/Transform")]
    public class F_Transform : Focus2D
	{
		//An offset added to the position of the focus point.
		[SerializeField] Vector2 m_Offset;
		public Vector2 offset
		{
			get { 	return m_Offset; }
			set { 	m_Offset = value; }
		}
		
		/// <summary>
		/// Gets where the point of interest of this transform is.
		/// </summary>
		public override Vector2 GetFocusPoint ()
		{
			//Return this object's point of interest
			return position2D + offset;
		}
	}
}