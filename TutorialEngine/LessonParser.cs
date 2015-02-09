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
            var title = headerLines.GetFirstLineValue("% TITLE =").Trim();
            document.Children.Add(new LessonTitle(title));

            // Process the steps
            foreach (var s in steps)
            {
                document.Children.Add(ParseStep(s));
            }

            // Add an end of file span (to catch pretext that was not processed)
            document.Children.Add(new LessonEnd(new StringWithIndex(text.Text, text.Length, 0)));

            DecorateSpansWithSkippedText(document);

            return document;
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
                lastSpan = span;

                if (span.Content.Index > nextExpectedIndex)
                {
                    // Get skipped content
                    var skipLength = span.Content.Index - nextExpectedIndex;
                    var skipContent = new StringWithIndex(document.Content.Source, nextExpectedIndex, skipLength);
                    span.SkippedPreText = skipContent;
                }
                // TODO: move to testing
                else if (span.Content.Index == nextExpectedIndex)
                {
                    // Blank Skipped
                    span.SkippedPreText = new StringWithIndex(document.Content.Source, nextExpectedIndex, 0);
                }
                else
                {
                    throw new ArgumentException("The document is malformed and has overlapping spans");
                }

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
            step.Children.Add(new LessonTitle(titlePart.TrimStart("# STEP =")));

            // Add instructions - the first section (no header)
            var instructionsPart = headerParts[1];
            step.Children.Add(ParseInstructions(instructionsPart));

            // Add Goal
            var goalPart = parts.First(p => p.Text.StartsWith("\n## GOAL"));
            step.Children.Add(ParseGoal(goalPart));


            // TODO: Add other parts

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

        private IEnumerable<LessonParagraph> GetParagraphs(StringWithIndex text)
        {
            var paragaphParts = text.SplitWithoutModificationRegex(@"\r\n(?:\s*\r\n)+");
            var paragraphs = paragaphParts.Select(p => ParseParagraph(p));

            return paragraphs;
        }

        private LessonParagraph ParseParagraph(StringWithIndex text)
        {
            text = text.Trim();

            var paragraph = new LessonParagraph(text);

            var lines = text.SplitLines();

            if (lines.Any(l => l.Text.StartsWith("-")))
            {
                var phrases = lines.Where(l => l.Text.StartsWith("-")).Select(l => new LessonPhrase(l));
                paragraph.Children.AddRange(phrases);

            }
            else if (lines.All(l => l.Text.StartsWith("\t")))
            {
                var codeText = text;
                paragraph.Children.Add(new LessonCode(text));
            }

            return paragraph;
        }

        //private LessonBlockBase ParseBlock(string lessonDoc, int start, int length)
        //{
        //    // Separate the major block elements
        //    throw new NotImplementedException();

        //}



        //private LessonNode ParseNode(string lessonDoc, int start, int length)
        //{
        //    throw new NotImplementedException();
        //}

    }


}
