/* Camera2D v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;
using System.Collections.Generic;
using GameEye2D.Focus;

/// <summary>
/// When added to an orthographic camera, provides helper functions and collects information for making camera behaviors.
/// </summary>
[AddComponentMenu("GameEye2D/Camera2D")]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Camera2D : MonoBehaviour
{
	#region Variables

	//Camera on this object
	Camera m_Camera;

	//Focus points used to calculate the action rect
	[SerializeField] List<Focus2D> m_FocusList;

	//The space added to the action rect on all sides.
	[SerializeField] float m_ActionRectSpaceAdded = 12f;

	//The camera will never go beyond these limits as long as you use position2D and zoom to set the camera's values.
    //The action rect will never go beyond these limits.
    [SerializeField] Rect m_CameraLimits = new Rect(-500, -500, 1000, 1000);

	//The rect of the action and when it last updated
    Rect m_ActionRect = new Rect(0f, 0f, 5f, 5f);
    float m_TimeLastUpdatedActionRect = TIME_TO_FORCE_UPDATE;
    const float TIME_TO_FORCE_UPDATE = -1f;


	#endregion


	#region Properties

	/// <summary>
	/// Gets the camera attached to the Camera2D.
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
    /// Gets or sets the list of focus's being used to create the action rect as an array.
    /// </summary>
    public Focus2D[] focusArray
    {
        get {
#if UNITY_EDITOR
			if (!Application.isPlaying && m_FocusList == null)
			{
				return new Focus2D[0];
			}
#endif
			return m_FocusList.ToArray();
		}
        set
        {
#if UNITY_EDITOR
			if (!Application.isPlaying && m_FocusList == null)
			{
				m_FocusList = new List<Focus2D>();
			}
#endif

            ClearFocusList();
            m_FocusList.AddRange(value); 
        }
    }
		
	/// <summary>
	/// Gets or sets the position of the camera as a vector2. This position is clamped to not put the camera's view outside of the camera limits.
	/// </summary>
	public Vector2 position2D
	{
#if GAMEEYE2D_XZ

		get
		{
			return new Vector2 (transform.position.x, transform.position.z);
		}
		set
		{ 	
			Vector2 viewSize = new Vector2(     zoom * gameCamera.aspect, zoom);
			transform.position = new Vector3(   Mathf.Clamp(value.x, cameraLimits.xMin + viewSize.x, cameraLimits.xMax - viewSize.x),
												transform.position.y,
												Mathf.Clamp(value.y, cameraLimits.yMin + viewSize.y, cameraLimits.yMax - viewSize.y));
		}
#else
		get
        {
			return new Vector2 (transform.position.x, transform.position.y);
		}
		set
        { 	
			Vector2 viewSize = new Vector2(     zoom * gameCamera.aspect, zoom);
			transform.position = new Vector3(   Mathf.Clamp(value.x, cameraLimits.xMin + viewSize.x, cameraLimits.xMax - viewSize.x),
												Mathf.Clamp(value.y, cameraLimits.yMin + viewSize.y, cameraLimits.yMax - viewSize.y),
												transform.position.z);
		}
#endif
	}

	/// <summary>
	/// Gets or sets the local position of the camera as a vector2. This position is clamped to not put the camera's view outside of the camera limits.
	/// </summary>
	public Vector2 localPosition2D
    {
#if GAMEEYE2D_XZ

		get
		{
		return new Vector2 (transform.localPosition.x, transform.localPosition.z);
		}
		set
		{ 	
		Vector2 viewSize = new Vector2(     zoom * gameCamera.aspect, zoom);
		transform.position = new Vector3(   Mathf.Clamp(value.x, cameraLimits.xMin + viewSize.x, cameraLimits.xMax - viewSize.x),
											transform.localPosition.y,
											Mathf.Clamp(value.y, cameraLimits.yMin + viewSize.y, cameraLimits.yMax - viewSize.y));
		}

#else
        get
		{
			return new Vector2 (transform.localPosition.x, transform.localPosition.y);
		}
		set
		{ 	if (transform.parent == null)
            {
                position2D = value;
            }
            else
            {
                Vector3 parentPosition = transform.parent.position;
#if GAMEEYE2D_XZ
                position2D = value + new Vector2(parentPosition.x, parentPosition.z);
#else
                position2D = value + new Vector2(parentPosition.x, parentPosition.y);
#endif
            }
		}
		#endif
	}

	/// <summary>
	/// Gets or sets the orthographic size of the camera. The greater the value, the more zoomed out the camera.
    /// If the new zoom is larger than can be contained within the camera limits, the zoom is clamped to be containable within the camera limits.
    /// If the new zoom would look outside of the camera limits, the camera's position is clamped to not look out of the camera limits.
	/// </summary>
	public float zoom
	{
		get
        { 
			return gameCamera.orthographicSize;
		}
		set
        {
            gameCamera.orthographicSize = Mathf.Min(value, MaximumZoom(cameraLimits.size));
            position2D = position2D;
		}
	}

    /// <summary>
    /// Gets a rect that encapsulates every focus point in the camera2D's list.
    /// The added rect size is added to each side, and the rect's sides are clamped within the camera's limits.
    /// If this component is disabled the action rect does not update unless set by you.
    /// When set the action rect will not update until the following frame.
    /// </summary>
    public Rect actionRect
    {
        //Gets a rect that encapsulates every focus point
        get
        {
#if UNITY_EDITOR
            //While in the editor, the action rect can update within the editor using real time instead of in-game time.
			if (!Application.isPlaying && (m_TimeLastUpdatedActionRect == Time.realtimeSinceStartup || m_FocusList == null))
            {
                return m_ActionRect;
            }

            //While in the editor and the game is not playing, keep track if there are any enabled focus's
            bool hasUpdated = false;
#endif
            //Check if there is one or multiple focus's, to determine how to calculate the action rect.
            //If this component has already updated on this frame or there are no focus's, do not update the action rect
            if (enabled && m_TimeLastUpdatedActionRect != Time.unscaledTime)
            {
                //If there is only one focus, they are the center of action
                if (m_FocusList.Count == 1)
                {
                    if (m_FocusList[0] != null && m_FocusList[0].enabled)
                    {
                        //The focus's influence is used to scale the offset of the focus point from the camera
                        Vector2 influence = m_FocusList[0].influence;
                        Vector2 pointOfInterest;
                        if (influence == Vector2.one)
                        {
                            pointOfInterest = m_FocusList[0].GetFocusPoint();
                        }
                        else if (influence != Vector2.zero)
                        {
                            pointOfInterest = position2D + Vector2.Scale(m_FocusList[0].GetFocusPoint() - position2D, m_FocusList[0].influence);
                        }
                        else
                        {
                            pointOfInterest = position2D;
                        }
                        actionRect = new Rect(pointOfInterest.x, pointOfInterest.y, 0f, 0f);
#if UNITY_EDITOR
                        hasUpdated = true;
#endif
                    }
                }

                //If there are multiple focus's, the action rect must encapsulate them all and be centered between them
                else if (m_FocusList.Count > 1)
                {
                    float left, right, top, bottom = 0;

                    //Get the first enabled focus point to be followed
                    Vector2 pointOfInterest;
                    Vector2 influence;
                    for (int i = 0; i < m_FocusList.Count; i++)
                    {
                        if (m_FocusList[i] != null && m_FocusList[i].enabled)
                        {
#if UNITY_EDITOR
                            hasUpdated = true;
#endif
                            //The focus's influence is used to scale the offset of the focus point from the camera
                            influence = m_FocusList[i].influence;
                            if (influence == Vector2.one)
                            {
                                pointOfInterest = m_FocusList[i].GetFocusPoint();
                            }
                            else if (influence != Vector2.zero)
                            {
                                pointOfInterest = position2D + Vector2.Scale(m_FocusList[i].GetFocusPoint() - position2D, m_FocusList[i].influence);
                            }
                            else
                            {
                                pointOfInterest = position2D;
                            }

                            //The first enabled focus found becomes the initial center of the action
                            left = pointOfInterest.x;
                            right = pointOfInterest.x;
                            top = pointOfInterest.y;
                            bottom = pointOfInterest.y;

                            //For each point afterwards, calculate if they are farther in any direction than the rect currently contains and grow the border to compensate
                            for (i++; i < m_FocusList.Count; i++)
                            {
                                if (m_FocusList[i] != null && m_FocusList[i].enabled)
                                {
                                    //Get a focus point, and utilize its influence to scale the point of interest's calculated offset from the camera
                                    influence = m_FocusList[i].influence;
                                    if (influence == Vector2.one)
                                    {
                                        pointOfInterest = m_FocusList[i].GetFocusPoint();
                                    }
                                    else if (influence != Vector2.zero)
                                    {
                                        pointOfInterest = position2D + Vector2.Scale(m_FocusList[i].GetFocusPoint() - position2D, m_FocusList[i].influence);
                                    }
                                    else
                                    {
                                        pointOfInterest = position2D;
                                    }

                                    //Check if this object is the further than the current border in any direction
                                    left = Mathf.Min(left, pointOfInterest.x);
                                    right = Mathf.Max(right, pointOfInterest.x);
                                    bottom = Mathf.Min(bottom, pointOfInterest.y);
                                    top = Mathf.Max(top, pointOfInterest.y);
                                }
                            }

                            //Set the new action rect with the given borders
                            actionRect = Rect.MinMaxRect(left, bottom, right, top);
                            break;
                        }
                    }
                }
            }

#if UNITY_EDITOR
            //While the game in not playing in the editor and the action rect has no enabled focus's, the action rect is centered on the camera
            if (!Application.isPlaying && !hasUpdated)
            {
                actionRect = new Rect(position2D.x, position2D.y, 0f, 0f);
            }
#endif
            return m_ActionRect;
        }

        //Sets the action rect and adds the minimum zoom as space on each side
        set
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                m_TimeLastUpdatedActionRect = Time.realtimeSinceStartup;
            }
            else
            {
#endif
            m_TimeLastUpdatedActionRect = Time.unscaledTime;
#if UNITY_EDITOR
            }
#endif
            m_ActionRect = Rect.MinMaxRect(Mathf.Max(value.xMin - m_ActionRectSpaceAdded, cameraLimits.xMin),
                                            Mathf.Max(value.yMin - m_ActionRectSpaceAdded, cameraLimits.yMin),
                                            Mathf.Min(value.xMax + m_ActionRectSpaceAdded, cameraLimits.xMax),
                                            Mathf.Min(value.yMax + m_ActionRectSpaceAdded, cameraLimits.yMax));
        }
    }

    /// <summary>
    /// Gets or sets the value that is added as space to the action rect on all sizes.
    /// </summary>
    public float addedActionRectSpace
	{
		get
        { 
			return m_ActionRectSpaceAdded;
		}
		set
        { 	
			m_ActionRectSpaceAdded = value;
		}
	}

    /// <summary>
	/// Gets or sets the rect that the camera's view and the action rect are clamped to.
    /// </summary>
    public Rect cameraLimits
    {
        get
        { 
			return m_CameraLimits;
		}
		set
        {	
			m_CameraLimits = value;
            zoom = zoom;
		}
    }

	#endregion


    #region Private Functions

    //Initializes by setting the initial action bounds to where the camera currently is
    void Awake()
    {
        actionRect = new Rect(0f, 0f, 0f, 0f);
        UpdateActionRect();
    }

    #endregion


    #region Public Functions

    /// <summary>
	/// Adds a focus to the focus list.
	/// </summary>
	public void AddFocus(Focus2D focus)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying && m_FocusList == null)
		{
			m_FocusList = new List<Focus2D>();
		}
#endif
		if (!m_FocusList.Contains(focus))
		{
			m_FocusList.Add(focus);
		}
	}

    /// <summary>
    /// Add focus's to the focus list.
    /// </summary>
    public void AddFocus (Focus2D[] focus)
	{
		for (int i = 0; i < focus.Length; i++)
		{
			AddFocus(focus[i]);
		}
	}

	/// <summary>
	/// Removes a focus from the focus list.
	/// </summary>
	public void RemoveFocus (Focus2D focus)
    {
#if UNITY_EDITOR
		if (!Application.isPlaying && m_FocusList == null)
		{
			return;
		}
#endif
		m_FocusList.Remove(focus);
    }

    /// <summary>
    /// Removes focus's from the focus list.
    /// </summary>
    public void RemoveFocus (Focus2D[] focus)
	{
		for (int i = 0; i < focus.Length; i++)
		{
            RemoveFocus(focus[i]);
		}
	}

    /// <summary>
    /// Removes all null focus’s from the focus list.
    /// </summary>
    [ContextMenu("Clean Focus List Of Empties")]
    public void CleanFocusList()
    {
#if UNITY_EDITOR
		if (!Application.isPlaying && m_FocusList == null)
		{
			return;
		}
#endif
        if (m_FocusList.Count == 0)
        {
            return;
        }

        //Remove the null focus's
        for (int i = 0; i < m_FocusList.Count; i++)
        {
            if (m_FocusList[i] == null)
            {
                m_FocusList.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Removes all focus’s from the focus list.
    /// </summary>
    [ContextMenu("Clear Focus List")]
	public void ClearFocusList ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying && m_FocusList == null)
		{
			return;
		}
#endif
    	m_FocusList.Clear();
	}

	/// <summary>
	/// Clamps the given position to the be within the camera's limits.
	/// </summary>
	public Vector2 ClampToCameraLimits (Vector2 position)
	{
		return new Vector2(Mathf.Clamp(position.x, cameraLimits.xMin, cameraLimits.xMax), Mathf.Clamp(position.y, cameraLimits.yMin, cameraLimits.yMax));
	}

	/// <summary>
	/// Clamps the given position to the be within the camera's limits.
	/// </summary>
	public Vector3 ClampToCameraLimits (Vector3 position)
    {
#if GAMEEYE2D_XZ

		return new Vector3(Mathf.Clamp(position.x, cameraLimits.xMin, cameraLimits.xMax), position.y, Mathf.Clamp(position.z, cameraLimits.yMin, cameraLimits.yMax));

#else

        return new Vector3(Mathf.Clamp(position.x, cameraLimits.xMin, cameraLimits.xMax), Mathf.Clamp(position.y, cameraLimits.yMin, cameraLimits.yMax), position.z);

#endif
	}

	/// <summary>
	/// Clamps a given rectangle to be within a camera limits and returns the center position.
	/// </summary>
	public Vector2 ClampToCameraLimits(Rect rect)
	{
		//Get the camera view rect and half the size of the camera
		Vector2 halfSize = new Vector2 (Mathf.Min(rect.width, cameraLimits.width), Mathf.Min(rect.height, cameraLimits.height)) / 2f;

		//Clamp the center of the rect as if the camera view was smaller by an amount equal to half the size of the rect
		return new Vector2(	Mathf.Clamp(rect.center.x, cameraLimits.xMin + halfSize.x, cameraLimits.xMax - halfSize.x),
							Mathf.Clamp(rect.center.y, cameraLimits.yMin + halfSize.y, cameraLimits.yMax - halfSize.y));
	}

	/// <summary>
	/// Clamps a given bounds to be within a camera limits and returns the center position.
	/// </summary>
	public Vector3 ClampToCameraLimits(Bounds bounds)
    {
#if GAMEEYE2D_XZ

		//Clamp the center of the bounds as if the camera view was smaller by an amount equal to the extents of the bounds
		return new Vector3(	Mathf.Clamp(bounds.center.x, cameraLimits.xMin + bounds.extents.x, cameraLimits.xMax - bounds.extents.x),
							bounds.center.y,
							Mathf.Clamp(bounds.center.z, cameraLimits.yMin + bounds.extents.y, cameraLimits.yMax - bounds.extents.y));

#else

        //Clamp the center of the bounds as if the camera view was smaller by an amount equal to the extents of the bounds
		return new Vector3(	Mathf.Clamp(	bounds.center.x, cameraLimits.xMin + bounds.extents.x, cameraLimits.xMax - bounds.extents.x),
							Mathf.Clamp(bounds.center.y, cameraLimits.yMin + bounds.extents.y, cameraLimits.yMax - bounds.extents.y),
							bounds.center.z);

#endif
	}

	/// <summary>
	/// Clamps a given position to be within the camera's view.
	/// </summary>
	public Vector2 ClampToCameraView(Vector2 position)
	{
		Rect cameraView = GetViewRectAsWorldSpace(gameCamera);
		return new Vector2(Mathf.Clamp(position.x, cameraView.xMin, cameraView.xMax), Mathf.Clamp(position.y, cameraView.yMin, cameraView.yMax));
	}

	/// <summary>
	/// Clamps a given position to be within the camera's view.
	/// </summary>
	public Vector3 ClampToCameraView(Vector3 position)
	{
		Rect cameraView = GetViewRectAsWorldSpace(gameCamera);

#if GAMEEYE2D_XZ

		return new Vector3(Mathf.Clamp(position.x, cameraView.xMin, cameraView.xMax), position.y, Mathf.Clamp(position.z, cameraView.yMin, cameraView.yMax));

#else

        return new Vector3(Mathf.Clamp(position.x, cameraView.xMin, cameraView.xMax), Mathf.Clamp(position.y, cameraView.yMin, cameraView.yMax), position.z);

#endif
	}

	/// <summary>
	/// Clamps a given rectangle to be within the camera's view and returns the center position.
	/// </summary>
	public Vector2 ClampToCameraView(Rect rect)
	{
		//Get the camera view rect and half the size of the camera
		Rect cameraView = GetViewRectAsWorldSpace(gameCamera);
		Vector2 halfSize = new Vector2 (Mathf.Min(rect.width, cameraView.width), Mathf.Min(rect.height, cameraView.height)) / 2f;

		//Clamp the center of the rect as if the camera view was smaller by an amount equal to half the size of the rect
		return new Vector2(	Mathf.Clamp(rect.center.x, cameraView.xMin + halfSize.x, cameraView.xMax - halfSize.x),
							Mathf.Clamp(rect.center.y, cameraView.yMin + halfSize.y, cameraView.yMax - halfSize.y));
	}

	/// <summary>
	/// Clamps a given bounds to be within the camera's view and returns the center position.
	/// </summary>
	public Vector3 ClampToCameraView(Bounds bounds)
	{
		Rect cameraView = GetViewRectAsWorldSpace(gameCamera);

#if GAMEEYE2D_XZ

		//Clamp the center of the bounds as if the camera view was smaller by an amount equal to the extents of the bounds
		return new Vector3(	Mathf.Clamp(bounds.center.x, cameraView.xMin + bounds.extents.x, cameraView.xMax - bounds.extents.x),
							bounds.center.y,
							Mathf.Clamp(bounds.center.z, cameraView.yMin + bounds.extents.y, cameraView.yMax - bounds.extents.y));

#else

        //Clamp the center of the bounds as if the camera view was smaller by an amount equal to the extents of the bounds
		return new Vector3(	Mathf.Clamp(	bounds.center.x, cameraView.xMin + bounds.extents.x, cameraView.xMax - bounds.extents.x),
							Mathf.Clamp(	bounds.center.y, cameraView.yMin + bounds.extents.y, cameraView.yMax - bounds.extents.y),
											bounds.center.z);

		#endif
	}

    /// <summary>
    /// Returns if the focus is in the focus list.
    /// </summary>
    public bool FocusListContains(Focus2D focus)
    {
#if UNITY_EDITOR
		if (!Application.isPlaying && m_FocusList == null)
		{
			return false;
		}
#endif
        return m_FocusList.Contains(focus);
    }

    /// <summary>
	/// Calculates the maximum orthographic size of the camera that would keep the view inside a rectangle of the given size.
	/// </summary>
	public float MaximumZoom(Vector2 size)
    {
        size /= 2f;
        return Mathf.Min(size.x / gameCamera.aspect, size.y);
    }

    /// <summary>
    /// If the action rect has already updated on this frame, this function causes the action rect to update again.
    /// </summary>
    public void UpdateActionRect()
    {
        m_TimeLastUpdatedActionRect = TIME_TO_FORCE_UPDATE;
    }

    /// <summary>
    /// Calculates the maximum orthographic size of the camera that would keep a rect of the given size inside the camera's view.
    /// </summary>
    public float WorldToZoom(Vector2 size)
	{
		size /= 2f;
		return Mathf.Max(size.x / gameCamera.aspect, size.y);
	}

    #endregion


    #region Static Functions

    /// <summary>
    /// Clamps a given position to be within a camera's view and returns the new position.
    /// </summary>
    public static Vector2 ClampToCameraView(Camera camera, Vector2 position)
    {
        Rect cameraView = GetViewRectAsWorldSpace(camera);
        return new Vector2(Mathf.Clamp(position.x, cameraView.xMin, cameraView.xMax), Mathf.Clamp(position.y, cameraView.yMin, cameraView.yMax));
    }

    /// <summary>
    /// Clamps a given position to be within a camera's view and returns the new position.
    /// </summary>
    public static Vector3 ClampToCameraView(Camera camera, Vector3 position)
    {
        Rect cameraView = GetViewRectAsWorldSpace(camera);

#if GAMEEYE2D_XZ

		return new Vector3(Mathf.Clamp(position.x, cameraView.xMin, cameraView.xMax), position.y, Mathf.Clamp(position.z, cameraView.yMin, cameraView.yMax));

#else

        return new Vector3(Mathf.Clamp(position.x, cameraView.xMin, cameraView.xMax), Mathf.Clamp(position.y, cameraView.yMin, cameraView.yMax), position.z);

#endif
    }

    /// <summary>
    /// Clamps a given rectangle to be within a camera's view and returns the center position.
    /// </summary>
    public static Vector2 ClampToCameraView(Camera camera, Rect rect)
    {
        //Get the camera view rect and half the size of the camera
        Rect cameraView = GetViewRectAsWorldSpace(camera);
        Vector2 halfSize = new Vector2(Mathf.Min(rect.width, cameraView.width), Mathf.Min(rect.height, cameraView.height)) / 2f;

        //Clamp the center of the rect as if the camera view was smaller by an amount equal to half the size of the rect
        return new Vector2(Mathf.Clamp(rect.center.x, cameraView.xMin + halfSize.x, cameraView.xMax - halfSize.x),
                            Mathf.Clamp(rect.center.y, cameraView.yMin + halfSize.y, cameraView.yMax - halfSize.y));
    }

    /// <summary>
    /// Clamps the given bounds to be within a camera's view and returns the center position.
    /// </summary>
    public static Vector3 ClampToCameraView(Camera camera, Bounds bounds)
    {
        Rect cameraView = GetViewRectAsWorldSpace(camera);

#if GAMEEYE2D_XZ

		//Clamp the center of the bounds as if the camera view was smaller by an amount equal to the extents of the bounds
		return new Vector3(	Mathf.Clamp(bounds.center.x, cameraView.xMin + bounds.extents.x, cameraView.xMax - bounds.extents.x),
							bounds.center.y,
							Mathf.Clamp(bounds.center.z, cameraView.yMin + bounds.extents.y, cameraView.yMax - bounds.extents.y));

#else

        //Clamp the center of the bounds as if the camera view was smaller by an amount equal to the extents of the bounds
        return new Vector3(Mathf.Clamp(bounds.center.x, cameraView.xMin + bounds.extents.x, cameraView.xMax - bounds.extents.x),
                                            Mathf.Clamp(bounds.center.y, cameraView.yMin + bounds.extents.y, cameraView.yMax - bounds.extents.y),
                                            bounds.center.z);

#endif
    }

    /// <summary>
    /// Calculates the view of the given camera as a rectangle in worldspace coordinates.
    /// </summary>
    public static Rect GetViewRectAsWorldSpace (Camera camera)
	{
		if (!camera.orthographic)
        {
#if UNITY_EDITOR
            Debug.Log(MESSAGE_REQUIRE_ORTHOGRAPHIC);
#endif
			return new Rect(0f, 0f, 1f, 1f);
        }

#if GAMEEYE2D_XZ

		return new Rect(camera.transform.position.x - camera.orthographicSize * camera.aspect,
						camera.transform.position.z - camera.orthographicSize,
						camera.orthographicSize * 2f * camera.aspect,
						camera.orthographicSize * 2f);

#else

        return new Rect(camera.transform.position.x - camera.orthographicSize * camera.aspect,
						camera.transform.position.y - camera.orthographicSize,
						camera.orthographicSize * 2f * camera.aspect,
						camera.orthographicSize * 2f);

		#endif
	}

    /// <summary>
    /// Calculates the maximum orthographic size of a view of the given aspect ratio that would keep the view inside a rectangle of the given size.
    /// </summary>
    public static float MaximumZoom(Vector2 size, float aspectRatio)
    {
        size /= 2f;
        return Mathf.Min(size.x / aspectRatio, size.y);
    }

    /// <summary>
    /// Calculates the maximum orthographic size that would keep a rect of the given size inside the view of the given aspect ratio.
    /// </summary>
    public static float WorldToZoom(Vector2 size, float aspectRatio)
    {
        size /= 2f;
        return Mathf.Max(size.x / aspectRatio, size.y);
    }

    #endregion


    #region Editor

#if UNITY_EDITOR

	//Track previous position and zoom while the game is not playing, to check if they have changed and if the camera should update them to be within the camera limits while in the scene window
	Vector3 m_LastPos = Vector3.zero;
	float m_LastZoom = 5f;

	//Whether to draw debug data or not
	[SerializeField] bool m_DrawDebug = false;

	//Whether to automatically add focus's added in the scene editor
	[SerializeField] bool m_AutoAddFocus = false;

	//The radius of the center sphere when drawing gizmo rectangles
	const float DRAW_ORIGIN_SIZE = 0.1f;

	//The minimum size the camera limits can be
	const float MINIMUM_CAMERA_LIMITS_SIZE = 1f;

	//Warning messages
	const string MESSAGE_NULL_FOCUS = " null in Camera2D's Focus2D list. They have been removed from the list.";
	const string MESSAGE_REQUIRE_ORTHOGRAPHIC = "The camera used for Camera2D must be set to orthographic.";


	/// <summary>
	/// Gets if this camera2D automatically adds focus's placed into the scene.
	/// Only works in the editor.
	/// </summary>
	public bool autoAddFocus
	{
		get { return m_AutoAddFocus;}
	}


	//Every frame in the editor that the game is not playing the camera is clamped to be within the camera's limits
	void Update()
	{
		if (!Application.isPlaying)
		{
			//If the camera moves or zooms in the unity editor, the camera checks if it has been set outside the camera limits
			if ((m_LastPos != transform.position || m_LastZoom != gameCamera.orthographicSize))
			{
				//Clamp the camera's view to be within the camera limits while in the editor
				zoom = gameCamera.orthographicSize;

				//Save the last position and zoom
				m_LastPos = transform.position;
				m_LastZoom = gameCamera.orthographicSize;
			}
		}
	}

	//When you reset through the context menu or add the component for the first time, change the camera to orthographic
	void Reset ()
	{
		gameCamera.orthographic = true;
	}

	//When you change something in the inspector, or when you start or stop the game in the editor
	void OnValidate ()
	{
		//Make sure the camera is set to orhographic in the editor
		if (gameCamera.orthographic == false)
		{
			gameCamera.orthographic = true;
			Debug.Log(MESSAGE_REQUIRE_ORTHOGRAPHIC);
		}

		//The camera limits cannot have the minimum greater than the maximum, or a minimum that would cause a camera error
		m_CameraLimits.width = Mathf.Max(MINIMUM_CAMERA_LIMITS_SIZE, m_CameraLimits.width);
		m_CameraLimits.height = Mathf.Max(MINIMUM_CAMERA_LIMITS_SIZE, m_CameraLimits.height);
        addedActionRectSpace = Mathf.Max(MINIMUM_CAMERA_LIMITS_SIZE, addedActionRectSpace);
	}

    //Adds every focus in the scene to this camera's focus list.
    [ContextMenu("Add Every Focus To List")]
    void AddAllFocusInSceneToList()
    {
#if UNITY_EDITOR
		if (!Application.isPlaying && m_FocusList == null)
		{
			m_FocusList = new List<Focus2D>();
		}
#endif
		Focus2D[] focusArray = Object.FindObjectsOfType<Focus2D>();
        m_FocusList.Clear();
        m_FocusList.AddRange(focusArray);
    }

    //Sets the camera's position between every focus point.
    [ContextMenu("Set Position To Action Center")]
    void SetPositionToActionCenter()
    {
        position2D = actionRect.center;
    }

    //Sets the camera's local XY position to 0. If you haved defined GAMEEYE2D_XZ this will set XZ to 0 instead.
    [ContextMenu("Set origin To (0, 0)")]
    void SetLocalPositionToZero()
    {
        if (transform.parent != null)
        {
            position2D = transform.parent.position;
        }
        else
        {
            position2D = Vector2.zero;
        }
    }

    //Logs the maximum zoom possible
    [ContextMenu("Log Maximum Zoom")]
    void LogMaximumZoom()
    {
        Debug.Log(MaximumZoom(cameraLimits.size, gameCamera.aspect));
    }

    //Logs the zoom that would fit the action rect
    [ContextMenu("Log Zoom To Fit Action Rect")]
    void LogZoomToFitActionRect()
    {
        Debug.Log(WorldToZoom(actionRect.size, gameCamera.aspect));
    }

    //Logs the action rect
    [ContextMenu("Log Action Rect")]
    void LogActionRect()
    {
        Debug.Log(actionRect);
    }

    //Logs the view rect in world space
    [ContextMenu("Log View Rect As World Space")]
    void LogViewRectAsWorldSpace()
    {
		Debug.Log(GetViewRectAsWorldSpace (gameCamera));
    }
		
	//In the scene window draw the camera limits and action rect
	void OnDrawGizmos ()
	{
        if (m_DrawDebug)
        {
            DrawGizmoRect(cameraLimits, Color.red);
            DrawGizmoRect(actionRect, Color.green);
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

#endif

    #endregion
}