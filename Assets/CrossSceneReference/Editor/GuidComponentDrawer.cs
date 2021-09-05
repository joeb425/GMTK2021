﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GuidComponent))]
public class GuidComponentDrawer : UnityEditor.Editor
{
    private GuidComponent guidComp;

    public override void OnInspectorGUI()
    {
        if (guidComp == null)
        {
            guidComp = (GuidComponent)target;
        }
       
        // Draw label
        EditorGUILayout.LabelField("Guid:", guidComp.GetGuidString());

        if (GUILayout.Button("Set GUID"))
        {
            guidComp.CreateGuid();
        }
    }
}