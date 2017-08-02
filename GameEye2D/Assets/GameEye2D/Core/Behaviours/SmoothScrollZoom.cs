/* Camera2D Smooth Scroll Zoom v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Behaviour
{
    /// <summary>
    /// When attached to a transform with a Camera2D, causes the camera’s orthographic size to smoothly change based on the input of the player. The most common use is to allow the mouse wheel to scroll the camera in and out. The behavior can be paused by disabling it.
    /// </summary>
    [AddComponentMenu("GameEye2D/Behaviours/Smooth Scroll Zoom")]
    public class SmoothScrollZoom : ScrollZoom
    {
        //How fast the camera zooms in and out to follow the input scroll.
        [SerializeField] float m_Speed = 1f;

        //How much the camera will zoom to
        float m_DesiredZoom = 5f;
        const float ZOOM_THRESHOLD = 0.1f;

        /// <summary>
        /// How fast the camera zooms in and out to follow the input scroll.
        /// </summary>
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }


        //Initialize the desired zoom
        void Start ()
        {
            m_DesiredZoom = camera2D.zoom;
        }

        //After all the objects have moved, check for axis input and zoom the camera accordingly
        void LateUpdate()
        {
            m_DesiredZoom = Mathf.Clamp(m_DesiredZoom - Input.GetAxis(scrollInput), minimumZoom, maximumZoom);
            float difference = m_DesiredZoom - camera2D.zoom;

            if (Mathf.Abs(difference) > ZOOM_THRESHOLD)
            {
                camera2D.zoom += difference * speed * Time.deltaTime;
            }
        }
    }
}
