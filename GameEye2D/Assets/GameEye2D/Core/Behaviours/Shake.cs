/* Camera2D Shake v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;

namespace GameEye2D.Behaviour
{
    /// <summary>
    /// When attached to a transform with a Camera2D, provides public functions that cause the camera's position to change as if a heavy force was applied. The behavior can be paused by disabling it.
	/// </summary>
	[AddComponentMenu("GameEye2D/Behaviours/Shake")]
    [RequireComponent(typeof(Camera2D))]
	public class Shake : MonoBehaviour {

		//The camera 2D that tracks the action and moves the camera
		Camera2D m_Camera2D;

		//The magnitude and length of the camera shake
	    //The minimum force to shake is subtracted from any shake
		[SerializeField] float m_MinForceToShake = 30f;
		[SerializeField] float m_BaseShakeAmount = 0.01f;
		[SerializeField] float m_BaseShakeTime = 0.004f;

		//Properties of the shake are stored for calculations when the shake function is called
		float m_OriginalShakeTimer = 0f;
		float m_ShakeTimer = 0f;
		float m_ShakeMagnitude = 0f;
        Vector2 m_ShakeOffset;


        /// <summary>
        /// Gets the camera2D used by this behavior.
        /// </summary>
        public Camera2D camera2D
        {
            get
            {
                if (m_Camera2D == null)
                {
                    m_Camera2D = GetComponent<Camera2D>();
                }
                return m_Camera2D;
            }
        }

        /// <summary>
        /// Gets or sets to minimum force magnitude required to shake the camera. This amount is subtracted from all shake forces.
        /// </summary>
        public float minimumForceToShake
		{
			get { return m_MinForceToShake; }
			set { m_MinForceToShake = value; }
		}

        /// <summary>
        /// Returns if the camera is shaking
        /// </summary>
        public bool isShaking
        {
            get { return m_ShakeTimer > 0f; }
        }

        /// <summary>
        /// Gets how much the camera is offset due to the camera shaking.
        /// </summary>
        public Vector2 shakeOffset
        {
            get { return m_ShakeOffset; }
        }


		//When the component is disabled, the camera will not shake
		void OnDisable ()
		{
			EndShake();
		}

		//After all the objects have moved, update the shake amount, timer, and direction on each frame
		void LateUpdate ()
		{
			//If the camera is shaking
            if (isShaking)
			{
				//Calculate the position of the camera without the shaking applied
				Vector2 originalPos = camera2D.position2D - m_ShakeOffset;

				//Calculate a shake offset based on the force of the crash, with a little randomness and gradually weakening as the shake ends.
				m_ShakeOffset += Random.insideUnitCircle * m_ShakeMagnitude * camera2D.zoom;
				m_ShakeOffset *= m_ShakeTimer / m_OriginalShakeTimer;
				m_ShakeTimer -= Time.unscaledDeltaTime;

				//Set camera position
				camera2D.position2D = originalPos + m_ShakeOffset;

				//Account for shifting due to being clamped to camera limits
				m_ShakeOffset = camera2D.position2D - originalPos;
			}
			//If the camera has just finished shaking
			else if (m_ShakeOffset != Vector2.zero)
			{
				EndShake();
			}
		}

		/// <summary>
		/// Causes the camera to shake, given a magnitude used for the amount of shaking and for how long.
		/// </summary>
		public void ShakeCamera(float magnitude)
		{
			if (!enabled)
			{
				return;
			}

			//Ignore minor forces
			float baseShake = (magnitude - m_MinForceToShake);
			float shakeMagnitude = baseShake * m_BaseShakeAmount;
			if (shakeMagnitude < 0 || shakeMagnitude < m_ShakeOffset.magnitude)
			{
				return;
			}

			//Set the new camera shake force
			m_ShakeMagnitude = shakeMagnitude;

			//Set the current shake time
			m_ShakeTimer = m_BaseShakeTime * baseShake;
			m_OriginalShakeTimer = m_ShakeTimer;
		}

		/// <summary>
		/// Causes the camera to shake, given a force that caused the shake to be used for direction, magnitude and how long the camera shakes.
		/// </summary>
		public void ShakeCamera(Vector2 force)
		{
			if (!enabled)
			{
				return;
			}

            //Ignore minor forces
            float baseShake = (force.magnitude - m_MinForceToShake);
			float shakeMagnitude = baseShake * m_BaseShakeAmount;
			if (shakeMagnitude < 0 || shakeMagnitude < m_ShakeOffset.magnitude)
			{
				return;
			}

			//Set the new camera shake force
			m_ShakeMagnitude = shakeMagnitude;

			//Set the current shake time
			m_ShakeTimer = m_BaseShakeTime * baseShake;
			m_OriginalShakeTimer = m_ShakeTimer;

			//Cause an instant shake following the force of the impact
			Vector2 instantShake = -force.normalized * m_ShakeMagnitude * camera2D.zoom;
			camera2D.position2D += instantShake;
			m_ShakeOffset += instantShake;
		}

		/// <summary>
		/// If the camera is currently shaking, it stops shaking.
		/// </summary>
		public void EndShake ()
		{
			m_ShakeTimer = 0f;
			camera2D.position2D -= m_ShakeOffset;
			m_ShakeOffset = Vector2.zero;
		}


#if UNITY_EDITOR

		//Whether to draw debug data or not
		[SerializeField] bool m_DrawDebug = false;


		//In the scene window draw the camera limits and action rect
		void OnDrawGizmos ()
		{
			if (m_DrawDebug && Application.isPlaying)
			{
				//Set the gizmo color for the line towards the desired position
				Gizmos.color = Color.yellow;

#if GAMEEYE2D_XZ
				//Draw the line towards the desired position
				Gizmos.DrawLine(transform.position, transform.position - new Vector3(m_ShakeOffset.x, transform.position.y, m_ShakeOffset.y));
#else
                //Draw the line towards the desired position
				Gizmos.DrawLine(transform.position, transform.position - new Vector3(m_ShakeOffset.x, m_ShakeOffset.y, transform.position.z));
#endif
			}
		}

#endif
	}
}
