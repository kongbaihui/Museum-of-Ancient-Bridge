using System;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PersonInfoPanel : MonoBehaviour
{
    private const float PanelWidth = 1500f;
    private const float PanelHeight = 860f;

    private CanvasGroup canvasGroup;
    private Image portraitImage;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI subtitleText;
    private TextMeshProUGUI[] sectionTitles;
    private TextMeshProUGUI[] sectionBodies;
    private Action closeRequested;

    public void Initialize(Action onClose)
    {
        closeRequested = onClose;
        gameObject.SetActive(false);

        try
        {
            Build();
        }
        finally
        {
            Hide();
        }
    }

    public void Show(ExhibitInfo info)
    {
        if (info == null)
        {
            return;
        }

        Build();

        ExhibitUi.SetText(nameText, info.title);
        ExhibitUi.SetText(subtitleText, info.subtitle);
        portraitImage.sprite = info.mainImage;
        portraitImage.enabled = info.mainImage != null;

        string[] titles =
        {
            info.section01Title, info.section02Title, info.section03Title, info.section04Title
        };
        string[] bodies =
        {
            info.section01Body, info.section02Body, info.section03Body, info.section04Body
        };

        for (int i = 0; i < sectionTitles.Length; i++)
        {
            ExhibitUi.SetText(sectionTitles[i], titles[i]);
            ExhibitUi.SetText(sectionBodies[i], bodies[i]);
        }

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        gameObject.SetActive(false);
    }

    private void Build()
    {
        if (canvasGroup != null)
        {
            return;
        }

        var root = gameObject.GetComponent<RectTransform>();
        ExhibitUi.SetStretch(root);

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        var backdrop = ExhibitUi.Image("Backdrop", root, ExhibitUi.SolidSprite(new Color32(20, 12, 7, 180)), new Color32(255, 255, 255, 255), Vector2.zero, Vector2.zero, true);
        backdrop.raycastTarget = true;

        var panel = ExhibitUi.Rect("PersonInfoPanelFrame", root, Vector2.zero, new Vector2(PanelWidth, PanelHeight));
        ExhibitUi.Image("WoodFrameImage", panel, ExhibitUi.SolidSprite(new Color32(78, 43, 20, 255)), Color.white, Vector2.zero, new Vector2(PanelWidth, PanelHeight));
        ExhibitUi.Image("PaperBackgroundImage", panel, ExhibitUi.PaperSprite(), Color.white, Vector2.zero, new Vector2(PanelWidth - 58f, PanelHeight - 58f));
        ExhibitUi.Image("PaperWash", panel, ExhibitUi.SolidSprite(new Color32(255, 246, 213, 122)), Color.white, Vector2.zero, new Vector2(PanelWidth - 120f, PanelHeight - 126f));
        ExhibitUi.Image("GoldTopLine", panel, ExhibitUi.SolidSprite(new Color32(202, 153, 64, 255)), Color.white, new Vector2(0f, 292f), new Vector2(1260f, 4f));
        ExhibitUi.Image("GoldBottomLine", panel, ExhibitUi.SolidSprite(new Color32(202, 153, 64, 255)), Color.white, new Vector2(0f, -358f), new Vector2(1260f, 4f));

        AddCornerPatterns(panel);

        var titlePlaque = ExhibitUi.Rect("TitlePlaque", panel, new Vector2(0f, 344f), new Vector2(650f, 54f));
        ExhibitUi.Image("PlaqueOuter", titlePlaque, ExhibitUi.SolidSprite(new Color32(82, 48, 24, 255)), Color.white, Vector2.zero, new Vector2(650f, 54f));
        ExhibitUi.Image("PlaqueInner", titlePlaque, ExhibitUi.SolidSprite(new Color32(38, 24, 15, 245)), Color.white, Vector2.zero, new Vector2(620f, 34f));
        var museumText = ExhibitUi.Text("Museum_TMP", titlePlaque, "中国古建筑博物馆 · 人物档案", 28, new Color32(232, 186, 93, 255), Vector2.zero, new Vector2(590f, 38f), TextAlignmentOptions.Center);
        museumText.fontStyle = FontStyles.Bold;

        var portraitArea = ExhibitUi.Rect("PortraitArea", panel, new Vector2(-500f, -20f), new Vector2(360f, 620f));
        ExhibitUi.Image("PortraitAreaShadow", portraitArea, ExhibitUi.SolidSprite(new Color32(68, 38, 18, 50)), Color.white, new Vector2(10f, -10f), new Vector2(360f, 620f));
        ExhibitUi.Image("PortraitAreaPaper", portraitArea, ExhibitUi.SolidSprite(new Color32(250, 241, 216, 242)), Color.white, Vector2.zero, new Vector2(360f, 620f));
        ExhibitUi.Image("PortraitAreaTopLine", portraitArea, ExhibitUi.SolidSprite(new Color32(202, 153, 64, 255)), Color.white, new Vector2(0f, 293f), new Vector2(320f, 4f));

        var portraitFrame = ExhibitUi.Rect("PortraitFrame", portraitArea, new Vector2(0f, 120f), new Vector2(292f, 332f));
        ExhibitUi.Image("PortraitFrameImage", portraitFrame, ExhibitUi.SolidSprite(new Color32(107, 63, 31, 255)), Color.white, Vector2.zero, new Vector2(292f, 332f));
        ExhibitUi.Image("PortraitMatImage", portraitFrame, ExhibitUi.PaperSprite(), Color.white, Vector2.zero, new Vector2(260f, 300f));
        portraitImage = ExhibitUi.Image("PortraitImage", portraitFrame, null, Color.white, Vector2.zero, new Vector2(238f, 276f));
        portraitImage.preserveAspect = true;

        nameText = ExhibitUi.Text("Name_TMP", portraitArea, string.Empty, 76, ExhibitUi.DarkBrown, new Vector2(0f, -98f), new Vector2(300f, 92f), TextAlignmentOptions.Center);
        nameText.font = ExhibitUi.TitleFont();
        nameText.fontStyle = FontStyles.Bold;
        subtitleText = ExhibitUi.Text("Subtitle_TMP", portraitArea, string.Empty, 24, ExhibitUi.SoftBrown, new Vector2(0f, -210f), new Vector2(300f, 118f), TextAlignmentOptions.Center);
        subtitleText.lineSpacing = 8f;

        AddSeal(panel, new Vector2(-668f, 300f), "人物\n档案");

        sectionTitles = new TextMeshProUGUI[4];
        sectionBodies = new TextMeshProUGUI[4];
        AddSection(panel, 0, "Section_Biography", "一", new Vector2(-55f, 112f), new Vector2(450f, 240f));
        AddSection(panel, 1, "Section_Achievement", "二", new Vector2(430f, 112f), new Vector2(450f, 240f));
        AddSection(panel, 2, "Section_Contribution", "三", new Vector2(-55f, -178f), new Vector2(450f, 240f));
        AddSection(panel, 3, "Section_Influence", "四", new Vector2(430f, -178f), new Vector2(450f, 240f));

        AddCloseButton(panel);
    }

    private void AddSection(RectTransform parent, int index, string name, string number, Vector2 position, Vector2 size)
    {
        var section = ExhibitUi.Rect(name, parent, position, size);
        ExhibitUi.Image("SectionShadow", section, ExhibitUi.SolidSprite(new Color32(68, 38, 18, 42)), Color.white, new Vector2(8f, -8f), size);
        ExhibitUi.Image("SectionPaper", section, ExhibitUi.SolidSprite(new Color32(250, 241, 216, 240)), Color.white, Vector2.zero, size);
        ExhibitUi.Image("SectionTopLine", section, ExhibitUi.SolidSprite(new Color32(202, 153, 64, 255)), Color.white, new Vector2(0f, size.y / 2f - 7f), new Vector2(size.x - 26f, 4f));
        ExhibitUi.Image("SectionNumberSeal", section, ExhibitUi.SolidSprite(new Color32(139, 37, 28, 235)), Color.white, new Vector2(-size.x / 2f + 46f, size.y / 2f - 48f), new Vector2(52f, 52f));
        var numberText = ExhibitUi.Text("SectionNumber_TMP", section, number, 27, new Color32(255, 238, 216, 255), new Vector2(-size.x / 2f + 46f, size.y / 2f - 48f), new Vector2(44f, 40f), TextAlignmentOptions.Center);
        numberText.fontStyle = FontStyles.Bold;

        sectionTitles[index] = ExhibitUi.Text("SectionTitle_TMP", section, string.Empty, 30, ExhibitUi.SealRed, new Vector2(-size.x / 2f + 116f, size.y / 2f - 48f), new Vector2(size.x - 146f, 42f), TextAlignmentOptions.Left);
        sectionTitles[index].fontStyle = FontStyles.Bold;
        sectionBodies[index] = ExhibitUi.Text("Body_TMP", section, string.Empty, 24, ExhibitUi.DarkBrown, new Vector2(0f, -34f), new Vector2(size.x - 70f, size.y - 106f), TextAlignmentOptions.TopLeft);
        sectionBodies[index].lineSpacing = 7f;
    }

    private void AddCloseButton(RectTransform panel)
    {
        var buttonRoot = ExhibitUi.Rect("CloseButton", panel, new Vector2(628f, 374f), new Vector2(128f, 48f));
        var buttonImage = ExhibitUi.Image("CloseButtonImage", buttonRoot, ExhibitUi.SolidSprite(new Color32(115, 37, 27, 255)), Color.white, Vector2.zero, new Vector2(128f, 48f));
        buttonImage.raycastTarget = true;
        var button = buttonRoot.gameObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(() => closeRequested?.Invoke());
        ExhibitUi.Text("CloseButton_TMP", buttonRoot, "关闭", 25, Color.white, Vector2.zero, new Vector2(116f, 38f), TextAlignmentOptions.Center).fontStyle = FontStyles.Bold;
    }

    private static void AddSeal(RectTransform parent, Vector2 position, string text)
    {
        var seal = ExhibitUi.Rect("RedSealImage", parent, position, new Vector2(76f, 76f));
        ExhibitUi.Image("SealShape", seal, ExhibitUi.SolidSprite(new Color32(145, 32, 24, 220)), Color.white, Vector2.zero, new Vector2(76f, 76f));
        var sealText = ExhibitUi.Text("Seal_TMP", seal, text, 25, new Color32(255, 238, 216, 255), Vector2.zero, new Vector2(58f, 62f), TextAlignmentOptions.Center);
        sealText.fontStyle = FontStyles.Bold;
    }

    private static void AddCornerPatterns(RectTransform parent)
    {
        var color = new Color32(196, 150, 68, 210);
        Vector2[] positions =
        {
            new Vector2(-674f, 374f), new Vector2(674f, 374f), new Vector2(-674f, -374f), new Vector2(674f, -374f)
        };

        foreach (var position in positions)
        {
            var corner = ExhibitUi.Rect("CornerPatternImage", parent, position, new Vector2(86f, 86f));
            ExhibitUi.Image("CornerHorizontal", corner, ExhibitUi.SolidSprite(color), Color.white, new Vector2(0f, 24f), new Vector2(86f, 4f));
            ExhibitUi.Image("CornerVertical", corner, ExhibitUi.SolidSprite(color), Color.white, new Vector2(-24f, 0f), new Vector2(4f, 86f));
            ExhibitUi.Image("CornerAccent", corner, ExhibitUi.SolidSprite(new Color32(145, 32, 24, 150)), Color.white, new Vector2(18f, -18f), new Vector2(24f, 24f));
        }
    }
}

