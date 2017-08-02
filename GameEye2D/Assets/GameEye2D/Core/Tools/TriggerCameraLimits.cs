/* Trigger Camera Limits v1.0
 * 
 * Created by Jason Hein
*/


using UnityEngine;

/// <summary>
/// When attached to a transform with a trigger collider, when that collider is entered the limits of a Camera2D will change. The transition can be instant or over time.
/// </summary>
[AddComponentMenu("GameEye2D/Tools/Trigger Camera Limits")]
public class TriggerCameraLimits : MonoBehaviour {

	//The camera2D that will have its camera limits changed
	[SerializeField] Camera2D m_Camera2D;

	//The tag of an object that will cause the camera's limits to be changed when it enters a trigger collider of this object
	[SerializeField] string m_TagThatTriggers = "Player";

	//After being triggered, this becomes the new camera limits
	[SerializeField] Rect m_CameraLimits =  new Rect(-500, -500, 1000, 1000);

	//How long it takes for the camera limits to change. A change time of 0 is instant.
	[SerializeField] float m_TransitionTime = 0f;

	//Collected values for transitions over time
	float m_TriggerTime = 0f;
	Rect m_OldLimits = new Rect (0f, 0f, 0f, 0f);
	Rect m_LimitsDifference = new Rect (0f, 0f, 0f, 0f);
    bool m_Triggered = false;


	//If the transition should start immediately, trigger the change
	void Start ()
	{
        enabled = false;
	}

	//Move the camera limits over time when triggered
	void Update ()
	{
		//How far the camera limits should be between the old and next camera limits
		float interpolateAmount = (Time.time - m_TriggerTime) / m_TransitionTime;

		//During the transition, change the camera limits
		if (interpolateAmount < 1f)
		{
			m_Camera2D.cameraLimits = new Rect(	m_OldLimits.xMin + m_LimitsDifference.xMin  * interpolateAmount,
												m_OldLimits.yMin + m_LimitsDifference.yMin  * interpolateAmount,
												m_OldLimits.width + m_LimitsDifference.width * interpolateAmount,
												m_OldLimits.height + m_LimitsDifference.height * interpolateAmount);
		}
		//If the transition is over, disable the transition and set the limits to exactly where the end limits would be.
		else
		{
			m_Camera2D.cameraLimits = m_CameraLimits;
            enabled = false;
		}
	}

	//When entered by the right object, the transition begins
	void OnTriggerEnter2D (Collider2D collider2D)
	{
        if (collider2D.tag == m_TagThatTriggers)
		{
			Activate ();
		}
	}
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == m_TagThatTriggers)
        {
            Activate();
        }
    }

	/// <summary>
	/// Resets the trigger.
	/// </summary>
	public void ResetTrigger ()
	{
		m_Triggered = false;
	}

	/// <summary>
	/// Causes the change of camera limits to either occur, or begin (depending on how long it takes to transition).
	/// </summary>
	public void Activate ()
	{
		//Check if the camera2D exists or if the trigger has already activated
        if (m_Camera2D == null)
		{
#if UNITY_EDITOR
			Debug.LogError(CAMERA_IS_NULL);
#endif
			return;
		}
		else if (m_Triggered)
		{
			return;
		}
        m_Triggered = true;

		//If the transition is instant, change the limits immediately
		if (m_TransitionTime == 0f)
		{
			m_Camera2D.cameraLimits = m_CameraLimits;
		}
		//Enable the transition 
		else
		{
			enabled = true;

			//Keep the old limits to interpolate from and the time of the change beginning
			m_OldLimits = m_Camera2D.cameraLimits;
			m_TriggerTime = Time.time;

			//Get the difference between the two camera limits
			m_LimitsDifference = new Rect(	m_CameraLimits.xMin - m_OldLimits.xMin,
											m_CameraLimits.yMin - m_OldLimits.yMin,
											m_CameraLimits.width - m_OldLimits.width,
											m_CameraLimits.height - m_OldLimits.height);

            Debug.Log("Triggered");
		}
	}

#if UNITY_EDITOR

	//Should the trigger draw debug rects?
	[SerializeField] bool m_DrawDebug = false;

	//The radius of the center sphere when drawing gizmo rectangles
	const float DRAW_ORIGIN_SIZE = 0.1f;

	//Error messages
	const string CAMERA_IS_NULL = "Trigger Change Camera Limits Error: Camera2D is null.";


	//In the scene window draw the camera limits and rect it will change to
	void OnDrawGizmos ()
	{
		if (m_DrawDebug)
        {
			if (m_Camera2D != null)
			{
				DrawGizmoRect(m_Camera2D.cameraLimits, Color.red);
			}
			DrawGizmoRect(m_CameraLimits, Color.magenta);
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

#if GAMEEYE2D_XZ

		Gizmos.DrawWireSphere(new Vector3(rect.center.x, transform.position.y, rect.center.y), DRAW_ORIGIN_SIZE);

#else

        Gizmos.DrawWireSphere(new Vector3(rect.center.x, rect.center.y, transform.position.z), DRAW_ORIGIN_SIZE);

#endif
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
