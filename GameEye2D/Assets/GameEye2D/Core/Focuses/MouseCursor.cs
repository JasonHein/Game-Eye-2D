/* Mouse Cursor v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When attached to a camera, create a focus point that is at the position of the mouse cursor.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Mouse Cursor")]
    [RequireComponent(typeof(Camera2D))]
	public class MouseCursor : Focus2D
	{
		//The camera the mouse focus is based on.
		Camera m_Camera;

		//How far the mouse must be from the center of the screen (0-0.5) to return a value that is not the center of the screen.
	    //This amount is reduced from camera's offset in view space from the center of the screen
		[SerializeField] float m_OffsetThreshold = 0.4f;

	    //Half the view size is kept to calculate the reduction of the offset from the center of the screen
		Vector3 HALF_SCREEN_SIZE = new Vector3 (0.5f, 0.5f, 0);

        /// <summary>
        /// The camera the mouse focus is based on.
        /// </summary>
        public Camera gameCamera
        {
            get
            {
                if (m_Camera == null)
                {
                    m_Camera = GetComponent<Camera>();
                }
                return m_Camera;
            }
        }


		/// <summary>
		/// A public function for the camera to use to find this object's point of interest.
		/// For a mouse cursor this is the mouse cursor's position on the screen reduced by the offset threshold from the center of the screen, and then put into world space.
		/// </summary>
		public override Vector2 GetFocusPoint ()
		{
	        //Calculate the offset the cursor is from the camera with the middle of the screen as the origin.
            Vector3 input = gameCamera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 viewPos = new Vector3(Mathf.Clamp01(input.x), Mathf.Clamp01(input.y), input.z) - HALF_SCREEN_SIZE;
			Vector3 offsetFromCamera = Vector3.zero;
			if (viewPos.x != 0f)
			{
				offsetFromCamera.x = Mathf.Max((Mathf.Abs(viewPos.x) - m_OffsetThreshold), 0f) * (viewPos.x / Mathf.Abs(viewPos.x));
			}
			if (viewPos.y != 0f)
			{
				offsetFromCamera.y = Mathf.Max((Mathf.Abs(viewPos.y) - m_OffsetThreshold), 0f) * (viewPos.y / Mathf.Abs(viewPos.y));
			}

			//Calculate the point of interest in world space coordinates
            Vector3 worldPos = gameCamera.ViewportToWorldPoint(offsetFromCamera + HALF_SCREEN_SIZE);

#if GAMEEYE2D_XZ

			return new Vector2(worldPos.x, worldPos.z);

#else

            return new Vector2(worldPos.x, worldPos.y);

			#endif
		}


#if UNITY_EDITOR

		//Offset threshold must be a value between 0 and 0.5
	    const float MAX_THRESHOLD = 0.5f;
		void OnValidate ()
		{
	        m_OffsetThreshold = Mathf.Clamp(m_OffsetThreshold, 0f, MAX_THRESHOLD);
		}

#endif
	}
}