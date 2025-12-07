using Caskev.GridToolkit;
using GridToolkitWorkingProject.Samples.RealTimeShooter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
[CustomEditor(typeof(GridMap))]
public class GridMapEditor : Editor
{
    private SerializedProperty _directionAtlasAsset;
    private GridMap _gridMap;
    private string _buttonLabel = "Generate DirectionAtlas";
    private CancellationTokenSource _cts;
    private string _atlasPath;
    private void OnEnable()
    {
        _atlasPath = Path.Combine(GetSamplePath(),"DirectionAtlas.bytes");
        _directionAtlasAsset = serializedObject.FindProperty("_directionAtlasAsset");
        _gridMap = (GridMap)target;
    }
    public override async void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        if (GUILayout.Button(_buttonLabel))
        {

            _gridMap.RegisterTiles();
            System.Progress<float> progressIndicator = new System.Progress<float>((progress) =>
            {
                _buttonLabel = "Generating atlas...\n" + (progress * 100).ToString("F0") + "%";
            });
            System.Progress<float> progressIndicator2 = new System.Progress<float>((progress) =>
            {
                _buttonLabel = "Serializing atlas...\n" + (progress * 100).ToString("F0") + "%";
            });
            _cts = new CancellationTokenSource();
            DirectionAtlas directionAtlas = null;
            try
            {
#if UNITY_WEBGL
                Debug.Log("Generating atlas with Awaitable for mono thread WEBGL");
                directionAtlas = await Pathfinding.GenerateDirectionAtlasAwaitable(_gridMap.Map, DiagonalsPolicy.NONE, progressIndicator, _cts.Token);
#else
                directionAtlas = await Pathfinding.GenerateDirectionAtlasAsync(_gridMap.Map, DiagonalsPolicy.NONE, progressIndicator, _cts.Token);
#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Atlas generation was cancelled");
                _buttonLabel = "Generate DirectionAtlas";
                return;
            }
            try
            {
                byte[] bytes = await directionAtlas.ToByteArrayAsync(progressIndicator2, _cts.Token);
                File.WriteAllBytes(_atlasPath, bytes);
                AssetDatabase.ImportAsset(_atlasPath, ImportAssetOptions.ForceSynchronousImport); // Pour le rendre visible dans le Project view
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Atlas serialization was cancelled");
                _buttonLabel = "Generate DirectionAtlas";
                return;
            }
            _buttonLabel = "Generate DirectionAtlas";
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(_atlasPath);
            _directionAtlasAsset.objectReferenceValue = textAsset;
            serializedObject.ApplyModifiedProperties();
        }
    }
    public static string GetSamplePath()
    {
        IEnumerable<Sample> samples = Sample.FindByPackage("com.caskev.grid-toolkit", null);

        foreach (var sample in samples)
        {
            if (sample.displayName == "RealTime Shooter")
            {
                return sample.importPath.Substring(sample.importPath.IndexOf("Assets"));
            }
        }
        return null;
    }
}
