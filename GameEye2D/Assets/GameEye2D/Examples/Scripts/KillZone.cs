using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {

	//The object affected by the killzone, and the respawn point
	[SerializeField] GameObject m_Player;
	[SerializeField] GameObject m_RespawnPoint;

	//A timer for when to respawn the player
	float m_TimeOfRespawn = 0f;
	const float RESPAWN_DELAY = 1f;


    //Check if the player and respanw points exist
    void Awake ()
    {
        if (m_Player == null || m_RespawnPoint == null)
        {
            enabled = false;
            Destroy(this);
        }

        //Don't enable the kilzone's timer to enable the player until the killzone triggers
        enabled = false;
    }

    //While the player is respawning, check if the player should respawn this frame
	void Update ()
	{
		if (Time.time > m_TimeOfRespawn)
		{
			m_TimeOfRespawn = 0f;
			m_Player.SetActive(true);
            enabled = false;
		}
	}

	//When the player enters the trigger, they are slain and are moved to the respawn point
	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.gameObject == m_Player)
		{
			Trigger();
		}
	}

	//Reset the game
	protected virtual void Trigger ()
	{
		if (m_RespawnPoint != null)
		{
			//Set the time the player will be respawned
			m_TimeOfRespawn = Time.time + RESPAWN_DELAY;

			//Put the player at the spawn point and de-activate them until the respawn delay is over
			m_Player.transform.position = m_RespawnPoint.transform.position;
			m_Player.SetActive(false);

			//Enable the timer to respawn
			enabled = true;
		}
	}
}
