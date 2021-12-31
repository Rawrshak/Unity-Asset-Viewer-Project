using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Rawrshak
{
    [CustomEditor(typeof(Content), true)]
    public class ContentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Content contract = (Content)target;

            EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
            EditorGUILayout.LabelField("Contracts Valid", contract.IsValid().ToString());
            if (GUILayout.Button("Verify Content Contracts"))
            {
                contract.VerifyContracts();
            }
            EditorGUI.EndDisabledGroup();

            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}