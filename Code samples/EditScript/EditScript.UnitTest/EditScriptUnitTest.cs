using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EditScript.UnitTest
{
    [TestClass]
    public class EditScriptUnitTest
    {
        private EditScriptService _editScriptService= new EditScriptService();

        [TestMethod]
        public void CaseMatch()
        {
            var tu = _editScriptService.CreateTu("This is a test.", "Dies ist ein Test.");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = false,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "test",
                ReplacementPattern = "xxxx"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("This is a xxxx.", tu.SourceSegment.ToPlain());
        }

        [TestMethod]
        public void CaseMismatch()
        {
            var tu = _editScriptService.CreateTu("This is a test.", "Dies ist ein Test.");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = false,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "Test",
                ReplacementPattern = "xxxx"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            // case-sensitive, and capital "Test" shouldn't match:
            Assert.IsFalse(applied);
        }

        [TestMethod]
        public void CaseInsensitive()
        {
            var tu = _editScriptService.CreateTu("This is a test.", "Dies ist ein Test.");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = true,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "test",
                ReplacementPattern = "Xxxx"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("This is a Xxxx.", tu.SourceSegment.ToPlain());
        }

        [TestMethod]
        public void CaseInsensitiveTolower()
        {
            var tu = _editScriptService.CreateTu("This is a Test.", "Dies ist ein Test.");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = true,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "test",
                ReplacementPattern = "xxxx"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("This is a Xxxx.", tu.SourceSegment.ToPlain());
        }

        [TestMethod]
        public void AtEnd()
        {
            var tu = _editScriptService.CreateTu("This is a test", "Dies ist ein Test");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = true,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "test",
                ReplacementPattern = "xxxx"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("This is a xxxx", tu.SourceSegment.ToPlain());
        }

        [TestMethod]
        public void ReplacementAddsPattern()
        {
            var tu = _editScriptService.CreateTu("aa", "aa");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = false,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "a",
                ReplacementPattern = "aaaa"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("aaaaaaaa", tu.SourceSegment.ToPlain());
        }

        [TestMethod]
        public void RegexSimple()
        {
            var tu = _editScriptService.CreateTu("aaa", "aaa");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = false,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.RegularExpression,
                SearchPattern = "a+",
                ReplacementPattern = "x"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("x", tu.SourceSegment.ToPlain());
        }

        [TestMethod]
        public void RegexSimple2()
        {
            var tu = _editScriptService.CreateTu("aaa", "aaa");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = false,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.RegularExpression,
                SearchPattern = "a$",
                ReplacementPattern = "x"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("aax", tu.SourceSegment.ToPlain());
        }

        [TestMethod]
        public void TargetFail()
        {
            var tu = _editScriptService.CreateTu("This is a test", "Dies ist ein Test");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = false,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "dies",
                ReplacementPattern = "xxxx"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsFalse(applied);
        }

        [TestMethod]
        public void TargetMatch()
        {
            var tu = _editScriptService.CreateTu("This is a test.", "Dies ist ein Test.");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = true,
                IgnoreCase = true,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "dies",
                ReplacementPattern = "xxxx"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("Xxxx ist ein Test.", tu.TargetSegment.ToPlain());
        }

        [TestMethod]
        public void Serialization()
        {
            Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditAction ea = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionDeleteFieldValue("Test");

            var ser = new System.Runtime.Serialization.DataContractSerializer(typeof(Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditAction));

            byte[] data;

            using (var str = new System.IO.MemoryStream())
            {
                using (var wtr = System.Xml.XmlWriter.Create(str))
                {
                    ser.WriteObject(wtr, ea);
                    wtr.Flush();
                }
                data = str.ToArray();
            }

            using (var rdr = new System.IO.MemoryStream(data))
            {
                var xmlrdr = System.Xml.XmlDictionaryReader.CreateTextReader(rdr, new System.Xml.XmlDictionaryReaderQuotas());

                var deserialized = ser.ReadObject(xmlrdr, true);
                Assert.IsNotNull(deserialized);

                var deserializedEa
                    = deserialized as Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditAction;
                Assert.IsNotNull(deserializedEa);

                Assert.IsInstanceOfType(deserializedEa, ea.GetType());

                var eadfv
                    = deserializedEa as Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionDeleteFieldValue;
                Assert.IsNotNull(eadfv);

                Assert.IsNotNull(eadfv.FieldNames);
                Assert.IsTrue(eadfv.FieldNames.Count == 1);
                Assert.AreEqual("Test", eadfv.FieldNames[0]);
            }

        }

        /// <summary>
        /// #40426 - TM batch edit unexpectedly shortens source segments during replace operations
        /// </summary>
        [TestMethod]
        public void MultipleHits()
        {
            var tu = _editScriptService.CreateTu("Close the door.", "Close the door.");

            var sr = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditActionSearchReplace
            {
                ApplyToTarget = false,
                IgnoreCase = false,
                PatternType = Sdl.LanguagePlatform.TranslationMemory.EditScripts.PatternType.Literal,
                SearchPattern = "e",
                ReplacementPattern = "E"
            };

            var script = new Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScript();
            script.Add(sr);

            var applied = Sdl.LanguagePlatform.TranslationMemory.EditScripts.EditScriptApplier.Apply(script, tu);

            Assert.IsTrue(applied);
            Assert.AreEqual("ClosE thE door.", tu.SourceSegment.ToPlain());
        }
    }
}