using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public static class AssetProbeTools
{
    [MenuItem("Codex/Print Intro Asset Dependencies")]
    public static void PrintIntroAssetDependencies()
    {
        var guid = "1a039cd4b31bc4d47bfdb53a345f61f3";
        var path = AssetDatabase.GUIDToAssetPath(guid);
        Debug.Log($"[AssetProbe] Path: {path}");

        var deps = AssetDatabase.GetDependencies(path, true)
            .OrderBy(x => x)
            .ToArray();

        foreach (var dep in deps)
        {
            Debug.Log($"[AssetProbe] Dep: {dep}");
        }

        var assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (var asset in assets)
        {
            Debug.Log($"[AssetProbe] SubAsset: {asset.name} ({asset.GetType().Name})");
            if (asset is Material material)
            {
                var tex = material.mainTexture;
                var texPath = tex != null ? AssetDatabase.GetAssetPath(tex) : "<none>";
                Debug.Log($"[AssetProbe] Material: {material.name} mainTex={texPath}");
            }
        }
    }

    [MenuItem("Codex/Rebuild Preface Overlay")]
    public static void RebuildPrefaceOverlay()
    {
        var texturePath = "Assets/Materials/preface.png";
        var scenePath = "Assets/Scenes/SampleScene.unity";
        var introGuid = "1a039cd4b31bc4d47bfdb53a345f61f3";

        var introObj = AssetDatabase.GUIDToAssetPath(introGuid);
        Debug.Log($"[Preface] Model path: {introObj}");

        var scene = EditorSceneManager.OpenScene(scenePath);
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        if (texture == null)
        {
            Debug.LogError($"[Preface] Missing texture: {texturePath}");
            return;
        }

        var intro = Object.FindObjectsOfType<GameObject>().FirstOrDefault(x => x.name == "介绍");
        if (intro != null)
        {
            intro.SetActive(false);
            Debug.Log("[Preface] Disabled old intro object");
        }

        var canvasObj = GameObject.Find("PrefaceCanvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("PrefaceCanvas");
        }

        var canvas = canvasObj.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = canvasObj.AddComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 2000;
        canvas.worldCamera = null;

        var canvasRect = canvasObj.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            canvasRect = canvasObj.AddComponent<RectTransform>();
        }
        canvasRect.sizeDelta = new Vector2(texture.width, texture.height);
        canvasObj.transform.position = new Vector3(-20.38f, 6.55f, -6.21f);
        canvasObj.transform.rotation = Quaternion.identity;
        canvasObj.transform.localScale = Vector3.one * 0.0068f;

        if (canvasObj.GetComponent<CanvasScaler>() == null)
        {
            canvasObj.AddComponent<CanvasScaler>();
        }
        if (canvasObj.GetComponent<GraphicRaycaster>() == null)
        {
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        var previousOverlay = canvasObj.transform.Find("PrefaceOverlay");
        if (previousOverlay != null)
        {
            Object.DestroyImmediate(previousOverlay.gameObject);
        }

        if (canvas == null)
        {
            Debug.LogError("[Preface] No Canvas found");
            return;
        }

        foreach (var extra in Object.FindObjectsOfType<Transform>().Where(t => t.name == "PrefaceOverlay" && t.parent != canvasObj.transform).ToArray())
        {
            Object.DestroyImmediate(extra.gameObject);
        }

        var existing = canvasObj.transform.Find("PrefaceOverlay");
        var overlay = existing != null ? existing.gameObject : new GameObject("PrefaceOverlay");
        overlay.transform.SetParent(canvasObj.transform, false);

        var rect = overlay.GetComponent<RectTransform>();
        if (rect == null)
        {
            rect = overlay.AddComponent<RectTransform>();
        }
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var raw = overlay.GetComponent<RawImage>();
        if (raw == null)
        {
            raw = overlay.AddComponent<RawImage>();
        }
        raw.texture = texture;
        raw.color = Color.white;

        if (overlay.GetComponent<CanvasRenderer>() == null)
        {
            overlay.AddComponent<CanvasRenderer>();
        }

        if (overlay.GetComponent<PrefaceOverlayDismiss>() == null)
        {
            overlay.AddComponent<PrefaceOverlayDismiss>();
        }

        overlay.transform.SetAsLastSibling();
        overlay.SetActive(true);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Preface] Preface overlay rebuilt");
    }
}
