using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeInfoPanel : MonoBehaviour
{
    private const float PanelWidth = 1460f;
    private const float PanelHeight = 840f;

    private CanvasGroup canvasGroup;
    private Image diagramImage;
    private Image calloutImage;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI subtitleText;
    private TextMeshProUGUI[] sectionTitles;
    private TextMeshProUGUI[] sectionBodies;
    private TextMeshProUGUI[] iconTexts;
    private Action closeRequested;

    public void Initialize(Action onClose)
    {
        closeRequested = onClose;
        Build();
        Hide();
    }

    public void Show(ExhibitInfo info)
    {
        if (info == null)
        {
            return;
        }

        Build();

        titleText.text = info.title;
        subtitleText.text = info.subtitle;
        diagramImage.sprite = info.mainImage;
        diagramImage.enabled = info.mainImage != null;
        calloutImage.sprite = info.detailImage != null ? info.detailImage : info.mainImage;
        calloutImage.enabled = calloutImage.sprite != null;

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

        iconTexts[0].text = "构件";
        iconTexts[1].text = "受力";
        iconTexts[2].text = "营造";

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
        var backdrop = ExhibitUi.Image("Backdrop", root, ExhibitUi.SolidSprite(new Color32(20, 12, 7, 180)), Color.white, Vector2.zero, Vector2.zero, true);
        backdrop.raycastTarget = true;

        var panel = ExhibitUi.Rect("KnowledgeInfoPanelFrame", root, Vector2.zero, new Vector2(PanelWidth, PanelHeight));
        ExhibitUi.Image("WoodFrameImage", panel, ExhibitUi.SolidSprite(new Color32(82, 48, 24, 255)), Color.white, Vector2.zero, new Vector2(PanelWidth, PanelHeight));
        ExhibitUi.Image("PaperBackgroundImage", panel, ExhibitUi.PaperSprite(), Color.white, Vector2.zero, new Vector2(PanelWidth - 56f, PanelHeight - 56f));
        ExhibitUi.Image("InkPatternImage", panel, ExhibitUi.SolidSprite(new Color32(74, 54, 36, 28)), Color.white, new Vector2(-490f, 245f), new Vector2(420f, 120f));
        ExhibitUi.Image("GoldHeaderLine", panel, ExhibitUi.SolidSprite(new Color32(198, 150, 62, 255)), Color.white, new Vector2(0f, 296f), new Vector2(1280f, 4f));
        ExhibitUi.Image("GoldFooterLine", panel, ExhibitUi.SolidSprite(new Color32(198, 150, 62, 255)), Color.white, new Vector2(0f, -354f), new Vector2(1280f, 4f));

        AddCornerDecorations(panel);
        AddHeader(panel);
        AddDiagramArea(panel);
        AddContentArea(panel);
        AddCloseButton(panel);
    }

    private void AddHeader(RectTransform panel)
    {
        titleText = ExhibitUi.Text("KnowledgeTitle_TMP", panel, string.Empty, 60, ExhibitUi.DarkBrown, new Vector2(-400f, 330f), new Vector2(720f, 78f), TextAlignmentOptions.Left);
        titleText.font = ExhibitUi.TitleFont();
        titleText.fontStyle = FontStyles.Bold;
        subtitleText = ExhibitUi.Text("Subtitle_TMP", panel, string.Empty, 25, ExhibitUi.SoftBrown, new Vector2(-348f, 280f), new Vector2(860f, 42f), TextAlignmentOptions.Left);

        var seal = ExhibitUi.Rect("RedSealImage", panel, new Vector2(584f, 314f), new Vector2(76f, 76f));
        ExhibitUi.Image("SealShape", seal, ExhibitUi.SolidSprite(new Color32(145, 32, 24, 220)), Color.white, Vector2.zero, new Vector2(76f, 76f));
        var sealText = ExhibitUi.Text("Seal_TMP", seal, "营造", 25, new Color32(255, 238, 216, 255), Vector2.zero, new Vector2(58f, 62f), TextAlignmentOptions.Center);
        sealText.fontStyle = FontStyles.Bold;
    }

    private void AddDiagramArea(RectTransform panel)
    {
        var diagramArea = ExhibitUi.Rect("DiagramArea", panel, new Vector2(-405f, -38f), new Vector2(555f, 610f));
        ExhibitUi.Image("DiagramPaper", diagramArea, ExhibitUi.SolidSprite(new Color32(248, 239, 214, 232)), Color.white, Vector2.zero, new Vector2(555f, 610f));
        ExhibitUi.Image("DiagramFrame", diagramArea, ExhibitUi.SolidSprite(new Color32(107, 63, 31, 255)), Color.white, new Vector2(0f, 105f), new Vector2(474f, 348f));
        diagramImage = ExhibitUi.Image("MainDiagramImage", diagramArea, null, Color.white, new Vector2(0f, 105f), new Vector2(444f, 318f));
        diagramImage.preserveAspect = true;

        ExhibitUi.Image("CalloutFrame", diagramArea, ExhibitUi.SolidSprite(new Color32(188, 146, 72, 255)), Color.white, new Vector2(-138f, -174f), new Vector2(218f, 145f));
        calloutImage = ExhibitUi.Image("DetailCalloutImage", diagramArea, null, new Color32(255, 255, 255, 230), new Vector2(-138f, -174f), new Vector2(196f, 123f));
        calloutImage.preserveAspect = true;

        var note = ExhibitUi.Text("DiagramNote_TMP", diagramArea, "图像用于结构观察，文字说明由右侧高清文本呈现。", 22, ExhibitUi.SoftBrown, new Vector2(140f, -170f), new Vector2(235f, 120f), TextAlignmentOptions.TopLeft);
        note.lineSpacing = 8f;

        iconTexts = new TextMeshProUGUI[3];
        AddIcon(diagramArea, 0, new Vector2(-170f, -280f));
        AddIcon(diagramArea, 1, new Vector2(0f, -280f));
        AddIcon(diagramArea, 2, new Vector2(170f, -280f));
    }

    private void AddIcon(RectTransform parent, int index, Vector2 position)
    {
        var icon = ExhibitUi.Rect("IconExplanationRow", parent, position, new Vector2(96f, 82f));
        ExhibitUi.Image("IconCircle", icon, ExhibitUi.SolidSprite(new Color32(139, 37, 28, 235)), Color.white, new Vector2(0f, 18f), new Vector2(45f, 45f));
        iconTexts[index] = ExhibitUi.Text("Icon_TMP", icon, string.Empty, 22, ExhibitUi.DarkBrown, new Vector2(0f, -24f), new Vector2(92f, 36f), TextAlignmentOptions.Center);
        iconTexts[index].fontStyle = FontStyles.Bold;
    }

    private void AddContentArea(RectTransform panel)
    {
        sectionTitles = new TextMeshProUGUI[4];
        sectionBodies = new TextMeshProUGUI[4];

        AddSection(panel, 0, "Section_Concept", new Vector2(340f, 185f), new Vector2(680f, 132f));
        AddSection(panel, 1, "Section_Structure", new Vector2(340f, 33f), new Vector2(680f, 132f));
        AddSection(panel, 2, "Section_Function", new Vector2(340f, -119f), new Vector2(680f, 132f));
        AddSection(panel, 3, "Section_CulturalValue", new Vector2(340f, -271f), new Vector2(680f, 132f));
    }

    private void AddSection(RectTransform parent, int index, string name, Vector2 position, Vector2 size)
    {
        var section = ExhibitUi.Rect(name, parent, position, size);
        ExhibitUi.Image("SectionPaper", section, ExhibitUi.SolidSprite(new Color32(248, 239, 214, 232)), Color.white, Vector2.zero, size);
        ExhibitUi.Image("SectionTopLine", section, ExhibitUi.SolidSprite(new Color32(198, 150, 62, 255)), Color.white, new Vector2(0f, size.y / 2f - 6f), new Vector2(size.x - 26f, 3f));

        sectionTitles[index] = ExhibitUi.Text("SectionTitle_TMP", section, string.Empty, 25, ExhibitUi.SealRed, new Vector2(12f, 37f), new Vector2(size.x - 42f, 32f), TextAlignmentOptions.Left);
        sectionTitles[index].fontStyle = FontStyles.Bold;
        sectionBodies[index] = ExhibitUi.Text("Body_TMP", section, string.Empty, 22, ExhibitUi.DarkBrown, new Vector2(12f, -19f), new Vector2(size.x - 42f, 78f), TextAlignmentOptions.TopLeft);
        sectionBodies[index].lineSpacing = 7f;
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

    private static void AddCornerDecorations(RectTransform parent)
    {
        var gold = new Color32(196, 150, 68, 210);
        for (int i = 0; i < 4; i++)
        {
            float x = i % 2 == 0 ? -650f : 650f;
            float y = i < 2 ? 360f : -360f;
            var corner = ExhibitUi.Rect("CornerDecorationImage", parent, new Vector2(x, y), new Vector2(86f, 86f));
            ExhibitUi.Image("CornerHorizontal", corner, ExhibitUi.SolidSprite(gold), Color.white, new Vector2(0f, 24f), new Vector2(86f, 4f));
            ExhibitUi.Image("CornerVertical", corner, ExhibitUi.SolidSprite(gold), Color.white, new Vector2(-24f, 0f), new Vector2(4f, 86f));
        }
    }
}
