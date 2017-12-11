using UnityEngine;

using System;
using System.Collections.Generic;

public class FractalTerrain
{
    private const float INITIAL_DITHER_RANGE = 20f;
    private const float DITHER_DECLINE_RANGE = 0.5f;
    private const int MAX_FRACTAL_TIMES = 10;
    private const float MAX_CLIFF_HEIGHT = 0.2f;

    private float length;
    private float width;
    private int fractalMaxTimes;
    private int size;
    private float[,] heightMap;

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

    private List<Vector3> normalList = new List<Vector3>();
    public List<Vector3> NormalList
    {
        get { return normalList; }
    }

    public void Init(float length, float width, int seed, int fractalMaxTimes = MAX_FRACTAL_TIMES)
    {
        if(length <= 0 || width <= 0)
        {
            return;
        }

        this.length = length;
        this.width = width;
        this.fractalMaxTimes = fractalMaxTimes;
        UnityEngine.Random.InitState(seed);

        size = (int)Mathf.Pow(2, fractalMaxTimes + 1) + 1;
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

        heightMap[bl.Col, bl.Row] = RandomOffset(INITIAL_DITHER_RANGE);
        heightMap[br.Col, br.Row] = RandomOffset(INITIAL_DITHER_RANGE);
        heightMap[tr.Col, tr.Row] = RandomOffset(INITIAL_DITHER_RANGE);
        heightMap[tl.Col, tl.Row] = RandomOffset(INITIAL_DITHER_RANGE);
        heightMap[center.Col, center.Row] = RandomOffset(INITIAL_DITHER_RANGE);

        FractalNonRecursive(bl, br, tr, tl, INITIAL_DITHER_RANGE, 0);

//        FractalRecursive(bl, br, tr, tl, center, INITIAL_DITHER_RANGE, 0);
//        Filter();
        Trianglize();
        SetNormals();
    }

    private void FractalNonRecursive(Pos bottomLeft, Pos bottomRight, Pos topRight, Pos topLeft, float ditherRange, int fractalTimes)
    {
        List<Pos> squareList = new List<Pos>();
        squareList.Add(bottomLeft);
        squareList.Add(bottomRight);
        squareList.Add(topRight);
        squareList.Add(topLeft);

        for(int j = 0; j < squareList.Count; ++j)
        {
            Debug.Log(squareList[j]);
        }

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
                float leftHeight = (blHeight + tlHeight ) / 2f + RandomOffset(ditherRange);
                heightMap[left.Col, left.Row] = leftHeight;

                Pos bottom = (bl + br) / 2;
                float bottomHeight = (blHeight + brHeight) / 2f + RandomOffset(ditherRange);
                heightMap[bottom.Col, bottom.Row] = bottomHeight;

                Pos right = (tr + br) / 2;
                float rightHeight = (trHeight + brHeight) / 2f + RandomOffset(ditherRange);
                heightMap[right.Col, right.Row] = rightHeight;

                Pos top = (tr + tl) / 2;
                float topHeight = (trHeight + tlHeight) / 2f + RandomOffset(ditherRange);
                heightMap[top.Col, top.Row] = topHeight;

                Pos center = (bl + tr) / 2;
                float centerHeight = (blHeight + brHeight + trHeight + tlHeight) / 4f + RandomOffset(ditherRange);
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

            ditherRange *= DITHER_DECLINE_RANGE;
            fractalTimes++;
        }

