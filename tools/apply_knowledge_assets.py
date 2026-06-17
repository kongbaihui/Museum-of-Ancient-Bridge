from pathlib import Path
from shutil import copyfile


ROOT = Path(__file__).resolve().parents[1]
KNOWLEDGE_DIR = ROOT / "pic" / "\u77e5\u8bc6"
UI_DIR = ROOT / "Assets" / "Materials" / "\u53e4\u4ee3\u6570\u5b66\u6210\u5c31\u56fe" / "UI"
PERSON_UI_DIR = UI_DIR / "person"


SURFACE_TARGETS = [
    ("\u6597\u62f1.png", "\u4e5d\u7ae0\u7b97\u672fT.png"),
    ("\u69ab\u536f.png", "\u51e0\u4f55\u539f\u672cT.png"),
    ("\u53f0\u57fa\u4e0e\u67f1\u7840.png", "\u5468\u9ac0\u7b97\u7ecfT.png"),
    ("\u53e4\u6865\u8425\u9020.png", "\u5b59\u5b50\u7b97\u7ecfT.png"),
    ("\u5c4b\u9876\u5f62\u5236.png", "\u7f09\u53e4\u7b97\u7ecfT.png"),
]

DETAIL_TARGETS = [
    ("\u6597\u62f1\u8be6\u7ec6\u4ecb\u7ecd.png", "\u5218\u5fbd 1.png"),
    ("\u69ab\u536f\u8be6\u7ec6\u4ecb\u7ecd.png", "\u5f20\u8861 1.png"),
    ("\u53f0\u57fa\u4e0e\u67f1\u7840\u8be6\u7ec6\u4ecb\u7ecd.png", "\u6768\u8f89 1.png"),
    ("\u53e4\u6865\u8425\u9020\u8be6\u7ec6\u4ecb\u7ecd.png", "\u7956\u51b2\u4e4b 1.png"),
    ("\u5c4b\u9876\u5f62\u5236\u8be6\u7ec6\u4ecb\u7ecd.png", "\u79e6\u4e5d\u662d 1.png"),
]


def replace_assets(pairs, destination_dir: Path) -> None:
    for source_name, target_name in pairs:
        source = KNOWLEDGE_DIR / source_name
        target = destination_dir / target_name
        copyfile(source, target)
        print(f"{source.name} -> {target.name}")


def main() -> None:
    replace_assets(SURFACE_TARGETS, UI_DIR)
    replace_assets(DETAIL_TARGETS, PERSON_UI_DIR)


if __name__ == "__main__":
    main()
