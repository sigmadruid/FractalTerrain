using System;
using System.IO;
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
            
            if (GUILayout.Button("Flatten"))
            {
                target.Flatten();
            }
            
            if (GUILayout.Button("Save Mesh"))
            {
                SaveMesh(target.Mesh);
            }
        }

        private void SaveMesh(Mesh mesh)
        {
            string randomName = Guid.NewGuid().ToString() + ".mesh";
            string path = Path.Combine("Assets/OutputMeshes", randomName);
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.Refresh();
        }
    }
}