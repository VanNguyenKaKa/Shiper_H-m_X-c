using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CreateTriangleMesh : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,0,0), // góc vuông
            new Vector3(1,0,0), // cạnh ngang
            new Vector3(0,1,0)  // cạnh dọc
        };

        int[] triangles = new int[]
        {
            0,1,2
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}