using System.Linq;
using Sdl.Community.AntidoteVerifier.Connectix;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;
using Sdl.Community.AntidoteVerifier.Tests.Fakes;
using Xunit;

namespace Sdl.Community.AntidoteVerifier.Tests
{
    /// <summary>
    /// Exercises the segment&lt;-&gt;zone mapping and edit gating that replace the old COM callbacks,
    /// driving the agent through an in-memory editor and a synchronous UI delegate.
    /// </summary>
    public class StudioWordProcessorAgentTests
    {
        private static StudioWordProcessorAgent CreateAgent(FakeEditorService editor)
            => new StudioWordProcessorAgent(editor, action => action());

        [Fact]
        public void ZonesToCorrect_corrector_returns_every_segment_focusing_active()
        {
            var editor = new FakeEditorService("first", "second", "third") { ActiveSegmentId = 2 };
            var agent = CreateAgent(editor);

            var zones = agent.ZonesToCorrect(forActiveSelection: false);

            Assert.Equal(3, zones.Count);
            Assert.Equal(new[] { "d1:1", "d1:2", "d1:3" }, zones.Select(z => z.ZoneId).ToArray());
            Assert.Equal(new[] { "first", "second", "third" }, zones.Select(z => z.Text).ToArray());
            Assert.True(zones[1].ZoneIsFocused);
            Assert.False(zones[0].ZoneIsFocused);
            Assert.False(zones[2].ZoneIsFocused);
        }

        [Fact]
        public void ZonesToCorrect_active_selection_returns_single_focused_active_zone()
        {
            var editor = new FakeEditorService("first", "second", "third") { ActiveSegmentId = 3 };
            var agent = CreateAgent(editor);

            var zones = agent.ZonesToCorrect(forActiveSelection: true);

            var zone = Assert.Single(zones);
            Assert.Equal("d1:3", zone.ZoneId);
            Assert.Equal("third", zone.Text);
            Assert.True(zone.ZoneIsFocused);
        }

        [Fact]
        public void ZonesToCorrect_reads_the_whole_document_in_one_batch_call()
        {
            // Performance guard. Antidote re-requests every zone whenever its window moves or the
            // editor scrolls, and resolving segments one by one made each refresh O(segments^2) in
            // EditorService -- minutes of freeze on medium documents (community report, Aug 2026).
            var editor = new FakeEditorService("first", "second", "third");
            var agent = CreateAgent(editor);

            agent.ZonesToCorrect(forActiveSelection: false);

            Assert.Equal(1, editor.GetSegmentTextsCalls);
            Assert.Equal(0, editor.GetSegmentTextCalls);
        }

        [Fact]
        public void ZonesToCorrect_returns_empty_when_no_segments()
        {
            var agent = CreateAgent(new FakeEditorService());

            Assert.Empty(agent.ZonesToCorrect(forActiveSelection: false));
            Assert.Empty(agent.ZonesToCorrect(forActiveSelection: true));
        }

        [Fact]
        public void ZonesToCorrect_never_emits_selection_positions()
        {
            // COM parity: the corrector always corrects the whole document. Antidote treats
            // positionSelection as "correct ONLY this range", so the plugin must never set it.
            var editor = new FakeEditorService("first", "second", "third") { ActiveSegmentId = 2 };
            var agent = CreateAgent(editor);

            var zones = agent.ZonesToCorrect(forActiveSelection: false);

            Assert.All(zones, z =>
            {
                Assert.Null(z.PositionSelectionStart);
                Assert.Null(z.PositionSelectionEnd);
            });
        }

        [Fact]
        public void AllowEdit_true_when_context_matches_segment_substring()
        {
            var agent = CreateAgent(new FakeEditorService("He eat two apple."));

            var allowed = agent.AllowEdit(new AllowEditParams
            {
                ZoneId = "1",
                Context = "eat",
                PositionStart = 3,
                PositionEnd = 6
            });

            Assert.True(allowed);
        }

        [Fact]
        public void AllowEdit_false_when_context_differs()
        {
            var agent = CreateAgent(new FakeEditorService("He eat two apple."));

            var allowed = agent.AllowEdit(new AllowEditParams
            {
                ZoneId = "1",
                Context = "ate",
                PositionStart = 3,
                PositionEnd = 6
            });

            Assert.False(allowed);
        }

        [Fact]
        public void AllowEdit_false_when_range_out_of_bounds_or_zone_unknown()
        {
            var agent = CreateAgent(new FakeEditorService("short"));

            Assert.False(agent.AllowEdit(new AllowEditParams { ZoneId = "1", Context = "x", PositionStart = 0, PositionEnd = 99 }));
            Assert.False(agent.AllowEdit(new AllowEditParams { ZoneId = "9", Context = "x", PositionStart = 0, PositionEnd = 1 }));
        }

