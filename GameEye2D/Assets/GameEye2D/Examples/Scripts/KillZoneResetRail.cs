using UnityEngine;
using System.Collections;
using GameEye2D.Focus;

public class KillZoneResetRail : KillZone {

	//The timed rail that will be reset
	[SerializeField] TimedRail m_Rail;

	//Reset the game
	protected override void Trigger ()
	{
		//Reset the timed rail back to the start
		if (m_Rail != null)
		{
			m_Rail.interpolation = 0f;
		}

		//Have the player character reset
		base.Trigger();

	}
}
