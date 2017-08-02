/* World Cursor v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;
using GameEye2D.Focus;

namespace GameEye2D.Focus
{
    /// <summary>
    /// When added to a transform, creatures a focus that moves that transform based on the movement of two input axis (by default mouse X and Y). The axis that moves the transform can be set through the inspector.
	/// </summary>
	[AddComponentMenu("GameEye2D/Focus2D/World Cursor")]
    public class WorldCursor : F_Transform
	{
	    //The camera the mouse focus is based on.
		[SerializeField] Camera m_Camera;

	    //How much the cursor moves based on the input
	    [SerializeField] float m_Sensitivity = 1f;
	    
		//The input that moves the cursor
		[SerializeField] string m_X_Input = "Mouse X";
		[SerializeField] string m_Y_Input = "Mouse Y";


	    /// <summary>
	    /// Gets or sets the camera the mouse focus is based on.
	    /// </summary>
	    public Camera gameCamera
	    {
	        get 
			{
				if (m_Camera == null)
				{
					gameCamera = Camera.main;
					if (m_Camera == null)
					{
						enabled = false;
					}
				}
				return m_Camera;
			}
			set { m_Camera = value; }
	    }

	    /// <summary>
	    /// How much the transform of the focus moves to follow the input.
	    /// </summary>
	    public float sensitivity
	    {
	        get { return m_Sensitivity; }
	        set { m_Sensitivity = value; }
	    }

        /// <summary>
        /// The input from the two axis.
        /// </summary>
        protected Vector2 input
        {
            get { return new Vector2(Input.GetAxis(m_X_Input), Input.GetAxis(m_Y_Input)) * sensitivity; }
        }


		//Hide the real cursor and set this  cursors starting position
		void Start ()
		{
			Cursor.visible = false;

			if (gameCamera != null)
			{
				Vector3 worldPos = gameCamera.ScreenToWorldPoint(Input.mousePosition);

#if GAMEEYE2D_XZ
				position2D = new Vector2(worldPos.x, worldPos.z);
#else
                position2D = new Vector2(worldPos.x, worldPos.y);
#endif
			}
		}


        //Move the transform based on the input axis
        void Update ()
	    {
            position2D += input;
	    }
	}
}
