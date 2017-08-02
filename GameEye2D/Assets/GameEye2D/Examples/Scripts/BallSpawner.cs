using UnityEngine;
using System.Collections.Generic;

public class BallSpawner : MonoBehaviour
{
	//Objects spawned
	[SerializeField] GameObject m_ObjectToSpawn;
	List<GameObject> m_ObjectsSpawned;

    //Firing speed
	const float FIRE_RATE = 0.3f;
	float m_FireTimer = -1.0f;


	//Load objects to spawn
	void Start ()
	{
        m_ObjectsSpawned = new List<GameObject>();
        if (m_ObjectToSpawn == null)
        {
            enabled = false;
        }
        else
        {
            SpawnBall();
        }
	}

	//Check for input to spawn a ball
	void Update ()
	{
        //If the user hits the escape button, end the application
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        //Check fire rate
		m_FireTimer -= Time.deltaTime;
        if (m_FireTimer > 0.0f)
        {
            return;
        }

		//Whenever the left mouse button is clicked spawn a ball and reset the firing timer
		if (Input.GetMouseButton(0))
		{
            SpawnBall();
		}

		//Whenever you click the Right mouse button remove the first ball in the array
        if (Input.GetMouseButton(1) && m_ObjectsSpawned.Count > 0)
		{
            DeSpawnBall();
		}
	}

    void SpawnBall ()
    {
        if (m_ObjectToSpawn != null)
        {
            m_FireTimer = FIRE_RATE;
            GameObject ball = ((GameObject)GameObject.Instantiate(m_ObjectToSpawn, transform.position, Quaternion.identity));
            ball.transform.parent = transform;

            if (ball == null)
            {
                Debug.Log("w");
            }
            m_ObjectsSpawned.Add(ball);
        }
    }

    void DeSpawnBall ()
    {
        m_FireTimer = FIRE_RATE;
        m_ObjectsSpawned[0].SetActive(false);
        Destroy(m_ObjectsSpawned[0]);
        m_ObjectsSpawned.RemoveAt(0);
    }
}
