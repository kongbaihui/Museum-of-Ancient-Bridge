using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class SceneScreenshotCapture
{
    private static readonly string RequestPath = Path.Combine(Path.GetFullPath("Temp"), "scene_capture_request.txt");

    public static void CaptureSampleScene()
    {
        CaptureScene("Assets/Scenes/SampleScene.unity", "sample_scene", "sample");
    }

    public static void CapturePrefaceScene()
    {
        CaptureScene("Assets/Scenes/SampleScene.unity", "preface_scene", "preface");
    }

    public static void CaptureStartScene()
    {
        CaptureScene("Assets/Scenes/start.unity", "start_scene", "start");
    }

    public static void CaptureGameScene()
    {
        CaptureScene("Assets/Scenes/Game.unity", "game_scene", "game");
    }

#if UNITY_EDITOR
    private static void CaptureScene(string scenePath, string outputPrefix, string captureMode)
    {
        var outputPath = PrepareOutputPath(outputPrefix);
        Directory.CreateDirectory(Path.GetDirectoryName(RequestPath) ?? ".");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
        File.WriteAllText(RequestPath, scenePath + Environment.NewLine + outputPath + Environment.NewLine + captureMode);
        Debug.Log($"[SceneCapture] Request written for {scenePath} -> {outputPath}");

        EditorSceneManager.OpenScene(scenePath);
        EditorApplication.delayCall += () =>
        {
            if (captureMode == "preface")
            {
                FocusPrefaceBoard();
            }

            CaptureCameraView(outputPath);
            ClearRequest();
            Debug.Log("[SceneCapture] Screenshot captured");
            EditorApplication.Exit(0);
        };
    }

    private static void CaptureCameraView(string outputPath)
    {
        var cam = Camera.main != null ? Camera.main : UnityEngine.Object.FindObjectOfType<Camera>();
        if (cam == null)
        {
            Debug.LogWarning("[SceneCapture] No camera found");
            return;
        }

        var width = 1920;
        var height = 1080;
        var rt = new RenderTexture(width, height, 24);
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        var previous = cam.targetTexture;
        var previousActive = RenderTexture.active;

        cam.targetTexture = rt;
        RenderTexture.active = rt;
        cam.Render();
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        File.WriteAllBytes(outputPath, tex.EncodeToPNG());

        cam.targetTexture = previous;
        RenderTexture.active = previousActive;
        UnityEngine.Object.DestroyImmediate(rt);
        UnityEngine.Object.DestroyImmediate(tex);
    }

    private static void FocusPrefaceBoard()
    {
        var cam = Camera.main != null ? Camera.main : UnityEngine.Object.FindObjectOfType<Camera>();
        if (cam == null)
        {
            return;
        }

        cam.transform.position = new Vector3(-20.38f, 6.55f, -22f);
        cam.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), Vector3.up);
    }
#endif

    private static void ClearRequest()
    {
        if (File.Exists(RequestPath))
        {
            File.Delete(RequestPath);
        }
    }

    private static string PrepareOutputPath(string prefix)
    {
        var logDir = Path.GetFullPath("Logs");
        Directory.CreateDirectory(logDir);
        foreach (var file in Directory.GetFiles(logDir, $"{prefix}_*.png"))
        {
            File.Delete(file);
        }

        return Path.Combine(logDir, $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
    }
}
