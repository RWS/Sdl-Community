using LanguageWeaverProvider.Services;
using Sdl.LanguagePlatform.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LanguageWeaverProviderTests.IntegrationTests
{
    public class InputData : IEnumerable<object[]>
    {
        public List<Segment> SourceSegments { get; set; } = new();

        public IEnumerator<object[]> GetEnumerator()
        {
            SourceSegments.AddRange(File.ReadAllLines(@"C:\TestData\SamplePhotoPrinter").Select(TestHelper.DeserializeSegmentFromBase64).ToList());
            SourceSegments.AddRange(File.ReadAllLines(@"C:\TestData\SecondSample").Select(TestHelper.DeserializeSegmentFromBase64).ToList());
            SourceSegments.AddRange(File.ReadAllLines(@"C:\TestData\Emoji").Select(TestHelper.DeserializeSegmentFromBase64).ToList());
            SourceSegments.AddRange(File.ReadAllLines(@"C:\TestData\Subscript").Select(TestHelper.DeserializeSegmentFromBase64).ToList());
            SourceSegments.AddRange(File.ReadAllLines(@"C:\TestData\SampleWithLockedTags").Select(TestHelper.DeserializeSegmentFromBase64).ToList());

            foreach (var segment in SourceSegments.Where(seg=>seg.HasTags))
                yield return [segment];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}