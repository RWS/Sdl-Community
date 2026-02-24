import zipfile
import xml.etree.ElementTree as ET
import os
import sys
import io

def find_sdlplugin_file(folder_path):
    for file in os.listdir(folder_path):
        if file.lower().endswith(".sdlplugin"):
            return os.path.join(folder_path, file)
    return None

def rename_sdlplugin_by_plugin_name(plugin_path):
    if not os.path.isfile(plugin_path):
        raise FileNotFoundError(f"File not found: {plugin_path}")

    with zipfile.ZipFile(plugin_path, 'r') as zip_ref:
        manifest_name = "pluginpackage.manifest.xml"
        xml_file = None

        for file in zip_ref.namelist():
            if file.lower().endswith(manifest_name.lower()):
                xml_file = file
                break

        if xml_file is None:
            raise FileNotFoundError(f"{manifest_name} not found inside {plugin_path}")

        xml_data = zip_ref.read(xml_file)

    # Detect namespaces
    namespaces = {}
    for event, elem in ET.iterparse(io.BytesIO(xml_data), events=("start-ns",)):
        prefix, uri = elem
        namespaces[prefix] = uri

    root = ET.fromstring(xml_data)

    # Non-namespaced
    plugin_name_element = root.find(".//PlugInName")

    # Namespaced
    if plugin_name_element is None:
        for uri in namespaces.values():
            plugin_name_element = root.find(f".//{{{uri}}}PlugInName")
            if plugin_name_element is not None:
                break

    if plugin_name_element is None or not plugin_name_element.text.strip():
        raise ValueError("<PlugInName> not found or empty in XML!")

    plugin_name = plugin_name_element.text.strip()

    new_plugin_path = os.path.join(os.path.dirname(plugin_path), f"{plugin_name}.sdlplugin")

    if os.path.exists(new_plugin_path):
        raise FileExistsError(f"Target file already exists: {new_plugin_path}")

    os.rename(plugin_path, new_plugin_path)

    print(f"SDL Plugin renamed to: {new_plugin_path}")
    return new_plugin_path


if __name__ == "__main__":
    folder_path = os.environ.get("PLUGIN_FOLDER_PATH")

    if not folder_path:
        print("ERROR: Environment variable PLUGIN_FOLDER_PATH is not set.")
        sys.exit(1)

    plugin_path = find_sdlplugin_file(folder_path)

    if not plugin_path:
        print("ERROR: No .sdlplugin file found in folder:", folder_path)
        sys.exit(1)

    rename_sdlplugin_by_plugin_name(plugin_path)