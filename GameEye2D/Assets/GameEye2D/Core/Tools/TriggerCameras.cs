/* Trigger Cameras v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;
using System.Collections;

/// <summary>
/// When attached to a transform with a trigger collider, when that collider is entered certain camera’s will be enabled, and others disabled. You can choose which cameras are enabled or disabled.
/// </summary>
[AddComponentMenu("GameEye2D/Tools/Trigger Cameras")]
public class TriggerCameras : MonoBehaviour {

    //The camera2D that will have its focus list changed
    [SerializeField] Camera[] m_EnabledCameras;
    [SerializeField] Camera[] m_DisabledCameras;

    //The tag of an object that will cause the focus list to be changed when it enters a trigger collider on this object
    [SerializeField] string m_TagThatTriggers = "Player";

    //If the camera's change back when the collider is exited
    [SerializeField] bool m_UndoOnExit = false;


    //When entered by the right object, the camera's change
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

    //When exited by the right object, the camera's change
    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (m_UndoOnExit && collider2D.tag == m_TagThatTriggers)
        {
            Undo();
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (m_UndoOnExit && collider.tag == m_TagThatTriggers)
        {
            Undo();
        }
    }

    /// <summary>
    /// Causes the changes of cameras.
    /// </summary>
    public void Activate()
    {
        //Disable camera's
        if (m_DisabledCameras != null)
        {
            for (int i = 0; i < m_DisabledCameras.Length; i++)
            {
                if (m_DisabledCameras[i] != null)
                {
                    m_DisabledCameras[i].enabled = false;
                }
            }
        }

        //Enabled camera's
        if (m_EnabledCameras != null)
        {
            for (int i = 0; i < m_EnabledCameras.Length; i++)
            {
                if (m_EnabledCameras[i] != null)
                {
                    m_EnabledCameras[i].enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Undoes the change of cameras.
    /// </summary>
    public void Undo()
    {
        //Enabled camera's are disabled
        if (m_EnabledCameras != null)
        {
            for (int i = 0; i < m_EnabledCameras.Length; i++)
            {
                if (m_EnabledCameras[i] != null)
                {
                    m_EnabledCameras[i].enabled = false;
                }
            }
        }

        //Disable camera's are enabled
        if (m_DisabledCameras != null)
        {
            for (int i = 0; i < m_DisabledCameras.Length; i++)
            {
                if (m_DisabledCameras[i] != null)
                {
                    m_DisabledCameras[i].enabled = true;
                }
            }
        }
    }
}
