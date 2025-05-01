#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(GSheetImporter))]
public class SO_GSheetImporter_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        GSheetImporter Target = (GSheetImporter)target;
        if (GUILayout.Button("Begin Import Process"))
        {
            Target.BeginImportProcess(null);
        }
    }
}
#endif

