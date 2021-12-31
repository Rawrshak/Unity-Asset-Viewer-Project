using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

[CustomEditor(typeof(WalletQueries))]
public class WalletQueriesInspector : Editor
{
    private WalletQueries script;

    private void OnEnable()
    {
        // Method 1
        script = (WalletQueries) target;
    }

    public override async void OnInspectorGUI()
    {
        // Draw default inspector
        base.OnInspectorGUI();

        if (GUILayout.Button("Query Blockchain"))
        {
            await script.Query();
        }
    }
}
