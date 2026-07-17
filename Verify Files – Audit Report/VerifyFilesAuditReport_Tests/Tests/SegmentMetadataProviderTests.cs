using VerifyFilesAuditReport.Components.SegmentMetadata;

namespace VerifyFilesAuditReport_Tests.Tests;

public class SegmentMetadataProviderTests : IDisposable
{
    private const string SampleSdlxliff =
        """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:1.2"
               xmlns:sdl="http://sdl.com/FileTypes/SdlXliff/1.0"
               version="1.2" sdl:version="1.0">
          <file original="doc.docx" source-language="en-US" target-language="de-DE" datatype="x-sdlfilterframework2">
            <header>
              <reference>
                <internal-file form="base64">QUJDREVGR0hJSktMTU5PUFFSU1RVVldYWVo=</internal-file>
              </reference>
            </header>
            <body>
              <trans-unit id="tu1">
                <source>Hello</source>
                <target>Hallo</target>
                <sdl:seg-defs>
                  <sdl:seg id="1" conf="Translated" />
                </sdl:seg-defs>
              </trans-unit>
              <trans-unit id="tu2">
                <sdl:seg-defs>
                  <sdl:seg id="2" conf="ApprovedSignOff" origin="tm" />
                  <sdl:seg id="3" />
                  <sdl:seg id="4" state="Draft" />
                </sdl:seg-defs>
              </trans-unit>
            </body>
          </file>
        </xliff>
        """;

    private readonly string _sdlxliffPath;

    public SegmentMetadataProviderTests()
    {
        _sdlxliffPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.sdlxliff");
        File.WriteAllText(_sdlxliffPath, SampleSdlxliff);
    }

    public void Dispose() => File.Delete(_sdlxliffPath);

    [Fact]
    public void ReadSegmentStatuses_ReadsAllSegments()
    {
        var segments = SegmentMetadataProvider.ReadSegmentStatuses(_sdlxliffPath);

        Assert.Equal(["1", "2", "3", "4"], segments.Select(s => s.Id));
    }

    [Fact]
    public void ReadSegmentStatuses_MapsConfToUiName()
    {
        var segments = SegmentMetadataProvider.ReadSegmentStatuses(_sdlxliffPath);

        Assert.Equal("Translated", segments.Single(s => s.Id == "1").Status);
        Assert.Equal("Signed Off", segments.Single(s => s.Id == "2").Status);
    }

    [Fact]
    public void ReadSegmentStatuses_FallsBackToStateThenNotTranslated()
    {
        var segments = SegmentMetadataProvider.ReadSegmentStatuses(_sdlxliffPath);

        Assert.Equal("Not Translated", segments.Single(s => s.Id == "3").Status);
        Assert.Equal("Draft", segments.Single(s => s.Id == "4").Status);
    }

    [Fact]
    public void ReadSegmentStatuses_IgnoresHeaderContent()
    {
        // The embedded original document in the header must not surface as segments.
        var segments = SegmentMetadataProvider.ReadSegmentStatuses(_sdlxliffPath);

        Assert.Equal(4, segments.Count);
    }
}
