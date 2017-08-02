/* ClampToScreen v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;

/// <summary>
/// When attached to a transform with a renderer, the transform’s position will become clamped to remain within an orthographic camera view.
/// For the smoothest movement I suggest setting this script's execution order to after all camera movement has completed.
/// </summary>
[AddComponentMenu("GameEye2D/Tools/Clamp To Screen")]
[RequireComponent(typeof(Renderer))]
public class ClampToScreen : MonoBehaviour {

	//The camera that the object will be clamped within the view of
	[SerializeField] Camera m_Camera;

	//The renderer of the object to keep on screen
	Renderer m_Renderer;

	//What axis to clamp
	enum Axis
	{
		XY = 0,
		X,
		Y
	}
	[SerializeField] Axis m_AxisToClamp = Axis.XY;


	/// <summary>
	/// Gets or sets the camera that the mesh will be clamped to the view of
	/// </summary>
	public Camera gameCamera
	{
		get { 	return m_Camera; }
		set { 	m_Camera = value; }
	}


	//Initialize by getting the object's renderer
	void Start ()
	{
		m_Renderer = GetComponent<Renderer>();
	}

	//If there is no screen, do nothing
	void OnEnable ()
	{
		if (gameCamera == null)
		{
			enabled = false;
#if UNITY_EDITOR
			Debug.LogError(CAMERA_IS_NULL);
#endif
		}
	}

	//After the objects have all moved, clamp the object to the screen
	void LateUpdate()
	{
		switch (m_AxisToClamp)
		{
			case Axis.X:
				transform.position = new Vector3 (Camera2D.ClampToCameraView(gameCamera, m_Renderer.bounds).x, transform.position.y, transform.position.z);
				break;
			case Axis.Y:
#if GAMEEYE2D_XZ
				transform.position = new Vector3 (transform.position.x, transform.position.y, Camera2D.ClampToCameraView(gameCamera, m_Renderer.bounds).z);
#else
				transform.position = new Vector3 (transform.position.x, Camera2D.ClampToCameraView(gameCamera, m_Renderer.bounds).y, transform.position.z);
#endif
				break;

				//In any other state (including XY or an error), clamp both X and Y axis.
			default:
				transform.position = Camera2D.ClampToCameraView(gameCamera, m_Renderer.bounds);
				break;
		}
	}


#if UNITY_EDITOR

	//Error messages
	const string CAMERA_IS_NULL = "Camera is null.";

	//When first put into the editor or the reset button is hit, the tool sets the camera to the main camera.
	void Reset ()
	{
		Camera mainCamera = Camera.main;
		if (mainCamera != null)
		{
			gameCamera = mainCamera;
		}
	}

#endif
}
