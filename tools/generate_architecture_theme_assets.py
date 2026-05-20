from pathlib import Path
import math

from PIL import Image, ImageChops, ImageDraw, ImageEnhance, ImageFilter, ImageFont, ImageOps


ROOT = Path(__file__).resolve().parents[1]

FONT_SANS = Path(r"C:\Windows\Fonts\NotoSansSC-VF.ttf")
FONT_SERIF = Path(r"C:\Windows\Fonts\NotoSerifSC-VF.ttf")
FONT_BOLD = Path(r"C:\Windows\Fonts\msyhbd.ttc")


def font(size, bold=False, serif=False):
    path = FONT_BOLD if bold else (FONT_SERIF if serif else FONT_SANS)
    if path.exists():
        return ImageFont.truetype(str(path), size)
    return ImageFont.truetype(r"C:\Windows\Fonts\msyh.ttc", size)


def color_lerp(a, b, t):
    return tuple(int(a[i] + (b[i] - a[i]) * t) for i in range(3))


def background(size, top=(250, 243, 226), bottom=(214, 187, 142)):
    w, h = size
    img = Image.new("RGB", size, top)
    px = img.load()
    for y in range(h):
        t = y / max(1, h - 1)
        c = color_lerp(top, bottom, t)
        for x in range(w):
            px[x, y] = c
    return img


def text_bbox(draw, xy, text, fnt):
    return draw.textbbox(xy, text, font=fnt)


def draw_centered(draw, y, text, fnt, fill, width):
    box = text_bbox(draw, (0, 0), text, fnt)
    x = (width - (box[2] - box[0])) // 2
    draw.text((x, y), text, font=fnt, fill=fill)


def draw_centered_in_box(draw, box, text, fnt, fill):
    x1, y1, x2, y2 = box
    tb = text_bbox(draw, (0, 0), text, fnt)
    tw = tb[2] - tb[0]
    th = tb[3] - tb[1]
    draw.text((x1 + (x2 - x1 - tw) / 2, y1 + (y2 - y1 - th) / 2 - tb[1]), text, font=fnt, fill=fill)


def wrap_cn(draw, text, fnt, max_width):
    lines = []
    for para in text.split("\n"):
        current = ""
        for ch in para:
            trial = current + ch
            if text_bbox(draw, (0, 0), trial, fnt)[2] <= max_width:
                current = trial
            else:
                if current:
                    lines.append(current)
                current = ch
        if current:
            lines.append(current)
    return lines


def draw_wrapped(draw, xy, text, fnt, fill, max_width, line_gap=10):
    x, y = xy
    for line in wrap_cn(draw, text, fnt, max_width):
        draw.text((x, y), line, font=fnt, fill=fill)
        y += int(fnt.size * 1.25) + line_gap
    return y


def draw_roof(draw, box, fill=(105, 36, 28), trim=(201, 155, 77)):
    x1, y1, x2, y2 = box
    w = x2 - x1
    h = y2 - y1
    draw.polygon(
        [(x1, y1 + h * 0.48), (x1 + w * 0.5, y1), (x2, y1 + h * 0.48),
         (x2 - w * 0.08, y1 + h * 0.58), (x1 + w * 0.08, y1 + h * 0.58)],
        fill=fill,
    )
    draw.rectangle((x1 + w * 0.18, y1 + h * 0.58, x2 - w * 0.18, y2), fill=(145, 65, 43))
    for i in range(4):
        px = x1 + w * (0.25 + i * 0.16)
        draw.rectangle((px, y1 + h * 0.58, px + w * 0.04, y2), fill=(86, 54, 40))
    draw.line((x1 + w * 0.08, y1 + h * 0.62, x2 - w * 0.08, y1 + h * 0.62), fill=trim, width=max(2, int(h * 0.04)))


def draw_dougong(draw, box):
    x1, y1, x2, y2 = box
    colors = [(124, 57, 38), (175, 99, 52), (74, 90, 62), (195, 149, 74)]
    layers = 5
    for i in range(layers):
        y = y1 + (y2 - y1) * (0.15 + i * 0.13)
        margin = (x2 - x1) * (0.1 + i * 0.045)
        draw.rectangle((x1 + margin, y, x2 - margin, y + 18), fill=colors[i % len(colors)])
        draw.polygon([(x1 + margin, y + 18), (x1 + margin + 34, y + 46), (x1 + margin + 70, y + 18)], fill=colors[(i + 1) % len(colors)])
        draw.polygon([(x2 - margin, y + 18), (x2 - margin - 34, y + 46), (x2 - margin - 70, y + 18)], fill=colors[(i + 2) % len(colors)])
    draw.rectangle((x1 + (x2 - x1) * 0.45, y1 + (y2 - y1) * 0.75, x1 + (x2 - x1) * 0.55, y2), fill=(88, 54, 37))


