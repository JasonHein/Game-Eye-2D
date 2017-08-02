/* Camera2D Zoom to fit. v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Behaviour
{
    /// <summary>
    /// When attached to a transform with a Camera2D, smoothly zooms the camera’s view in or out to contain the action rect calculated by Camera2D. The behavior can be paused by disabling it.
    /// </summary>
	[AddComponentMenu("GameEye2D/Behaviours/ZoomToFit")]
    [RequireComponent(typeof(Camera2D))]
	public class ZoomToFit : MonoBehaviour {

		//The camera 2D that tracks the action and moves the camera
		Camera2D m_Camera2D;

		//The speed the camera zooms in and out and the desired orthographic size to zoom to
		[SerializeField] float m_Speed = 1f;

		//How much the action bounds must change to update the desired zoom to lerp to.
		[SerializeField] float m_StillThreshold = 3f;
        float m_LastDistance = 0f;

        //The lowest that ZoomToFit will update the camera’s orthographic size to.
		[SerializeField] float m_MinimumZoom = 5f;


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
        /// Gets or sets how fast the camera zooms in or out.
        /// </summary>
        public float speed
		{
			get { 	return m_Speed; }
			set { 	m_Speed = value; }
		}

		/// <summary>
		/// Gets or sets how much the action rect must expand or shrink in order to update the desired zoom.
		/// </summary>
		public float stillThreshold
		{
			get { 	return m_StillThreshold; }
			set { 	m_StillThreshold = value; }
		}

        /// <summary>
        /// Gets or sets the lowest that ZoomToFit will update the camera’s orthographic size to.
        /// </summary>
        public float minimumZoom
		{
			get { 	return m_MinimumZoom;}
			set { 	m_MinimumZoom = value;}
		}
			
		//After all the objects have moved, lerp the camera's zoom to fit the points of interest.
		void LateUpdate()
		{
			//Optimal zoom to fit everything on screen
			float perfectZoom = camera2D.WorldToZoom(camera2D.actionRect.size);

			//If the camera is already zooming, or the action rect has changed significantly from the last desired zoom, change the desired zoom to lerp to.
			if (Mathf.Abs(perfectZoom - camera2D.zoom) > stillThreshold || Mathf.Abs(m_LastDistance) > m_Speed)
            {
                m_LastDistance = Mathf.Max(perfectZoom, minimumZoom) - camera2D.zoom;
				camera2D.zoom += m_LastDistance * Mathf.Min(speed * Time.unscaledDeltaTime, 1f);
            }
            else
            {
                m_LastDistance = 0f;
            }
        }

#if UNITY_EDITOR

		//Whether to draw debug data or not
		[SerializeField] bool m_DrawDebug = false;


		//In the scene window draw the camera limits and action rect
		void OnDrawGizmosSelected ()
		{
			if (m_DrawDebug)
			{
				//Calculate and clamp the zooms that would cause the desired zoom to change
				float smallestZoom = Mathf.Max(camera2D.zoom - stillThreshold, minimumZoom);
				float largestZoom = Mathf.Min(camera2D.zoom + stillThreshold, camera2D.MaximumZoom(camera2D.cameraLimits.size));

				//Calculate half the width of the rect's
				float XsmallestSize = smallestZoom * camera2D.gameCamera.aspect;
				float XlargestSize = largestZoom * camera2D.gameCamera.aspect;

				//Draw the smallest and largest zooms, and the action rect at the center of the camera for comparison
				DrawGizmoRect(	new Rect (	camera2D.position2D.x - XsmallestSize, camera2D.position2D.y - smallestZoom,
											XsmallestSize * 2f, smallestZoom * 2f),
								Color.cyan);
				DrawGizmoRect(	new Rect (	camera2D.position2D.x - XlargestSize, camera2D.position2D.y - largestZoom,
											XlargestSize  * 2f, largestZoom  * 2f),
								Color.cyan);
				Rect actionRectAtCamera = camera2D.actionRect;
				actionRectAtCamera.center = camera2D.position2D;
				DrawGizmoRect(actionRectAtCamera, Color.green);
			}
		}

		//Draws the given rect using gizmos
		void DrawGizmoRect (Rect rect, Color color)
		{
			//Set the gizmo color
			Gizmos.color = color;

#if GAMEEYE2D_XZ
			//Get the rect's corner positions
			Vector3 topLeft = new Vector3(rect.xMin, transform.position.y, rect.yMax);
			Vector3 botLeft = new Vector3(rect.xMin, transform.position.y, rect.yMin);
			Vector3 topRight = new Vector3(rect.xMax, transform.position.y, rect.yMax);
			Vector3 botRight = new Vector3(rect.xMax, transform.position.y, rect.yMin);
#else
            //Get the rect's corner positions
			Vector3 topLeft = new Vector3(rect.xMin, rect.yMax, transform.position.z);
			Vector3 botLeft = new Vector3(rect.xMin, rect.yMin, transform.position.z);
			Vector3 topRight = new Vector3(rect.xMax, rect.yMax, transform.position.z);
			Vector3 botRight = new Vector3(rect.xMax, rect.yMin, transform.position.z);
#endif
			//Draw the rect
			Gizmos.DrawLine(topLeft, botLeft);
			Gizmos.DrawLine(topRight, botRight);
			Gizmos.DrawLine(topLeft, topRight);
			Gizmos.DrawLine(botLeft, botRight);
		}
#endif
	}
}