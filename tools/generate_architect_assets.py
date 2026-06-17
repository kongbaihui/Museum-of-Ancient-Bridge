from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
from typing import Iterable

from PIL import Image, ImageDraw, ImageFont, ImageOps


ROOT = Path(__file__).resolve().parents[1]
PIC_DIR = ROOT / "pic"
UI_DIR = ROOT / "Assets" / "Materials" / "古代数学成就图" / "UI"
PERSON_UI_DIR = UI_DIR / "person"
MATERIALS_DIR = ROOT / "Assets" / "Materials" / "古代数学成就图" / "Materials"

BG = (239, 224, 190)
BG_ACCENT = (247, 236, 210)
BORDER = (132, 94, 54)
TEXT = (35, 22, 11)
SUBTEXT = (77, 56, 39)


@dataclass(frozen=True)
class Slot:
    name: str
    subtitle_1: str
    subtitle_2: str
    portrait_source: str
    detail_source: str
    person_large: str
    person_banner: str
    person_vertical: str
    detail_large: str
    detail_banner: str
    detail_vertical: str


SLOTS = [
    Slot(
        name="鲁班",
        subtitle_1="春秋时期杰出工匠",
        subtitle_2="中国古代营造祖师",
        portrait_source="鲁班.png",
        detail_source="鲁班详细介绍.png",
        person_large="刘徽 1.png",
        person_banner="LH.png",
        person_vertical="刘徽.png",
        detail_large="九章算术.png",
        detail_banner="九章算术T.png",
        detail_vertical="九章算术.png",
    ),
    Slot(
        name="李春",
        subtitle_1="隋代杰出桥梁工匠",
        subtitle_2="赵州桥设计与建造代表人物",
        portrait_source="李春.png",
        detail_source="李春详细介绍.png",
        person_large="张衡 1.png",
        person_banner="ZH.png",
        person_vertical="张衡.png",
        detail_large="几何原本.png",
        detail_banner="几何原本T.png",
        detail_vertical="几何原本.png",
    ),
    Slot(
        name="宇文恺",
        subtitle_1="隋代杰出建筑设计家",
        subtitle_2="大兴城规划营建代表人物",
        portrait_source="宇文恺.png",
        detail_source="宇文恺详细介绍.png",
        person_large="杨辉 1.png",
        person_banner="YH.png",
        person_vertical="杨辉.png",
        detail_large="周髀算经.png",
        detail_banner="周髀算经T.png",
        detail_vertical="周髀算经.png",
    ),
    Slot(
        name="李诫",
        subtitle_1="北宋杰出建筑学家",
        subtitle_2="《营造法式》主持编修者",
        portrait_source="李诫.png",
        detail_source="李诫详细介绍.png",
        person_large="祖冲之 1.png",
        person_banner="ZCZ.png",
        person_vertical="祖冲之.png",
        detail_large="孙子算经.png",
        detail_banner="孙子算经T.png",
        detail_vertical="孙子算经.png",
    ),
    Slot(
        name="蒯祥",
        subtitle_1="明代杰出宫殿营建大师",
        subtitle_2="紫禁城宫殿设计代表人物",
        portrait_source="蒯祥.png",
        detail_source="蒯祥详细介绍.png",
        person_large="秦九昭 1.png",
        person_banner="QJZ.png",
        person_vertical="秦九昭.png",
        detail_large="缉古算经.png",
        detail_banner="缉古算经T.png",
        detail_vertical="缉古算经.png",
    ),
]


def font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont:
    choices = [
        "msyhbd.ttc" if bold else "msyh.ttc",
        "simhei.ttf" if bold else "simsun.ttc",
        "SourceHanSansSC-Bold.otf" if bold else "SourceHanSansSC-Regular.otf",
    ]
    for filename in choices:
        path = Path("C:/Windows/Fonts") / filename
        if path.exists():
            return ImageFont.truetype(str(path), size)
    return ImageFont.load_default()


def open_rgb(path: Path) -> Image.Image:
    return Image.open(path).convert("RGB")


