from __future__ import annotations

from pathlib import Path
import re


ROOT = Path(__file__).resolve().parents[1]
UI_MANAGER_GUID = "00d969bb026335d40a0871228ab50a1d"


def read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8", errors="ignore")


def build_guid_map() -> dict[str, str]:
    guid_to_path: dict[str, str] = {}
    for meta in ROOT.rglob("*.meta"):
        match = re.search(r"^guid:\s*([0-9a-f]+)", read_text(meta), re.M)
        if match:
            guid_to_path[match.group(1)] = str(meta.with_suffix("").relative_to(ROOT))
    return guid_to_path


def build_material_texture_map(guid_to_path: dict[str, str]) -> dict[str, list[str]]:
    material_textures: dict[str, list[str]] = {}
    for mat in ROOT.rglob("*.mat"):
        meta = Path(str(mat) + ".meta")
        if not meta.exists():
            continue
        match = re.search(r"^guid:\s*([0-9a-f]+)", read_text(meta), re.M)
        if not match:
            continue
        material_guid = match.group(1)
        guids = re.findall(r"guid:\s*([0-9a-f]{32})", read_text(mat))
        material_textures[material_guid] = [guid_to_path.get(guid, guid) for guid in guids]
    return material_textures


def parse_scene(path: Path) -> dict[str, dict[str, object]]:
    text = read_text(path)
    docs: dict[str, dict[str, object]] = {}
    for match in re.finditer(r"--- !u!(\d+) &(\d+)\n(.*?)(?=\n--- !u!|\Z)", text, re.S):
        docs[match.group(2)] = {"class": int(match.group(1)), "body": match.group(3)}
    return docs


def game_objects(docs: dict[str, dict[str, object]]) -> tuple[dict[str, dict[str, object]], dict[str, str]]:
    objects: dict[str, dict[str, object]] = {}
    component_to_object: dict[str, str] = {}
    for file_id, doc in docs.items():
        body = str(doc["body"])
        if doc["class"] != 1:
            continue
        name = re.search(r"^  m_Name:\s*(.*)$", body, re.M)
        active = re.search(r"^  m_IsActive:\s*(\d+)", body, re.M)
        components = re.findall(r"- component: \{fileID: (\d+)\}", body)
        objects[file_id] = {
            "name": name.group(1) if name else "",
            "active": active.group(1) if active else "?",
            "components": components,
        }
        for component in components:
            component_to_object[component] = file_id
    return objects, component_to_object


def object_name(file_id: str, objects: dict[str, dict[str, object]]) -> str:
    return str(objects.get(file_id, {}).get("name", "?"))


def object_for_component(
    component_id: str,
    component_to_object: dict[str, str],
    objects: dict[str, dict[str, object]],
) -> tuple[str, str]:
    object_id = component_to_object.get(component_id, "")
    return object_id, object_name(object_id, objects)


def find_sprite(body: str, guid_to_path: dict[str, str]) -> str:
    match = re.search(r"m_Sprite: \{fileID: -?\d+, guid: ([0-9a-f]{32}), type: \d+\}", body)
    if not match:
        return ""
    return guid_to_path.get(match.group(1), match.group(1))


def find_material(body: str, guid_to_path: dict[str, str]) -> str:
    match = re.search(r"m_Material: \{fileID: -?\d+, guid: ([0-9a-f]{32}), type: \d+\}", body)
    if not match:
        return ""
    return guid_to_path.get(match.group(1), match.group(1))


def find_color(body: str) -> str:
    match = re.search(r"m_Color:\s*\{r: ([^,]+), g: ([^,]+), b: ([^,]+), a: ([^}]+)\}", body)
    return match.group(0).strip() if match else ""


def print_ui_refs(scene: str, docs: dict[str, dict[str, object]], guid_to_path: dict[str, str]) -> None:
    objects, component_to_object = game_objects(docs)
    print(f"\nSCENE {scene}")
    for file_id, doc in docs.items():
        body = str(doc["body"])
        if doc["class"] != 114 or UI_MANAGER_GUID not in body:
            continue
        owner_id, owner_name = object_for_component(file_id, component_to_object, objects)
        print(f"UImanager {file_id} on {owner_name} ({owner_id})")
        keys = [f"image{i}" for i in [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13]]
        keys += ["Video", "Video1"]
        for key in keys:
            ref = re.search(rf"^  {re.escape(key)}: \{{fileID: (\d+)\}}", body, re.M)
            if not ref:
                continue
            component_id = ref.group(1)
            target_doc = docs.get(component_id)
            object_id, name = object_for_component(component_id, component_to_object, objects)
            active = objects.get(object_id, {}).get("active", "?")
            sprite = material = color = ""
            if target_doc:
                target_body = str(target_doc["body"])
                sprite = find_sprite(target_body, guid_to_path)
                material = find_material(target_body, guid_to_path)
                color = find_color(target_body)
            print(f"  {key:7} -> {name:18} active={active} sprite={sprite} material={material} {color}")


def print_clickable_refs(
    scene: str,
    docs: dict[str, dict[str, object]],
    guid_to_path: dict[str, str],
    material_textures: dict[str, list[str]],
) -> None:
    objects, _ = game_objects(docs)
    name_to_id = {str(data["name"]): file_id for file_id, data in objects.items()}
    targets = ["ZTBl1", "ZTBl2", "ZTBl3", "ZK1", "ZK2", "ZK3", "ZK4", "ZK5", "ZKCJ1", "ZKCJ2", "ZKCJ3", "ZKCJ4", "ZKCJ5"]
    print(f"\nCLICKABLES {scene}")
    for name in targets:
        object_id = name_to_id.get(name)
        if not object_id:
            print(f"  MISSING {name}")
            continue
        print(f"  {name} ({object_id}) active={objects[object_id]['active']}")
        for component_id in objects[object_id]["components"]:
            doc = docs.get(str(component_id))
            if not doc:
                continue
            body = str(doc["body"])
            if doc["class"] == 4:
                pos = re.search(r"m_LocalPosition: \{x: ([^,]+), y: ([^,]+), z: ([^}]+)\}", body)
                scale = re.search(r"m_LocalScale: \{x: ([^,]+), y: ([^,]+), z: ([^}]+)\}", body)
                print(f"    Transform {pos.group(0) if pos else ''} {scale.group(0) if scale else ''}")
            if doc["class"] == 23:
                guids = re.findall(r"guid: ([0-9a-f]{32})", body)
                mats = [guid_to_path.get(guid, guid) for guid in guids]
                textures = [material_textures.get(guid, []) for guid in guids]
                print(f"    Renderer materials={mats} textures={textures}")
            if doc["class"] in (33, 64, 65, 135, 136):
                print(f"    Collider/Mesh component class={doc['class']}")


def main() -> None:
    guid_to_path = build_guid_map()
    material_textures = build_material_texture_map(guid_to_path)
    for scene in ["Assets/Scenes/SampleScene.unity", "Assets/Scenes/start.unity", "Assets/Scenes/Game.unity"]:
        path = ROOT / scene
        if not path.exists():
            continue
        docs = parse_scene(path)
        print_ui_refs(scene, docs, guid_to_path)
        print_clickable_refs(scene, docs, guid_to_path, material_textures)


if __name__ == "__main__":
    main()
