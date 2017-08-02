/* Camera2D Scroll Zoom v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Behaviour
{
    /// <summary>
    /// When attached to a transform with a Camera2D, causes the camera’s orthographic size to change based on the input of the player. The most common use is to allow the mouse wheel to scroll the camera in and out. The behavior can be paused by disabling it.
	/// </summary>
	[AddComponentMenu("GameEye2D/Behaviours/Scroll Zoom")]
    [RequireComponent(typeof(Camera2D))]
	public class ScrollZoom : MonoBehaviour
    {
        //The camera 2D that tracks the action and moves the camera
        Camera2D m_Camera2D;

        //The minimum zoom scrolling will allow
        [SerializeField] float m_MinimumZoom = 5f;

		//The highest zoom scrolling will allow
		[SerializeField] float m_MaxZoom = 20f;

		//Input for scrolling
		[SerializeField] string m_ScrollInput = "Mouse ScrollWheel";


        /// <summary>
        /// Gets the camera2D used by this behavior.
        /// </summary>
        public Camera2D camera2D
        {
            get
            {
                if (m_Camera2D == null)
                {
                    m_Camera2D = GetComponent<Camera2D>();
                }
                return m_Camera2D;
            }
        }

        /// <summary>
        /// Gets or sets the minimum orthographic size possible by scrolling the camera.
        /// </summary>
        public float minimumZoom
        {
            get { 	return m_MinimumZoom; }
            set { 	m_MinimumZoom = value; }
        }

		/// <summary>
		/// Gets or sets the maximum orthographic size possible by scrolling the camera.
		/// </summary>
		public float maximumZoom
		{
            get { 	return m_MaxZoom; }
            set { 	m_MaxZoom = value; }
		}

		/// <summary>
		/// Gets or sets the input used to scroll the camera.
		/// </summary>
		public string scrollInput
		{
			get { 	return m_ScrollInput; }
			set { 	m_ScrollInput = value; }
		}

		
		//After all the objects have moved, check for axis input and zoom the camera accordingly
		void LateUpdate ()
		{
			camera2D.zoom = Mathf.Clamp(camera2D.zoom - Input.GetAxis(scrollInput), minimumZoom, maximumZoom);
		}
	}
}
