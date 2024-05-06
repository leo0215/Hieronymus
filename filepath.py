import json
from pathlib import Path
from typing import Any

FP_SRC = Path(__file__).parent.parent
root = FP_SRC.parent
FP_DATA = root / "data"
FP_EXTRACTED = FP_DATA / "extracted"
FP_FLATDATA = FP_SRC / "FlatData"
FP_RAW = FP_DATA / "raw"
FP_TEMP = root / "temp"
FP_TEMP.mkdir(exist_ok=True)
FP_TOOLS = root / "tools"
FP_VER = root / "version.txt"
FP_UNPACK_EXE = root / "csharp/Unpack/exe/Unpack"
FP_VERSION = root / "version"
FP_VERSION.mkdir(exist_ok=True)


FP_BUNDLE_JSON = FP_RAW / "bundleDownloadInfo.json"
"""`ba-data/data/raw/bundleDownloadInfo.json`"""
FP_MEDIA_JSON = FP_RAW / "MediaCatalog.json"
"""`ba-data/data/raw/MediaCatalog.json`"""
FP_TABLE_JSON = FP_RAW / "TableCatalog.json"
"""`ba-data/data/raw/TableCatalog.json`"""


def write_json(path: Path, obj: Any, indent: int | None = 2):
    """Write `obj` as JSON to `path`. Create all parent folders."""
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, "w", encoding="utf8") as f:
        json.dump(obj, f, ensure_ascii=False, indent=indent)


FP_EXCEL = FP_EXTRACTED / "TableBundles/rawdata/table/excel"
FP_EXCELDB = FP_EXTRACTED / "TableBundles/DB/ExcelDB"


def get_excel(name: str):
    path = FP_EXCEL / f"{name}.json"
    with open(path, "r", encoding="utf8") as f:
        dicts = json.load(f)
    return dicts


def get_db(name: str):
    path = FP_EXCELDB / f"{name}.json"
    with open(path, "r", encoding="utf8") as f:
        dicts = json.load(f)
    return dicts


FP_DUMP_CS = FP_TEMP / "dump.cs"
FP_FBS = FP_VERSION / "BlueArchive.fbs"


FP_JP_META = FP_RAW / "version.txt"
FP_JP_APK = FP_TEMP / "com.YostarJP.BlueArchive.apk"
"""`ba-data/temp/com.YostarJP.BlueArchive.apk`"""