def create_panel(size: tuple[int, int]) -> Image.Image:
    image = Image.new("RGB", size, BG)
    draw = ImageDraw.Draw(image)
    w, h = size
    draw.rectangle((0, 0, w - 1, h - 1), outline=BORDER, width=max(3, min(w, h) // 70))
    draw.rectangle((18, 18, w - 19, h - 19), outline=(194, 146, 70), width=max(2, min(w, h) // 180))
    draw.rectangle((42, 42, w - 43, h - 43), fill=BG_ACCENT, outline=(210, 167, 92), width=max(2, min(w, h) // 220))
    return image


def paste_contained(canvas: Image.Image, content: Image.Image, box: tuple[int, int, int, int]) -> None:
    x0, y0, x1, y1 = box
    region_w = max(1, x1 - x0)
    region_h = max(1, y1 - y0)
    fitted = ImageOps.contain(content, (region_w, region_h), method=Image.Resampling.LANCZOS)
    left = x0 + (region_w - fitted.width) // 2
    top = y0 + (region_h - fitted.height) // 2
    canvas.paste(fitted, (left, top))


def paste_cover(canvas: Image.Image, content: Image.Image, box: tuple[int, int, int, int]) -> None:
    x0, y0, x1, y1 = box
    fitted = ImageOps.fit(
        content,
        (max(1, x1 - x0), max(1, y1 - y0)),
        method=Image.Resampling.LANCZOS,
        centering=(0.5, 0.35),
    )
    canvas.paste(fitted, (x0, y0))


def draw_centered_text(
    draw: ImageDraw.ImageDraw,
    text: str,
    box: tuple[int, int, int, int],
    text_font: ImageFont.FreeTypeFont,
    fill: tuple[int, int, int],
) -> None:
    bbox = draw.textbbox((0, 0), text, font=text_font)
    x0, y0, x1, y1 = box
    text_w = bbox[2] - bbox[0]
    text_h = bbox[3] - bbox[1]
    x = x0 + (x1 - x0 - text_w) / 2
    y = y0 + (y1 - y0 - text_h) / 2
    draw.text((x, y), text, font=text_font, fill=fill)


def create_landscape_person(slot: Slot) -> Image.Image:
    canvas = create_panel((1288, 954))
    draw = ImageDraw.Draw(canvas)
    portrait = open_rgb(PIC_DIR / slot.portrait_source)

    portrait_box = (94, 220, 448, 740)
    draw.rectangle((85, 210, 458, 750), outline=BORDER, width=6)
    draw.rectangle((96, 221, 447, 739), outline=(204, 155, 78), width=3)
    paste_cover(canvas, portrait, portrait_box)

    draw.rectangle((68, 68, 1218, 206), fill=(235, 222, 195))

    name_font = font(88, bold=True)
    title_font = font(44, bold=True)
    sub_font = font(28, bold=False)

    draw.text((528, 84), slot.name, font=name_font, fill=TEXT)
    draw.text((528, 182), slot.subtitle_1, font=title_font, fill=TEXT)
    draw.text((528, 250), slot.subtitle_2, font=title_font, fill=TEXT)

    draw.line((92, 806, 1196, 806), fill=TEXT, width=4)
    draw.text((88, 840), "中国古代建筑大师", font=font(34, bold=False), fill=SUBTEXT)
    return canvas


def create_landscape_banner(slot: Slot) -> Image.Image:
    canvas = create_panel((985, 318))
    draw = ImageDraw.Draw(canvas)
    portrait = open_rgb(PIC_DIR / slot.portrait_source)

    portrait_box = (56, 54, 254, 264)
    draw.rectangle((46, 44, 264, 274), outline=BORDER, width=5)
    paste_cover(canvas, portrait, portrait_box)

    draw.text((334, 70), slot.name, font=font(72, bold=True), fill=TEXT)
    draw.text((336, 154), slot.subtitle_1, font=font(32, bold=False), fill=SUBTEXT)
    draw.text((336, 214), slot.subtitle_2, font=font(32, bold=True), fill=TEXT)
    return canvas


def create_vertical_person(slot: Slot, size: tuple[int, int]) -> Image.Image:
    canvas = create_panel(size)
    draw = ImageDraw.Draw(canvas)
    portrait = open_rgb(PIC_DIR / slot.portrait_source)
    w, h = size
    portrait_box = (int(w * 0.10), int(h * 0.07), int(w * 0.90), int(h * 0.72))
    draw.rectangle(
        (portrait_box[0] - 10, portrait_box[1] - 10, portrait_box[2] + 10, portrait_box[3] + 10),
        outline=BORDER,
        width=max(3, w // 60),
    )
    paste_cover(canvas, portrait, portrait_box)
    name_area = (int(w * 0.08), int(h * 0.76), int(w * 0.92), int(h * 0.95))
    draw_centered_text(draw, slot.name, name_area, font(max(18, h // 10), bold=True), TEXT)
    return canvas


def create_landscape_detail(slot: Slot) -> Image.Image:
    canvas = create_panel((1288, 954))
    detail = open_rgb(PIC_DIR / slot.detail_source)
    paste_contained(canvas, detail, (86, 34, 1202, 920))
    return canvas


def create_vertical_detail(slot: Slot, size: tuple[int, int]) -> Image.Image:
    canvas = create_panel(size)
    detail = open_rgb(PIC_DIR / slot.detail_source)
    paste_contained(canvas, detail, (18, 18, size[0] - 18, size[1] - 18))
    return canvas


def save(image: Image.Image, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    image.save(path, format="PNG")


def image_size(path: Path) -> tuple[int, int]:
    with open_rgb(path) as image:
        return image.size


def replace_slot(slot: Slot) -> None:
    save(create_landscape_person(slot), PERSON_UI_DIR / slot.person_large)
    save(create_landscape_banner(slot), PERSON_UI_DIR / slot.person_banner)

    vertical_person_path = PERSON_UI_DIR / slot.person_vertical
    save(create_vertical_person(slot, image_size(vertical_person_path)), vertical_person_path)

    save(create_landscape_detail(slot), UI_DIR / slot.detail_large)
    save(create_landscape_banner(slot), UI_DIR / slot.detail_banner)

    vertical_detail_path = MATERIALS_DIR / slot.detail_vertical
    save(create_vertical_detail(slot, image_size(vertical_detail_path)), vertical_detail_path)


def main(slots: Iterable[Slot]) -> None:
    for slot in slots:
        replace_slot(slot)
        print(f"Generated assets for {slot.name}")


if __name__ == "__main__":
    main(SLOTS)
