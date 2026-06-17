from pathlib import Path
from shutil import copyfile
import re
import uuid


ROOT = Path(__file__).resolve().parents[1]
SOURCE_DIR = ROOT / "pic" / "\u5efa\u7b51"
TARGET_DIR = ROOT / "Assets" / "Materials" / "Buildings"
SCENE = ROOT / "Assets" / "Scenes" / "SampleScene.unity"


SLOTS = [
    {
        "id": "foguangsi",
        "surface": "\u4f5b\u5149\u5bfa\u4e1c\u5927\u6bbf.png",
        "detail": "\u4f5b\u5149\u5bfa\u4e1c\u5927\u6bbf\u8be6\u7ec6\u4ecb\u7ecd.png",
        "click_name": "ZTBl1",
        "detail_image": "2119558245",
        "detail_rect": "2119558244",
    },
    {
        "id": "tiantan",
        "surface": "\u5929\u575b\u7948\u5e74\u6bbf.png",
        "detail": "\u5929\u575b\u7948\u5e74\u6bbf\u8be6\u7ec6\u4ecb\u7ecd.png",
        "click_name": "ZTBl2",
        "detail_image": "2011246581",
        "detail_rect": "2011246580",
    },
    {
        "id": "taihedian",
        "surface": "\u6545\u5bab\u592a\u548c\u6bbf.png",
        "detail": "\u6545\u5bab\u592a\u548c\u6bbf\u8be6\u7ec6\u4ecb\u7ecd.png",
        "click_name": "ZTBl3",
        "detail_image": "1492767742",
        "detail_rect": "1492767741",
    },
]

DISABLE_DETAIL_CHILDREN = [
    "&437151132",
    "&1842334063",
    "&1367236157",
    "&1611433584",
    "&989154413",
    "&1049191602",
]


def make_guid() -> str:
    return uuid.uuid4().hex


def read_guid(meta_path: Path) -> str | None:
    if not meta_path.exists():
        return None

    match = re.search(r"^guid: ([0-9a-f]{32})$", meta_path.read_text(encoding="utf-8"), re.MULTILINE)
    return match.group(1) if match else None


def ensure_folder_meta(folder: Path) -> None:
    folder.mkdir(parents=True, exist_ok=True)
    meta_path = folder.with_name(folder.name + ".meta")
    if meta_path.exists():
        return

    meta_path.write_text(
        "\n".join(
            [
                "fileFormatVersion: 2",
                f"guid: {make_guid()}",
                "folderAsset: yes",
                "DefaultImporter:",
                "  externalObjects: {}",
                "  userData: ",
                "  assetBundleName: ",
                "  assetBundleVariant: ",
                "",
            ]
        ),
        encoding="utf-8",
    )


def texture_meta(guid: str, sprite: bool) -> str:
    texture_type = 8 if sprite else 0
    sprite_mode = 1 if sprite else 0
    sprite_id = "5e97eb03825dee720800000000000000" if sprite else ""
    wrap = 1 if sprite else 0
    filter_mode = 0 if sprite else 1

    return f"""fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 12
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMasterTextureLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 4096
  textureSettings:
    serializedVersion: 2
    filterMode: {filter_mode}
    aniso: 1
    mipBias: 0
    wrapU: {wrap}
    wrapV: {wrap}
    wrapW: 0
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 100
  spriteMode: {sprite_mode}
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: {1 if sprite else 0}
  spriteTessellationDetail: -1
  textureType: {texture_type}
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 4096
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 100
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  - serializedVersion: 3
    buildTarget: Standalone
    maxTextureSize: 4096
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 100
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  - serializedVersion: 3
    buildTarget: Server
    maxTextureSize: 4096
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 100
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: {sprite_id}
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  spritePackingTag: 
  pSDRemoveMatte: 0
  pSDShowRemoveMatteOption: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def material_text(name: str, texture_guid: str) -> str:
    return f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {name}
  m_Shader: {{fileID: 46, guid: 0000000000000000f000000000000000, type: 0}}
  m_ValidKeywords: []
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {{}}
  disabledShaderPasses: []
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BumpMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _DetailAlbedoMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _DetailMask:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _DetailNormalMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _EmissionMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _MainTex:
        m_Texture: {{fileID: 2800000, guid: {texture_guid}, type: 3}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _MetallicGlossMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _OcclusionMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _ParallaxMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    m_Ints: []
    m_Floats:
    - _BumpScale: 1
    - _Cutoff: 0.5
    - _DetailNormalMapScale: 1
    - _DstBlend: 0
    - _GlossMapScale: 1
    - _Glossiness: 0
    - _GlossyReflections: 1
    - _Metallic: 0
    - _Mode: 0
    - _OcclusionStrength: 1
    - _Parallax: 0.02
    - _SmoothnessTextureChannel: 0
    - _SpecularHighlights: 0
    - _SrcBlend: 1
    - _UVSec: 0
    - _ZWrite: 1
    m_Colors:
    - _Color: {{r: 1, g: 1, b: 1, a: 1}}
    - _EmissionColor: {{r: 0, g: 0, b: 0, a: 1}}
  m_BuildTextureStacks: []
"""