        Debug.Log((DateTime.Now.Ticks - dt.Ticks));
    }
    private void FractalRecursive(Pos bl, Pos br, Pos tr, Pos tl, Pos center, float ditherRange, int fractalTimes)
    {
        if(fractalTimes > fractalMaxTimes)
        {
            return;
        }

        float blHeight = heightMap[bl.Col, bl.Row];
        float brHeight = heightMap[br.Col, br.Row];
        float trHeight = heightMap[tr.Col, tr.Row];
        float tlHeight = heightMap[tl.Col, tl.Row];
        float centerHeight = heightMap[center.Col, center.Row];

        //Diamond Phase
        Pos left = (bl + tl) / 2;
//        Debug.LogError("left:" + left);
        float leftHeight = (blHeight + tlHeight ) / 2f + RandomOffset(ditherRange);
        heightMap[left.Col, left.Row] = leftHeight;

        Pos bottom = (bl + br) / 2;
//        Debug.LogError("bottom:" + bottom);
        float bottomHeight = (blHeight + brHeight) / 2f + RandomOffset(ditherRange);
        heightMap[bottom.Col, bottom.Row] = bottomHeight;

        Pos right = (tr + br) / 2;
//        Debug.LogError("right:" + right);
        float rightHeight = (trHeight + brHeight) / 2f + RandomOffset(ditherRange);
        heightMap[right.Col, right.Row] = rightHeight;

        Pos top = (tr + tl) / 2;
//        Debug.LogError("top:" + top);
        float topHeight = (trHeight + tlHeight) / 2f + RandomOffset(ditherRange);
        heightMap[top.Col, top.Row] = topHeight;

        //Square Phase
        Pos blCenter = (bl + center) / 2;
//        Debug.LogError("blCenter:" + blCenter);
        float blCenterHeight = (blHeight + centerHeight) / 2f + RandomOffset(ditherRange);
        heightMap[blCenter.Col, blCenter.Row] = blCenterHeight;
            
        Pos brCenter = (br + center) / 2;
//        Debug.LogError("brCenter:" + brCenter);
        float brCenterHeight = (brHeight + centerHeight) / 2f + RandomOffset(ditherRange);
        heightMap[brCenter.Col, brCenter.Row] = brCenterHeight;

        Pos trCenter = (tr + center) / 2;
//        Debug.LogError("trCenter:" + trCenter);
        float trCenterHeight = (trHeight + centerHeight) / 2f + RandomOffset(ditherRange);
        heightMap[trCenter.Col, trCenter.Row] = trCenterHeight;

        Pos tlCenter = (tl + center) / 2;
//        Debug.LogError("tlCenter:" + tlCenter);
        float tlCenterHeight = (tlHeight + centerHeight) / 2f + RandomOffset(ditherRange);
        heightMap[tlCenter.Col, tlCenter.Row] = tlCenterHeight;

        ditherRange *= DITHER_DECLINE_RANGE;
//        Debug.LogError(ditherRange);
        fractalTimes++;

        FractalRecursive(bl, bottom, center, left, blCenter, ditherRange, fractalTimes);
        FractalRecursive(bottom, br, right, center, brCenter, ditherRange, fractalTimes);
        FractalRecursive(center, right, tr, top, trCenter, ditherRange, fractalTimes);
        FractalRecursive(left, center, top, tl, tlCenter, ditherRange, fractalTimes);
    }

    private const int FILTER_AREA_SIZE = 4;
    private const float FILTER_RATIO = 0.8f;

    private void Filter()
    {
        for(int i = 0; i < size - FILTER_AREA_SIZE; i = i + FILTER_AREA_SIZE)
        {
            for(int j = 0; j < size - FILTER_AREA_SIZE; j = j + FILTER_AREA_SIZE)
            {
                float heightAverage = 0f;
                for(int m = i; m < i + FILTER_AREA_SIZE; ++m)
                {
                    for(int n = j; n < j + FILTER_AREA_SIZE; ++n)
                    {
                        heightAverage += heightMap[m, n];
                    }
                }
                heightAverage /= (FILTER_AREA_SIZE * FILTER_AREA_SIZE);
                for(int m = i; m < i + FILTER_AREA_SIZE; ++m)
                {
                    for(int n = j; n < j + FILTER_AREA_SIZE; ++n)
                    {
                        float height = heightMap[m, n];
                        float newHeight = heightAverage + (height - heightAverage) * FILTER_RATIO;
                        heightMap[m, n] = newHeight;
                    }
                }
            }
        }
    }

    private void Trianglize()
    {
        for(int i = 0; i < size; ++i)
        {
            for(int j = 0; j < size; ++j)
            {
                Vector3 vertex = new Vector3(i * length / size, heightMap[i, j], j * width / size);
                vertexList.Add(vertex);
//                Debug.LogError(vertex);
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

    private void SetNormals()
    {
        for(int i = 0; i < size; ++i)
        {
            for(int j = 0; j < size; ++j)
            {
                Vector3 normal = Vector3.up;
                normal += GetNeighborNormalOffset(i, j);
                normalList.Add(normal.normalized);
            }
        }
    }
    private Vector3 GetNeighborNormalOffset(int col, int row)
    {
        Vector3 offset = Vector3.zero;
        float h = 0;
        if(col != 0)
        {
            h = heightMap[col - 1, row] - heightMap[col, row];
            offset -= new Vector3(-1, 0, 0) * h;
        }
        if(row != 0)
        {
            h = heightMap[col, row - 1] - heightMap[col, row];
            offset -= new Vector3(0, 0, -1) * h;
        }
        if(col != size - 1)
        {
            h = heightMap[col + 1, row] - heightMap[col, row];
            offset -= new Vector3(1, 0, 0) * h;
        }
        if(row != size - 1)
        {
            h = heightMap[col, row + 1] - heightMap[col, row];
            offset -= new Vector3(0, 0, 1) * h;
        }
        return offset;
    }

    private float RandomOffset(float ditherRange)
    {
        return UnityEngine.Random.Range(-ditherRange, ditherRange);
    }


}

