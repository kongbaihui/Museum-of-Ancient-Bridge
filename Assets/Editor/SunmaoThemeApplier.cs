using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class SunmaoThemeApplier
{
    private const string MuseumScenePath = "Assets/Scenes/SampleScene.unity";
    private const string GameScenePath = "Assets/Scenes/Game.unity";
    private const string EntranceImagePath = "Assets/Materials/Game.png";
    private const string BgFolder = "Assets/Table/Interlocked/sprites/level/白盘bg";
    private const string MaterialFolder = "Assets/Materials/SunmaoTheme";
    private static readonly string[] BgNames = { "2.png", "3.png", "4.png", "5.png", "6.png", "7.png", "8.png" };

    [MenuItem("Tools/Sunmao/Apply Exhibit Theme")]
    public static void Apply()
    {
        Directory.CreateDirectory(MaterialFolder);
        Directory.CreateDirectory(BgFolder);

        GenerateEntranceImage();
        GenerateLevelBackgrounds();
        AssetDatabase.Refresh();
        ConfigureTexture(EntranceImagePath);
        foreach (string name in BgNames)
        {
            ConfigureTexture($"{BgFolder}/{name}");
        }

        ApplyMuseumSceneModel();
        ApplyGameSceneBackgrounds();
        AssetDatabase.SaveAssets();
        Debug.Log("Sunmao exhibit theme applied: museum model and game backgrounds updated.");
    }

    private static void GenerateEntranceImage()
    {
        Texture2D tex = CreateTexture(1200, 800, new Color32(35, 28, 22, 255), new Color32(78, 55, 36, 255));
        DrawRect(tex, 70, 70, 1060, 660, new Color32(186, 127, 70, 255));
        DrawRect(tex, 95, 95, 1010, 610, new Color32(42, 32, 24, 255));
        DrawRect(tex, 135, 135, 930, 530, new Color32(221, 174, 108, 255));
        DrawRect(tex, 165, 165, 870, 470, new Color32(235, 202, 145, 255));
        DrawWoodGrain(tex, new RectInt(165, 165, 870, 470), 22);

        DrawBeam(tex, 290, 355, 620, 86, new Color32(128, 72, 36, 255), true);
        DrawBeam(tex, 552, 220, 96, 390, new Color32(160, 92, 46, 255), false);
        DrawRect(tex, 470, 340, 250, 120, new Color32(86, 46, 25, 255));
        DrawRect(tex, 505, 370, 180, 60, new Color32(235, 202, 145, 255));
        DrawLine(tex, 300, 560, 900, 240, new Color32(93, 60, 35, 255), 5);
        DrawLine(tex, 300, 240, 900, 560, new Color32(93, 60, 35, 255), 5);
        SavePng(tex, EntranceImagePath);
    }

    private static void GenerateLevelBackgrounds()
    {
        for (int i = 0; i < BgNames.Length; i++)
        {
            Texture2D tex = CreateTexture(1920, 1080, new Color32(239, 220, 176, 255), new Color32(165, 112, 66, 255));
            DrawRect(tex, 0, 0, 1920, 1080, new Color32(30, 24, 20, 28));
            DrawWoodGrain(tex, new RectInt(0, 0, 1920, 1080), 42 + i * 3);
            DrawRect(tex, 90, 90, 1740, 900, new Color32(247, 232, 196, 210));
            DrawRect(tex, 130, 130, 1660, 820, new Color32(113, 72, 39, 70));

            int cx = 960;
            int cy = 540;
            int offset = i * 18;
            DrawBeam(tex, cx - 470, cy - 40 + offset, 940, 100, new Color32(139, 78, 38, 255), true);
            DrawBeam(tex, cx - 60 - offset / 2, cy - 330, 120, 660, new Color32(170, 101, 51, 255), false);
            DrawRect(tex, cx - 190, cy - 95, 380, 190, new Color32(83, 45, 25, 255));
            DrawRect(tex, cx - 135, cy - 48, 270, 96, new Color32(247, 232, 196, 255));
            DrawLine(tex, 360, 780 - offset, 1560, 300 + offset, new Color32(99, 61, 34, 255), 6);
            DrawLine(tex, 360, 300 + offset, 1560, 780 - offset, new Color32(99, 61, 34, 255), 6);
            DrawRect(tex, 80, 80, 1760, 18, new Color32(91, 52, 28, 255));
            DrawRect(tex, 80, 982, 1760, 18, new Color32(91, 52, 28, 255));
            DrawRect(tex, 80, 80, 18, 920, new Color32(91, 52, 28, 255));
            DrawRect(tex, 1822, 80, 18, 920, new Color32(91, 52, 28, 255));
            SavePng(tex, $"{BgFolder}/{BgNames[i]}");
        }
    }

    private static void ApplyMuseumSceneModel()
    {
        EditorSceneManager.OpenScene(MuseumScenePath);
        GameObject old = GameObject.Find("Sunmao_Exhibit_Model");
        if (old != null)
        {
            Object.DestroyImmediate(old);
        }

        Material darkWood = GetMaterial("Sunmao_DarkWood", new Color(0.45f, 0.22f, 0.09f));
        Material lightWood = GetMaterial("Sunmao_LightWood", new Color(0.78f, 0.46f, 0.18f));
        Material cutFace = GetMaterial("Sunmao_CutFace", new Color(0.95f, 0.76f, 0.42f));

        GameObject anchor = GameObject.Find("ZKCJ4") ?? GameObject.Find("ZhanTai") ?? GameObject.Find("Image1");
        Vector3 basePosition = anchor != null ? anchor.transform.position : new Vector3(15.0f, 2.4f, -28.5f);
        Quaternion baseRotation = Quaternion.Euler(0f, 25f, 0f);

        GameObject root = new GameObject("Sunmao_Exhibit_Model");
        root.transform.position = basePosition + new Vector3(0f, 0.95f, -0.25f);
        root.transform.rotation = baseRotation;
        root.transform.localScale = Vector3.one;

        AddCube(root.transform, "Lower_CrossBeam", new Vector3(0f, 0f, 0f), new Vector3(2.8f, 0.26f, 0.38f), darkWood);
        AddCube(root.transform, "Upper_CrossBeam", new Vector3(0f, 0.52f, 0f), new Vector3(2.8f, 0.26f, 0.38f), lightWood);
        AddCube(root.transform, "Vertical_Tenon", new Vector3(0f, 0.26f, 0f), new Vector3(0.38f, 1.2f, 0.52f), lightWood);
        AddCube(root.transform, "Mortise_Shadow", new Vector3(0f, 0.26f, -0.285f), new Vector3(0.64f, 0.38f, 0.045f), cutFace);
        AddCube(root.transform, "Left_Wedge", new Vector3(-0.48f, 0.26f, 0.3f), new Vector3(0.5f, 0.18f, 0.18f), cutFace);
        AddCube(root.transform, "Right_Wedge", new Vector3(0.48f, 0.26f, 0.3f), new Vector3(0.5f, 0.18f, 0.18f), cutFace);

        GameObject label = new GameObject("Sunmao_Exhibit_Label");
        label.transform.SetParent(root.transform, false);
        label.transform.localPosition = new Vector3(0f, -0.45f, 0.52f);
        label.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
        TextMesh text = label.AddComponent<TextMesh>();
        text.text = "榫卯机关";
        text.fontSize = 48;
        text.characterSize = 0.045f;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = Color.red;

        Image entranceImage = GameObject.Find("Image1")?.GetComponent<Image>();
        Sprite entranceSprite = AssetDatabase.LoadAssetAtPath<Sprite>(EntranceImagePath);
        if (entranceImage != null && entranceSprite != null)
        {
            entranceImage.sprite = entranceSprite;
            entranceImage.preserveAspect = true;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static void ApplyGameSceneBackgrounds()
    {
        EditorSceneManager.OpenScene(GameScenePath);
        gamemanager1 manager = Object.FindObjectOfType<gamemanager1>();
        if (manager != null)
        {
            manager.bg_sprites = new Sprite[BgNames.Length];
            for (int i = 0; i < BgNames.Length; i++)
            {
                manager.bg_sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>($"{BgFolder}/{BgNames[i]}");
            }
            EditorUtility.SetDirty(manager);
        }

        Image bg = GameObject.Find("bg")?.GetComponent<Image>();
        Sprite first = AssetDatabase.LoadAssetAtPath<Sprite>($"{BgFolder}/{BgNames[0]}");
        if (bg != null && first != null)
        {
            bg.sprite = first;
            bg.preserveAspect = false;
            EditorUtility.SetDirty(bg);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static Material GetMaterial(string name, Color color)
    {
        string path = $"{MaterialFolder}/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.color = color;
        mat.SetFloat("_Glossiness", 0.28f);
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void AddCube(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localScale = localScale;
        Renderer renderer = cube.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
    }

    private static void ConfigureTexture(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            return;
        }
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }

    private static Texture2D CreateTexture(int width, int height, Color32 top, Color32 bottom)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            float t = y / (float)(height - 1);
            Color row = Color.Lerp(bottom, top, t);
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, row);
            }
        }
        return tex;
    }

    private static void DrawWoodGrain(Texture2D tex, RectInt area, int stride)
    {
        Color32 line = new Color32(96, 55, 28, 42);
        for (int y = area.yMin; y < area.yMax; y += stride)
        {
            int wobble = (y / Mathf.Max(1, stride)) % 2 == 0 ? 18 : -18;
            DrawLine(tex, area.xMin, y, area.xMax, y + wobble, line, 3);
        }
    }

    private static void DrawBeam(Texture2D tex, int x, int y, int w, int h, Color32 color, bool horizontal)
    {
        DrawRect(tex, x, y, w, h, color);
        Color32 edge = new Color32(70, 37, 18, 255);
        DrawRect(tex, x, y, w, 6, edge);
        DrawRect(tex, x, y + h - 6, w, 6, edge);
        DrawRect(tex, x, y, 6, h, edge);
        DrawRect(tex, x + w - 6, y, 6, h, edge);
        int step = horizontal ? 32 : 26;
        for (int i = 12; i < (horizontal ? w : h); i += step)
        {
            if (horizontal)
            {
                DrawLine(tex, x + i, y + 8, x + i + 40, y + h - 8, new Color32(229, 165, 88, 60), 3);
            }
            else
            {
                DrawLine(tex, x + 8, y + i, x + w - 8, y + i + 36, new Color32(229, 165, 88, 60), 3);
            }
        }
    }

    private static void DrawRect(Texture2D tex, int x, int y, int w, int h, Color32 color)
    {
        int maxX = Mathf.Clamp(x + w, 0, tex.width);
        int maxY = Mathf.Clamp(y + h, 0, tex.height);
        int minX = Mathf.Clamp(x, 0, tex.width);
        int minY = Mathf.Clamp(y, 0, tex.height);
        for (int yy = minY; yy < maxY; yy++)
        {
            for (int xx = minX; xx < maxX; xx++)
            {
                Color dst = tex.GetPixel(xx, yy);
                Color src = color;
                Color blended = Color.Lerp(dst, src, src.a);
                blended.a = 1f;
                tex.SetPixel(xx, yy, blended);
            }
        }
    }

    private static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color32 color, int thickness)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            DrawRect(tex, x0 - thickness / 2, y0 - thickness / 2, thickness, thickness, color);
            if (x0 == x1 && y0 == y1)
            {
                break;
            }
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    private static void SavePng(Texture2D tex, string path)
    {
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }
}
