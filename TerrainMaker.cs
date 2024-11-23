using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMaker : MonoBehaviour
{

    [SerializeField] int xSize = 10;
    [SerializeField] int zSize = 10;

    [SerializeField] int xOffset;
    [SerializeField] int zOffset;

    [SerializeField] float noiseScale = 0.03f;
    [SerializeField] float heightMultiplier = 7;

    

    private Mesh mesh;
    Vector3[] vertices;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateTerrain();     
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            GenerateTerrain();
        }
               
    }

    private void GenerateTerrain()
    {
        //Vertices
        vertices = new Vector3[(xSize + 1 ) * (zSize + 1)];

        int i = 0;
        for(int z = 0; z <= zSize; z++)
        {
            for(int x = 0; x <= xSize; x++)
            {
                float yPos = Mathf.PerlinNoise((x + xOffset) * noiseScale, (z + zOffset) * noiseScale) * heightMultiplier;
                vertices[i] = new Vector3(x, yPos, z);
                i++;
            }
        }

        //triangle stuff
        int[] triangles = new int[xSize * zSize * 6];

        int vertex = 0;
        int triangleIndex = 0;

        for(int z = 0; z < zSize; z++)
        {
            for(int x = 0; x < xSize; x++)
            {
                triangles[triangleIndex + 0] = vertex + 0;
                triangles[triangleIndex + 1] = vertex + xSize + 1;
                triangles[triangleIndex + 2] = vertex + 1;

                triangles[triangleIndex + 3] = vertex + 1;
                triangles[triangleIndex + 4] = vertex + xSize + 1;
                triangles[triangleIndex + 5] = vertex + xSize + 2;

                vertex++;
                triangleIndex += 6;
            }
            vertex++;
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = null; // Clear old mesh to avoid issues
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
         if (vertices == null)
            return;
        
        foreach(Vector3 pos in vertices)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }
    }
}
