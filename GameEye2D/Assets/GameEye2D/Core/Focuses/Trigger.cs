/* Trigger v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;
using System.Collections;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When attached to a transform with a trigger collider or collider2D, creates a focus that adds itself to the focus list of camera2Ds when an object with a certain tag enters the collider. There is an option for the camera2Ds to remove the focus when the trigger collider is exited.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Trigger")]
    public class Trigger : F_Transform
	{
        //The camera2D's that will add and remove the focus from a focus list.
        [SerializeField] protected Camera2D[] m_Camera2Ds;

		//The tag of an object that will cause this focus point to be added to the camera
		[SerializeField] string m_TagThatTriggers = "Player";

		//The amount of objects with that tag that must be inside the trigger to add it to the focus list
		[SerializeField] protected int m_MinimumInsideTrigger = 1;

		//Causes the focus to be removed when the trigger is exited
		[SerializeField] bool m_RemoveOnExit = false;

		//Storing if the focus has been triggered
		protected int m_TriggerExitsToBeRemoved = 0;
		protected bool m_Triggered = false;


        /// <summary>
        /// The camera2D's that will add and remove the focus from a focus list.
        /// </summary>
        public Camera2D[] camera2Ds
        {
            get { return m_Camera2Ds; }
            set { m_Camera2Ds = value; }
        }


		//When a collider is entered by the trigger object, this position is added to each camera2d's focus list.
		void OnTriggerEnter2D (Collider2D collider2D)
		{
			if (collider2D.tag == m_TagThatTriggers && m_Camera2Ds != null && m_Camera2Ds.Length != 0)
			{
				m_TriggerExitsToBeRemoved++;
				if (m_TriggerExitsToBeRemoved == m_MinimumInsideTrigger)
				{
					Activate();
				}
			}
		}
        void OnTriggerEnter(Collider collider)
        {
			if (collider.tag == m_TagThatTriggers && m_Camera2Ds != null && m_Camera2Ds.Length != 0)
            {
                m_TriggerExitsToBeRemoved++;
				if (m_TriggerExitsToBeRemoved == m_MinimumInsideTrigger)
                {
					Activate();
                }
            }
        }

		//When collider is exited by the trigger object and few other objects with that tag are inside, this position is removed from each camera2d's focus list.
		void OnTriggerExit2D (Collider2D collider2D)
		{
			if (m_RemoveOnExit && collider2D.tag == m_TagThatTriggers)
			{
				m_TriggerExitsToBeRemoved = Mathf.Max(m_TriggerExitsToBeRemoved--, 0);
				if (m_TriggerExitsToBeRemoved < m_MinimumInsideTrigger)
				{
					Undo();
				}
			}
		}
		void OnTriggerExit (Collider collider)
		{
			if (m_RemoveOnExit && collider.tag == m_TagThatTriggers)
			{
				m_TriggerExitsToBeRemoved = Mathf.Max(m_TriggerExitsToBeRemoved--, 0);
				if (m_TriggerExitsToBeRemoved < m_MinimumInsideTrigger)
				{
					Undo();
				}
			}
		}

		/// <summary>
		/// Adds the focus to each of the camera2d's provided through the inspector.
		/// </summary>
		public virtual void Activate ()
		{
			if (!m_Triggered)
			{
				m_Triggered = true;
				for (int i = 0; i < m_Camera2Ds.Length; i++)
				{
					if (m_Camera2Ds[i] != null)
					{
						m_Camera2Ds[i].AddFocus(this);
					}
				}
			}
		}

		/// <summary>
		/// Removes the focus from the camera2d's it is added to when triggered.
		/// </summary>
		public virtual void Undo ()
		{
			if (m_Triggered)
			{
				m_Triggered = false;
				m_TriggerExitsToBeRemoved = m_MinimumInsideTrigger - 1;
				for (int i = 0; i < m_Camera2Ds.Length; i++)
				{
					if (m_Camera2Ds[i] != null)
					{
						m_Camera2Ds[i].RemoveFocus(this);
					}
				}
			}
		}


#if UNITY_EDITOR

		//When a value is changed in the editor, make sure that the minimum inside the trigger is greater than 1.
		void OnValidate ()
		{
			m_MinimumInsideTrigger = Mathf.Max(m_MinimumInsideTrigger, 1);
		}

#endif
	}
}
