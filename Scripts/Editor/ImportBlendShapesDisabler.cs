using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportBlendShapesDisabler : AssetPostprocessor
{
    private const string NoBlendShapesFolder = "NoBlendShapes";

    private void OnPreprocessModel()
    {
        if(assetPath.Contains("NoBlendShapes"))
        {
            var modelImporter = assetImporter as ModelImporter;

            if(modelImporter.importBlendShapes)
            {
                var assetName = Path.GetFileNameWithoutExtension(assetPath);
                Debug.LogWarning(
                    $"[{assetName}]: Blendshapes are disabled in paths containing [{NoBlendShapesFolder}].");
                modelImporter.importBlendShapes = false;
            }
        }
    }
}