using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    [CustomEditor(typeof(TerrainMesh))]
    public class TerrainMeshEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            TerrainMesh target = serializedObject.targetObject as TerrainMesh;

            if (GUILayout.Button("Regenerate"))
            {
                target.Regenerate();
            }
        }
    }
}