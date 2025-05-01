#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(GSheetImportManager))]
public class SO_GSheetImportManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        GSheetImportManager Target = (GSheetImportManager)target;
        if (GUILayout.Button("Import All"))
        {
            Target.BeginImportProcess(null);
        }
    }
}
#endif