def material_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 2100000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def ensure_meta(path: Path, sprite: bool) -> str:
    meta = path.with_name(path.name + ".meta")
    guid = read_guid(meta) or make_guid()
    meta.write_text(texture_meta(guid, sprite), encoding="utf-8")
    return guid


def ensure_material(slot_id: str, texture_guid: str) -> str:
    material_path = TARGET_DIR / f"{slot_id}_surface.mat"
    meta_path = material_path.with_name(material_path.name + ".meta")
    guid = read_guid(meta_path) or make_guid()

    material_path.write_text(material_text(f"{slot_id}_surface", texture_guid), encoding="utf-8")
    meta_path.write_text(material_meta(guid), encoding="utf-8")
    return guid


def replace_once(text: str, old: str, new: str) -> str:
    if old not in text:
        raise RuntimeError(f"Missing expected scene text: {old}")
    return text.replace(old, new, 1)


def update_component_block(scene: str, unity_type: str, file_id: str, updater) -> str:
    pattern = rf"(--- !u!{unity_type} &{file_id}\n.*?)(?=\n--- !u!)"
    match = re.search(pattern, scene, flags=re.S)
    if match is None:
        raise RuntimeError(f"Could not find component {unity_type} &{file_id}")

    block = updater(match.group(1))
    return scene[: match.start(1)] + block + scene[match.end(1) :]


def set_yaml_field(block: str, field: str, value: str) -> str:
    block, count = re.subn(rf"  {re.escape(field)}: .*\n", f"  {field}: {value}\n", block, count=1)
    if count != 1:
        raise RuntimeError(f"Could not set {field}")
    return block


def update_rect_transform(scene: str, rect_id: str, anchored_position: str, size_delta: str) -> str:
    def updater(block: str) -> str:
        block = set_yaml_field(block, "m_AnchorMin", "{x: 0.5, y: 0.5}")
        block = set_yaml_field(block, "m_AnchorMax", "{x: 0.5, y: 0.5}")
        block = set_yaml_field(block, "m_AnchoredPosition", anchored_position)
        block = set_yaml_field(block, "m_SizeDelta", size_delta)
        return block

    return update_component_block(scene, "224", rect_id, updater)


def update_image_component(scene: str, image_id: str, sprite_guid: str, image_type: int = 0, preserve_aspect: int = 1) -> str:
    def updater(block: str) -> str:
        block = set_yaml_field(block, "m_Sprite", f"{{fileID: 21300000, guid: {sprite_guid}, type: 3}}")
        block = set_yaml_field(block, "m_Type", str(image_type))
        block = set_yaml_field(block, "m_PreserveAspect", str(preserve_aspect))
        return block

    return update_component_block(scene, "114", image_id, updater)


