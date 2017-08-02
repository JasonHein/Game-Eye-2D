using UnityEngine;
using System.Collections;
using GameEye2D.Focus;

public class KillZoneResetTrigger : KillZone {

	//The trigger that will be reset
	[SerializeField] Trigger m_Trigger;

	//Reset the game
	protected override void Trigger ()
	{
		//Remove the trigger focus from the camera focus list
		if (m_Trigger != null)
		{
			m_Trigger.Undo();
		}

		//Have the player character reset
		base.Trigger();

	}
}
