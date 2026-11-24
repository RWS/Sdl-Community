# XLIFF XSLT Preview Sample

Adds an extra preview, driven by your own XSLT stylesheet, to Trados Studio's built-in **XLIFF 1.1-1.2** file type, without creating a new file type.

After installing, open an `.xlf` file in the Editor, open **View > Preview**, and pick **XSLT** in the preview's *View* list. The built-in **Alt-Trans** view stays available.

Built and tested against Trados Studio 2026 (`Studio19`).

## How it works

Studio lets a plugin extend an existing file type with `FileTypeComponentBuilderExtension`:

- `XliffXsltPreviewAdapter` implements `IFileTypeComponentBuilderAdapter` and names the built-in XLIFF file type in `OriginalFileType = "XLIFF 1.1-1.2 v 2.0.0.0"`. When Studio loads it, it sets `Original` to the built-in XLIFF component builder.
- Every member passes through to `Original`, including `BuildFileTypeInformation`. Because the file type id stays the same, the adapter replaces the XLIFF file type in place: existing settings and file matching keep working.
- `BuildPreviewSetsFactory` takes the original preview sets and adds one more, "XSLT". Its target is an `IControlPreviewType` with control id `InternalPreview` (Studio's built-in WebView2 preview pane) and generator id `XsltPreview`.
- Studio asks for that generator as `BuildAbstractGenerator("Generator_XsltPreview")`. The adapter returns the regular XLIFF file generator with an extra `IFilePostTweaker`, `XsltPreviewTweaker`.
- After the XLIFF file is generated, `XsltPreviewTweaker` runs `preview.xslt` over it and writes the resulting HTML over the generated file. Studio copies that file to `<file>.out.html` and shows it in the Preview pane.

## Segment sync between the Editor and the preview

Selecting a segment in the Editor highlights it in the preview and scrolls to it; clicking in the preview moves the Editor to that segment.

- `SegmentMarker`, a bilingual processor on the `XsltPreview` generator, puts a text marker `{{SDLSEG:<paragraph unit id>DELIMITER<segment id>}}` at the start of every source and target segment before the XLIFF is written.
- `XsltPreviewTweaker` turns each marker in the HTML into an empty `<span class="sdl-seg" data-seg="…">` anchor and adds `sync.js` (an embedded resource) before `</body>`.
- `sync.js` speaks the protocol of Studio's WebView2 preview control: Studio calls `onSegmentSelectedInEditor(id)` when the Editor selection changes; the page posts `selectSegment` on a click, `applyInitialSelection` on load, and `cannotNavigateToSegment` when the segment is not in the page. The element that contains an anchor gets the class `sdl-seg-active`.

Your stylesheet needs no change as long as it outputs the segment text as element text (e.g. `<xsl:value-of select="target"/>` inside a `<td>`). Things to keep in mind:

- The marker is part of the text the stylesheet sees: string functions such as `substring` or `string-length` on `source`/`target` include it.
- Segment text written into an attribute (e.g. `title="{target}"`) would get the anchor markup inside the attribute; leave such content out of the preview or strip `{{SDLSEG:…}}` from it.
- The highlight style is `.sdl-seg-active { background: #fff3b0; outline: 2px solid #f0b400; }`; override it in your stylesheet's CSS with a more specific selector.
- Clicking anywhere selects the segment whose anchor is inside the clicked element, or else the last anchor before it.

## Using your own stylesheet

Replace `preview.xslt`. The plugin loads it from its own folder, so the file must be listed in `pluginpackage.manifest.xml`:

```xml
<Include>
  <File>preview.xslt</File>
</Include>
```

## Notes

- The extension applies to every XLIFF 1.1-1.2 file in every project.
- Only the **Target** tab is available for the new view. Add `Source` / `SideBySide` preview types to the preview set to enable the other tabs.
- The Studio assemblies the plugin uses must be resolvable when building (do not mark `Sdl.FileTypeSupport.Framework.Core` as `Private=false`). Otherwise the plugin build silently leaves the extension out of the generated `plugin.xml`, and Studio never loads the adapter.
- The tests use `PocoFilterManager` and `XliffComponentBuilder` from Studio's implementation assemblies to run the adapter against the real XLIFF file type. Those are not public SDK API and may change between Studio versions.