internal static class ExhibitUi
{
    public static readonly Color32 DarkBrown = new Color32(47, 31, 19, 255);
    public static readonly Color32 SoftBrown = new Color32(98, 65, 39, 255);
    public static readonly Color32 SealRed = new Color32(139, 37, 28, 255);

    private static Sprite solidSprite;
    private static Sprite paperSprite;
    private static TMP_FontAsset chineseFont;
    private static TMP_FontAsset titleFont;

    public static void SetStretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

    public static RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
    {
        var gameObject = new GameObject(name, typeof(RectTransform));
        var rect = gameObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;
        return rect;
    }

    public static Image Image(string name, Transform parent, Sprite sprite, Color color, Vector2 position, Vector2 size, bool stretch = false)
    {
        var rect = Rect(name, parent, position, size);
        if (stretch)
        {
            SetStretch(rect);
        }

        var image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    public static TextMeshProUGUI Text(string name, Transform parent, string text, float fontSize, Color color, Vector2 position, Vector2 size, TextAlignmentOptions alignment)
    {
        var rect = Rect(name, parent, position, size);
        var tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.font = ChineseFont();
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.extraPadding = true;
        tmp.parseCtrlCharacters = true;
        tmp.raycastTarget = false;
        SetText(tmp, text);
        return tmp;
    }

    public static void SetText(TextMeshProUGUI tmp, string text)
    {
        if (tmp == null)
        {
            return;
        }

        string value = text ?? string.Empty;
        EnsureCharacters(tmp.font != null ? tmp.font : ChineseFont(), value);
        tmp.text = value;
    }

    private static void EnsureCharacters(TMP_FontAsset fontAsset, string text)
    {
        if (fontAsset == null || string.IsNullOrEmpty(text))
        {
            return;
        }

        if (fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
        {
            fontAsset.TryAddCharacters(text, out _);
        }

        if (fontAsset.fallbackFontAssetTable == null)
        {
            return;
        }

        foreach (var fallback in fontAsset.fallbackFontAssetTable)
        {
            if (fallback != null && fallback.atlasPopulationMode == AtlasPopulationMode.Dynamic)
            {
                fallback.TryAddCharacters(text, out _);
            }
        }
    }

    public static Sprite SolidSprite(Color32 color)
    {
        if (solidSprite != null && color.r == 255 && color.g == 255 && color.b == 255 && color.a == 255)
        {
            return solidSprite;
        }

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = { color, color, color, color };
        texture.SetPixels32(pixels);
        texture.Apply();

        var sprite = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f));
        if (color.r == 255 && color.g == 255 && color.b == 255 && color.a == 255)
        {
            solidSprite = sprite;
        }

        return sprite;
    }

