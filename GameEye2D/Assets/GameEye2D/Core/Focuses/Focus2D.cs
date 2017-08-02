/* Focus2D v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// An abstract class that focuses inherit from, allowing the camera to use inheriting classes as a focus when calculating the action rect.
    /// Focus2Ds calculate the point of interest of any gameobject they are attached to.Camera2D uses these points to determine the optimal view.
	/// </summary>
	public abstract class Focus2D : MonoBehaviour
	{
		/// <summary>
		/// A public function for the camera to use to find this object's point of interest. When making your own focus's you must implement this function.
		/// </summary>
		public abstract Vector2 GetFocusPoint ();


		//How much the action rect is influenced by this focus by scaling the focus point's offset from the camera.
		[SerializeField] Vector2 m_Influence = Vector2.one;


		/// <summary>
		/// Gets or sets the position of the focus object as a vector2.
		/// </summary>
		public virtual Vector2 position2D
        {
#if GAMEEYE2D_XZ

			get {return new Vector2 (transform.position.x, transform.position.z);}
			set {transform.position = new Vector3(value.x, transform.position.y, value.y); }

#else

            get {	return new Vector2 (transform.position.x, transform.position.y);}
			set {	transform.position = new Vector3(value.x, value.y, transform.position.z); }

			#endif
		}

		/// <summary>
		/// Gets or sets the local position of the focus object as a vector2.
		/// </summary>
		public virtual Vector2 localPosition2D
        {
#if GAMEEYE2D_XZ

			get {	return new Vector2 (transform.localPosition.x, transform.localPosition.z);}
			set {	transform.localPosition = new Vector3(value.x, transform.localPosition.y, value.y); }

#else

            get {	return new Vector2 (transform.localPosition.x, transform.localPosition.y);}
			set {	transform.localPosition = new Vector3(value.x, value.y, transform.localPosition.z); }

			#endif
		}

		/// <summary>
		/// Gets or sets the influence of this focus on the Camera2D's action rect.
		/// This value is used to scale the focus point's offset from the camera.
		/// </summary>
		public virtual Vector2 influence
		{
			get {   return m_Influence; }
			set {	m_Influence = value; }
		}


#if UNITY_EDITOR

		//Whether to draw debug data or not
		[SerializeField] bool m_DrawDebug = false;

		//The radius of the center sphere when drawing gizmo rectangles (the green and red rects that shows up in the scene when you select the camera)
		const float DRAW_ORIGIN_SIZE = 0.6f;


		//In the scene window draw the focus point
		void OnDrawGizmos ()
		{
			if (m_DrawDebug)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawSphere(GetFocusPoint(), DRAW_ORIGIN_SIZE);
			}
		}
			
		//When reset and a camera2D has auto add enabled, a focus will add itself to that camera2D's focus list when dragged into the scene.
		void Reset ()
		{
			AddFocusToEveryAutoAddCamera2D();
		}

		/// <summary>
		/// Adds the focus to every camera2D in the scene.
		/// Only works in the editor.
		/// </summary>
		[ContextMenu("Add Focus To Every Camera 2D")]
		protected void AddFocusToEveryCamera2D()
		{
			Camera2D[] camera2DArray = Object.FindObjectsOfType<Camera2D>();
			for (int i = 0; i < camera2DArray.Length; i++)
			{
				camera2DArray[i].AddFocus(this);
			}
		}

		/// <summary>
		/// Adds the focus to every camera2D in the scene with auto add enabled.
		/// Only works in the editor.
		/// </summary>
		[ContextMenu("Add Focus To Every Auto Add Camera 2D")]
		protected void AddFocusToEveryAutoAddCamera2D()
		{
			Camera2D[] camera2DArray = FindObjectsOfType<Camera2D>();
			if (camera2DArray.Length > 0)
			{
				for (int i = 0; i < camera2DArray.Length; i++)
				{
					if (camera2DArray[i].autoAddFocus)
					{
						camera2DArray[i].AddFocus(this);
					}
				}
			}
		}

		/// <summary>
		/// Remove the focus from every camera2D in the scene with auto add enabled.
		/// Only works in the editor.
		/// </summary>
		[ContextMenu("Remove Focus From Every Camera 2D")]
		protected void RemoveFocusFromEveryCamera2D()
		{
			Camera2D[] camera2DArray = Object.FindObjectsOfType<Camera2D>();
			for (int i = 0; i < camera2DArray.Length; i++)
			{
				camera2DArray[i].RemoveFocus(this);
			}
		}

		/// <summary>
		/// Remove the focus from every camera2D in the scene.
		/// Only works in the editor.
		/// </summary>
		[ContextMenu("Remove Focus From Every Auto Add Camera 2D")]
		protected void RemoveFocusFromEveryAutoAddCamera2D()
		{
			Camera2D[] camera2DArray = Object.FindObjectsOfType<Camera2D>();
			for (int i = 0; i < camera2DArray.Length; i++)
			{
				if (camera2DArray[i].autoAddFocus)
				{
					camera2DArray[i].RemoveFocus(this);
				}
			}
		}

		/// <summary>
        /// Sets the camera's local XY position to 0. If you haved defined GAMEEYE2D_XZ this will set XZ to 0 instead.
		/// Only works in the editor.
		/// </summary>
		[ContextMenu("Set Origin to (0, 0)")]
		protected void SetLocalPositionToZero()
		{
			localPosition2D = Vector2.zero;
		}

		/// <summary>
        /// Sets the camera's local XY position to 0. If you haved defined GAMEEYE2D_XZ this will set XZ to 0 instead.
		/// Only works in the editor.
		/// </summary>
		[ContextMenu("Set Rotation to Identity")]
		protected void SetLocalRotationToIdentity()
		{
			transform.localRotation = Quaternion.identity;
		}

#endif
	}
}