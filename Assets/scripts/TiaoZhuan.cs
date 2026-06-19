using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TiaoZhuan : MonoBehaviour
{
    private const string StartSceneName = "start";
    private const string MuseumSceneName = "SampleScene";
    private const string SundialSceneName = "Standard Demo";
    private const string MechanismSceneName = "Game";
    private const string StartButtonLabel = "\u8FDB\u5165\u4E3B\u5C55\u9986";
    private const float ButtonGapBelowPoster = 78f;

    private IEnumerator Start()
    {
        if (SceneManager.GetActiveScene().name != StartSceneName)
        {
            yield break;
        }

        yield return null;
        ConfigureStartSceneMenu();
    }

    public void Jump()
    {
        LoadScene(MechanismSceneName);
    }

    public void EnterRiGui()
    {
        LoadScene(SundialSceneName);
    }

    public void ExitRiGui()
    {
        LoadScene(MuseumSceneName);
    }

    public void EnterMuseum()
    {
        LoadScene(MuseumSceneName);
    }

    private static void LoadScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not enabled in Build Settings.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void ConfigureStartSceneMenu()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        ConfigureCanvas(canvas);
        Image poster = ConfigurePoster(canvas);
        ConfigureStartButton(canvas, poster);
    }

    private static void ConfigureCanvas(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            return;
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600f, 900f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    private static Image ConfigurePoster(Canvas canvas)
    {
        Image poster = FindLargestNonButtonImage(canvas);
        if (poster == null)
        {
            return null;
        }

        poster.preserveAspect = true;

        RectTransform posterRect = poster.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (posterRect == null || canvasRect == null)
        {
            return poster;
        }

        Canvas.ForceUpdateCanvases();

        float canvasWidth = canvasRect.rect.width > 1f ? canvasRect.rect.width : 1600f;
        float canvasHeight = canvasRect.rect.height > 1f ? canvasRect.rect.height : 900f;
        float aspect = 1180f / 968f;
        if (poster.sprite != null && poster.sprite.rect.height > 0f)
        {
            aspect = poster.sprite.rect.width / poster.sprite.rect.height;
        }

        float targetWidth = canvasWidth * 0.78f;
        float targetHeight = targetWidth / aspect;
        float maxHeight = canvasHeight * 0.72f;
        if (targetHeight > maxHeight)
        {
            targetHeight = maxHeight;
            targetWidth = targetHeight * aspect;
        }

        posterRect.anchorMin = new Vector2(0.5f, 0.5f);
        posterRect.anchorMax = new Vector2(0.5f, 0.5f);
        posterRect.pivot = new Vector2(0.5f, 0.5f);
        posterRect.anchoredPosition = new Vector2(0f, canvasHeight * 0.1f);
        posterRect.sizeDelta = new Vector2(targetWidth, targetHeight);

        return poster;
    }

    private static Image FindLargestNonButtonImage(Canvas canvas)
    {
        Image[] images = canvas.GetComponentsInChildren<Image>(true);
        Image largest = null;
        float largestArea = 0f;

        foreach (Image image in images)
        {
            if (image.GetComponent<Button>() != null)
            {
                continue;
            }

            RectTransform rectTransform = image.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                continue;
            }

            float area = Mathf.Abs(rectTransform.sizeDelta.x * rectTransform.sizeDelta.y);
            if (area > largestArea)
            {
                largestArea = area;
                largest = image;
            }
        }

        return largest;
    }

    private static void ConfigureStartButton(Canvas canvas, Image poster)
    {
        Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
        if (buttons.Length == 0)
        {
            return;
        }

        for (int i = 1; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }

        ConfigureButton(
            buttons[0],
            "StartMenuButton_Museum",
            StartButtonLabel,
            MuseumSceneName,
            GetStartButtonPosition(canvas, poster),
            GetStartButtonSize(canvas));
    }

    private static void ConfigureButton(
        Button button,
        string objectName,
        string label,
        string sceneName,
        Vector2 buttonPosition,
        Vector2 buttonSize)
    {
        if (button == null)
        {
            return;
        }

        button.gameObject.name = objectName;
        button.gameObject.SetActive(true);
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => LoadScene(sceneName));

        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = buttonPosition;
            rectTransform.sizeDelta = buttonSize;
        }

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.62f, 0.50f, 0.34f, 0.95f);
            button.targetGraphic = image;
        }

        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpText != null)
        {
            tmpText.gameObject.SetActive(false);
        }

        ConfigureLegacyButtonText(FindOrCreateLegacyTextObject(button.transform), label);
    }

    private static Vector2 GetStartButtonPosition(Canvas canvas, Image poster)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasHeight = canvasRect != null && canvasRect.rect.height > 1f ? canvasRect.rect.height : 900f;
        float buttonY = -canvasHeight * 0.36f;

        RectTransform posterRect = poster != null ? poster.GetComponent<RectTransform>() : null;
        if (posterRect != null)
        {
            float posterBottom = posterRect.anchoredPosition.y - (posterRect.sizeDelta.y * 0.5f);
            buttonY = posterBottom - ButtonGapBelowPoster;
        }

        float lowestSafeY = (-canvasHeight * 0.5f) + 46f;
        return new Vector2(0f, Mathf.Max(buttonY, lowestSafeY));
    }

    private static Vector2 GetStartButtonSize(Canvas canvas)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect != null && canvasRect.rect.width > 1f ? canvasRect.rect.width : 1600f;
        float canvasHeight = canvasRect != null && canvasRect.rect.height > 1f ? canvasRect.rect.height : 900f;

        float width = Mathf.Clamp(canvasWidth * 0.18f, 240f, 320f);
        float height = Mathf.Clamp(canvasHeight * 0.065f, 54f, 64f);
        return new Vector2(width, height);
    }

    private static GameObject FindOrCreateLegacyTextObject(Transform buttonTransform)
    {
        Transform existing = buttonTransform.Find("LegacyText");
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject labelObject = new GameObject("LegacyText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        labelObject.transform.SetParent(buttonTransform, false);
        return labelObject;
    }

    private static void ConfigureLegacyButtonText(GameObject labelObject, string label)
    {
        Text text = labelObject.GetComponent<Text>();
        if (text == null)
        {
            text = labelObject.AddComponent<Text>();
        }

        text.text = label;
        text.font = GetMenuButtonFont();
        text.fontSize = 28;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.12f, 0.09f, 0.05f, 1f);
        text.raycastTarget = false;

        RectTransform textRect = labelObject.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;
        }
    }

    private static Font GetMenuButtonFont()
    {
        Font font = Font.CreateDynamicFontFromOSFont(
            new[] { "Microsoft YaHei UI", "Microsoft YaHei", "DengXian", "SimHei", "Arial" },
            28);

        return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

}
