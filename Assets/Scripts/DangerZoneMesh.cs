using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class DangerZoneMesh : MonoBehaviour
{
    [SerializeField] private float angle = 45;
    [SerializeField] private int arcPoints = 1;
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 scale = Vector3.one;
        
#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += _OnValidate;
    }

    private void _OnValidate()
    {
        if (this == null) return;

        var verticesList = new List<Vector3> { position, position + Vector3.up };
        for (var a = -angle; a <= angle + 0.01f; a += angle * 2 / (arcPoints + 1))
        {
            var vertex = (Quaternion.Euler(0, a, 0) * Vector3.forward).normalized;
            verticesList.Add(Vector3.Scale(vertex, scale) + position);
            verticesList.Add(Vector3.Scale(vertex + Vector3.up, scale) + position);
        }
        
        var mesh = new Mesh
        {
            name = "Danger Zone",
            vertices = verticesList.ToArray()
        };

        GetComponent<MeshFilter>().sharedMesh = mesh;
        var meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
#endif
}