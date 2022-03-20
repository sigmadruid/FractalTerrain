using UnityEngine;

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class FractalTerrain
{
    private float _initialDitherRange = 50f;
    private float _ditherDeclineRatio = 0.5f;

    private float length;
    private float width;
    private int fractalMaxTimes;
    private int size;
    private float[,] heightMap;

    private Pos _centerPos;

    private List<Vector3> vertexList = new List<Vector3>();
    public List<Vector3> VertexList
    {
        get { return vertexList; }
    }

    private List<int> triangleList = new List<int>();
    public List<int> TriangleList
    {
        get { return triangleList; }
    }

    private List<Vector2> uvList = new List<Vector2>();
    public List<Vector2> UVList
    {
        get { return uvList; }
    }

    private Texture2D terrainMap;
    public Texture2D TerrainMap
    {
        get { return terrainMap; }
    }

    public void Init(float length, float width, int seed, int initialDitherRange, float ditherDeclineRatio, int fractalMaxTimes)
    {
        if(length <= 0 || width <= 0)
        {
            return;
        }

        _initialDitherRange = initialDitherRange;
        _ditherDeclineRatio = ditherDeclineRatio;

        this.length = length;
        this.width = width;
        this.fractalMaxTimes = fractalMaxTimes;
        UnityEngine.Random.seed = seed;


        size = (int)Mathf.Pow(2, fractalMaxTimes + 1) + 1;

        int centerPosX = Random.Range(size / 3, size * 2 / 3);
        int centerPosY = Random.Range(size / 3, size * 2 / 3);
        _centerPos = new Pos(centerPosX, centerPosY);

        Debug.Log("size:" + size);
        heightMap = new float[size, size];
    }

    public void Generate()
    {
        Pos bl = new Pos(0, 0);
        Pos br = new Pos(size - 1, 0);
        Pos tr = new Pos(size - 1, size - 1);
        Pos tl = new Pos(0, size - 1);
        Pos center = new Pos((size - 1) / 2, (size - 1) / 2);

        heightMap[bl.Col, bl.Row] = RandomOffset(bl, _initialDitherRange);
        heightMap[br.Col, br.Row] = RandomOffset(br, _initialDitherRange);
        heightMap[tr.Col, tr.Row] = RandomOffset(tr, _initialDitherRange);
        heightMap[tl.Col, tl.Row] = RandomOffset(tl, _initialDitherRange);
        heightMap[center.Col, center.Row] = RandomOffset(center, _initialDitherRange);

        FractalNonRecursive(bl, br, tr, tl, _initialDitherRange, 0);

        SetTerrainMap();
        Trianglize();
    }

    public void Flatten(float height)
    {
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                float h = heightMap[i, j];
                if (h < height 
                    || i == 0 || j == 0
                    || i == size - 1
                    || j == size - 1)
                    heightMap[i, j] = height;
            }
        }
        
        SetTerrainMap();
        Trianglize();
    }

    private void FractalNonRecursive(Pos bottomLeft, Pos bottomRight, Pos topRight, Pos topLeft, float ditherRange, int fractalTimes)
    {
        List<Pos> squareList = new List<Pos>();
        squareList.Add(bottomLeft);
        squareList.Add(bottomRight);
        squareList.Add(topRight);
        squareList.Add(topLeft);

        var dt = DateTime.Now;
        while(fractalTimes <= fractalMaxTimes)
        {
            List<Pos> newSquareList = new List<Pos>();
            for(int i = 0; i <= squareList.Count - 4; i = i + 4)
            {
                Pos bl = squareList[i];
                Pos br = squareList[i + 1];
                Pos tr = squareList[i + 2];
                Pos tl = squareList[i + 3];

                float blHeight = heightMap[bl.Col, bl.Row];
                float brHeight = heightMap[br.Col, br.Row];
                float trHeight = heightMap[tr.Col, tr.Row];
                float tlHeight = heightMap[tl.Col, tl.Row];

                Pos left = (bl + tl) / 2;
                float leftHeight = (blHeight + tlHeight ) / 2f + RandomOffset(left, ditherRange);
                heightMap[left.Col, left.Row] = leftHeight;

                Pos bottom = (bl + br) / 2;
                float bottomHeight = (blHeight + brHeight) / 2f + RandomOffset(bottom, ditherRange);
                heightMap[bottom.Col, bottom.Row] = bottomHeight;

                Pos right = (tr + br) / 2;
                float rightHeight = (trHeight + brHeight) / 2f + RandomOffset(right, ditherRange);
                heightMap[right.Col, right.Row] = rightHeight;

                Pos top = (tr + tl) / 2;
                float topHeight = (trHeight + tlHeight) / 2f + RandomOffset(top, ditherRange);
                heightMap[top.Col, top.Row] = topHeight;

                Pos center = (bl + tr) / 2;
                float centerHeight = (blHeight + brHeight + trHeight + tlHeight) / 4f + RandomOffset(center, ditherRange);
                heightMap[center.Col, center.Row] = centerHeight;

                newSquareList.Add(bl);
                newSquareList.Add(bottom);
                newSquareList.Add(center);
                newSquareList.Add(left);

                newSquareList.Add(bottom);
                newSquareList.Add(br);
                newSquareList.Add(right);
                newSquareList.Add(center);

                newSquareList.Add(center);
                newSquareList.Add(right);
                newSquareList.Add(tr);
                newSquareList.Add(top);

                newSquareList.Add(left);
                newSquareList.Add(center);
                newSquareList.Add(top);
                newSquareList.Add(tl);
            }

            squareList.Clear();
            squareList.AddRange(newSquareList);

            ditherRange *= _ditherDeclineRatio;
            fractalTimes++;
        }
    }
    
    private void Trianglize()
    {
        vertexList.Clear();
        uvList.Clear();
        triangleList.Clear();
        
        for(int i = 0; i < size; ++i)
        {
            for(int j = 0; j < size; ++j)
            {
                Vector3 vertex = new Vector3(i * length / size, heightMap[i, j], j * width / size);
                vertexList.Add(vertex);
                Vector2 uv = new Vector2(i * 1f / size, j * 1f / size);
                uvList.Add(uv);
            }
        }

        for(int i = 0; i < vertexList.Count - size; ++i)
        {
            if((i + 1) % size == 0)
                continue;
            triangleList.Add(i);
            triangleList.Add(i + 1);
            triangleList.Add(i + size + 1);

            triangleList.Add(i + size + 1);
            triangleList.Add(i + size);
            triangleList.Add(i);
        }
    }

    private const float COEF_1 = 0.2f;
    private const float COEF_2 = 0.6f;
    private const float COEF_3 = 0.8f;

    private void SetTerrainMap()
    {
        terrainMap = new Texture2D(size, size);

        float min = float.MaxValue;
        float max = float.MinValue;
        for(int i = 0; i < size; ++i)
        {
            for(int j = 0; j < size; ++j)
            {
                float height = heightMap[i, j];
                if(height < min)
                    min = height;
                if(height > max)
                    max = height;
            }
        }

        for(int i = 0; i < size; ++i)
        {
            for(int j = 0; j < size; ++j)
            {
                int pixelX = i;
                int pixelY = j;

                float height = heightMap[i, j];
                float t = (height - min) / (max - min);
                if (t < COEF_1)
                {
                    terrainMap.SetPixel(pixelX, pixelY, new Color(1, 0, 0, 0));
                }
                else if (t > COEF_1 && t < COEF_2)
                {
                    terrainMap.SetPixel(pixelX, pixelY, new Color(0, 1, 0, 0));
                }
                else if (t > COEF_2 && t < COEF_3)
                {
                    terrainMap.SetPixel(pixelX, pixelY, new Color(0, 0, 1, 0));
                }
                else
                {
                    terrainMap.SetPixel(pixelX, pixelY, new Color(0, 0, 0, 1));
                }
            }
        }
        terrainMap.Apply();
    }

    private float RandomOffset(Pos pos, float ditherRange)
    {
        return RandomOffsetComplicated(pos, ditherRange);
    }
    
    private float RandomOffsetSimple(Pos pos, float ditherRange)
    {
        return UnityEngine.Random.Range(0, ditherRange);
    }

    private float RandomOffsetComplicated(Pos pos, float ditherRange)
    {
        // float dx = Mathf.Max(pos.Col, _centerPos.Col) - Mathf.Min(pos.Col, _centerPos.Col);
        // float tx = dx / (pos.Col > _centerPos.Col ? size - _centerPos.Col : _centerPos.Col);
        // float dy = Mathf.Max(pos.Row, _centerPos.Row) - Mathf.Min(pos.Row, _centerPos.Row);
        // float ty = dy / (pos.Row > _centerPos.Row ? size - _centerPos.Row : _centerPos.Row);
        // float t = (tx * dx + ty * dy) / (dx + dy);
        
        float distance = Mathf.Sqrt((pos.Col - _centerPos.Col) * (pos.Col - _centerPos.Col) +
                                    (pos.Row - _centerPos.Row) * (pos.Row - _centerPos.Row));
        float t = distance / size * 2;
        t = (t - 1) * (t - 1);
        return UnityEngine.Random.Range(-ditherRange, ditherRange) * t;
    }
}

