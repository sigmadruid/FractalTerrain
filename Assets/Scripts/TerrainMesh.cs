using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour 
{
    private MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        FractalTerrain terrain = new FractalTerrain();
        terrain.Init(100, 100, Random.Range(0, 20140413), 6);
        terrain.Generate();

        mesh.vertices = terrain.VertexList.ToArray();
        mesh.triangles = terrain.TriangleList.ToArray();
        mesh.normals = terrain.NormalList.ToArray();
        mesh.uv = terrain.UVList.ToArray();

        meshFilter.GetComponent<Renderer>().sharedMaterial.SetTexture("_TerrainMap", terrain.TerrainMap);

        MeshCollider mc = meshFilter.gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
    }



}
