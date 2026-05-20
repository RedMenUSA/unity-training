using UnityEditor;
using UnityEngine;

public class MeshImportProcessor : AssetPostprocessor
{
    private void OnPreprocessModel()
    {
        var importer = (ModelImporter)assetImporter;

        // -----------------------------
        // Mesh Import Settings
        // -----------------------------
        importer.globalScale = 1.0f;
        importer.meshCompression = ModelImporterMeshCompression.Medium;
        importer.isReadable = false;
        importer.optimizeMeshPolygons = true;
        importer.optimizeMeshVertices = true;
        importer.importBlendShapes = false;
        importer.importVisibility = false;
        importer.importCameras = false;
        importer.importLights = false;
        importer.materialImportMode = ModelImporterMaterialImportMode.None;

        // -----------------------------
        // Animation Settings
        // -----------------------------
        importer.importAnimation = false;

        // -----------------------------
        // Logging
        // -----------------------------
        Debug.Log($"Importing FBX: {assetPath}");
    }

    private void OnPostprocessModel(GameObject importedObject)
    {
        // TODO: We need to report imported assets that dont' follow the naming convention.
        importedObject.name = importedObject.name.Replace(" ", "_");

        // Automatically add MeshColliders
        var meshFilters = importedObject.GetComponentsInChildren<MeshFilter>();

        foreach (var meshFilter in meshFilters)
        {
            var go = meshFilter.gameObject;

            if (go.GetComponent<MeshCollider>() == null)
            {
                var collider = go.AddComponent<MeshCollider>();
                collider.sharedMesh = meshFilter.sharedMesh;
            }
        }

        Debug.Log($"Finished Processing: {importedObject.name}");
    }
}