def update_ztbl_material(scene: str, click_name: str, material_guid: str) -> str:
    material_entries = (
        f"  - {{fileID: 2100000, guid: {material_guid}, type: 2}}\n"
        f"  - {{fileID: 2100000, guid: {material_guid}, type: 2}}\n"
    )
    pattern = (
        r"(m_Name: " + re.escape(click_name) + r".*?"
        r"  m_Materials:\n"
        r")  - \{fileID: 2100000, guid: [0-9a-f]{32}, type: 2\}\n"
        r"  - \{fileID: 2100000, guid: [0-9a-f]{32}, type: 2\}\n"
    )
    scene, count = re.subn(pattern, lambda match: match.group(1) + material_entries, scene, count=1, flags=re.S)
    if count != 1:
        raise RuntimeError(f"Could not update materials for {click_name}")
    return scene


def disable_game_object(scene: str, game_object_anchor: str) -> str:
    if re.search(re.escape(f"--- !u!1 {game_object_anchor}") + r".*?m_IsActive: 0", scene, flags=re.S):
        return scene

    pattern = re.escape(f"--- !u!1 {game_object_anchor}") + r"(.*?m_IsActive: )1"
    scene, count = re.subn(pattern, lambda match: match.group(0)[:-1] + "0", scene, count=1, flags=re.S)
    if count != 1:
        raise RuntimeError(f"Could not disable GameObject {game_object_anchor}")
    return scene


def update_scene(texture_guids: dict[str, dict[str, str]], material_guids: dict[str, str]) -> None:
    scene = SCENE.read_text(encoding="utf-8")

    scene = re.sub(r"  groundAcceleration: [-0-9.]+", "  groundAcceleration: 120", scene, count=1)
    scene = re.sub(r"  airAcceleration: [-0-9.]+", "  airAcceleration: 120", scene, count=1)
    scene = re.sub(r"  idleDeceleration: [-0-9.]+", "  idleDeceleration: 100", scene, count=1)
    scene = re.sub(r"  groundDrag: [-0-9.]+", "  groundDrag: 0", scene, count=1)

    for slot in SLOTS:
        slot_id = slot["id"]
        scene = update_ztbl_material(scene, slot["click_name"], material_guids[slot_id])
        scene = update_rect_transform(scene, slot["detail_rect"], "{x: -9, y: 0}", "{x: 520, y: 694}")
        scene = update_image_component(scene, slot["detail_image"], texture_guids[slot_id]["detail"])

    scene = update_component_block(
        scene,
        "224",
        "32519701",
        lambda block: set_yaml_field(
            set_yaml_field(block, "m_AnchorMin", "{x: 0, y: 0}"),
            "m_AnchorMax",
            "{x: 1, y: 1}",
        ),
    )
    scene = update_image_component(scene, "437151135", "cdc0c9ac4f661dd4dabfc7a690b54cda", image_type=1, preserve_aspect=0)

    for game_object_anchor in DISABLE_DETAIL_CHILDREN:
        scene = disable_game_object(scene, game_object_anchor)

    SCENE.write_text(scene, encoding="utf-8", newline="")


def main() -> None:
    ensure_folder_meta(TARGET_DIR)
    texture_guids: dict[str, dict[str, str]] = {}
    material_guids: dict[str, str] = {}

    for slot in SLOTS:
        slot_id = slot["id"]
        surface_target = TARGET_DIR / f"{slot_id}_surface.png"
        detail_target = TARGET_DIR / f"{slot_id}_detail.png"

        copyfile(SOURCE_DIR / slot["surface"], surface_target)
        copyfile(SOURCE_DIR / slot["detail"], detail_target)

        surface_guid = ensure_meta(surface_target, sprite=False)
        detail_guid = ensure_meta(detail_target, sprite=True)
        material_guid = ensure_material(slot_id, surface_guid)

        texture_guids[slot_id] = {"surface": surface_guid, "detail": detail_guid}
        material_guids[slot_id] = material_guid
        print(f"{slot['click_name']}: {slot['surface']} / {slot['detail']}")

    update_scene(texture_guids, material_guids)


if __name__ == "__main__":
    main()
