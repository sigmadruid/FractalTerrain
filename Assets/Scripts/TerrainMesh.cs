using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour 
{
    public GameObject testPrefab;
    private MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

//        List<Vector3> vertexList = new List<Vector3>();
//        vertexList.Add(Vector3.zero);
//        vertexList.Add(Vector3.left);
//        vertexList.Add(Vector3.forward);
//
//        List<int> triangleList = new List<int>();
//        triangleList.Add(0);
//        triangleList.Add(1);
//        triangleList.Add(2);
//
//        mesh.vertices = vertexList.ToArray();
//        mesh.triangles = triangleList.ToArray();

        FractalTerrain terrain = new FractalTerrain();
        terrain.Init(100, 100, Random.Range(0, 20140413), 6);
        terrain.Generate();

//        for(int i = 0; i < terrain.VertexList.Count; ++i)
//        {
//            Debug.Log(terrain.VertexList[i]);
//        }
//        for(int i = 0; i < terrain.TriangleList.Count; ++i)
//        {
//            Debug.Log(terrain.TriangleList[i]);
//            if((i + 1) % 3 == 0)
//            {
//                Debug.Log("-----------------");
//            }
//        }
        mesh.vertices = terrain.VertexList.ToArray();
        mesh.triangles = terrain.TriangleList.ToArray();
        mesh.normals = terrain.NormalList.ToArray();
    }



}
