using UnityEngine;
using System.Collections;
using GameEye2D.Focus;
using GameEye2D.Behaviour;

[RequireComponent(typeof(Rigidbody))]
public class Ball3DMove : F_RigidBody
{
    //Camea2d that uses the ball as a focus, and the camera shake for when a collison occurs
    Camera2D m_Camera2D;
    Shake m_CameraShake;

    //Starting speed and gravity of this ball
    public float Speed = 20f;
    const float GRAVITY = -500f;


    //When the ball becomes enabled
    void OnEnable()
    {
        //Set initial speed
        body.AddForce(Random.insideUnitSphere.normalized * Speed / Time.fixedDeltaTime);

        //Tell the camera to follow this ball
        m_Camera2D = Camera.main.GetComponent<Camera2D>();
        m_CameraShake = Camera.main.GetComponent<Shake>();
        if (m_Camera2D != null)
        {
            m_Camera2D.AddFocus(this);
        }
    }

    //When the ball becomes disabled the camera should stop following it
    void OnDisable()
    {
        if (m_Camera2D != null)
        {
            m_Camera2D.RemoveFocus(this);
        }
    }

    //Cause gravity
    void FixedUpdate()
    {
        body.AddForce(Vector3.up * GRAVITY * Time.fixedDeltaTime);
    }

    //When the ball collides the camera may shake
    void OnCollisionEnter(Collision collision)
    {
        if (m_CameraShake != null)
        {
            m_CameraShake.ShakeCamera(collision.relativeVelocity);
        }
    }
}
