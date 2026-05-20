using UnityEngine;
using UnityEditor;

public class SceneMeshValidator
{
    // Vertex budget threshold
    private const int MAX_VERTEX_COUNT = 65000;

    [MenuItem("Tools/Validate Scene Meshes")]
    public static void ValidateSceneMeshes()
    {
        var meshFilters = Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);

        var invalidMeshCount = 0;

        Debug.Log("=== Scene Mesh Validation Started ===");

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh == null)
                continue;

            Mesh mesh = meshFilter.sharedMesh;

            int vertexCount = mesh.vertexCount;

            if (vertexCount > MAX_VERTEX_COUNT)
            {
                invalidMeshCount++;

                Debug.LogWarning(
                    $"[VERTEX LIMIT EXCEEDED] " +
                    $"GameObject: {meshFilter.gameObject.name} | " +
                    $"Mesh: {mesh.name} | " +
                    $"Vertices: {vertexCount}",
                    meshFilter.gameObject
                );
            }
        }

        Debug.Log(
            $"=== Validation Complete | Invalid Meshes: {invalidMeshCount} ===");
    }
}