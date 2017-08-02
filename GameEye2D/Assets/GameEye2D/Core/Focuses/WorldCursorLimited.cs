/* World Cursor Limited v1.0
 * 
 * By Jason Hein
*/


using UnityEngine;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When added to a transform, creatures a focus that moves that transform based on the movement of two input axis (by default mouse X and Y) and leashes it to not be farther away from the transform’s parent than the leash distance. The axis that moves the transform can be set through the inspector.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/World Cursor Limited")]
    public class WorldCursorLimited : WorldCursor
	{
        //How far the cursor can be from this transform's parent
		[SerializeField] float m_LeashDistance = 10f;


        //Move the transform based on the input axis
        void Update ()
        {
            position2D += input;
			localPosition2D = localPosition2D.normalized * Mathf.Min(localPosition2D.magnitude, m_LeashDistance);
        }
	}
}