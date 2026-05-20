using System;
using System.Collections;
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
        Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
        Lightmapping.Cancel();
        EditorSceneManager.OpenScene(scenePath);
        EditorApplication.delayCall += () =>
        {
            Lightmapping.Cancel();
            Debug.Log("[SceneCapture] Entering play mode");
            EditorApplication.isPlaying = true;
        };
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (!File.Exists(RequestPath))
        {
            return;
        }

        Debug.Log("[SceneCapture] Bootstrap runtime driver");
        var go = new GameObject("SceneScreenshotCaptureDriver");
        go.hideFlags = HideFlags.HideAndDontSave;
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.AddComponent<SceneScreenshotCaptureDriver>();
    }

    private static string ReadOutputPath()
    {
        var lines = File.ReadAllLines(RequestPath);
        return lines.Length > 1 ? lines[1] : BuildOutputPath("scene_capture");
    }

    private static string ReadCaptureMode()
    {
        var lines = File.ReadAllLines(RequestPath);
        return lines.Length > 2 ? lines[2] : "sample";
    }

    private static void ClearRequest()
    {
        if (File.Exists(RequestPath))
        {
            File.Delete(RequestPath);
        }
    }

    private static string BuildOutputPath(string prefix)
    {
        var fileName = $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        return Path.Combine(Path.GetFullPath("Logs"), fileName);
    }

    private static string PrepareOutputPath(string prefix)
    {
        var logDir = Path.GetFullPath("Logs");
        Directory.CreateDirectory(logDir);
        foreach (var file in Directory.GetFiles(logDir, $"{prefix}_*.png"))
        {
            File.Delete(file);
        }

        return BuildOutputPath(prefix);
    }

    private sealed class SceneScreenshotCaptureDriver : MonoBehaviour
    {
        private IEnumerator Start()
        {
            var outputPath = ReadOutputPath();
            var captureMode = ReadCaptureMode();
            Debug.Log($"[SceneCapture] Driver started, output: {outputPath}");
            yield return new WaitForSecondsRealtime(2f);
            if (captureMode == "preface")
            {
                FocusPrefaceBoard();
                yield return new WaitForSecondsRealtime(0.5f);
            }
            Debug.Log("[SceneCapture] Capturing screenshot");
            CaptureCameraView(outputPath);

            for (var i = 0; i < 20; i++)
            {
                yield return new WaitForSecondsRealtime(0.25f);
                if (File.Exists(outputPath) && new FileInfo(outputPath).Length > 0)
                {
                    Debug.Log("[SceneCapture] Screenshot file detected");
                    break;
                }
            }

            ClearRequest();
            Debug.Log("[SceneCapture] Exiting editor");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            EditorApplication.Exit(0);
#endif
        }

        private static void CaptureCameraView(string outputPath)
        {
            var cam = Camera.main;
            if (cam == null)
            {
                cam = FindObjectOfType<Camera>();
            }

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

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
            File.WriteAllBytes(outputPath, tex.EncodeToPNG());

            cam.targetTexture = previous;
            RenderTexture.active = previousActive;
            Destroy(rt);
            Destroy(tex);
        }

        private static void FocusPrefaceBoard()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                cam = FindObjectOfType<Camera>();
            }

            if (cam == null)
            {
                return;
            }

            cam.transform.position = new Vector3(-20.38f, 8.75f, -22f);
            cam.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), Vector3.up);
        }
    }
}
