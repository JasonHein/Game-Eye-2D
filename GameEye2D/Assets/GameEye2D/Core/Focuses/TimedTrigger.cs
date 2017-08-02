/* Timed Trigger v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;
using System.Collections;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When attached to a transform with a trigger collider or collider2D, creates a focus that adds itself to the focus list of camera2Ds when an object with a certain tag enters the collider after a delay. There is an option for the camera2Ds to remove the focus when the trigger collider is exited.
    /// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Timed Trigger")]
	public class TimedTrigger : Trigger
    {
        //How long it takes for the trigger to activate
        [SerializeField] float m_TimeToActivate = 0f;
        float m_ActivationTimer = -1f;


        //The timer to activate
        void Update ()
        {
			//Check if the timer has expired
			if (m_ActivationTimer > 0f)
            {
				m_ActivationTimer -= Time.deltaTime;
				if (m_ActivationTimer <= 0f)
				{
					//If the trigger does not have enough objects inside, remove the focus from the camera2D
					if (m_TriggerExitsToBeRemoved < m_MinimumInsideTrigger)
					{
						for (int i = 0; i < m_Camera2Ds.Length; i++)
						{
							if (m_Camera2Ds[i] != null)
							{
								m_Camera2Ds[i].RemoveFocus(this);
							}
						}
					}
					//If the trigger has enough objects inside, add the focus to the camera2D
					else
					{
						for (int i = 0; i < m_Camera2Ds.Length; i++)
						{
							if (m_Camera2Ds[i] != null)
							{
								m_Camera2Ds[i].AddFocus(this);
							}
						}
					}
				}
            }
        }

		/// <summary>
		/// Adds the focus to each of the camera2d's provided through the inspector after a delay.
		/// </summary>
		public override void Activate ()
		{
			if (!m_Triggered)
			{
				m_Triggered = true;
				if (m_TimeToActivate == 0f)
				{
					for (int i = 0; i < m_Camera2Ds.Length; i++)
					{
						if (m_Camera2Ds[i] != null)
						{
							m_Camera2Ds[i].AddFocus(this);
						}
					}
				}
				else if (m_ActivationTimer > 0f)
				{
					m_ActivationTimer = -1f;
				}
				else
				{
					m_ActivationTimer = m_TimeToActivate;
				}
			}
		}

		/// <summary>
		/// Removes the focus from the camera2d's it is added to after a delay.
		/// </summary>
		public override void Undo ()
		{
			if (m_Triggered)
			{
				m_Triggered = false;
				m_TriggerExitsToBeRemoved = m_MinimumInsideTrigger - 1;
				if (m_TimeToActivate == 0f)
				{
					for (int i = 0; i < m_Camera2Ds.Length; i++)
					{
						if (m_Camera2Ds[i] != null)
						{
							m_Camera2Ds[i].RemoveFocus(this);
						}
					}
				}
				else if (m_ActivationTimer > 0f)
				{
					m_ActivationTimer = -1f;
				}
				else
				{
					m_ActivationTimer = m_TimeToActivate;
				}
			}
		}
    }
}