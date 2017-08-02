/* Camera2D Fixed Follow v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;

namespace GameEye2D.Behaviour
{
    /// <summary>
    /// When attached to a transform with a Camera2D, causes the camera’s position to be exactly at the center of the action. The behavior can be paused by disabling it.
	/// </summary>
	[AddComponentMenu("GameEye2D/Behaviours/Fixed Follow")]
    [RequireComponent(typeof(Camera2D))]
	public class FixedFollow : MonoBehaviour {

		//The camera 2D that tracks the action and moves the camera
		Camera2D m_Camera2D;


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

        //After the objects have all moved set the camera position to look at the desired position
        void LateUpdate()
		{
            camera2D.position2D = camera2D.actionRect.center;
		}
	}
}