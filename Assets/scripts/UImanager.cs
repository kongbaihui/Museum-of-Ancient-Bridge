using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class UImanager : MonoBehaviour
{
    public Image image1;
    public Image image2;
    public Image image3;
    public Image image4;
    public Image image5;
    public Image image6;
    public Image image7;
    public Image image8;
    public Image image9;
    public Image image10;
    public Image image11;
    public Image image12;
    public Image image13;

    public Transform Video;
    public Transform Video1;

    private readonly List<Image> panels = new List<Image>();

    private void Awake()
    {
        panels.AddRange(new[]
        {
            image1, image2, image3, image4, image5, image6, image7,
            image8, image9, image10, image11, image12, image13
        });

        HideAllPanels(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            var ray = Camera.main.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out var hit))
            {
                switch (hit.collider.gameObject.name)
                {
                    case "ZTBl1":
                        ShowPanel(image1);
                        break;
                    case "ZTBl2":
                        ShowPanel(image2);
                        break;
                    case "ZTBl3":
                        ShowPanel(image13);
                        break;
                    case "ZK1":
                        ShowPanel(image3);
                        break;
                    case "ZK2":
                        ShowPanel(image4);
                        break;
                    case "ZK3":
                        ShowPanel(image5);
                        break;
                    case "ZK4":
                        ShowPanel(image6);
                        break;
                    case "ZK5":
                        ShowPanel(image7);
                        break;
                    case "ZKCJ1":
                        ShowPanel(image8);
                        break;
                    case "ZKCJ2":
                        ShowPanel(image9);
                        break;
                    case "ZKCJ3":
                        ShowPanel(image10);
                        break;
                    case "ZKCJ4":
                        ShowPanel(image11);
                        break;
                    case "ZKCJ5":
                        ShowPanel(image12);
                        break;
                }

                if (Video != null && hit.collider.name == Video.name)
                {
                    var player = Video.GetComponent<VideoPlayer>();
                    if (player.isPlaying)
                    {
                        player.Stop();
                    }
                    else
                    {
                        player.Play();
                    }
                }

                if (Video1 != null && hit.collider.name == Video1.name)
                {
                    var player = Video1.GetComponent<VideoPlayer>();
                    if (player.isPlaying)
                    {
                        player.Stop();
                    }
                    else
                    {
                        player.Play();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideAllPanels(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void ShowPanel(Image panel)
    {
        if (panel == null)
        {
            return;
        }

        HideAllPanels(true);

        panel.gameObject.SetActive(true);
        var canvasGroup = EnsureCanvasGroup(panel);
        var rectTransform = panel.rectTransform;

        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.one * 0.94f;
        canvasGroup.DOFade(1f, 0.22f);
        rectTransform.DOScale(1f, 0.24f).SetEase(Ease.OutBack);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideAllPanels(bool instant)
    {
        foreach (var panel in panels)
        {
            if (panel == null || !panel.gameObject.activeSelf)
            {
                continue;
            }

            var canvasGroup = EnsureCanvasGroup(panel);
            var rectTransform = panel.rectTransform;

            if (instant)
            {
                canvasGroup.alpha = 0f;
                rectTransform.localScale = Vector3.one;
                panel.gameObject.SetActive(false);
            }
            else
            {
                rectTransform.DOKill();
                canvasGroup.DOKill();
                canvasGroup.DOFade(0f, 0.15f).OnComplete(() => panel.gameObject.SetActive(false));
            }
        }
    }

    private CanvasGroup EnsureCanvasGroup(Image panel)
    {
        var canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        }

        return canvasGroup;
    }
}
