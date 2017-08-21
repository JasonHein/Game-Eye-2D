/* Camera2D Smooth Follow v1.0
 * 
 * By Jason Hein
*/ 


using UnityEngine;

namespace GameEye2D.Behaviour
{
    /// <summary>
    /// When attached to a transform with a Camera2D, provides public functions that cause the camera's position to change as if a heavy force was applied. The behavior can be paused by disabling it.
	/// </summary>
	[AddComponentMenu("GameEye2D/Behaviours/Smooth Follow")]
    [RequireComponent(typeof(Camera2D))]
	public class SmoothFollow : MonoBehaviour {

		//The camera 2D that tracks the action and moves the camera
		Camera2D m_Camera2D;

		//Lerp speed and the position to lerp to
		[SerializeField] Vector2 m_Speed = new Vector2(2f, 2f);

		//How much the action rect must move to update the desired position to move to.
		[SerializeField] Vector2 m_ViewChangedThreshold = new Vector2(3f, 5f);
		Vector2 m_LastMotion;
        Vector2 m_LastMotionAbs;

        //If the camera moved more than this much last frame, the camera should continue moving.
        [SerializeField] Vector2 m_IsMovingThreshold = new Vector2(3f, 5f);



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
        /// Gets or sets how fast the position changes to follow the action.
        /// </summary>
        public Vector2 speed
		{
			get { 	return m_Speed; }
			set { 	m_Speed = value; }
		}

		/// <summary>
		/// Gets or sets how much the action rect must move in order to update the desired position to move to.
		/// The X axis of the threshold is altered based on the aspect ratio of the camera.
		/// </summary>
		public Vector2 stillThreshold
		{
			get { 	return new Vector2(m_ViewChangedThreshold.x * camera2D.gameCamera.aspect, m_ViewChangedThreshold.y); }
			set { 	m_ViewChangedThreshold = value; }
		}


		//Initialize by getting the camera2D and setting the initial desired position to the current position
		void Start ()
		{
			m_Camera2D = GetComponent<Camera2D>();
		}

		//After all the objects have all moved, lerp the camera's position to follow the action
		void LateUpdate()
		{
			Vector2 actionCenter = camera2D.actionRect.center;
			Vector2 desiredPositionOffset = actionCenter - camera2D.position2D;

			//If the camera is currently in significant motion, or if the camera is not near the desired position update the desired position
			if (Mathf.Abs(desiredPositionOffset.x) > stillThreshold.x || Mathf.Abs(desiredPositionOffset.y) > stillThreshold.y ||
                m_LastMotionAbs.x > m_IsMovingThreshold.x || m_LastMotionAbs.y > m_IsMovingThreshold.y)
			{
                //Move the camera towards the action
				//m_DesiredPosition = actionCenter;
                m_LastMotion = desiredPositionOffset;
                m_LastMotion.x *= speed.x;
                m_LastMotion.y *= speed.y;
                Vector2 lastPos = camera2D.position2D;
				camera2D.position2D += m_LastMotion * Time.unscaledDeltaTime;
				m_LastMotion = (camera2D.position2D - lastPos) / Time.unscaledDeltaTime;
                m_LastMotionAbs = new Vector2(Mathf.Abs(m_LastMotion.x), Mathf.Abs(m_LastMotion.y));
			}
            else
            {
                m_LastMotionAbs = Vector2.zero;
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
				//Set the gizmo color for the line towards the desired position
				Gizmos.color = Color.yellow;

#if GAMEEYE2D_XZ
				
				//Draw the line towards the desired position
				if (Application.isPlaying)
				{
                    Vector2 action = camera2D.actionRect.center;
					Gizmos.DrawLine(transform.position, new Vector3(action.x, transform.position.y, action.y));
				}

				//Get the threshold rect's corner positions
				Vector3 topLeft = new Vector3(transform.position.x - m_ViewChangedThreshold.x, transform.position.y, transform.position.z + m_ViewChangedThreshold.y);
				Vector3 botLeft = new Vector3(transform.position.x - m_ViewChangedThreshold.x, transform.position.y, transform.position.z - m_ViewChangedThreshold.y);
				Vector3 topRight = new Vector3(transform.position.x + m_ViewChangedThreshold.x, transform.position.y, transform.position.z + m_ViewChangedThreshold.y);
				Vector3 botRight = new Vector3(transform.position.x + m_ViewChangedThreshold.x, transform.position.y, transform.position.z - m_ViewChangedThreshold.y);
				
#else

                //Draw the line towards the desired position
                if (Application.isPlaying)
				{
                    Vector2 action = camera2D.actionRect.center;
                    Gizmos.DrawLine(transform.position, new Vector3(action.x, action.y, transform.position.z));
				}

				//Get the threshold rect's corner positions
				Vector3 topLeft = new Vector3(transform.position.x - m_ViewChangedThreshold.x, transform.position.y + m_ViewChangedThreshold.y, transform.position.z);
				Vector3 botLeft = new Vector3(transform.position.x - m_ViewChangedThreshold.x, transform.position.y - m_ViewChangedThreshold.y, transform.position.z);
				Vector3 topRight = new Vector3(transform.position.x + m_ViewChangedThreshold.x, transform.position.y + m_ViewChangedThreshold.y, transform.position.z);
				Vector3 botRight = new Vector3(transform.position.x + m_ViewChangedThreshold.x, transform.position.y - m_ViewChangedThreshold.y, transform.position.z);
#endif

				//Set the gizmo color for the threshold rect
				Gizmos.color = Color.cyan;

				//Draw the threshold rect
				Gizmos.DrawLine(topLeft, botLeft);
				Gizmos.DrawLine(topRight, botRight);
				Gizmos.DrawLine(topLeft, topRight);
				Gizmos.DrawLine(botLeft, botRight);
			}
		}

#endif
	}
}