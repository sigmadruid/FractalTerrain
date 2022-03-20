using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{
    [SerializeField] private float _length = 100;
    [SerializeField] private float _width = 100;
    [SerializeField] private int _fractalMaxTimes = 6;
    [SerializeField] private int _initialDitherRange = 20;
    [SerializeField] private float _ditherDeclineRatio = 0.5f;

    [SerializeField] private bool _autoRotate = true;
    [SerializeField] private float _rotateSpeed = 10;
    
    [SerializeField] private float _flattenHeight = 0;
    
    private MeshFilter meshFilter;
    
    private FractalTerrain _terrain = new FractalTerrain();

    void Awake()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
        Regenerate();
    }

    public void Regenerate()
    {
        meshFilter.transform.localPosition = new Vector3(-0.5f * _length, 0, -0.5f * _width);

        _terrain.Init(_length, _width, Random.Range(0, 20140413), _initialDitherRange, _ditherDeclineRatio, _fractalMaxTimes);
        _terrain.Generate();

        GenerateMesh();
    }

    public void Flatten()
    {
        _terrain.Flatten(_flattenHeight);
        GenerateMesh();
    }

    public Mesh Mesh
    {
        get
        {
            return meshFilter.mesh;
        }
    }

    private void GenerateMesh()
    {
        if (meshFilter.mesh)
        {
            Destroy(meshFilter.mesh);
        }
        
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.vertices = _terrain.VertexList.ToArray();
        mesh.triangles = _terrain.TriangleList.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = _terrain.UVList.ToArray();

        meshFilter.GetComponent<Renderer>().sharedMaterial.SetTexture("_TerrainMap", _terrain.TerrainMap);
    }

    void Update()
    {
        if (_autoRotate)
        {
            transform.localEulerAngles += _rotateSpeed * Vector3.up * Time.deltaTime;
        }
    }

}
