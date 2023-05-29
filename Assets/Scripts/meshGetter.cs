using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshGetter : MonoBehaviour
{
    List<Triangle> triangles = new List<Triangle>();
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter[] meshFilters = GameObject.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

        foreach (MeshFilter meshFilter in meshFilters) {
            Debug.Log(meshFilter.gameObject.name);
            getTriangles(meshFilter.mesh.vertices, meshFilter.mesh.triangles);
        }
    }

    void getTriangles(Vector3[] vertexArray, int[] triangleArray) {
        if (triangleArray.Length % 3 != 0) {
            throw new System.ArgumentException("triangle array is not a multiple of 3");
        }
        
        for (int i = 0; i < triangleArray.Length / 3; i++) {
            Vector3[] vertices = new Vector3[] { vertexArray[triangleArray[3 * i]], vertexArray[triangleArray[3 * i + 1]], vertexArray[triangleArray[3 * i + 2]] };
            var t = new Triangle(vertices);
            this.triangles.Add(t);
        }
    }
}
