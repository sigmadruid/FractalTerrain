using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{
    [SerializeField] private float _length = 100;
    [SerializeField] private float _width = 100;
    [SerializeField] private int _fractalMaxTimes = 6;
    [SerializeField] private bool _autoRotate = true;
    [SerializeField] private float _rotateSpeed = 10;
    
    private MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
        Regenerate();
    }

    public void Regenerate()
    {
        if (meshFilter.mesh)
        {
            Destroy(meshFilter.mesh);
        }
        
        meshFilter.transform.localPosition = new Vector3(-0.5f * _length, 0, -0.5f * _width);

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        FractalTerrain terrain = new FractalTerrain();
        terrain.Init(_length, _width, Random.Range(0, 20140413), _fractalMaxTimes);
        terrain.Generate();

        mesh.vertices = terrain.VertexList.ToArray();
        mesh.triangles = terrain.TriangleList.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = terrain.UVList.ToArray();

        meshFilter.GetComponent<Renderer>().sharedMaterial.SetTexture("_TerrainMap", terrain.TerrainMap);
    }

    void Update()
    {
        if (_autoRotate)
        {
            transform.localEulerAngles += _rotateSpeed * Vector3.up * Time.deltaTime;
        }
    }

}
