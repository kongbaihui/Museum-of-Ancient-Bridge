using System;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public class PersonInfoPanel : MonoBehaviour
{
    private const float PanelWidth = 1460f;
    private const float PanelHeight = 840f;

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

        nameText.text = info.title;
        subtitleText.text = info.subtitle;
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
            sectionTitles[i].text = titles[i];
            sectionBodies[i].text = bodies[i];
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
        ExhibitUi.Image("PaperBackgroundImage", panel, ExhibitUi.PaperSprite(), Color.white, Vector2.zero, new Vector2(PanelWidth, PanelHeight));
        ExhibitUi.Image("WoodFrameImage", panel, ExhibitUi.SolidSprite(new Color32(92, 54, 26, 255)), Color.white, Vector2.zero, new Vector2(PanelWidth, PanelHeight));
        ExhibitUi.Image("InnerPaper", panel, ExhibitUi.PaperSprite(), Color.white, Vector2.zero, new Vector2(PanelWidth - 56f, PanelHeight - 56f));
        ExhibitUi.Image("GoldTopLine", panel, ExhibitUi.SolidSprite(new Color32(198, 150, 62, 255)), Color.white, new Vector2(0f, 296f), new Vector2(1280f, 4f));
        ExhibitUi.Image("GoldBottomLine", panel, ExhibitUi.SolidSprite(new Color32(198, 150, 62, 255)), Color.white, new Vector2(0f, -354f), new Vector2(1280f, 4f));

        AddCornerPatterns(panel);

        nameText = ExhibitUi.Text("Name_TMP", panel, string.Empty, 68, ExhibitUi.DarkBrown, new Vector2(-420f, 330f), new Vector2(760f, 84f), TextAlignmentOptions.Left);
        nameText.font = ExhibitUi.TitleFont();
        nameText.fontStyle = FontStyles.Bold;
        subtitleText = ExhibitUi.Text("Subtitle_TMP", panel, string.Empty, 26, ExhibitUi.SoftBrown, new Vector2(-395f, 280f), new Vector2(820f, 42f), TextAlignmentOptions.Left);

        AddSeal(panel, new Vector2(565f, 314f), "馆藏");

        var portraitFrame = ExhibitUi.Rect("PortraitArea", panel, new Vector2(482f, 88f), new Vector2(380f, 455f));
        ExhibitUi.Image("PortraitFrameImage", portraitFrame, ExhibitUi.SolidSprite(new Color32(107, 63, 31, 255)), Color.white, Vector2.zero, new Vector2(380f, 455f));
        ExhibitUi.Image("PortraitMatImage", portraitFrame, ExhibitUi.PaperSprite(), Color.white, Vector2.zero, new Vector2(344f, 419f));
        portraitImage = ExhibitUi.Image("PortraitImage", portraitFrame, null, Color.white, Vector2.zero, new Vector2(320f, 392f));
        portraitImage.preserveAspect = true;

        sectionTitles = new TextMeshProUGUI[4];
        sectionBodies = new TextMeshProUGUI[4];
        AddSection(panel, 0, "Section_Biography", new Vector2(-308f, 162f), new Vector2(780f, 158f));
        AddSection(panel, 1, "Section_Achievement", new Vector2(-308f, -23f), new Vector2(780f, 175f));
        AddSection(panel, 2, "Section_Contribution", new Vector2(-308f, -223f), new Vector2(780f, 185f));
        AddSection(panel, 3, "Section_Influence", new Vector2(482f, -223f), new Vector2(380f, 185f));

        AddCloseButton(panel);
    }

    private void AddSection(RectTransform parent, int index, string name, Vector2 position, Vector2 size)
    {
        var section = ExhibitUi.Rect(name, parent, position, size);
        ExhibitUi.Image("SectionPaper", section, ExhibitUi.SolidSprite(new Color32(248, 239, 214, 232)), Color.white, Vector2.zero, size);
        ExhibitUi.Image("SectionLeftLine", section, ExhibitUi.SolidSprite(new Color32(154, 42, 32, 255)), Color.white, new Vector2(-size.x / 2f + 5f, 0f), new Vector2(5f, size.y - 22f));

        sectionTitles[index] = ExhibitUi.Text("SectionTitle_TMP", section, string.Empty, 27, ExhibitUi.SealRed, new Vector2(18f, size.y / 2f - 30f), new Vector2(size.x - 48f, 34f), TextAlignmentOptions.Left);
        sectionTitles[index].fontStyle = FontStyles.Bold;
        sectionBodies[index] = ExhibitUi.Text("Body_TMP", section, string.Empty, 23, ExhibitUi.DarkBrown, new Vector2(18f, -16f), new Vector2(size.x - 52f, size.y - 68f), TextAlignmentOptions.TopLeft);
        sectionBodies[index].lineSpacing = 8f;
    }

    private void AddCloseButton(RectTransform panel)
    {
        var buttonRoot = ExhibitUi.Rect("CloseButton", panel, new Vector2(625f, 350f), new Vector2(120f, 42f));
        var buttonImage = ExhibitUi.Image("CloseButtonImage", buttonRoot, ExhibitUi.SolidSprite(new Color32(115, 37, 27, 255)), Color.white, Vector2.zero, new Vector2(120f, 42f));
        buttonImage.raycastTarget = true;
        var button = buttonRoot.gameObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(() => closeRequested?.Invoke());
        ExhibitUi.Text("CloseButton_TMP", buttonRoot, "关闭", 24, Color.white, Vector2.zero, new Vector2(112f, 36f), TextAlignmentOptions.Center).fontStyle = FontStyles.Bold;
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
            new Vector2(-650f, 360f), new Vector2(650f, 360f), new Vector2(-650f, -360f), new Vector2(650f, -360f)
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
        tmp.text = text;
        tmp.font = ChineseFont();
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.raycastTarget = false;
        return tmp;
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
            Font font = Font.CreateDynamicFontFromOSFont(
                new[] { "Microsoft YaHei", "SimHei", "SimSun", "KaiTi" },
                72);

            if (font != null)
            {
                chineseFont = TMP_FontAsset.CreateFontAsset(
                    font,
                    72,
                    9,
                    GlyphRenderMode.SDFAA,
                    4096,
                    4096,
                    AtlasPopulationMode.Dynamic);
                chineseFont.name = "RuntimeChineseTMPFont";
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
            Font font = Font.CreateDynamicFontFromOSFont(
                new[] { "STXingkai", "KaiTi", "FZShuTi", "Microsoft YaHei", "SimHei" },
                88);

            if (font != null)
            {
                titleFont = TMP_FontAsset.CreateFontAsset(
                    font,
                    88,
                    9,
                    GlyphRenderMode.SDFAA,
                    4096,
                    4096,
                    AtlasPopulationMode.Dynamic);
                titleFont.name = "RuntimeChineseTitleTMPFont";
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
}
