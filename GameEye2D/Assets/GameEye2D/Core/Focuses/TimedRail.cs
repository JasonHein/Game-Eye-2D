/* Timed Rail v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// Creates a rail system when attached to an object.
	/// The focus point of the rail starts at the front and moves along the rail until it reaches the end.
	/// If looping is enabled through the inspector then the rail will start over once it reaches the end.
	/// Can be paused by disabling it.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/Timed Rail")]
	[ExecuteInEditMode]
    public class TimedRail : Rail
    {
        //How long it will take to move along the rail
        [SerializeField] float m_TimeToFinish = 10f;

        //Where the focus is along the rail
		[SerializeField] float m_StartInterpolation = 0f;

        //If the rail should start at the beginning when it completes
        [SerializeField] bool m_Loop;


        /// <summary>
        /// How far the focus is along the rail. Clamped between 0 and 1.
		/// If you want to reset the rail to the beginning. You can set this value to 0.
        /// </summary>
        public float interpolation
        {
            get { return m_StartInterpolation; }
			set { m_StartInterpolation = Mathf.Clamp01(value); }
        }

		/// <summary>
		/// How long it takes for the rail to move from one end to the other.
		/// </summary>
		public float timeToFinish
		{
			get { return m_TimeToFinish; }
			set { m_TimeToFinish = Mathf.Max(m_TimeToFinish, 0.0001f); }
		}


        //Each frame
        void Update()
		{
#if UNITY_EDITOR

			//While auto scale time is enabled in the editor, when the rail grows so does it time it takes to reach the end.
			if (!Application.isPlaying)
			{
				if (m_AutoScaleTime)
				{
					float change = ((float)length) / ((float)m_LastNumberOfPoints);
					if (change != 1f)
					{
						m_TimeToFinish *= change;
					}
					if (m_LastNumberOfPoints != length)
					{
						m_LastNumberOfPoints = length;
					}
				}
			}
			else
			{

#endif
				//Move further along the rail.
				if (interpolation < 1f)
				{
					interpolation += Time.deltaTime / m_TimeToFinish;

					//If enabled, loop the rail back to the beginning
					if (m_Loop && interpolation == 1f)
					{
						interpolation = 0f;
					}
				}
#if UNITY_EDITOR
			}
#endif
        }

        /// <summary>
        /// Gets the point along the rail where the focus is.
        /// </summary>
        public override Vector2 GetFocusPoint()
        {
            //Determine which curve the focus point is on
            int currentCurve;
            float curveInterpolation = interpolation;
            if (curveInterpolation == 1f)
            {
                currentCurve = length - 4;
            }
            else
            {
                curveInterpolation *= curveCount;
				currentCurve = (int)Mathf.Floor(curveInterpolation);
                curveInterpolation -= currentCurve;
                currentCurve *= 3;
            }

            return GetPoint(curveInterpolation, currentCurve);
        }

#if UNITY_EDITOR

		//If the rail getting addition points scales the rail timer to match
		[SerializeField] bool m_AutoScaleTime = true;
		int m_LastNumberOfPoints = 4;


        //Make sure there are always 4, 7, 10, etc points on the rail to avoid errors
        void OnValidate()
		{
			ValidatePoints();
			m_StartInterpolation = Mathf.Clamp01(m_StartInterpolation);
			m_TimeToFinish = Mathf.Max(m_TimeToFinish, 0.0001f);
        }

#endif
    }
}
