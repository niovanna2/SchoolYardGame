using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    //Vector2[] uvs;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;

    public int textureWidth = 1024;
    public int textureHeight = 1024;

    public float noise01Scale = 2f;
    public float noise01Amp = 2f;

    public float noise02Scale = 4f;
    public float noise02Amp = 4f;

    public float noise03Scale = 6f;
    public float noise03Amp = 6f;

    public Gradient gradient;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;       
    }

    private void Update()
    {
        CreateShape();
        UpdateShape();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for(int i = 0, z = 0; z <= zSize; z++)
        {
            for(int x=0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;//GetNoiseSample(x, z);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for(int z = 0; z< zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = vertices[i].y;
                //colors[i] = new Vector2((float)x / xSize, (float)z / zSize);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }

    }

    void UpdateShape()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = colors;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    /*private void OnDrawGizmos()
    {
        if(vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }*/
}
