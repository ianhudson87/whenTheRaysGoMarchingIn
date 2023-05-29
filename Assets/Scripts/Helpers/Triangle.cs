using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    Vector3[] vertices; // in counter clockwise order

    public Triangle(Vector3[] vertices) {
        if (vertices.Length != 3) {
            throw new System.ArgumentException("vertices is the wrong length");
        }
        this.vertices = vertices;
    }
}
