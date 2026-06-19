using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeInfoPanel : MonoBehaviour
{
    private const float PanelWidth = 1500f;
    private const float PanelHeight = 860f;

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI titleText;
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

        ExhibitUi.SetText(titleText, info.title);
        ExhibitUi.SetText(subtitleText, info.subtitle);

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
        var backdrop = ExhibitUi.Image("Backdrop", root, ExhibitUi.SolidSprite(new Color32(20, 12, 7, 180)), Color.white, Vector2.zero, Vector2.zero, true);
        backdrop.raycastTarget = true;

        var panel = ExhibitUi.Rect("KnowledgeInfoPanelFrame", root, Vector2.zero, new Vector2(PanelWidth, PanelHeight));
        ExhibitUi.Image("WoodFrameImage", panel, ExhibitUi.SolidSprite(new Color32(78, 43, 20, 255)), Color.white, Vector2.zero, new Vector2(PanelWidth, PanelHeight));
        ExhibitUi.Image("PaperBackgroundImage", panel, ExhibitUi.PaperSprite(), Color.white, Vector2.zero, new Vector2(PanelWidth - 58f, PanelHeight - 58f));
        ExhibitUi.Image("PaperWash", panel, ExhibitUi.SolidSprite(new Color32(255, 246, 213, 126)), Color.white, Vector2.zero, new Vector2(PanelWidth - 120f, PanelHeight - 126f));
        ExhibitUi.Image("GoldFooterLine", panel, ExhibitUi.SolidSprite(new Color32(202, 153, 64, 255)), Color.white, new Vector2(0f, -358f), new Vector2(1260f, 4f));

        AddCornerDecorations(panel);
        AddHeader(panel);
        AddContentArea(panel);
        AddCloseButton(panel);
    }

    private void AddHeader(RectTransform panel)
    {
        var museumPlaque = ExhibitUi.Rect("MuseumPlaque", panel, new Vector2(0f, 344f), new Vector2(660f, 54f));
        ExhibitUi.Image("PlaqueOuter", museumPlaque, ExhibitUi.SolidSprite(new Color32(82, 48, 24, 255)), Color.white, Vector2.zero, new Vector2(660f, 54f));
        ExhibitUi.Image("PlaqueInner", museumPlaque, ExhibitUi.SolidSprite(new Color32(38, 24, 15, 245)), Color.white, Vector2.zero, new Vector2(628f, 34f));
        var museumText = ExhibitUi.Text("MuseumPlaque_TMP", museumPlaque, "中国古建筑知识馆 · 文字讲解", 28, new Color32(232, 186, 93, 255), Vector2.zero, new Vector2(596f, 38f), TextAlignmentOptions.Center);
        museumText.fontStyle = FontStyles.Bold;

        titleText = ExhibitUi.Text("KnowledgeTitle_TMP", panel, string.Empty, 82, ExhibitUi.DarkBrown, new Vector2(0f, 263f), new Vector2(930f, 92f), TextAlignmentOptions.Center);
        titleText.font = ExhibitUi.TitleFont();
        titleText.fontStyle = FontStyles.Bold;
        subtitleText = ExhibitUi.Text("Subtitle_TMP", panel, string.Empty, 31, ExhibitUi.SoftBrown, new Vector2(0f, 204f), new Vector2(1040f, 46f), TextAlignmentOptions.Center);

        var seal = ExhibitUi.Rect("RedSealImage", panel, new Vector2(532f, 262f), new Vector2(72f, 108f));
        ExhibitUi.Image("SealShape", seal, ExhibitUi.SolidSprite(new Color32(145, 32, 24, 232)), Color.white, Vector2.zero, new Vector2(70f, 104f));
        var sealText = ExhibitUi.Text("Seal_TMP", seal, "古建\n知识", 25, new Color32(255, 238, 216, 255), Vector2.zero, new Vector2(48f, 84f), TextAlignmentOptions.Center);
        sealText.fontStyle = FontStyles.Bold;
    }

    private void AddContentArea(RectTransform panel)
    {
        sectionTitles = new TextMeshProUGUI[4];
        sectionBodies = new TextMeshProUGUI[4];

        AddSection(panel, 0, "Section_Concept", "壹", new Vector2(-346f, 60f), new Vector2(610f, 250f));
        AddSection(panel, 1, "Section_Structure", "贰", new Vector2(346f, 60f), new Vector2(610f, 250f));
        AddSection(panel, 2, "Section_Function", "叁", new Vector2(-346f, -226f), new Vector2(610f, 250f));
        AddSection(panel, 3, "Section_CulturalValue", "肆", new Vector2(346f, -226f), new Vector2(610f, 250f));
    }

    private void AddSection(RectTransform parent, int index, string name, string number, Vector2 position, Vector2 size)
    {
        var section = ExhibitUi.Rect(name, parent, position, size);
        ExhibitUi.Image("SectionShadow", section, ExhibitUi.SolidSprite(new Color32(68, 38, 18, 42)), Color.white, new Vector2(8f, -8f), size);
        ExhibitUi.Image("SectionPaper", section, ExhibitUi.SolidSprite(new Color32(250, 241, 216, 242)), Color.white, Vector2.zero, size);
        ExhibitUi.Image("SectionTopLine", section, ExhibitUi.SolidSprite(new Color32(202, 153, 64, 255)), Color.white, new Vector2(0f, size.y / 2f - 7f), new Vector2(size.x - 28f, 4f));
        ExhibitUi.Image("SectionNumberSeal", section, ExhibitUi.SolidSprite(new Color32(139, 37, 28, 235)), Color.white, new Vector2(-size.x / 2f + 48f, size.y / 2f - 52f), new Vector2(54f, 54f));
        var numberText = ExhibitUi.Text("SectionNumber_TMP", section, number, 27, new Color32(255, 238, 216, 255), new Vector2(-size.x / 2f + 48f, size.y / 2f - 52f), new Vector2(44f, 40f), TextAlignmentOptions.Center);
        numberText.fontStyle = FontStyles.Bold;

        sectionTitles[index] = ExhibitUi.Text("SectionTitle_TMP", section, string.Empty, 31, ExhibitUi.SealRed, new Vector2(44f, size.y / 2f - 52f), new Vector2(size.x - 142f, 42f), TextAlignmentOptions.Left);
        sectionTitles[index].fontStyle = FontStyles.Bold;
        sectionBodies[index] = ExhibitUi.Text("Body_TMP", section, string.Empty, 26, ExhibitUi.DarkBrown, new Vector2(0f, -28f), new Vector2(size.x - 74f, 148f), TextAlignmentOptions.TopLeft);
        sectionBodies[index].lineSpacing = 8f;
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

    private static void AddCornerDecorations(RectTransform parent)
    {
        var gold = new Color32(196, 150, 68, 210);
        for (int i = 0; i < 4; i++)
        {
            float x = i % 2 == 0 ? -674f : 674f;
            float y = i < 2 ? 374f : -374f;
            var corner = ExhibitUi.Rect("CornerDecorationImage", parent, new Vector2(x, y), new Vector2(86f, 86f));
            ExhibitUi.Image("CornerHorizontal", corner, ExhibitUi.SolidSprite(gold), Color.white, new Vector2(0f, 24f), new Vector2(86f, 4f));
            ExhibitUi.Image("CornerVertical", corner, ExhibitUi.SolidSprite(gold), Color.white, new Vector2(-24f, 0f), new Vector2(4f, 86f));
        }
    }
}