def draw_tenon(draw, box):
    x1, y1, x2, y2 = box
    wood = (151, 89, 47)
    dark = (95, 54, 32)
    draw.rounded_rectangle((x1, y1 + 80, x2 - 110, y1 + 160), radius=18, fill=wood, outline=dark, width=4)
    draw.rectangle((x2 - 145, y1 + 102, x2 - 35, y1 + 138), fill=wood, outline=dark, width=4)
    draw.rounded_rectangle((x1 + 85, y2 - 165, x2, y2 - 85), radius=18, fill=(174, 113, 63), outline=dark, width=4)
    draw.rectangle((x1 + 37, y2 - 145, x1 + 145, y2 - 105), fill=(250, 243, 226), outline=dark, width=4)
    draw.line((x1 + 20, y1 + 45, x2 - 20, y2 - 45), fill=(168, 47, 37), width=5)


def draw_roof_types(draw, box):
    x1, y1, x2, y2 = box
    labels = ["庑殿", "歇山", "悬山"]
    for i, label in enumerate(labels):
        y = y1 + i * ((y2 - y1) // 3)
        bx = (x1 + 25, y + 16, x2 - 25, y + 100)
        draw_roof(draw, bx)
        draw.text((x1 + 40, y + 112), label, font=font(28, bold=True), fill=(0, 0, 0))


def draw_platform(draw, box):
    x1, y1, x2, y2 = box
    h = y2 - y1
    base_h = max(12, h * 0.10)
    lower = y2 - base_h
    upper = lower - base_h * 0.72
    roof_bottom = y1 + h * 0.26
    column_top = roof_bottom + h * 0.06
    column_bottom = max(column_top + h * 0.30, upper - h * 0.08)
    column_bottom = min(column_bottom, upper - 4)
    draw.rectangle((x1 + 30, lower, x2 - 30, y2 - base_h * 0.35), fill=(125, 112, 92))
    draw.rectangle((x1 + 65, upper, x2 - 65, lower), fill=(169, 154, 122))
    for i in range(4):
        cx = x1 + 90 + i * ((x2 - x1 - 180) // 3)
        col_w = max(6, (x2 - x1) * 0.045)
        draw.rectangle((cx - col_w, column_top, cx + col_w, column_bottom), fill=(130, 63, 43))
        draw.ellipse((cx - col_w * 1.9, column_bottom - col_w * 0.7, cx + col_w * 1.9, column_bottom + col_w * 0.9), fill=(180, 159, 116), outline=(76, 60, 45), width=2)
    draw_roof(draw, (x1 + 30, y1 + 10, x2 - 30, y1 + h * 0.30))


def draw_bridge(draw, box):
    x1, y1, x2, y2 = box
    arch = (146, 122, 91)
    draw.arc((x1 + 30, y1 + 50, x2 - 30, y2 + 90), 180, 360, fill=arch, width=28)
    draw.line((x1 + 20, y1 + 105, x2 - 20, y1 + 105), fill=arch, width=24)
    for i in range(7):
        x = x1 + 60 + i * ((x2 - x1 - 120) // 6)
        draw.rectangle((x - 5, y1 + 55, x + 5, y1 + 105), fill=(92, 79, 64))
    for i in range(4):
        y = y2 - 52 + i * 11
        draw.arc((x1, y, x2, y + 45), 8, 172, fill=(79, 122, 135), width=3)


def draw_portrait(draw, box, accent):
    x1, y1, x2, y2 = box
    w = x2 - x1
    h = y2 - y1
    cx = (x1 + x2) // 2
    head_r = max(10, min(w, h) // 7)
    head_y = y1 + h * 0.14
    outline_w = max(2, int(min(w, h) * 0.012))
    draw.ellipse((cx - head_r, head_y, cx + head_r, head_y + 2 * head_r), fill=(221, 179, 136), outline=(75, 56, 42), width=outline_w)
    draw.polygon([(cx - head_r * 1.4, head_y + head_r * 0.15), (cx, y1 + h * 0.03), (cx + head_r * 1.4, head_y + head_r * 0.15)], fill=accent)
    body_top = head_y + 2.3 * head_r
    body_bottom = y2 - h * 0.20
    if body_bottom > body_top + 8:
        draw.rounded_rectangle((cx - w * 0.24, body_top, cx + w * 0.24, body_bottom), radius=max(6, int(w * 0.07)), fill=(102, 66, 49), outline=(70, 49, 38), width=outline_w)
        draw.rectangle((cx - w * 0.13, body_top + h * 0.03, cx + w * 0.13, body_bottom - h * 0.03), fill=(224, 205, 166))
    roof_top = y2 - h * 0.19
    if y2 - roof_top > 12:
        draw_roof(draw, (x1 + w * 0.08, roof_top, x2 - w * 0.08, y2 - h * 0.03), fill=accent)


def border(draw, size):
    w, h = size
    draw.rectangle((24, 24, w - 24, h - 24), outline=(109, 78, 48), width=max(3, w // 300))
    draw.rectangle((42, 42, w - 42, h - 42), outline=(198, 153, 82), width=2)


knowledge = [
    {
        "old": "九章算术",
        "title": "斗拱",
        "subtitle": "层层出跳的木构承托体系",
        "tag": "结构智慧",
        "icon": draw_dougong,
        "points": [
            "位于柱头、额枋与屋檐之间，负责承托深远出檐。",
            "通过拱、昂、斗的组合把屋顶荷载逐级传递到柱网。",
            "也是等级与审美的标志，宫殿、寺观常见复杂铺作。",
        ],
    },
    {
        "old": "几何原本",
        "title": "榫卯",
        "subtitle": "不用铁钉也能咬合成架",
        "tag": "连接工法",
        "icon": draw_tenon,
        "points": [
            "榫是凸出的木舌，卯是相应的凹槽，两者互相咬合。",
            "节点既能定位构件，又能保留一定变形余量。",
            "传统木构依靠榫卯、斗拱、梁架共同形成整体稳定。",
        ],
    },
    {
        "old": "周髀算经",
        "title": "屋顶形制",
        "subtitle": "屋面等级与空间性格",
        "tag": "形制制度",
        "icon": draw_roof_types,
        "points": [
            "庑殿顶、歇山顶、悬山顶、硬山顶共同构成传统屋顶谱系。",
            "屋顶形制常与建筑等级、用途和地域气候相关。",
            "飞檐、脊饰与瓦作让建筑轮廓具有强烈识别度。",
        ],
    },
    {
        "old": "孙子算经",
        "title": "台基柱础",
        "subtitle": "把建筑抬离潮湿地面",
        "tag": "营造基础",
        "icon": draw_platform,
        "points": [
            "台基抬高主体建筑，形成礼制空间和防潮基础。",
            "柱础承接木柱，隔绝地气并分散上部荷载。",
            "月台、踏跺、栏杆共同组织进入建筑的仪式感。",
        ],
    },
    {
        "old": "缉古算经",
        "title": "古桥营造",
        "subtitle": "以拱券分散水上跨越压力",
        "tag": "工程遗产",
        "icon": draw_bridge,
        "points": [
            "赵州桥采用敞肩石拱，减轻桥身自重并利于泄洪。",
            "拱券把竖向荷载转化为向两侧传递的推力。",
            "桥梁营造体现了材料、结构和水文环境的综合判断。",
        ],
    },
]


people = [
    {
        "old": "刘徽",
        "abbr": "LH",
        "name": "梁思成",
        "subtitle": "中国古建筑研究的重要奠基者",
        "years": "1901-1972",
        "source": "liang_sicheng.jpg",
        "crop": (0.12, 0.02, 0.88, 0.96),
        "body": "梁思成是中国建筑史学和古建筑保护的重要奠基者。二十世纪三十年代起，他和中国营造学社成员实地测绘山西、河北、河南等地木构建筑，记录斗拱、梁架、屋顶、台基和尺度制度。他重视用实物遗存校正文献，把佛光寺东大殿等建筑纳入清晰的年代谱系，并持续呼吁保护北京城整体格局。展馆中以梁思成代表现代学术方法，让玩家理解古建筑不仅是景观，更是可被测量、比较和保护的历史证据。",
    },
    {
        "old": "张衡",
        "abbr": "ZH",
        "name": "林徽因",
        "subtitle": "中国古建筑田野调查与研究者",
        "years": "1904-1955",
        "source": "lin_huiyin.jpg",
        "crop": (0.10, 0.03, 0.90, 0.88),
        "body": "林徽因是中国古建筑研究和文化保护中的重要人物。她参与营造学社多地田野调查，承担测绘、摄影、资料整理和文字研究工作，关注木构比例、构件名称、装饰纹样与历史环境之间的关系。她参与佛光寺等遗存的发现和研究，也推动公众理解古建筑的审美与历史价值。她的文字兼具学术判断和艺术感受。展馆中以林徽因代表细致的现场观察，说明建筑研究需要图纸、文献、审美判断和保护意识共同支撑。",
    },
    {
        "old": "杨辉",
        "abbr": "YH",
        "name": "吕彦直",
        "subtitle": "近代中国建筑民族形式探索者",
        "years": "1894-1929",
        "source": "lv_yanzhi.jpg",
        "crop": (0.06, 0.02, 0.94, 0.98),
        "body": "吕彦直是近代中国建筑师中探索民族形式的重要代表。他主持或参与中山陵、广州中山纪念堂等纪念性建筑设计，在现代结构和公共建筑功能中吸收中国传统屋顶、台基、轴线、牌坊和大屋顶意象。他善于把纪念性、礼仪感和清晰流线结合起来。他的实践说明传统建筑元素可以转化为新的空间秩序，而不是简单复制旧式外观。展馆中以吕彦直连接古代形制和近代设计，让玩家看到古建语言在现代公共建筑中的延续。",
    },
    {
        "old": "祖冲之",
        "abbr": "ZCZ",
        "name": "刘士英",
        "subtitle": "中国近代建筑教育与实践人物",
        "years": "1893-1973",
        "source": "liu_shiying.jpg",
        "crop": (0.12, 0.03, 0.88, 0.92),
        "body": "刘士英是近代中国建筑教育和建筑实践的重要人物，见证了建筑师职业从传统匠作经验向现代学院训练的转变。他参与早期建筑学科建设，重视制图、结构、材料和工程管理等基础能力，也关注本土建筑传统在现代设计中的表达。他的经历体现了建筑教育从单纯师徒传授走向系统课程、图纸训练和工程协作。展馆中以刘士英代表建筑教育，把人物线索延伸到现代保护、研究与设计人才培养。",
    },
    {
        "old": "秦九昭",
        "abbr": "QJZ",
        "name": "贝聿铭",
        "subtitle": "现代建筑中转译中国空间意象的代表",
        "years": "1917-2019",
        "source": "pei.jpg",
        "crop": (0.10, 0.02, 0.90, 0.98),
        "body": "贝聿铭是国际知名华裔建筑师。虽然他的作品属于现代建筑，但他在苏州博物馆新馆等项目中主动回应江南园林、粉墙黛瓦、院落组织和借景关系，把传统空间意象转化为现代材料、几何秩序和光影体验。他没有直接复刻古建筑外形，而是提炼尺度、庭院、墙面和水院关系。展馆中以贝聿铭作为收束人物，说明中国建筑传统并不只停留在古代遗存，也能通过当代设计继续被理解、改写和传播。",
    },
]


def draw_info_card(path, title, subtitle, tag, points, icon_func, size=(1288, 954)):
    img = background(size)
    draw = ImageDraw.Draw(img)
    w, h = size
    border(draw, size)
    draw.rectangle((70, 70, w - 70, 205), fill=(235, 220, 191))
    draw.text((96, 78), title, font=font(52, bold=True, serif=True), fill=(0, 0, 0))
    draw.text((100, 154), subtitle, font=font(28, bold=True), fill=(0, 0, 0))
    chip = (w - 250, 101, w - 92, 151)
    draw.rounded_rectangle(chip, radius=18, fill=(201, 168, 98))
    draw_centered_in_box(draw, chip, tag, font(25, bold=True), (0, 0, 0))
    icon_func(draw, (92, 225, 472, 710))
    draw.rectangle((532, 225, w - 92, 730), outline=(140, 99, 58), width=3)
    y = 252
    for idx, p in enumerate(points, 1):
        draw.ellipse((560, y + 8, 584, y + 32), fill=(0, 0, 0))
        draw.text((594, y - 2), f"{idx}", font=font(28, bold=True), fill=(0, 0, 0))
        y = draw_wrapped(draw, (645, y), p, font(33), (0, 0, 0), w - 760, 16) + 34
    draw.line((92, 795, w - 92, 795), fill=(0, 0, 0), width=2)
    draw_wrapped(draw, (110, 825), "展馆提示：点击展台可查看本页知识；按 Esc 关闭弹窗继续参观。", font(28), (0, 0, 0), w - 220, 10)
    img.save(ROOT / path)


def draw_poster(path, item, size):
    img = background(size, top=(245, 235, 210), bottom=(204, 174, 126))
    draw = ImageDraw.Draw(img)
    w, h = size
    border(draw, size)
    item["icon"](draw, (w * 0.16, h * 0.17, w * 0.84, h * 0.55))
    draw_centered(draw, int(h * 0.61), item["title"], font(max(32, w // 8), bold=True, serif=True), (0, 0, 0), w)
    draw_centered(draw, int(h * 0.71), item["tag"], font(max(18, w // 20), bold=True), (0, 0, 0), w)
    text = " · ".join([p.split("，")[0].split("。")[0] for p in item["points"][:2]])
    draw_wrapped(draw, (int(w * 0.13), int(h * 0.79)), text, font(max(18, w // 23)), (0, 0, 0), int(w * 0.74), 8)
    img.save(ROOT / path)


def draw_title_strip(path, title, subtitle, size):
    img = background(size, top=(244, 235, 213), bottom=(215, 188, 145))
    draw = ImageDraw.Draw(img)
    w, h = size
    border(draw, size)
    draw_roof(draw, (w * 0.06, h * 0.22, w * 0.32, h * 0.72))
    draw.text((int(w * 0.37), int(h * 0.20)), title, font=font(56, bold=True, serif=True), fill=(0, 0, 0))
    draw.text((int(w * 0.37), int(h * 0.47)), subtitle, font=font(30), fill=(0, 0, 0))
    draw.text((int(w * 0.37), int(h * 0.67)), "中国古代建筑展馆", font=font(24, bold=True), fill=(0, 0, 0))
    img.save(ROOT / path)


def cover_crop(img, box, crop=None):
    img = ImageOps.exif_transpose(img.convert("RGB"))
    if crop:
        w, h = img.size
        x1, y1, x2, y2 = crop
        img = img.crop((int(w * x1), int(h * y1), int(w * x2), int(h * y2)))
    target_w = max(1, int(box[2] - box[0]))
    target_h = max(1, int(box[3] - box[1]))
    return ImageOps.fit(img, (target_w, target_h), method=Image.Resampling.LANCZOS, centering=(0.5, 0.40))


def parchment_portrait(person, size):
    src = ROOT / "tools" / "portrait_sources" / person["source"]
    base = background(size, top=(246, 237, 216), bottom=(218, 194, 153)).convert("RGB")
    if not src.exists():
        draw = ImageDraw.Draw(base)
        draw_portrait(draw, (size[0] * 0.12, size[1] * 0.06, size[0] * 0.88, size[1] * 0.86), (122, 49, 36))
        return base

    crop = cover_crop(Image.open(src), (0, 0, size[0], size[1]), person.get("crop"))
    gray = ImageOps.grayscale(crop)
    gray = ImageOps.autocontrast(gray, cutoff=2)
    gray = ImageEnhance.Contrast(gray).enhance(1.18)
    gray = ImageEnhance.Brightness(gray).enhance(0.96)
    sepia = ImageOps.colorize(gray, black=(76, 56, 41), white=(238, 224, 194))
    sepia = sepia.filter(ImageFilter.SMOOTH_MORE)
    texture = noisy_fill(size, (231, 214, 180), 6)
    portrait = Image.blend(texture, sepia, 0.82)
    return portrait


def paste_portrait(draw, img, person, box):
    x1, y1, x2, y2 = [int(v) for v in box]
    portrait = parchment_portrait(person, (x2 - x1, y2 - y1))
    img.paste(portrait, (x1, y1))
    draw.rectangle((x1, y1, x2, y2), outline=(91, 63, 42), width=max(3, (x2 - x1) // 90))
    draw.rectangle((x1 + 10, y1 + 10, x2 - 10, y2 - 10), outline=(176, 132, 72), width=2)


def draw_person_strip(path, person, size):
    img = background(size, top=(246, 236, 214), bottom=(214, 184, 139))
    draw = ImageDraw.Draw(img)
    w, h = size
    border(draw, size)
    portrait_box = (54, 38, int(w * 0.29), h - 38)
    paste_portrait(draw, img, person, portrait_box)
    draw.text((int(w * 0.34), int(h * 0.18)), person["name"], font=font(max(36, h // 4), bold=True, serif=True), fill=(0, 0, 0))
    draw.text((int(w * 0.34), int(h * 0.46)), person["subtitle"], font=font(max(24, h // 10), bold=True), fill=(0, 0, 0))
    draw.text((int(w * 0.34), int(h * 0.66)), "中国古代建筑研究人物", font=font(max(20, h // 13)), fill=(0, 0, 0))
    img.save(ROOT / path)


def draw_person_card(path, person, size=(1288, 954)):
    img = background(size, top=(249, 240, 222), bottom=(209, 181, 132))
    draw = ImageDraw.Draw(img)
    w, h = size
    border(draw, size)
    draw.rectangle((70, 70, w - 70, 205), fill=(235, 220, 191))
    draw.text((96, 78), person["name"], font=font(52, bold=True, serif=True), fill=(0, 0, 0))
    draw.text((100, 154), person["subtitle"], font=font(28, bold=True), fill=(0, 0, 0))
    paste_portrait(draw, img, person, (120, 230, 465, 720))
    draw.text((145, 745), person["years"], font=font(28, bold=True), fill=(0, 0, 0))
    draw.rectangle((540, 230, w - 92, 720), outline=(140, 99, 58), width=3)
    draw_wrapped(draw, (570, 262), person["body"], font(26), (0, 0, 0), w - 700, 7)
    draw.line((92, 795, w - 92, 795), fill=(0, 0, 0), width=2)
    draw_wrapped(draw, (110, 825), "人物线索：古代建筑的传承，既包括匠师经验，也包括测绘、教育、保护与现代研究。", font(28), (0, 0, 0), w - 220, 8)
    img.save(ROOT / path)


def draw_person_portrait(path, person, size):
    img = background(size, top=(245, 233, 208), bottom=(204, 174, 126))
    draw = ImageDraw.Draw(img)
    w, h = size
    border(draw, size)
    if w > h * 1.8:
        draw_person_strip(path, person, size)
        return
    paste_portrait(draw, img, person, (int(w * 0.12), int(h * 0.06), int(w * 0.88), int(h * 0.74)))
    draw_centered(draw, int(h * 0.78), person["name"], font(max(20, min(w // 5, h // 7)), bold=True, serif=True), (0, 0, 0), w)
    draw_centered(draw, int(h * 0.88), person["years"], font(max(14, min(w // 13, h // 16)), bold=True), (0, 0, 0), w)
    img.save(ROOT / path)


def draw_entry(path, title, subtitle, body, icon_func, size):
    img = background(size, top=(248, 239, 219), bottom=(205, 171, 121))
    draw = ImageDraw.Draw(img)
    w, h = size
    border(draw, size)
    icon_func(draw, (int(w * 0.07), int(h * 0.20), int(w * 0.42), int(h * 0.78)))
    title_size = max(28, min(48, w // 18))
    sub_size = max(18, min(28, w // 36))
    body_size = max(15, min(20, w // 54))
    draw.text((int(w * 0.46), int(h * 0.18)), title, font=font(title_size, bold=True, serif=True), fill=(0, 0, 0))
    draw.text((int(w * 0.46), int(h * 0.30)), subtitle, font=font(sub_size, bold=True), fill=(0, 0, 0))
    draw_wrapped(draw, (int(w * 0.46), int(h * 0.40)), body, font(body_size), (0, 0, 0), int(w * 0.46), 6)
    img.save(ROOT / path)


def draw_start():
    path = ROOT / "Assets/Materials/start.png"
    size = (971, 797)
    img = background(size, top=(248, 240, 224), bottom=(197, 159, 105))
    draw = ImageDraw.Draw(img)
    w, h = size
    border(draw, size)
    draw_roof(draw, (90, 90, w - 90, 285), fill=(110, 42, 32))
    draw_centered(draw, 330, "中国古代建筑博物馆", font(66, bold=True, serif=True), (0, 0, 0), w)
    draw_centered(draw, 418, "斗拱 · 榫卯 · 屋顶 · 台基 · 古桥", font(32, bold=True), (0, 0, 0), w)
    draw_wrapped(draw, (150, 500), "进入虚拟展馆，浏览中国古代建筑的结构、工法、人物与工程遗产。点击展品查看知识卡片，也可以进入专题场景体验古建空间中的方位、日影和机关结构。", font(28), (0, 0, 0), w - 300, 10)
    chip = (w - 132, h - 115, w - 44, h - 27)
    draw.rounded_rectangle(chip, radius=10, fill=(201, 168, 98))
    draw_centered_in_box(draw, chip, "古建", font(25, bold=True, serif=True), (0, 0, 0))
    img.save(path)


def draw_icon(path, size):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    w, h = size
    draw.ellipse((18, 18, w - 18, h - 18), fill=(201, 155, 77, 255), outline=(96, 58, 37, 255), width=6)
    draw_roof(draw, (44, 64, w - 44, h - 72), fill=(116, 43, 34), trim=(255, 229, 169))
    img.save(ROOT / path)


def noisy_fill(size, base, swing=10):
    w, h = size
    img = Image.new("RGB", size, base)
    px = img.load()
    for y in range(h):
        for x in range(w):
            n = int((math.sin(x * 0.09) + math.cos(y * 0.07) + math.sin((x + y) * 0.025)) * swing)
            px[x, y] = tuple(max(0, min(255, c + n)) for c in base)
    return img


def draw_brick_texture(path, size, base, mortar, row_h=54, brick_w=128):
    img = noisy_fill(size, base, 8)
    draw = ImageDraw.Draw(img)
    w, h = size
    for y in range(0, h + row_h, row_h):
        draw.line((0, y, w, y), fill=mortar, width=4)
        offset = (brick_w // 2) if (y // row_h) % 2 else 0
        for x in range(-offset, w + brick_w, brick_w):
            draw.line((x, y, x, y + row_h), fill=mortar, width=3)
    img.save(ROOT / path)


def draw_wood_texture(path, size, base=(126, 72, 42), dark=(76, 45, 30)):
    img = noisy_fill(size, base, 12)
    draw = ImageDraw.Draw(img)
    w, h = size
    plank_h = max(64, h // 6)
    for y in range(0, h, plank_h):
        draw.line((0, y, w, y), fill=dark, width=5)
        for k in range(4):
            yy = y + 15 + k * (plank_h // 5)
            draw.line((0, yy, w, yy + int(math.sin(k + y) * 5)), fill=(154, 96, 54), width=2)
    for x in range(35, w, 115):
        draw.arc((x - 28, 80, x + 52, 190), 15, 330, fill=(95, 55, 34), width=2)
    img.save(ROOT / path)


def draw_tile_roof_texture(path, size, base=(118, 48, 38), line=(88, 34, 28)):
    img = noisy_fill(size, base, 7)
    draw = ImageDraw.Draw(img)
    w, h = size
    tile_w = 64
    tile_h = 44
    for y in range(-tile_h, h + tile_h, tile_h):
        offset = tile_w // 2 if (y // tile_h) % 2 else 0
        for x in range(-offset, w + tile_w, tile_w):
            draw.arc((x, y, x + tile_w, y + tile_h * 2), 180, 360, fill=line, width=4)
            draw.line((x, y + tile_h, x + tile_w, y + tile_h), fill=(151, 82, 47), width=2)
    img.save(ROOT / path)


def draw_stone_texture(path, size, base=(166, 153, 125), line=(104, 93, 76)):
    img = noisy_fill(size, base, 14)
    draw = ImageDraw.Draw(img)
    w, h = size
    cell = 128
    for y in range(0, h + cell, cell):
        draw.line((0, y, w, y), fill=line, width=3)
    for x in range(0, w + cell, cell):
        draw.line((x, 0, x, h), fill=line, width=3)
    for x in range(25, w, 90):
        y = (x * 37) % h
        draw.line((x, y, min(w, x + 45), min(h, y + 18)), fill=(130, 118, 96), width=2)
    img.save(ROOT / path)


def draw_lattice_texture(path, size):
    img = noisy_fill(size, (218, 194, 151), 8)
    draw = ImageDraw.Draw(img)
    w, h = size
    wood = (103, 59, 39)
    for x in range(0, w + 1, 64):
        draw.rectangle((x - 3, 0, x + 3, h), fill=wood)
    for y in range(0, h + 1, 64):
        draw.rectangle((0, y - 3, w, y + 3), fill=wood)
    for x in range(-w, w, 64):
        draw.line((x, 0, x + h, h), fill=(151, 91, 52), width=3)
        draw.line((x + h, 0, x, h), fill=(151, 91, 52), width=3)
    img.save(ROOT / path)


def generate_surface_textures():
    draw_wood_texture("Assets/Materials/木板.png", Image.open(ROOT / "Assets/Materials/木板.png").size)
    draw_brick_texture("Assets/Materials/资源 1.png", Image.open(ROOT / "Assets/Materials/资源 1.png").size, (93, 116, 115), (48, 64, 62))
    draw_tile_roof_texture("Assets/Materials/资源 2.png", Image.open(ROOT / "Assets/Materials/资源 2.png").size, (70, 128, 132), (34, 78, 82))
    draw_brick_texture("Assets/Materials/资源 3.png", Image.open(ROOT / "Assets/Materials/资源 3.png").size, (151, 93, 61), (93, 58, 43), 58, 150)
    draw_stone_texture("Assets/Materials/资源 4.png", Image.open(ROOT / "Assets/Materials/资源 4.png").size, (190, 181, 156), (122, 111, 92))
    draw_wood_texture("Assets/Materials/资源 5.png", Image.open(ROOT / "Assets/Materials/资源 5.png").size, (103, 49, 36), (57, 31, 26))
    draw_tile_roof_texture("Assets/Materials/资源 6.png", Image.open(ROOT / "Assets/Materials/资源 6.png").size, (126, 47, 35), (83, 28, 25))
    draw_lattice_texture("Assets/Materials/资源 7.png", Image.open(ROOT / "Assets/Materials/资源 7.png").size)
    draw_stone_texture("Assets/Materials/资源 8.png", Image.open(ROOT / "Assets/Materials/资源 8.png").size, (134, 130, 113), (82, 79, 68))
    draw_brick_texture("Assets/Materials/资源 9.png", Image.open(ROOT / "Assets/Materials/资源 9.png").size, (112, 116, 108), (63, 68, 64), 50, 112)
    draw_brick_texture("Assets/Materials/资源 10.png", Image.open(ROOT / "Assets/Materials/资源 10.png").size, (206, 172, 100), (140, 102, 56), 64, 128)


def main():
    for item in knowledge:
        draw_info_card(f"Assets/Materials/古代数学成就图/UI/{item['old']}.png", item["title"], item["subtitle"], item["tag"], item["points"], item["icon"])
        draw_poster(f"Assets/Materials/古代数学成就图/Materials/{item['old']}.png", item, Image.open(ROOT / f"Assets/Materials/古代数学成就图/Materials/{item['old']}.png").size)
        draw_title_strip(f"Assets/Materials/古代数学成就图/UI/{item['old']}T.png", item["title"], item["subtitle"], (985, 318))

    for person in people:
        draw_person_card(f"Assets/Materials/古代数学成就图/UI/person/{person['old']} 1.png", person)
        for name in (person["old"], person["abbr"]):
            path = ROOT / f"Assets/Materials/古代数学成就图/UI/person/{name}.png"
            draw_person_portrait(str(path.relative_to(ROOT)), person, Image.open(path).size)

    draw_entry(
        "Assets/Materials/Game.png",
        "榫卯机关",
        "旋转构件，连通路径",
        "玩家通过旋转机关让路径对齐，理解传统木构节点中“对位、承托、传力”的关系。",
        draw_tenon,
        (500, 350),
    )
    draw_entry(
        "Assets/Materials/Rigui.png",
        "日影与方位",
        "古建选址中的光照秩序",
        "日照、朝向和阴影会影响院落布局、檐口尺度与空间使用。进入场景后可观察太阳位置和环境变化。",
        draw_roof_types,
        (1390, 886),
    )
    draw_entry(
        "Assets/Materials/赵州桥.png",
        "赵州桥专题",
        "敞肩石拱的工程智慧",
        "以赵州桥为代表的古桥营造，展示了拱券受力、排洪开孔、石材拼砌和交通尺度之间的综合设计。",
        draw_bridge,
        (1288, 954),
    )
    draw_start()
    draw_icon("Assets/Materials/Images/donut.png", (234, 234))

    tiny = Image.new("RGB", (5, 5), (122, 49, 36))
    tiny.save(ROOT / "Assets/Materials/mat/自定义大小 – 1.png")
    generate_surface_textures()


if __name__ == "__main__":
    main()
