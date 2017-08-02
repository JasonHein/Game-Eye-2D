/* Trigger Zoom v1.0
 * 
 * Created by Jason Hein
 * 
 * 
 * When enabled, changes the camera's orthographic size.
 */


using UnityEngine;

/// <summary>
/// When attached to a transform with a trigger collider, when that collider is entered the camera will begin to change orthographic sizes, zooming the camera in or out.
/// </summary>
[AddComponentMenu("GameEye2D/Tools/Trigger Zoom")]
public class TriggerZoom : MonoBehaviour {

	//The camera2D that will have its orthographic size changed
	[SerializeField] Camera2D m_Camera2D;

	//The tag of an object that will cause the camera's  orthographic size to be changed when it enters a trigger collider of this object
	[SerializeField] string m_TagThatTriggers = "Player";

	//After being triggered, this becomes the new camera orthographic size
	[SerializeField] float m_EnterZoom = 5f;

	//After being exited, this becomes the new camera orthographic size
	[SerializeField] float m_ExitZoom = 5f;

	//How long it takes for the zoom to change. A change time of 0 is instant.
	[SerializeField] float m_TransitionTime = 0f;

	//Collected values for transitions over time
	float m_TriggerTime = 0f;
	float m_ZoomDifference = 0f;
	float m_OldZoom = 5f;


	//Turn off the component until a transition begins so update is not called needlessly
	void Start ()
	{
		enabled = false;
	}

	//Move the camera limits over time when triggered
	void Update ()
	{
		//How far the camera orthographic size should be between the old and next camera orthographic size
		float interpolateAmount = Mathf.Min((Time.time - m_TriggerTime) / m_TransitionTime, 1f);

		//During the transition, change the camera orthographic size
		if (interpolateAmount < 1f)
		{
			m_Camera2D.zoom = m_OldZoom + m_ZoomDifference * interpolateAmount;
		}
		//If the transition is over, disable the transition and set the orthographic size to exactly where the final orthographic size would be.
		else
		{
			enabled = false;
			m_Camera2D.zoom = m_OldZoom + m_ZoomDifference;
			m_ZoomDifference = 0f;
			m_TriggerTime = 0f;
		}
	}

	//When entered by the right object, the transition begins
	void OnTriggerEnter2D (Collider2D collider2D)
	{
		if (collider2D.tag == m_TagThatTriggers)
		{
			Activate (m_EnterZoom);
		}
	}
	void OnTriggerEnter (Collider aCollider)
	{
		if (aCollider.tag == m_TagThatTriggers)
		{
			Activate (m_EnterZoom);
		}
	}

	//When exited by the right object, the transition reverses
	void OnTriggerExit2D (Collider2D collider2D)
	{
		if (collider2D.tag == m_TagThatTriggers)
		{
			Activate (m_ExitZoom);
		}
	}
	void OnTriggerExit (Collider aCollider)
	{
		if (aCollider.tag == m_TagThatTriggers)
		{
			Activate (m_ExitZoom);
		}
	}

	/// <summary>
	/// Causes the zoom of the camera2D to change to the given zoom over the time set in the inspector.
	/// </summary>
	public void Activate (float newZoom)
	{
        //Check if the camera2D exists or if the trigger has already activated
		if (m_Camera2D == null)
        {
#if UNITY_EDITOR
			Debug.LogError(CAMERA_IS_NULL);
#endif
            return;
        }

		if (m_Camera2D.zoom == newZoom)
		{
			return;
		}

		//If the transition is over time, enable the transition and collect transition values
		if (m_TransitionTime > 0f)
		{
			enabled = true;
			m_OldZoom = m_Camera2D.zoom;
			m_TriggerTime = Time.time;
			m_ZoomDifference = newZoom - m_OldZoom;
		}
		//If the transition is instant, change the zoom immediately
		else
		{
			m_Camera2D.zoom = newZoom;
		}
	}


#if UNITY_EDITOR

	//Should the trigger draw debug rects?
	[SerializeField] bool m_DrawDebug = false;

	//Error messages
	const string CAMERA_IS_NULL = "Trigger Zoom Error: Camera2D is null.";


	//When you change the component in the editor, clamp the new and old zoom to not cause errors
	void OnValidate ()
	{
		m_EnterZoom = Mathf.Max(m_EnterZoom, 1f);
		m_ExitZoom = Mathf.Max(m_ExitZoom, 1f);
	}

	//In the scene window draw the zoom the camera becomes before and after leaving the trigger
	void OnDrawGizmos ()
	{
		if (m_DrawDebug && m_Camera2D != null)
		{
			float halfWidth = m_EnterZoom * m_Camera2D.gameCamera.aspect;
			DrawGizmoRect(	new Rect(	transform.position.x - halfWidth,
				                        transform.position.y - m_EnterZoom,
										halfWidth * 2f,
				                        m_EnterZoom * 2f),
							Color.cyan);
			halfWidth = m_ExitZoom * m_Camera2D.gameCamera.aspect;
			DrawGizmoRect(	new Rect(	transform.position.x - halfWidth,
				                        transform.position.y - m_ExitZoom,
										halfWidth * 2f,
				                        m_ExitZoom * 2f),
							Color.blue);
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


	/// <summary>
	/// When first put into the editor or the reset button is hit, the tool sets the camera to the main camera.
	/// Only works in the editor.
	/// </summary>
	public void Reset ()
	{
		Camera mainCamera = Camera.main;
		if (mainCamera != null)
		{
			m_Camera2D = mainCamera.GetComponent<Camera2D>();
		}
	}

#endif
}
