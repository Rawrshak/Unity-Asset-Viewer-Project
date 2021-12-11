using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

[CustomEditor(typeof(ObjectBounds))]
public class ObjectBoundsInspector : Editor
{
    private ObjectBounds script;

    private void OnEnable()
    {
        // Method 1
        script = (ObjectBounds) target;
    }

    public override void OnInspectorGUI()
    {
        // Draw default inspector
        base.OnInspectorGUI();

        // Draw a few buttons
        if (GUILayout.Button("Refresh Target Object"))
        {
            script.RefreshTargetObject();
        }

        // Draw a few buttons
        if (GUILayout.Button("Clear Target Object"))
        {
            script.ClearTargetObject();
        }
    }
}