    public static Sprite PaperSprite()
    {
        if (paperSprite != null)
        {
            return paperSprite;
        }

        const int size = 128;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Repeat;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noise = Mathf.PerlinNoise(x * 0.09f, y * 0.09f);
                byte delta = (byte)Mathf.RoundToInt(noise * 22f);
                texture.SetPixel(x, y, new Color32((byte)(228 + delta), (byte)(213 + delta), (byte)(177 + delta), 255));
            }
        }

        texture.Apply();
        paperSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        return paperSprite;
    }

    private static TMP_FontAsset ChineseFont()
    {
        if (chineseFont != null)
        {
            return chineseFont;
        }

        try
        {
            Font font = LoadProjectFont("Assets/Materials/Fonts/DENG.TTF") ??
                        LoadProjectFont("Assets/Materials/Fonts/DENGB.TTF") ??
                        CreateOsFont(new[] { "DengXian", "Microsoft YaHei UI", "Microsoft YaHei", "SimHei", "SimSun", "KaiTi" }, 72);

            if (font != null)
            {
                chineseFont = CreateRuntimeFontAsset(font, 72, "RuntimeChineseTMPFont");
                return chineseFont;
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to create runtime Chinese TMP font. Falling back to TMP default font. {exception.Message}");
        }

        chineseFont = TMP_Settings.defaultFontAsset;
        return chineseFont;
    }

    public static TMP_FontAsset TitleFont()
    {
        if (titleFont != null)
        {
            return titleFont;
        }

        try
        {
            Font font = LoadProjectFont("Assets/Materials/Fonts/STXINWEI.TTF") ??
                        LoadProjectFont("Assets/Materials/Fonts/STKAITI.TTF") ??
                        CreateOsFont(new[] { "STXinwei", "STXingkai", "STKaiti", "KaiTi", "FZShuTi", "Microsoft YaHei", "SimHei" }, 88);

            if (font != null)
            {
                titleFont = CreateRuntimeFontAsset(font, 88, "RuntimeChineseTitleTMPFont");
                AddFallback(titleFont, ChineseFont());
                return titleFont;
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to create runtime title TMP font. Falling back to body font. {exception.Message}");
        }

        titleFont = ChineseFont();
        return titleFont;
    }

    private static TMP_FontAsset CreateRuntimeFontAsset(Font font, int samplingPointSize, string assetName)
    {
        var fontAsset = TMP_FontAsset.CreateFontAsset(
            font,
            samplingPointSize,
            9,
            GlyphRenderMode.SDFAA,
            4096,
            4096,
            AtlasPopulationMode.Dynamic,
            true);

        fontAsset.name = assetName;
        fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
        fontAsset.isMultiAtlasTexturesEnabled = true;
        return fontAsset;
    }

    private static Font LoadProjectFont(string path)
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<Font>(path);
#else
        return null;
#endif
    }

    private static Font CreateOsFont(string[] fontNames, int size)
    {
        foreach (string fontName in fontNames)
        {
            Font font = Font.CreateDynamicFontFromOSFont(fontName, size);
            if (font != null)
            {
                return font;
            }
        }

        return null;
    }

    private static void AddFallback(TMP_FontAsset target, TMP_FontAsset fallback)
    {
        if (target == null || fallback == null || target == fallback)
        {
            return;
        }

        if (target.fallbackFontAssetTable == null)
        {
            target.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset>();
        }

        if (!target.fallbackFontAssetTable.Contains(fallback))
        {
            target.fallbackFontAssetTable.Add(fallback);
        }
    }
}
