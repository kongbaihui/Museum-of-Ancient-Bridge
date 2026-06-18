using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class UImanager : MonoBehaviour
{
    public List<ExhibitInfo> exhibits = new List<ExhibitInfo>();

    public Transform Video;
    public Transform Video1;

    private readonly Dictionary<string, ExhibitInfo> exhibitLookup = new Dictionary<string, ExhibitInfo>();
    private PersonInfoPanel personPanel;
    private KnowledgeInfoPanel knowledgePanel;

    private void Awake()
    {
        EnsureDefaultExhibits();
        BuildLookup();
        HideLegacyImagePanels();
        CreateInfoPanels();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            var ray = Camera.main.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out var hit))
            {
                if (exhibitLookup.TryGetValue(hit.collider.gameObject.name, out var info))
                {
                    ShowExhibit(info);
                }

                ToggleVideoIfHit(hit, Video);
                ToggleVideoIfHit(hit, Video1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCurrentPage();
        }
    }

    private void ShowExhibit(ExhibitInfo info)
    {
        if (info == null)
        {
            return;
        }

        personPanel.Hide();
        knowledgePanel.Hide();

        switch (info.exhibitType)
        {
            case ExhibitType.Person:
                personPanel.Show(info);
                break;

            case ExhibitType.Knowledge:
                knowledgePanel.Show(info);
                break;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseCurrentPage()
    {
        if (personPanel != null)
        {
            personPanel.Hide();
        }

        if (knowledgePanel != null)
        {
            knowledgePanel.Hide();
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ToggleVideoIfHit(RaycastHit hit, Transform videoTransform)
    {
        if (videoTransform == null || hit.collider.name != videoTransform.name)
        {
            return;
        }

        var player = videoTransform.GetComponent<VideoPlayer>();
        if (player == null)
        {
            return;
        }

        if (player.isPlaying)
        {
            player.Stop();
        }
        else
        {
            player.Play();
        }
    }

    private void BuildLookup()
    {
        exhibitLookup.Clear();
        foreach (var info in exhibits)
        {
            if (info == null || string.IsNullOrWhiteSpace(info.colliderName))
            {
                continue;
            }

            exhibitLookup[info.colliderName] = info;
        }
    }

    private void CreateInfoPanels()
    {
        var canvasObject = GameObject.Find("ExhibitInfoCanvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("ExhibitInfoCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        }

        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        var canvasRect = canvasObject.GetComponent<RectTransform>();
        ExhibitUi.SetStretch(canvasRect);

        EnsureEventSystem();

        personPanel = CreatePanel<PersonInfoPanel>("PersonInfoPanel", canvasObject.transform);
        personPanel.Initialize(CloseCurrentPage);

        knowledgePanel = CreatePanel<KnowledgeInfoPanel>("KnowledgeInfoPanel", canvasObject.transform);
        knowledgePanel.Initialize(CloseCurrentPage);
    }

    private T CreatePanel<T>(string panelName, Transform parent) where T : Component
    {
        var panelObject = new GameObject(panelName, typeof(RectTransform));
        panelObject.transform.SetParent(parent, false);
        var rect = panelObject.GetComponent<RectTransform>();
        ExhibitUi.SetStretch(rect);
        return panelObject.AddComponent<T>();
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private void HideLegacyImagePanels()
    {
        foreach (var image in FindObjectsOfType<Image>(true))
        {
            string objectName = image.gameObject.name;
            if (objectName.StartsWith("ImageZK1"))
            {
                image.gameObject.SetActive(false);
            }
        }
    }

    private void EnsureDefaultExhibits()
    {
        if (exhibits != null && exhibits.Count > 0)
        {
            return;
        }

        exhibits = new List<ExhibitInfo>
        {
            CreatePerson(
                "ZKCJ1",
                "鲁班",
                "春秋时期 / 工匠祖师 / 木作与营造技艺象征",
                "ExhibitImages/Persons/luban",
                "鲁班，亦称公输般，是中国古代工匠精神的代表人物。传说他精通木作、器械与建筑营造，重视尺度、结构和施工方法。",
                "相传鲁班改良锯、曲尺、墨斗、刨等工具，使木作加工更准确。许多发明传说体现了古代工匠对效率和精度的追求。",
                "鲁班所代表的木构营造经验，与榫卯连接、构件预制、尺度校验密切相关，是理解中国古建筑工艺传统的重要入口。",
                "后世尊鲁班为工匠祖师，他的形象超越个人史实，成为技艺传承、职业伦理和营造智慧的文化象征。"),
            CreatePerson(
                "ZKCJ2",
                "李春",
                "隋代 / 桥梁工匠 / 赵州桥设计建造者",
                "ExhibitImages/Persons/lichun",
                "李春是隋代著名桥梁工匠，因主持设计建造赵州桥而闻名。赵州桥以单孔敞肩石拱结构展现出高超的工程判断。",
                "赵州桥采用大跨度石拱与敞肩小拱组合，既减轻自重，又利于泄洪，体现了结构受力与水文环境的统一。",
                "李春的桥梁实践说明古代营造并非只靠经验堆砌，而是包含比例控制、材料组织和荷载分配等工程思想。",
                "赵州桥长期保存并持续使用，使李春成为中国古代桥梁史的重要代表，也展示了传统工程技术的持久价值。"),
            CreatePerson(
                "ZKCJ3",
                "宇文恺",
                "隋代 / 建筑规划大师 / 大兴城与洛阳城规划",
                "ExhibitImages/Persons/yuwenkai",
                "宇文恺是隋代杰出的建筑家与城市规划者，参与大兴城、东都洛阳等重大工程，善于统筹尺度、轴线和功能分区。",
                "他在都城规划中强调礼制秩序、道路网格和宫城布局，使城市空间服务于政治、交通和生活管理。",
                "宇文恺的工作体现了建筑、城市、制度之间的紧密关系，也展示了大型工程组织与空间规划能力。",
                "隋唐都城格局对后世城市建设产生深远影响，宇文恺代表了中国古代城市营造从单体建筑走向整体规划的高度。"),
            CreatePerson(
                "ZKCJ4",
                "李诫",
                "北宋 / 建筑学家 / 《营造法式》编修者",
                "ExhibitImages/Persons/lijie",
                "李诫是北宋建筑学家，主持编修《营造法式》。该书系统记录建筑制度、构件尺度、材料用量和施工规范。",
                "《营造法式》将斗拱、梁架、彩画、用料等内容整理成制度化文本，是研究宋代建筑技术的核心文献。",
                "李诫的贡献在于把工匠经验转化为可传授、可核算、可管理的规则，使大型营建更具标准化基础。",
                "《营造法式》影响了后世对古建筑的认识和修缮方法，李诫也因此成为中国建筑史上少有的理论型营造人物。"),
            CreatePerson(
                "ZKCJ5",
                "蒯祥",
                "明代 / 宫廷营建匠师 / 紫禁城营造代表人物",
                "ExhibitImages/Persons/kuaixiang",
                "蒯祥是明代著名宫廷营建匠师，活跃于大型皇家建筑工程中，因参与紫禁城等宫殿营造而被后世记述。",
                "他以木构技术、尺度把控和施工组织见长，能够在复杂宫殿建筑中协调构件、装饰和整体空间效果。",
                "蒯祥所代表的明代官式营造重视轴线、等级、屋顶形制和细部装饰，是宫殿建筑制度成熟的重要体现。",
                "他的事迹体现了匠师在国家工程中的技术地位，也使传统木构建筑的精密组织能力被更多人认识。"),

            // ZK slots follow the current wall display order in the scene.
            CreateKnowledge(
                "ZK1",
                "斗拱",
                "承托屋檐、传递荷载并体现等级秩序的核心木构件。",
                "ExhibitImages/Knowledge/dougong",
                "斗拱位于柱头、额枋与屋檐之间，由斗、拱、昂等小构件层层叠合，形成外挑承托体系。",
                "基本构成包括方形斗、弓形拱、斜向昂以及连接枋材。不同组合决定出檐深度、受力路径和装饰效果。",
                "斗拱能够把屋顶荷载逐级传到柱网，同时扩大檐口出挑，保护墙身，也形成强烈的建筑节奏。",
                "斗拱是中国木构建筑最具辨识度的构件之一，其尺度和繁简常与建筑等级、时代风格密切相关。"),
            CreateKnowledge(
                "ZK2",
                "榫卯",
                "以木构件咬合连接形成稳定结构的传统营造方式。",
                "ExhibitImages/Knowledge/sunmao",
                "榫卯通过凸出的榫头与凹入的卯口相互咬合，使木构件在少用或不用金属钉的情况下连接成整体。",
                "常见形式包括直榫、燕尾榫、抱肩榫、穿带榫等。不同榫卯承担定位、抗拉、抗剪和转角连接等任务。",
                "榫卯既便于装配和维修，也能让木结构在湿度变化、震动和荷载作用下保留一定柔性。",
                "榫卯体现了古代匠人对材料性能和结构关系的深刻理解，是中国木作技艺的重要文化符号。"),
            CreateKnowledge(
                "ZK3",
                "台基与柱础",
                "抬升建筑、隔绝潮气并承托柱网的基础系统。",
                "ExhibitImages/Knowledge/taiji_zhuchu",
                "台基是建筑下部的承台，柱础是柱子下方的承托石件。二者共同完成抬升、找平和承重。",
                "台基通常由夯土、砖石包砌和踏跺组成；柱础可呈圆形、方形、莲瓣等形式，承担分散柱底压力的作用。",
                "台基提高建筑标高，减少雨水和潮气侵袭；柱础保护木柱不直接接触地面，延长木构寿命。",
                "台基高度、踏道形式和柱础雕饰常体现建筑等级，也让建筑在空间上更具庄重感和仪式感。"),
            CreateKnowledge(
                "ZK4",
                "古桥营造",
                "结合地形、水文、材料与受力逻辑的传统桥梁工程。",
                "ExhibitImages/Knowledge/guqiao",
                "古桥营造需要综合考虑河道宽度、水流速度、地基条件、通行需求与材料来源，是高度综合的工程活动。",
                "常见结构包括梁桥、拱桥、索桥和廊桥。桥台、桥墩、拱券、栏板与铺面共同组成完整桥梁系统。",
                "桥梁承担交通连接、防洪泄水和空间组织功能。优秀桥梁会在轻量化、稳定性和耐久性之间取得平衡。",
                "古桥往往也是地方景观和公共记忆的一部分，既是工程设施，也是审美、交通与社会生活的交汇点。"),
            CreateKnowledge(
                "ZK5",
                "屋顶形制",
                "由坡面、屋脊、檐口和等级制度共同构成的建筑冠部。",
                "ExhibitImages/Knowledge/wuding",
                "屋顶是中国古建筑最醒目的部分，承担排水、防晒、遮蔽和塑造建筑轮廓的功能。",
                "常见形制包括庑殿顶、歇山顶、悬山顶、硬山顶、攒尖顶等。屋脊、垂脊、戗脊和瓦作共同构成屋面系统。",
                "屋顶通过坡度排水，通过深远出檐保护墙体，并借助梁架和斗拱把荷载传递到柱网。",
                "不同屋顶形制与建筑等级、地域气候和审美传统相关，是识别古建筑身份与时代风格的重要线索。")
        };
    }

    private ExhibitInfo CreatePerson(
        string colliderName,
        string title,
        string subtitle,
        string imagePath,
        string biography,
        string achievement,
        string contribution,
        string influence)
    {
        return new ExhibitInfo
        {
            colliderName = colliderName,
            exhibitType = ExhibitType.Person,
            title = title,
            subtitle = subtitle,
            section01Title = "生平简介",
            section01Body = biography,
            section02Title = "代表成就",
            section02Body = achievement,
            section03Title = "建筑贡献",
            section03Body = contribution,
            section04Title = "历史影响",
            section04Body = influence,
            mainImage = Resources.Load<Sprite>(imagePath)
        };
    }

    private ExhibitInfo CreateKnowledge(
        string colliderName,
        string title,
        string subtitle,
        string imagePath,
        string concept,
        string structure,
        string function,
        string culturalValue)
    {
        var image = Resources.Load<Sprite>(imagePath);
        return new ExhibitInfo
        {
            colliderName = colliderName,
            exhibitType = ExhibitType.Knowledge,
            title = title,
            subtitle = subtitle,
            section01Title = "基本概念",
            section01Body = concept,
            section02Title = "结构组成",
            section02Body = structure,
            section03Title = "主要作用",
            section03Body = function,
            section04Title = "文化价值",
            section04Body = culturalValue,
            mainImage = image,
            detailImage = image
        };
    }
}
