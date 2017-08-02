/* Camera2DMenu v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;
using UnityEditor;
using GameEye2D.Focus;

public class GameEye2DMenu : EditorWindow {

    //Menu paths
	const string MENU_STRING = "GameObject/GameEye2D/";
	const string FOCUS2D_MENU_STRING = "GameObject/GameEye2D/Focuses/";
	const string TOOL_MENU_STRING = "GameObject/GameEye2D/Tools/";
	const string AXIS_MENU_STRING = "Assets/GameEye2D/";

    //Spawned Focus2D Types
    const string FOLLOW_RAIL = "Follow Rail Focus2D";
	const string FOLLOW_RAILX = "Follow X Rail Focus2D";
	const string FOLLOW_RAILY = "Follow Y Rail Focus2D";
    const string RIGIDBODY = "RigidyBody Focus2D";
    const string RIGIDBODY2D = "RigidyBody2D Focus2D";
    const string SIMPLE_MOVING = "Simple Moving Focus2D";
	const string TIMED_RAIL = "Timed Rail Focus2D";
    const string TIMED_TRIGGER = "Timed Trigger Focus2D";
    const string TRANSFORM = "Transform Focus2D";
    const string TRIGGER = "Trigger Focus2D";
    const string WORLD_CURSOR = "World Cursor Focus2D";
	const string WORLD_CURSOR_LIMITED = "World Cursor Limited Focus2D";

	//Spawned Tool Types
	const string CLAMP_TO_SCREEN = "Clamp To Screen";
	const string PARALLAX = "Parralax";
	const string TRIGGER_CHANGE_CAMERA_LIMITS = "Trigger Camera Limits";
	const string TRIGGER_CHANGE_CAMERAS = "Trigger Cameras";
	const string TRIGGER_CHANGE_FOCUS_LIST = "Trigger Focus List";
	const string TRIGGER_ZOOM = "Trigger Zoom";

	//Spawned setup Types
    const string GAME_CAMERA_STRING = "Game Camera";
	const string ARENA_STRING = "Arena";
    const string PLATFORMER_STRING = "Platformer";
    const string ROLE_PLAYING_GAME = "Role Playing Game";
	const string RUNNER_STRING = "Runner";
    const string SHOOTEMUP_STRING = "Shoot Em Up";
    const string STRATEGY_STRING = "Strategy Cam2D";

    //Spawn position of a new camera
	const float CAMERA_SPAWN_DISTANCE_FROM_ACTION = 10f;

	//Axis strings
	const string XZ = "GAMEEYE2D_XZ";
	const char SEMI_COLON_AS_CHAR = ';';
	const string SEMI_COLON = ";";
	const string DOUBLE_SEMI_COLON = ";;";

	//Debug Messages
	const string FAILED_TO_INSTANTIATE = "Failed to instantiate object: ";
	const string PREFAB_NOT_IN_RESOURCES = "Prefab missing from GameEye2D resources folder: ";
	const string PREFAB_ALTERED_AND_WAS_NOT_ABLE_TO_LOAD = "Prefab was altered and unable to load: ";



	#region Switch Axis

	[MenuItem(AXIS_MENU_STRING + "Switch To Front View")]
	static void SwitchXY()
	{
		ChangeAxis(null);

	}
	[MenuItem(AXIS_MENU_STRING + "Switch to Top Down View")]
	static void SwitchXZ()
	{
		ChangeAxis(XZ);
	}

	//Changes GameEye2D's current axis to compile for
	static void ChangeAxis (string newAxis)
    {
#if UNITY_5_4
        ChangeAxisForBuild (newAxis, BuildTargetGroup.Android);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.iOS);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.Nintendo3DS);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.PS3);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.PS4);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.PSM);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.PSP2);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.SamsungTV);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.Standalone);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.Tizen);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.tvOS);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.WebGL);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.WiiU);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.WSA);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.XBOX360);
		ChangeAxisForBuild (newAxis, BuildTargetGroup.XboxOne);
#endif
#if UNITY_5_5
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Android);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.iOS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.N3DS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PS4);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PSM);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PSP2);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.SamsungTV);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Standalone);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Tizen);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.tvOS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WebGL);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WiiU);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WSA);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.XboxOne);
#endif
#if UNITY_5_6
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Android);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Facebook);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.iOS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.N3DS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PS4);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PSM);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PSP2);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.SamsungTV);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Standalone);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Tizen);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.tvOS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WebGL);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WiiU);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WSA);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.XboxOne);
#endif
#if UNITY_2017
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Android);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Facebook);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.iOS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.N3DS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PS4);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PSM);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.PSP2);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.SamsungTV);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Standalone);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Switch);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.Tizen);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.tvOS);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WebGL);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WiiU);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.WSA);
        ChangeAxisForBuild(newAxis, BuildTargetGroup.XboxOne);
#endif
    }

	//Sets a build's axis that GameEye2D compiles for
	static void ChangeAxisForBuild (string newAxis, BuildTargetGroup target)
	{
		if (target != BuildTargetGroup.Unknown)
		{
			//Gets symbols for the target build group
			string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

			//Replace existing axis symbols
			if (symbols.Length > 0)
			{
				symbols = symbols.Replace(XZ, "");
				symbols = symbols.Replace(DOUBLE_SEMI_COLON, SEMI_COLON);

				//Remove semi colons from the start of the string
				if (symbols.Length > 0 && symbols[0] == SEMI_COLON_AS_CHAR)
				{
					symbols.Remove(0, 1);
				}
			}

			//Add the new axis symbol
			if (newAxis != null)
			{
				if (symbols.Length > 0)
				{

					symbols += SEMI_COLON + newAxis;
				}
				else
				{
					symbols += newAxis;
				}
			}

			//Set the new symbols string
			PlayerSettings.SetScriptingDefineSymbolsForGroup(target, symbols);
		}
	}

	#endregion

    #region Create Focus

    //Create a focus when a menu item is selected
    [MenuItem(FOCUS2D_MENU_STRING + "Follow Rail")]
	static void FollowRail()
	{
		SpawnFocusAsIdentity(FOLLOW_RAIL);
	}
    [MenuItem(FOCUS2D_MENU_STRING + "Follow X Rail")]
	static void FollowRailX()
	{
		SpawnFocusAsIdentity(FOLLOW_RAILX);
	}
    [MenuItem(FOCUS2D_MENU_STRING + "Follow Y Rail")]
	static void FollowRailY()
	{
		SpawnFocusAsIdentity(FOLLOW_RAILY);
	}
    [MenuItem(FOCUS2D_MENU_STRING + "RigidyBody")]
    static void RigidBodyFocus()
    {
        SpawnFocus(RIGIDBODY);
    }
    [MenuItem(FOCUS2D_MENU_STRING + "RigidyBody2D")]
    static void RigidyBody2DFocus()
    {
        SpawnFocus(RIGIDBODY2D);
    }
    [MenuItem(FOCUS2D_MENU_STRING + "Simple Moving")]
    static void SimpleMoving()
    {
        SpawnFocus(SIMPLE_MOVING);
    }
    [MenuItem(FOCUS2D_MENU_STRING + "Timed Rail")]
	static void TimedRail()
	{
		SpawnFocusAsIdentity(TIMED_RAIL);
	}
    [MenuItem(FOCUS2D_MENU_STRING + "Timed Trigger")]
    static void TimedTrigger()
    {
        SpawnFocus(TIMED_TRIGGER);
    }
    [MenuItem(FOCUS2D_MENU_STRING + "Transform")]
    static void TransformFocus()
    {
        SpawnFocus(TRANSFORM);
    }
    [MenuItem(FOCUS2D_MENU_STRING + "Trigger")]
    static void Trigger()
    {
        SpawnFocus(TRIGGER);
    }
    [MenuItem(FOCUS2D_MENU_STRING + "World Cursor")]
    static void WorldCursor()
    {
        SpawnFocus(WORLD_CURSOR);
    }
    [MenuItem(FOCUS2D_MENU_STRING + "World Cursor Limited")]
	static void WorldCursorLimited()
	{
		SpawnFocus(WORLD_CURSOR_LIMITED);
	}

    #endregion


	#region Create Packs and Cameras

	//Create a camera or pack
    [MenuItem(MENU_STRING + "Camera")]
    static void GameCamera()
    {
        SpawnCamera(GAME_CAMERA_STRING);
    }
	[MenuItem(MENU_STRING + "Arena")]
	static void Arena ()
	{
		SpawnPack(ARENA_STRING);
	}
	[MenuItem(MENU_STRING + "Platformer")]
	static void Platformer()
	{
		SpawnPack(PLATFORMER_STRING);
	}
    [MenuItem(MENU_STRING + "Puzzle or Strategy")]
    static void Strategy()
    {
        SpawnCamera(STRATEGY_STRING);
    }
    [MenuItem(MENU_STRING + "Role Playing Game")]
    static void RolePlayingGame()
    {
        SpawnPack(ROLE_PLAYING_GAME);
    }
	[MenuItem(MENU_STRING + "Runner")]
	static void Runner()
	{
		SpawnPack(RUNNER_STRING);
	}
	[MenuItem(MENU_STRING + "Shoot-em-up")]
	static void ShootEmUp()
	{
		SpawnPack(SHOOTEMUP_STRING);
	}

	#endregion


	#region Create Tools

	[MenuItem(TOOL_MENU_STRING + "Clamp To Screen")]
	static void ClampToScreen()
	{
		//Get the prefab
		GameObject obj = Spawn (CLAMP_TO_SCREEN);
		if (obj == null)
		{
			return;
		}

		//Reset the tool
		ClampToScreen tool = obj.GetComponent<ClampToScreen>();
		if (tool == null)
		{
			Debug.LogError(PREFAB_ALTERED_AND_WAS_NOT_ABLE_TO_LOAD + CLAMP_TO_SCREEN);
			DestroyImmediate(obj, true);
			return;
		}

		//Set camera to default
		Camera mainCamera = Camera.main;
		if (mainCamera != null)
		{
			tool.gameCamera = mainCamera;
		}
	}
    [MenuItem(TOOL_MENU_STRING + "Parallax")]
	static void Parallax()
	{
		//Get the prefab
		GameObject obj = Spawn (PARALLAX);
		if (obj == null)
		{
			return;
		}

		//Reset the tool
		Parallax tool = obj.GetComponent<Parallax>();
		if (tool == null)
		{
			Debug.LogError(PREFAB_ALTERED_AND_WAS_NOT_ABLE_TO_LOAD + PARALLAX);
			DestroyImmediate(obj, true);
			return;
		}
		tool.Reset();
	}
	[MenuItem(TOOL_MENU_STRING + "Trigger Camera Limits")]
	static void TriggerChangeCameraLimits()
	{
		//Get the prefab
		GameObject obj = Spawn (TRIGGER_CHANGE_CAMERA_LIMITS);
		if (obj == null)
		{
			return;
		}

		//Reset the tool
		TriggerCameraLimits tool = obj.GetComponent<TriggerCameraLimits>();
		if (tool == null)
		{
			Debug.LogError(PREFAB_ALTERED_AND_WAS_NOT_ABLE_TO_LOAD + TRIGGER_CHANGE_CAMERA_LIMITS);
			DestroyImmediate(obj, true);
			return;
		}
		tool.Reset();
	}
	[MenuItem(TOOL_MENU_STRING + "Trigger Cameras")]
	static void TriggerChangeCameras()
	{
		//Get the prefab
		GameObject obj = Spawn (TRIGGER_CHANGE_CAMERAS);
		if (obj == null)
		{
			Debug.LogError(PREFAB_ALTERED_AND_WAS_NOT_ABLE_TO_LOAD + TRIGGER_CHANGE_CAMERAS);
			DestroyImmediate(obj, true);
			return;
		}
	}
	[MenuItem(TOOL_MENU_STRING + "Trigger Focus List")]
	static void TriggerChangeFocusList()
	{
		//Get the prefab
		GameObject obj = Spawn (TRIGGER_CHANGE_FOCUS_LIST);
		if (obj == null)
		{
			return;
		}

		//Reset the tool
		TriggerFocusList tool = obj.GetComponent<TriggerFocusList>();
		if (tool == null)
		{
			Debug.LogError(PREFAB_ALTERED_AND_WAS_NOT_ABLE_TO_LOAD + TRIGGER_CHANGE_FOCUS_LIST);
			DestroyImmediate(obj, true);
			return;
		}
		tool.Reset();
	}
	[MenuItem(TOOL_MENU_STRING + "Trigger Zoom")]
	static void TriggerZoom()
	{
		//Get the prefab
		GameObject obj = Spawn (TRIGGER_ZOOM);
		if (obj == null)
		{
			return;
		}

		//Reset the tool
		TriggerZoom tool = obj.GetComponent<TriggerZoom>();
		if (tool == null)
		{
			Debug.LogError(PREFAB_ALTERED_AND_WAS_NOT_ABLE_TO_LOAD + TRIGGER_ZOOM);
			DestroyImmediate(obj, true);
			return;
		}
		tool.Reset();
	}

	#endregion


	#region Spawn Functions

	//Spawns an object from the given path
	static GameObject Spawn (string path)
	{
        //Check if there is a view of the scene
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null)
        {
            return null;
        }

		//Get the prefab
		GameObject obj = Resources.Load(path) as GameObject;

		//Null check the object to spawn
		if (obj == null)
		{
			Debug.LogError(PREFAB_NOT_IN_RESOURCES + path);
			DestroyImmediate(obj, true);
			return null;
		}

        //Make the object, with the selected object as the parent
		obj = (GameObject)GameObject.Instantiate(obj);
		Undo.RegisterCreatedObjectUndo(obj, "Spawn " + path);
        if (obj == null)
		{
			Debug.LogError(FAILED_TO_INSTANTIATE + path);
			DestroyImmediate(obj, true);
			return null;
		}

        //Set the focus in front of the scene camera
        obj.transform.position = sceneView.pivot;
		obj.name = obj.name.Replace("(Clone)", "").Trim();
		obj.transform.parent = Selection.activeTransform;

		//Select the object
		Selection.activeGameObject = obj;

		//Return the new object
		return obj;
	}

	//Spawns a pack or tool from the given path
	static void SpawnPack (string path)
	{
#if GAMEEYE2D_XZ
		GameObject obj = Spawn (path);

		//In top down mode, rotate the camera downwards
        if (obj != null)
		{
            Camera[] cameras = obj.GetComponentsInChildren<Camera>();
            if (cameras.Length > 0)
            {
                Vector3 localPosition;
                foreach (Camera cam in cameras)
                {
                    localPosition = cam.transform.localPosition;
                    cam.transform.localPosition = new Vector3(localPosition.x, -localPosition.z, -localPosition.y);
					cam.transform.rotation = new Quaternion(1f, 0f, 0f, 1f);
                }
            }
		}
#else

		Spawn (path);
#endif
	}

    //Spawn a focus from the given path
	static GameObject SpawnFocus(string path)
    {
		//Get the prefab
        GameObject obj = SpawnFocusAsIdentity(path);

#if GAMEEYE2D_XZ
        //In XZ mode, rotate the object upwards
		if (obj != null)
		{
			obj.transform.rotation = new Quaternion(-1f, 0f, 0f, 1f);
		}
#endif
        //Return the spawned object
		return obj;
    }

	//Spawn a focus from the given path, with the ration unchanged
	static GameObject SpawnFocusAsIdentity(string path)
	{
		//Get the prefab
		GameObject obj = Spawn (path);
		if (obj == null)
		{
			return null;
		}

		//Null check for a focus
		Focus2D focus = obj.GetComponent<Focus2D>();
		if (focus == null)
		{
			Debug.LogError(PREFAB_ALTERED_AND_WAS_NOT_ABLE_TO_LOAD + path);
			DestroyImmediate(obj, true);
			return null;
		}

		//Add focus to add auto add cameras
		Camera2D[] camera2DArray = FindObjectsOfType<Camera2D>();
		if (camera2DArray.Length > 0)
		{
			for (int i = 0; i < camera2DArray.Length; i++)
			{
				if (camera2DArray[i].autoAddFocus)
				{
					camera2DArray[i].AddFocus(focus);
				}
			}
		}

		//Return the spawned object
		return obj;
	}

	//Spawn a camera from the given path
	static GameObject SpawnCamera (string path)
	{
		//Get the prefab
		GameObject obj = Spawn (path);
        if (obj != null)
        {
#if GAMEEYE2D_XZ
        	//Cause the camera to be in front of the origin
			obj.transform.rotation = new Quaternion(1f, 0f, 0f, 1f);
#endif
        }

        //Return the spawned object
		return obj;
	}

    #endregion
}