        [Fact]
        public void Replace_applies_minimal_edit_and_returns_true()
        {
            var editor = new FakeEditorService("He eat two apple.");
            var agent = CreateAgent(editor);

            var applied = agent.Replace(new ReplaceParams
            {
                ZoneId = "1",
                NewString = "eats",
                PositionStartReplace = 3,
                PositionReplaceEnd = 6
            });

            Assert.True(applied);
            Assert.Equal("He eats two apple.", editor.Segments[0]);
        }

        [Fact]
        public void Replace_returns_false_and_keeps_text_when_segment_locked()
        {
            var editor = new FakeEditorService("He eat two apple.") { CanReplaceResult = false };
            var agent = CreateAgent(editor);

            var applied = agent.Replace(new ReplaceParams
            {
                ZoneId = "1",
                NewString = "eats",
                PositionStartReplace = 3,
                PositionReplaceEnd = 6
            });

            Assert.False(applied);
            Assert.Equal("He eat two apple.", editor.Segments[0]);
        }

        [Fact]
        public void Replace_returns_false_for_unknown_zone()
        {
            var editor = new FakeEditorService("text");
            var agent = CreateAgent(editor);

            Assert.False(agent.Replace(new ReplaceParams { ZoneId = "5", NewString = "x", PositionStartReplace = 0, PositionReplaceEnd = 1 }));
            Assert.Equal("text", editor.Segments[0]);
        }

        [Fact]
        public void Select_forwards_zone_and_positions_to_editor()
        {
            var editor = new FakeEditorService("first", "second");
            var agent = CreateAgent(editor);

            agent.Select(new SelectParams { ZoneId = "2", PositionStart = 1, PositionEnd = 4 });

            Assert.Equal(1, editor.SelectCalls);
            Assert.Equal((2, 1, 4), editor.LastSelect);
        }

        [Fact]
        public void ReturnToDocument_activates_editor()
        {
            var editor = new FakeEditorService("x");
            var agent = CreateAgent(editor);

            agent.ReturnToDocument();

            Assert.Equal(1, editor.ActivateCalls);
        }

        [Fact]
        public void Configuration_reports_text_markup_and_replace_without_selection()
        {
            var editor = new FakeEditorService("x") { DocumentName = "MyDoc" };
            var agent = CreateAgent(editor);

            var config = agent.Configuration();

            Assert.Equal("MyDoc", config.DocumentTitle);
            Assert.Equal("text", config.ActiveMarkup);
            Assert.True(config.ReplaceWithoutSelection);
            Assert.Equal(CacheIdentifierType.ForcePath, config.CacheIdType);
        }

        [Fact]
        public void DocumentPath_returns_editor_document_path()
        {
            var editor = new FakeEditorService("x") { DocumentPath = @"C:\docs\report.sdlxliff" };
            var agent = CreateAgent(editor);

            Assert.Equal(@"C:\docs\report.sdlxliff", agent.DocumentPath());
        }

        [Fact]
        public void SetActiveEditor_retargets_documentPath_and_zones_for_next_launch()
        {
            // The persistent session reuses one agent across launches and points it at the active document
            // via SetActiveEditor before each launch, so Antidote keys the correction tab by the right path.
            var documentA = new FakeEditorService("first in A", "second in A") { DocumentPath = @"C:\docs\A.sdlxliff" };
            var documentB = new FakeEditorService("only in B") { DocumentPath = @"C:\docs\B.sdlxliff" };
            var agent = CreateAgent(documentA);

            Assert.Equal(@"C:\docs\A.sdlxliff", agent.DocumentPath());
            Assert.Equal(2, agent.ZonesToCorrect(forActiveSelection: false).Count);

            agent.SetActiveEditor(documentB);

            Assert.Equal(@"C:\docs\B.sdlxliff", agent.DocumentPath());
            var zonesB = agent.ZonesToCorrect(forActiveSelection: false);
            Assert.Equal("only in B", Assert.Single(zonesB).Text);
        }

        [Fact]
        public void Zones_from_different_documents_get_distinct_prefixes_and_relaunch_keeps_the_prefix()
        {
            var documentA = new FakeEditorService("in A") { DocumentPath = @"C:\docs\A.sdlxliff" };
            var documentB = new FakeEditorService("in B") { DocumentPath = @"C:\docs\B.sdlxliff" };
            var agent = CreateAgent(documentA);

            Assert.Equal("d1:1", Assert.Single(agent.ZonesToCorrect(forActiveSelection: false)).ZoneId);

            agent.SetActiveEditor(documentB);
            Assert.Equal("d2:1", Assert.Single(agent.ZonesToCorrect(forActiveSelection: false)).ZoneId);

            // Relaunching document A (a fresh EditorService, same path) must reuse its zone-id prefix so
            // callbacks Antidote issues against zones from the FIRST launch still route to document A.
            var documentARelaunched = new FakeEditorService("in A") { DocumentPath = @"C:\docs\A.sdlxliff" };
            agent.SetActiveEditor(documentARelaunched);
            Assert.Equal("d1:1", Assert.Single(agent.ZonesToCorrect(forActiveSelection: false)).ZoneId);
        }

