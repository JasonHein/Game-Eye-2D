using UnityEngine;
using System.Collections;
using GameEye2D.Focus;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
	//Horizontal movement
	const float HORIZONTAL_SPEED = 300f;
	const float TIME_TO_REACH_FULL_HORIZONTAL_SPEED = 0.075f;
	const float TIME_TO_END_HORIZONTAL_SPEED = 0.075f;

	//Vertical movement
	const float JUMP_SPEED = 900f;
	const float MINIMUM_INPUT_TO_JUMP = 0.5f;
	const float INPUT_AFFECT_ON_GRAVITY = 0.4f;
	const float GRAVITY = -3000f;

	//Checking if the player can jump
	bool m_Grounded = false;
	const float GROUNDED_NORMAL = 0.0f;

	//Input
	const string VERTICAL = "Vertical";
	const string HORIZONTAL = "Horizontal";
    float m_VerticalInput = 0f;
    float m_HorizontalInput = 0f;

	//The player's rigidbody2D
	Rigidbody2D m_RigidBody2D;


	//Get the rigidBody of the player.
	void Start ()
	{
		m_RigidBody2D = GetComponent<Rigidbody2D>();
	}

	//Get input in update, to make input more consistent
	void Update ()
	{
        //Vertical input
        if (Input.GetKey(KeyCode.Space))
        {
            m_VerticalInput = Time.deltaTime;
        }
        else
        {
            m_VerticalInput = Input.GetAxisRaw(VERTICAL);
        }

        ///Horizontal input
        m_HorizontalInput = Input.GetAxisRaw(HORIZONTAL);
	}
	
	//A set number of times per second (by default occurs 50 times a second)
	void FixedUpdate ()
	{
		//Get the players current speed and initialize acceleration this frame to 0
		Vector2 velocity = m_RigidBody2D.velocity;
		Vector2 acceleration = Vector2.zero;

		//Get the players horizontal acceleration this frame
        acceleration.x += HORIZONTAL_SPEED / TIME_TO_REACH_FULL_HORIZONTAL_SPEED * m_HorizontalInput;

		//Decelleration due to no input and to clamp the maximum speed gradually
		acceleration.x -= (velocity.x / TIME_TO_REACH_FULL_HORIZONTAL_SPEED) / TIME_TO_END_HORIZONTAL_SPEED;

		//Jumping
        if (m_Grounded && m_VerticalInput > MINIMUM_INPUT_TO_JUMP)
		{
			acceleration.y = JUMP_SPEED / Time.fixedDeltaTime;
			m_Grounded = false;
		}
        //Falling
        else
        {
            //Add gravity to make the player fall, and affect gravity with the players input
            acceleration.y += GRAVITY * (1f - INPUT_AFFECT_ON_GRAVITY * m_VerticalInput);

            //Decleleration due to no input, and to clmap the maximumn speed gradually
            acceleration.y -= velocity.y;
        }

		//Add the given force to the player
		m_RigidBody2D.AddForce(acceleration * Time.fixedDeltaTime);
	}

	//Check if the player has become grounded
	void OnCollisionEnter2D (Collision2D collision)
	{
		//Get the average normal of the collision
		Vector2 averageNormal = Vector2.zero;
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			averageNormal += collision.contacts[i].normal;
		}
		averageNormal /= collision.contacts.Length;

		//If the collision the player entered was with the ground, we are now grounded
		if (averageNormal.y > GROUNDED_NORMAL)
		{
			m_Grounded = true;
		}
	}

	//Check if the player has left the ground
	void OnCollisionExit2D (Collision2D collision)
	{
		//Get the average normal of the collision
		Vector2 averageNormal = Vector2.zero;
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			averageNormal += collision.contacts[i].normal;
		}
		averageNormal /= collision.contacts.Length;

		//If the collision the player exited was with the ground, we are no longer grounded
		if (averageNormal.y > GROUNDED_NORMAL)
		{
			m_Grounded = false;
		}
	}
}
