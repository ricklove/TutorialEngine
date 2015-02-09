using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialEngine
{
    public class LessonParser
    {
        public LessonSyntaxTree ParseLesson(string lessonDoc)
        {
            var document = ParseDocument(new StringWithIndex(lessonDoc, 0, lessonDoc.Length));
            return new LessonSyntaxTree() { Document = document };
        }

        private LessonDocument ParseDocument(StringWithIndex text)
        {
            var sections = text.SplitWithoutModification(new string[] { "\n# STEP = " });

            var header = sections[0];
            var steps = sections.Skip(1).ToList();

            var document = new LessonDocument(text);

            // Get the header metadata
            var headerLines = header.GetLines();
            var title = headerLines.GetFirstLineValue("% TITLE = ").Trim();
            document.Children.Add(new LessonDocumentTitle(title));

            // Process the steps
            foreach (var s in steps)
            {
                document.Children.Add(ParseStep(s));
            }

            // Add an end of file span (to catch pretext that was not processed)
            document.Children.Add(new LessonEnd(new StringWithIndex(text.Text, text.Length, 0)));

            DecorateNodesWithParent(document);
            DecorateSpansWithSkippedText(document);

            return document;
        }

        private void DecorateNodesWithParent(LessonBlockBase parent)
        {
            foreach (var c in parent.Children)
            {
                c.Parent = parent;

                if (c is LessonBlockBase)
                {
                    DecorateNodesWithParent(c as LessonBlockBase);
                }
            }
        }

        private static void DecorateSpansWithSkippedText(LessonDocument document)
        {
            // Decorate the LessonSpans with skipped parts
            var spans = document.FlattenSpans();

            // Find gaps between the spans
            LessonSpan lastSpan = null;

            foreach (var span in spans)
            {
                var nextExpectedIndex = lastSpan != null ? lastSpan.Content.GetIndexAfter() : 0;

                if (span.Content.Index > nextExpectedIndex)
                {
                    // Get skipped content
                    var skipLength = span.Content.Index - nextExpectedIndex;
                    var skipContent = new StringWithIndex(document.Content.Source, nextExpectedIndex, skipLength);
                    span.SkippedPreText = skipContent;
                }
                else if (span.Content.Index == nextExpectedIndex)
                {
                    // Blank Skipped
                    span.SkippedPreText = new StringWithIndex(document.Content.Source, nextExpectedIndex, 0);
                }
                // TODO: move to testing
                else
                {
                    var lastSpanContent = lastSpan.Content;
                    var skipped = span.SkippedPreText;
                    var spanContent = span.Content;

                    throw new ArgumentException("The document is malformed and has overlapping spans");
                }


                lastSpan = span;
            }

        }

        private LessonNode ParseStep(StringWithIndex text)
        {
            text = text.Trim();
            var step = new LessonStep(text);

            // Divide the parts
            var parts = text.SplitWithoutModification("\n#");

            // The header includes the title and the instructions
            var header = parts[0];
            var headerParts = header.SplitAfterFirstLine();

            // Parse the step title
            var titlePart = headerParts[0];
            step.Children.Add(new LessonStepTitle(titlePart.TrimStart("# STEP = ")));

            // Add instructions - the first section (no header)
            var instructionsPart = headerParts[1];
            step.Children.Add(ParseInstructions(instructionsPart));

            // Add Goal
            var goalPart = parts.First(p => p.Text.StartsWith("\n## GOAL"));
            step.Children.Add(ParseGoal(goalPart.TrimStart("\n## GOAL")));

            // Add File
            var filePart = parts.FirstOrDefault(p => p.Text.StartsWith("\n## FILE = "));
            if (filePart != null)
            {
                step.Children.Add(ParseFile(filePart.TrimStart("\n## FILE = ")));
            }


            // TODO: Add other parts

            // Order children
            var ordered = step.Children.OrderBy(c => c.Content.Index).ToList();
            step.Children.Clear();
            step.Children.AddRange(ordered);

            return step;
        }

        private LessonInstructions ParseInstructions(StringWithIndex text)
        {
            text = text.Trim();

            var instructions = new LessonInstructions(text);

            var paragraphs = GetParagraphs(text);

            // Remove blank paragraphs
            paragraphs = paragraphs.Where(p => p.Phrases.Count > 0);

            instructions.Children.AddRange(paragraphs);

            return instructions;
        }

        private LessonGoal ParseGoal(StringWithIndex text)
        {
            text = text.Trim();

            var goal = new LessonGoal(text);

            var paragraphs = GetParagraphs(text);

            // Remove blank paragraphs
            paragraphs = paragraphs.Where(p => p.Phrases.Count > 0 || p.Code != null);

            goal.Children.AddRange(paragraphs);

            return goal;
        }

        private LessonFile ParseFile(StringWithIndex text)
        {
            text = text.Trim();

            var file = new LessonFile(text);

            var parts = text.SplitAfterFirstLine();

            // Parse the title
            var titlePart = parts[0];
            file.Children.Add(new LessonFileMethodReference(titlePart));

            // Parse the paragraphs
            var paragraphsText = parts[1];
            var paragraphs = GetParagraphs(paragraphsText);

            // Remove blank paragraphs
            paragraphs = paragraphs.Where(p => p.Code != null);

            file.Children.AddRange(paragraphs);

            return file;
        }

        private IEnumerable<LessonParagraph> GetParagraphs(StringWithIndex text)
        {
            var paragaphParts = text.SplitWithoutModificationRegex(@"\r\n(?:\s*\r\n)+");
            var paragraphs = paragaphParts.Select(p => ParseParagraph(p));

            return paragraphs.ToList();
        }

        private LessonParagraph ParseParagraph(StringWithIndex text)
        {
            //text = text.Trim();

            var paragraph = new LessonParagraph(text);

            var lines = text.SplitLines();

            // Remove comments and blank lines
            lines = lines.Where(l => !l.Text.StartsWith("//") && !string.IsNullOrWhiteSpace(l.Text)).ToList();

            if (lines.Count == 0)
            {
                return paragraph;
            }

            if (lines.All(l => l.Text.StartsWith("-")))
            {
                var phrases = lines.Where(l => l.Text.StartsWith("-")).Select(l => new LessonPhrase(l));
                paragraph.Children.AddRange(phrases);

            }
            else if (lines.All(l => l.Text.StartsWith("\t") || l.Text.StartsWith("    ")))
            {
                var codeText = text;
                paragraph.Children.Add(new LessonCode(text));
            }
            else
            {
                throw new ArgumentException("All lines in a paragraph must be the same type: " + text);
            }

            return paragraph;
        }


    }


}
