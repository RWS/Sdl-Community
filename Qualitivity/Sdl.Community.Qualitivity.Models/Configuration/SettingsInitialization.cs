using System;
using Sdl.Community.Structures.iProperties;
using Sdl.Community.Structures.QualityMetrics;

namespace Sdl.Community.Structures.Configuration
{
    public class SettingsInitialization
    {
        public static void Initialize_ViewSettings(Settings settings)
        {
            settings.ViewSettings = new ViewSettings();
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "paragraph_id", typeof(bool).ToString(), "false", "Paragraph ID"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "segment_id", typeof(bool).ToString(), "true", "Segment ID"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "date_started", typeof(bool).ToString(), "true", "Date Started"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "date_stopped", typeof(bool).ToString(), "true", "Date Stopped"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "elapsed_time", typeof(bool).ToString(), "false", "Elapsed Time"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "translaton_status", typeof(bool).ToString(), "true", "Translation Status"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "translation_match", typeof(bool).ToString(), "true", "Translation Match"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "source_word_cound", typeof(bool).ToString(), "true", "Source Word Count"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "source_content", typeof(bool).ToString(), "true", "Source Content"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "target_content_original", typeof(bool).ToString(), "false", "Target Content {Original}"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "target_content_updated", typeof(bool).ToString(), "true", "Target Content {Updated}"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "target_track_changes", typeof(bool).ToString(), "true", "Target {Track Changes}"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "target_comparison", typeof(bool).ToString(), "true", "Target {Comparison}"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "modifications_distance", typeof(bool).ToString(), "false", "Modifications Distance"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "pem_percentage", typeof(bool).ToString(), "false", "PEM Percentage"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "target_comments", typeof(bool).ToString(), "false", "Target Comments"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "keystroke_data", typeof(bool).ToString(), "false", "Keystroke data"));
            settings.ViewSettings.ViewProperties.Add(new ViewProperty("DocumentRecordsView", "quality_metrics", typeof(bool).ToString(), "false", "Quality Metrics"));
        }


        public static void Initialize_BackupSettings(Settings settings)
        {
            settings.BackupSettings = new BackupSettings();
            settings.BackupSettings.BackupProperties.Add(new GeneralProperty("backupFolder", typeof(string).ToString(), settings.ApplicationPaths.ApplicationBackupDatabasePath));

            DateTime? dtNow = DateTime.Now;
            settings.BackupSettings.BackupProperties.Add(new GeneralProperty("backupLastDate", typeof(string).ToString(), Helper.DateTimeToSqLite(dtNow)));
            settings.BackupSettings.BackupProperties.Add(new GeneralProperty("backupEvery", typeof(int).ToString(), "1"));
            settings.BackupSettings.BackupProperties.Add(new GeneralProperty("backupEveryType", typeof(int).ToString(), "1"));
        }
        public static void Initialize_GeneralSettings(Settings settings)
        {
            settings.GeneralSettings = new GeneralSettings();
            settings.GeneralSettings.GeneralProperties.Add(new GeneralProperty(name: "defaultCurrency", valueType: typeof(string).ToString(), value: "EUR"));
            settings.GeneralSettings.GeneralProperties.Add(new GeneralProperty("defaultFilterProjectStatus", typeof(string).ToString(), "Show all projects"));
            settings.GeneralSettings.GeneralProperties.Add(new GeneralProperty("defaultFilterActivityStatus", typeof(string).ToString(), "Show all activities"));
            settings.GeneralSettings.GeneralProperties.Add(new GeneralProperty("defaultIncludeUnlistedProjects", typeof(bool).ToString(), "true"));
            settings.GeneralSettings.GeneralProperties.Add(new GeneralProperty("defaultFilterGroupBy", typeof(string).ToString(), "Client name"));
            settings.GeneralSettings.GeneralProperties.Add(new GeneralProperty("defaultActivityViewGroupsIsOn", typeof(bool).ToString(), "true"));
            
        }
        public static void Initialize_TrackerSettings(Settings settings)
        {
            settings.TrackingSettings = new TrackingSettings();
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("idleTimeOut", typeof(bool).ToString(), "false"));
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("idleTimeOutMinutes", typeof(int).ToString(), "15"));
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("idleTimeOutShow", typeof(bool).ToString(), "false"));
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("startOnLoad", typeof(bool).ToString(), "true"));
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("confirmActivitiesOnComplete", typeof(bool).ToString(), "true"));
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("recordNonUpdatedSegments", typeof(bool).ToString(), "false"));
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("recordKeyStokes", typeof(bool).ToString(), "true"));
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("autoStartTrackingOnDocumentOpenEvent", typeof(bool).ToString(), "true"));
            settings.TrackingSettings.TrackingProperties.Add(new GeneralProperty("warningMessageActivityTrackingNotRunning", typeof(bool).ToString(), "false"));
        }

        public static QualityMetricGroup get_QualityMetrics(string type = "")
        {


            var metricGroup = new QualityMetricGroup
            {
                Name = "",
                Description = "",
                MaxSeverityValue = 50,
                MaxSeverityInValue = 1000,
                MaxSeverityInType = "words"
            };



            metricGroup.Severities.Add(new Severity("Minor", 1, -1));
            metricGroup.Severities.Add(new Severity("Major", 5, -1));
            metricGroup.Severities.Add(new Severity("Critical", 10, -1));


            QualityMetric qm = null;


            if (type.Trim() == string.Empty || string.Compare("SAE J2450", type, StringComparison.OrdinalIgnoreCase) == 0)
            {
                metricGroup.Name = "SAE J2450";
                metricGroup.Description = "SAE J2450 translation quality metric standard\r\n\r\nReference: http://www.apex-translations.com/documents/sae_j2450.pdf";

                qm = new QualityMetric
                {
                    Name = "Wrong Term",
                    Modifed = DateTime.Now,
                    Description =
                        "Given the definition of term, we define a ‘wrong term’ to be any target language term that:  \r\n\r\na. Violates a client term glossary;\r\nb. Is in clear conflict with de facto standard translation(s) of the source language term in the automotive field;\r\nc. Is inconsistent with other translations of the source language term in the same document or type of document unless the context for the source language term justifies the use of a different target language term, for example due to ambiguity of the source language term;\r\nd. Denotes a concept in the target language that is clearly and significantly different from the concept denoted by the source language term\r\n\r\nReference: http://www.apex-translations.com/documents/sae_j2450.pdf",
                    MetricSeverity = metricGroup.Severities[2]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Omission",
                    Modifed = DateTime.Now,
                    Description =
                        "An error of omission has occurred if: \r\n\r\na. A continuous block of text in the source language has no counterpart in the target language text and, as a result, the semantics of the source text is absent in the translation;  \r\nb. A graphic which contains source language text has been deleted from the target language deliverable.  \r\n\r\nReference: http://www.apex-translations.com/documents/sae_j2450.pdf",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Syntactic error",
                    Modifed = DateTime.Now,
                    Description =
                        "A syntactic error comprises the following cases:  \r\n\r\na. A source term is assigned the wrong part of speech in its target language counterpart.  \r\nb. The target text contains an incorrect phrase structure, e.g., a relative clause when a verb phrase is needed.\r\nc. The target language words are correct, but in the wrong linear order according to the syntactic rules of the target language.\r\n\r\nReference: http://www.apex-translations.com/documents/sae_j2450.pdf",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Word structure or agreement error",
                    Modifed = DateTime.Now,
                    Description =
                        "Word Structure or agreement error: \r\n\r\na. An error of incorrect word structure has occurred if an otherwise correct target language word (or term) is expressed in an incorrect morphological form, e.g., case, gender, number, tense, prefix, suffix, infix, or any other inflection.  \r\nb. An error of agreement has occurred when two or more target language words disagree in any form of inflection as would be required by the grammatical rules of that language.  \r\n\r\nReference: http://www.apex-translations.com/documents/sae_j2450.pdf",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Misspelling",
                    Modifed = DateTime.Now,
                    Description =
                        "A misspelling has occurred if a target language term:  \r\n\r\na. Violates the spelling as stated in a client glossary, \r\nb. Violates the accepted norms for spelling in the target language,\r\nc. Is written in an incorrect or inappropriate writing system for the target language.  \r\n\r\nReference: http://www.apex-translations.com/documents/sae_j2450.pdf",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Punctuation error",
                    Modifed = DateTime.Now,
                    Description =
                        "The target language text contains an error according to the punctuation rules for that language.  \r\n\r\nReference: http://www.apex-translations.com/documents/sae_j2450.pdf",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Miscellaneous error",
                    Modifed = DateTime.Now,
                    Description =
                        "Any linguistic error related to the target language text which is not clearly attributable to the other categories listed previously should be classified as a miscellaneous error.  \r\n\r\nReference: http://www.apex-translations.com/documents/sae_j2450.pdf",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);
            }
            else if (string.Compare("MQM Core", type, StringComparison.OrdinalIgnoreCase) == 0)
            {

                metricGroup.Name = "MQM Core";
                metricGroup.Description = "MQM Core (Multidimensional Quality Metrics)\r\n\r\n";
                metricGroup.Description += "In order to simplify the application of MQM, MQM defines a smaller “Core” consisting of 22 issue types that represent the most common issues arising in linguistic quality assessment of translated texts.  The Core does not address formatting and many applications may wish to add items from the Design branch to the Core.  The Core represents a relatively high level of granularity suitable for many tasks.  Where possible, users of MQM are encouraged to use issues from the Core to promote greater interoperability between systems";

                qm = new QualityMetric
                {
                    Name = "Accuracy > Mistranslation",
                    Modifed = DateTime.Now,
                    Description =
                        "The target content does not accurately represent the source content.\r\n\r\nExample: A source text states that a medicine should not be administered in doses greater than 200 mg, but the translation states that it should be administered in doses greater than 200 mg (i.e., negation has been omitted).",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Accuracy > Terminology",
                    Modifed = DateTime.Now,
                    Description =
                        "A term (domain-specific word) is translated with a term other than the one expected for the domain or otherwise specified.\r\n\r\nExample: A French text translates English e-mail as e-mail but terminology guidelines mandated that courriel be used.",
                    MetricSeverity = metricGroup.Severities[2]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Accuracy > Omission",
                    Modifed = DateTime.Now,
                    Description =
                        "Content is missing from the translation that is present in the source.\r\n\r\nExample: A paragraph present in the source is missing in the translation.",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Accuracy > Untranslated",
                    Modifed = DateTime.Now,
                    Description =
                        "Content that should have been translated has been left untranslated.\r\n\r\nExample: A sentence in a Japanese document translated into English is left in Japanese.",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);



                qm = new QualityMetric
                {
                    Name = "Verity > Completeness",
                    Modifed = DateTime.Now,
                    Description =
                        "The text is incomplete.\r\n\r\nExample: A process description leaves out key steps needed to complete the process, resulting in an incomplete description of the process.",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Verity > Legal requirements",
                    Modifed = DateTime.Now,
                    Description =
                        "A text does not meet legal requirements as set forth in the specifications.\r\n\r\nExample: Specifications stated that FCC regulatory notices be replaced by CE notices rather than translated, but they were translated instead, rendering the text legally problematic for use in Europe.",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Verity > Locale-specific content",
                    Modifed = DateTime.Now,
                    Description =
                        "Content specific to the source locale does not apply to the intended target locale, audience, or purpose.\r\n\r\nExample: An advertising text translated for Sweden refers to special offers available only in Germany and therefore is misleading.\r\n\r\nNotes: This issue type is distinguished from locale-convention in that this category applies to cases where text corresponds to the conventions of the target locale, but does not apply to the intended audience in the target locale. For example, if the Swedish advertising text mentioned above is properly translated and follows all mechanical locale conventions (e.g., using Swedish kronor instead of euros) but the offer does not apply to Sweden, cocale-specific-content should be chosen. If, however, the text applies to the locale, but does not follow locale conventions (e.g., numbers are formatted incorrectly for the locale), locale-convention should be used instead.",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);




                qm = new QualityMetric
                {
                    Name = "Fluency > Content > Register",
                    Modifed = DateTime.Now,
                    Description =
                        "The text uses a linguistic register inconsistent with the specifications or general language conventions.\r\n\r\nExample: A legal notice in German uses the informal du instead of the formal Sie.",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Fluency > Content > Stylistics",
                    Modifed = DateTime.Now,
                    Description =
                        "The text has stylistic problems, other than those related to language register.\r\n\r\nExample: A text uses a confusing style with long sentences that are difficult to understand.",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);



                qm = new QualityMetric
                {
                    Name = "Fluency > Content > Inconsistency ",
                    Modifed = DateTime.Now,
                    Description =
                        "The text shows internal inconsistency.\r\n\r\nExample: The text states that bug reports should be submitted to a mailing list in one place and via an online bug tracker tool in another.",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Fluency > Mechanical > Spelling",
                    Modifed = DateTime.Now,
                    Description =
                        "Issues related to spelling of words.\r\n\r\nExample: The German word Zustellung is spelled Zustetlugn.",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Fluency > Mechanical > Typography",
                    Modifed = DateTime.Now,
                    Description =
                        "Issues related to the mechanical presentation of text. This category should be used for any typographical errors other than spelling.\r\n\r\nExample: A text uses punctuation incorrectly.\r\n\r\nNotes: Do not use for issues related to spelling.",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);



                qm = new QualityMetric
                {
                    Name = "Fluency > Mechanical > Style guide",
                    Modifed = DateTime.Now,
                    Description =
                        "The text violates style defined in a normative style specification.\r\n\r\nExample: Specifications stated that English text was to be formatted according to the Chicago Manual of Style, but the text delivered followed the American Psychological Association style guide.",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);



                qm = new QualityMetric
                {
                    Name = "Fluency > Mechanical > Grammar",
                    Modifed = DateTime.Now,
                    Description =
                        "Issues related to the grammar or syntax of the text, other than spelling and orthography.\r\n\r\nExample: An English text reads “The man was seeing the his wife.”",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Fluency > Mechanical > Locale convention",
                    Modifed = DateTime.Now,
                    Description =
                        "The text does not adhere to locale-specific mechanical conventions and violates requirements for the presentation of content in the target locale.\r\n\r\nExample: An incorrect format for currency is used for a German text, with a period (.) instead of a comma (,) as a thousands separator.\r\n\r\nNotes: This issue type is distinguished from locale-specific-content in that this category refers only to whether the text is given the proper mechanical form for the locale, not whether the content applies to the locale or not. If text conforms to conventions for the locale, but does not apply to the target locale, locale-specific-content should be used instead.",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);



                qm = new QualityMetric
                {
                    Name = "Fluency > Unintelligible ",
                    Modifed = DateTime.Now,
                    Description =
                        "The exact nature of the error cannot be determined. Indicates a major break down in fluency.\r\n\r\nExample: The following text appears in an English translation of a German automotive manual: “The brake from whe this કુતારો િસ S149235 part numbr,,.”",
                    MetricSeverity = metricGroup.Severities[2]
                };
                metricGroup.Metrics.Add(qm);

              

            }
            else if (string.Compare("TAUS DQF", type, StringComparison.OrdinalIgnoreCase) == 0)
            {
                metricGroup.Name = "TAUS DQF";
                metricGroup.Description = "TAUS DQF (Dynamic Quality Framework)\r\n\r\nReference: https://evaluate.taus.net/evaluate/dqf/dynamic-quality-framework";

                qm = new QualityMetric
                {
                    Name = "Language > Grammar",
                    Modifed = DateTime.Now,
                    Description = "Grammar - syntax: noncompliance with target language rules",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Language > Punctuation",
                    Modifed = DateTime.Now,
                    Description = "Punctuation: noncompliance with target language rules or with style guide",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Language > Spelling",
                    Modifed = DateTime.Now,
                    Description = "Spelling: errors, incorrect use of accents and capital letters",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Terminology > Company",
                    Modifed = DateTime.Now,
                    Description = "Noncompliance with company terminology",
                    MetricSeverity = metricGroup.Severities[2]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Terminology > Normative",
                    Modifed = DateTime.Now,
                    Description = "Noncompliance with 3rd party or product/application terminology",
                    MetricSeverity = metricGroup.Severities[2]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Terminology > Inconsistent",
                    Modifed = DateTime.Now,
                    Description = "Inconsistent",
                    MetricSeverity = metricGroup.Severities[2]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Accuracy > Mistranslation",
                    Modifed = DateTime.Now,
                    Description = "Incorrect interpretation of source text – mistranslation",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Accuracy > Misunderstanding",
                    Modifed = DateTime.Now,
                    Description = "Misunderstanding of technical concept",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Accuracy > Ambiguous",
                    Modifed = DateTime.Now,
                    Description = "Ambiguous translation of a clear source segment",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Accuracy > Omission",
                    Modifed = DateTime.Now,
                    Description = "Omission (essential element in the source text missing in the translation)",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Accuracy > Addition",
                    Modifed = DateTime.Now,
                    Description =
                        "Addition (unnecessary elements in the translation not originally present in the source text)",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Accuracy > Improper (exact)",
                    Modifed = DateTime.Now,
                    Description = "100% match not well translated or not appropriate for context",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Accuracy > Untranslated",
                    Modifed = DateTime.Now,
                    Description = "Untranslated text",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Style > Company Style",
                    Modifed = DateTime.Now,
                    Description = "Noncompliance with company style guides",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Style > Inconsistent (reference)",
                    Modifed = DateTime.Now,
                    Description = "Inconsistent with other reference material",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Style > Inconsistent",
                    Modifed = DateTime.Now,
                    Description = "Inconsistent within text",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Style > Overly literal",
                    Modifed = DateTime.Now,
                    Description = "Literal translation",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Style > Grammar",
                    Modifed = DateTime.Now,
                    Description = "Awkward syntax",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Style > Unidiomatic",
                    Modifed = DateTime.Now,
                    Description = "Unidiomatic use of target language",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Style > Tone",
                    Modifed = DateTime.Now,
                    Description = "Tone",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Others > Query",
                    Modifed = DateTime.Now,
                    Description = "Query implementation",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Others > Client edit",
                    Modifed = DateTime.Now,
                    Description = "Shows client preferences",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Others > Repeat",
                    Modifed = DateTime.Now,
                    Description = "Refers to a repeated error",
                    MetricSeverity = metricGroup.Severities[0]
                };
                metricGroup.Metrics.Add(qm);
            }
            else if (string.Compare("LISA QA Metric", type, StringComparison.OrdinalIgnoreCase) == 0)
            {
                metricGroup.Name = "LISA QA Metric";
                metricGroup.Description = "LISA and its marks are the property of the Localization Industry Standards Association and are used with permission.  LISA is one of the default QA models which your company can use to structure the feedback given by reviewers on the quality of translations. This model is based on the LISA QA metric from the Localization Industry Standards Association. The feedback categories and severities defined in the model are made available to reviewers in the Reviewer Grading window on the Translation screen.\r\n\r\nIn the LISA QA metric, errors are categorized as Minor, Major or Critical. For example if a segment contains one minor and one major mistranslation, you would enter 1 in the Minor column and 1 in the Major column for DOC Language - Mistranslation.  When you enter a number in a category it is multiplied by a weighting figure. The weighting figures can be seen in the table below. For example, under DOC Language, if you record two minor mistranslations they will generate a score of 2. But if you record two critical mistranslations, they will generate a score of 20. Scores for all segments in the task are added together to give a total score for the task. If the overall quality of the task falls below a predefined threshold, the task will fail the LISA check.  The pass or fail status of the check is for information only. A failed task can be submitted to the next stage as usual.";

                qm = new QualityMetric
                {
                    Name = "Doc Language > Mistranslation",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Language > Accuracy",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Language > Terminology",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Language > Language",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Language > Style",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Language > Country",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Language > Consistency",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting > Layout",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting > Typography",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric();
                qm.Name = "Doc Formatting > Graphics";
                qm.Modifed = DateTime.Now;
                qm.Description = "";
                qm.MetricSeverity = metricGroup.Severities[1];
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting > Call Outs and Captions",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting > TOC",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting > Index",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric();
                qm.Name = "Help Formatting > Index";
                qm.Modifed = DateTime.Now;
                qm.Description = "";
                qm.MetricSeverity = metricGroup.Severities[1];
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Help Formatting > Layout",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Help Formatting > Typography",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Help Formatting > Graphics",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Help Formatting - Asian > Index",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Help Formatting - Asian > Graphics",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Help Formatting - Asian > Localizable Text",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Help Formatting - Asian > Hyper Text Functionality, Jumps/Popups",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Software Formatting > Graphics",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Software Formatting > Alignment",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Software Formatting > Sizing",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Software Formatting > Truncation/Overlap",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Software Formatting > Character Formatting",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric();
                qm.Name = "Software Functionality Testing > Localizable Text";
                qm.Modifed = DateTime.Now;
                qm.Description = "";
                qm.MetricSeverity = metricGroup.Severities[1];
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Software Functionality Testing > Dialog Functionality",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric();
                qm.Name = "Software Functionality Testing > Menu Functionality";
                qm.Modifed = DateTime.Now;
                qm.Description = "";
                qm.MetricSeverity = metricGroup.Severities[1];
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Software Functionality Testing > Hotkeys/Accelerators",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Software Functionality Testing > Jumps/Links",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting - Asian > TOC",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting - Asian > Index",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting - Asian > Layout",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting - Asian > Typography",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting - Asian > Graphics",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting - Asian > Call Outs And Captions",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);


                qm = new QualityMetric
                {
                    Name = "Doc Formatting - Asian > Double/Single Size",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);

                qm = new QualityMetric
                {
                    Name = "Doc Formatting - Asian > Punctuation Marks",
                    Modifed = DateTime.Now,
                    Description = "",
                    MetricSeverity = metricGroup.Severities[1]
                };
                metricGroup.Metrics.Add(qm);
              
            }

            return metricGroup;
        }
    }
}