        [Fact]
        public void Select_routes_to_the_document_the_zone_belongs_to_not_the_active_one()
        {
            // The live capture showed Antidote sending select for a background tab's zones; with routed
            // zone ids the selection must land in THAT document, not the most recently launched one.
            var documentA = new FakeEditorService("first in A", "second in A") { DocumentPath = @"C:\docs\A.sdlxliff" };
            var documentB = new FakeEditorService("only in B") { DocumentPath = @"C:\docs\B.sdlxliff" };
            var agent = CreateAgent(documentA);
            agent.SetActiveEditor(documentB);

            agent.Select(new SelectParams { ZoneId = "d1:2", PositionStart = 1, PositionEnd = 4 });

            Assert.Equal(1, documentA.SelectCalls);
            Assert.Equal((2, 1, 4), documentA.LastSelect);
            Assert.Equal(0, documentB.SelectCalls);
        }

        [Fact]
        public void Replace_routes_to_the_document_the_zone_belongs_to_not_the_active_one()
        {
            var documentA = new FakeEditorService("He eat two apple.") { DocumentPath = @"C:\docs\A.sdlxliff" };
            var documentB = new FakeEditorService("untouched") { DocumentPath = @"C:\docs\B.sdlxliff" };
            var agent = CreateAgent(documentA);
            agent.SetActiveEditor(documentB);

            var applied = agent.Replace(new ReplaceParams
            {
                ZoneId = "d1:1",
                NewString = "eats",
                PositionStartReplace = 3,
                PositionReplaceEnd = 6
            });

            Assert.True(applied);
            Assert.Equal("He eats two apple.", documentA.Segments[0]);
            Assert.Equal("untouched", documentB.Segments[0]);
        }

        [Fact]
        public void AllowEdit_routes_to_the_document_the_zone_belongs_to()
        {
            var documentA = new FakeEditorService("He eat two apple.") { DocumentPath = @"C:\docs\A.sdlxliff" };
            var documentB = new FakeEditorService("completely different") { DocumentPath = @"C:\docs\B.sdlxliff" };
            var agent = CreateAgent(documentA);
            agent.SetActiveEditor(documentB);

            var allowed = agent.AllowEdit(new AllowEditParams
            {
                ZoneId = "d1:1",
                Context = "eat",
                PositionStart = 3,
                PositionEnd = 6
            });

            Assert.True(allowed);
        }

        [Fact]
        public void Callbacks_for_unknown_document_keys_are_safe_no_ops()
        {
            var editor = new FakeEditorService("text");
            var agent = CreateAgent(editor);

            Assert.False(agent.AllowEdit(new AllowEditParams { ZoneId = "d9:1", Context = "t", PositionStart = 0, PositionEnd = 1 }));
            Assert.False(agent.Replace(new ReplaceParams { ZoneId = "d9:1", NewString = "x", PositionStartReplace = 0, PositionReplaceEnd = 1 }));
            agent.Select(new SelectParams { ZoneId = "d9:1", PositionStart = 0, PositionEnd = 1 });

            Assert.Equal(0, editor.SelectCalls);
            Assert.Equal("text", editor.Segments[0]);
        }

        [Fact]
        public void DocIsAvailable_is_always_true_even_across_document_switches()
        {
            // Answering false to announce a document switch was tried and ruled out live (2026-07-11):
            // Antidote re-inits but never re-asks documentPath, and a sustained false kills the launch
            // in a "(closed)" tab. Availability must therefore always be answered truthfully.
            var documentA = new FakeEditorService("in A") { DocumentPath = @"C:\docs\A.sdlxliff" };
            var documentB = new FakeEditorService("in B") { DocumentPath = @"C:\docs\B.sdlxliff" };
            var agent = CreateAgent(documentA);

            Assert.True(agent.IsDocumentAvailable());
            agent.SetActiveEditor(documentB);
            Assert.True(agent.IsDocumentAvailable());
        }

        [Fact]
        public void Bare_index_zone_ids_fall_back_to_the_active_document()
        {
            // Ids issued before the routing scheme (e.g. restored from Antidote's persisted correction
            // state) are bare segment indexes; they resolve against the active document as before.
            var editor = new FakeEditorService("He eat two apple.");
            var agent = CreateAgent(editor);

            Assert.True(agent.AllowEdit(new AllowEditParams { ZoneId = "1", Context = "eat", PositionStart = 3, PositionEnd = 6 }));

            agent.Select(new SelectParams { ZoneId = "1", PositionStart = 0, PositionEnd = 2 });
            Assert.Equal(1, editor.SelectCalls);
        }
    }
}
