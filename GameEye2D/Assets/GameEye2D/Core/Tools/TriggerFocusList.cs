/* Trigger Focus List v1.0
 * 
 * Created by Jason Hein
 * 
 */


using UnityEngine;
using System.Collections;
using GameEye2D.Focus;

/// <summary>
/// When attached to a transform with a trigger collider, when that collider is entered the focus list of a Camera2D is changed to a new list.
/// </summary>
[AddComponentMenu("GameEye2D/Tools/Trigger Focus List")]
public class TriggerFocusList : MonoBehaviour {

    //The camera2D that will have its focus list changed
    [SerializeField] Camera2D m_Camera2D;

    //The new focus list
	[SerializeField] Focus2D[] m_NewFocusList;

    //The tag of an object that will cause the focus list to be changed when it enters a trigger collider on this object
    [SerializeField] string m_TagThatTriggers = "Player";


    //When entered by the right object, the list changes
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.tag == m_TagThatTriggers)
        {
            Activate();
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
    /// Causes the change of the focus list to the given focus2D array.
    /// </summary>
    public void Activate()
    {
		//Check if the camera2D exists.
        if (m_Camera2D == null)
        {
#if UNITY_EDITOR
				Debug.LogError(CAMERA_IS_NULL);
#endif
            return;
        }

        m_Camera2D.ClearFocusList();
        m_Camera2D.AddFocus(m_NewFocusList);
    }


#if UNITY_EDITOR

	//Error messages
	const string CAMERA_IS_NULL = "Trigger Change Focus List Error: Camera2D is null.";

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
