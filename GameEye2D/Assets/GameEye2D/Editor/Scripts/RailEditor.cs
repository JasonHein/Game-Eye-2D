/* RailEditor v1.0
 * 
 * By Jason Hein
 */


using UnityEngine;
using System;
using System.Collections;
using GameEye2D.Focus;
using UnityEditor;

[CustomEditor(typeof(Rail), true)]
public class RailEditor : Editor {

    //The rail object
    Rail m_Rail;
	Transform m_HandleTransform;
	Quaternion m_HandleRotation;

    //The points along the rail
	const float m_HandleSize = 0.06f;
	const float m_PickSize = 0.1f;
	int m_SelectedIndex = -1;
    Vector2[] m_Points;

	//How far the points are seperated when created.
	public const float ADDED_POINT_SEPERATION = 10f;


    //When the scene draws
	void OnSceneGUI ()
	{
        //Get the rail
        m_Rail = target as Rail;
        if (m_Rail == null)
        {
            return;
        }
        m_Points = m_Rail.points;
		m_HandleTransform = m_Rail.transform;
		m_HandleRotation = Tools.pivotRotation == PivotRotation.Local ? m_HandleTransform.rotation : Quaternion.identity;

        //Draw the points, and allow the editor to move these points
		if (m_Points.Length > 3)
        {
            Vector3 point0 = ShowPoint(0);
            for (int i = 1; i < m_Points.Length; i += 3)
            {
                Vector3 point1 = ShowPoint(i);
                Vector3 point2 = ShowPoint(i + 1);
                Vector3 point3 = ShowPoint(i + 2);
                Handles.DrawBezier(point0, point3, point1, point2, Color.cyan, null, 2f);
                point0 = point3;
            }
        }
	}

    //Shows the given point.
	Vector3 ShowPoint (int index)
	{
        //Get where the point is
#if GAMEEYE2D_XZ
		Vector3 point = m_HandleTransform.TransformPoint(new Vector3 (m_Points[index].x, 0f, m_Points[index].y));
#else
        Vector3 point = m_HandleTransform.TransformPoint(new Vector3 (m_Points[index].x, m_Points[index].y, 0f));
#endif
		Handles.color = Color.white;
		float size = HandleUtility.GetHandleSize(point);

        //Create a button at the point
		if (Handles.Button(point, m_HandleRotation, size * m_HandleSize, size * m_PickSize, Handles.DotHandleCap))
		{
			m_SelectedIndex = index;
		}

        //If the point is selected and has changed, update the point's position
		if (m_SelectedIndex == index)
		{
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, m_HandleRotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_Rail, "Move Point");
				EditorUtility.SetDirty(m_Rail);

				Vector3 newPoint = m_HandleTransform.InverseTransformPoint(point);
#if GAMEEYE2D_XZ
				m_Rail.points[index] = new Vector2 (newPoint.x, newPoint.z);
#else
                m_Rail.points[index] = new Vector2 (newPoint.x, newPoint.y);
#endif
			}
		}

		//Return the point
		return point;
	}

    //Add buttons to grow or shrink the rail
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();
        m_Rail = target as Rail;
		if (m_Rail == null)
		{
			return;
		}
		m_Points = m_Rail.points;

        if (GUILayout.Button("Add Curve to Beginning"))
		{
            Undo.RecordObject(m_Rail, "Add Curve to Beginning");
			AddCurveToBeginning();
			EditorUtility.SetDirty(m_Rail);
		}
        else if (GUILayout.Button("Add Curve To End"))
        {
            Undo.RecordObject(m_Rail, "Add Curve To End");
            AddCurveToEnd();
            EditorUtility.SetDirty(m_Rail);
        }
        else if (GUILayout.Button("Remove Curve From Beginning"))
        {
            Undo.RecordObject(m_Rail, "Remove Curve From Beginning");
            RemoveCurveFromBeginning();
            EditorUtility.SetDirty(m_Rail);
        }
        else if (GUILayout.Button("Remove Curve From End"))
        {
            Undo.RecordObject(m_Rail, "Remove Curve From End");
            RemoveCurveFromEnd();
            EditorUtility.SetDirty(m_Rail);
        }
	}
		
	//Adds 3 points to the beginning of the rail.
	void AddCurveToBeginning()
	{
		Array.Resize(ref m_Points, m_Points.Length + 3);
		for (int i = 1; i < m_Points.Length - 3; i += 3)
		{
			m_Points[i + 3] = m_Points[i];
			m_Points[i + 4] = m_Points[i + 1];
			m_Points[i + 5] = m_Points[i + 2];
		}
		m_Points[3] = m_Points[0];

		Vector2 point = m_Points[3];
		point.x -= ADDED_POINT_SEPERATION;
		m_Points[2] = point;
		point.x -= ADDED_POINT_SEPERATION;
		m_Points[1] = point;
		point.x -= ADDED_POINT_SEPERATION;
		m_Points[0] = point;
		m_Rail.points = m_Points;
	}
		
	//Adds 3 points to the end of the rail.
	void AddCurveToEnd()
	{
		Vector2 point = m_Points[m_Points.Length - 1];
		Array.Resize(ref m_Points, m_Points.Length + 3);
		point.x += ADDED_POINT_SEPERATION;
		m_Points[m_Points.Length - 3] = point;
		point.x += ADDED_POINT_SEPERATION;
		m_Points[m_Points.Length - 2] = point;
		point.x += ADDED_POINT_SEPERATION;
		m_Points[m_Points.Length - 1] = point;
		m_Rail.points = m_Points;
	}
		
	//Removes 3 points on the rail from the beginning.
	void RemoveCurveFromBeginning()
	{
		if (m_Points.Length > 6)
		{
			m_Points[0] = m_Points[3];
			for (int i = 1; i < m_Points.Length - 3; i += 3)
			{
				m_Points[i] = m_Points[i + 3];
				m_Points[i + 1] = m_Points[i + 4];
				m_Points[i + 2] = m_Points[i + 5];
			}
			Array.Resize(ref m_Points, m_Points.Length - 3);
			m_Rail.points = m_Points;
		}
	}
		
	//Removes 3 points on the rail from the end.
	void RemoveCurveFromEnd()
	{
		if (m_Points.Length > 6)
		{
			Array.Resize(ref m_Points, m_Points.Length - 3);
			m_Rail.points = m_Points;
		}
	}
}
