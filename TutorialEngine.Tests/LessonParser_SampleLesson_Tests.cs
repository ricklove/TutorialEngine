using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutorialEngine.LessonSyntaxTree;

namespace TutorialEngine.Tests
{
    [TestClass]
    public class LessonParser_SampleLesson_Tests
    {
        private static Lesson ParseSampleLesson()
        {
            var lesson = Lessons.LessonLoader.LoadSampleLesson();
            var parser = new LessonParser();
            var result = parser.ParseLesson(lesson);
            return result;
        }

        [TestMethod]
        public void CanParse()
        {
            var result = ParseSampleLesson();
            var resultStr = result.ToString();
        }

        [TestMethod]
        public void DocumentHasTitle()
        {
            var result = ParseSampleLesson();
            Assert.IsNotNull(result.Document.Title);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.Document.Title.Content.Text));
        }

        [TestMethod]
        public void DocumentHasSteps()
        {
            var result = ParseSampleLesson();
            Assert.IsNotNull(result.Document.Steps);
            Assert.IsTrue(result.Document.Steps.Count > 0);
        }

        [TestMethod]
        public void StepsHaveATitle()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                Assert.IsNotNull(step.Title);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(step.Title.Content.Text));
            }
        }

        [TestMethod]
        public void StepsHaveTheInstructions()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                Assert.IsNotNull(step.Instructions);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(step.Instructions.Content.Text));
            }
        }

        [TestMethod]
        public void StepsHaveAGoal()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                Assert.IsNotNull(step.Goal);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(step.Goal.Content.Text));
            }
        }

        [TestMethod]
        public void InstructionsHaveParagraphs()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var instructions = step.Instructions;
                Assert.IsNotNull(instructions.Paragraphs);
                Assert.IsTrue(instructions.Paragraphs.Count > 0);
            }
        }

        [TestMethod]
        public void InstructionParagraphsHavePhrases()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var instructions = step.Instructions;

                foreach (var paragraph in instructions.Paragraphs)
                {
                    Assert.IsNotNull(paragraph.Phrases);
                    Assert.IsTrue(paragraph.Phrases.Count > 0);
                }
            }
        }


        [TestMethod]
        public void GoalsHaveParagraphs()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var goal = step.Goal;
                Assert.IsNotNull(goal.Paragraphs);
                Assert.IsTrue(goal.Paragraphs.Count > 0);
            }
        }

        [TestMethod]
        public void GoalParagraphsHaveOnlyPhrasesOrCode()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var goal = step.Goal;

                foreach (var paragraph in goal.Paragraphs)
                {
                    Assert.IsTrue(
                        (paragraph.Phrases.Count > 0 && paragraph.Code == null) ||
                        (paragraph.Phrases.Count == 0 && paragraph.Code != null));
                }
            }
        }

        [TestMethod]
        public void AtLeastOneFileSectionExists()
        {
            var result = ParseSampleLesson();
            LessonFile fileSection = null;

            foreach (var step in result.Document.Steps)
            {
                foreach (var c in step.Children)
                {
                    if (c is LessonFile)
                    {
                        return;
                    }
                }
            }

            Assert.Fail("No FILE section exists");
        }

        [TestMethod]
        public void FileSectionIsDeclaredBeforeAnyTestSections()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                foreach (var c in step.Children)
                {
                    if (c is LessonFile)
                    {
                        return;
                    }

                    if (c is LessonTest)
                    {
                        Assert.Fail("A TEST section was found before a FILE section");
                    }
                }
            }
        }

        [TestMethod]
        public void FileHasCode()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var file = step.File;

                if (file == null) { continue; }

                Assert.IsTrue(file.Code != null);
            }
        }

        [TestMethod]
        public void FileHasFileMethodReference()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var file = step.File;

                if (file == null) { continue; }

                Assert.IsTrue(file.FileMethodReference != null);
            }
        }

        [TestMethod]
        public void TestHasCode()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var test = step.Test;

                if (test == null) { continue; }

                Assert.IsTrue(test.Code != null);
            }
        }

        [TestMethod]
        public void ExplanationHasCodeExplanations()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var explanation = step.Explanation;

                if (explanation == null) { continue; }

                Assert.IsTrue(explanation.CodeExplanations.Count > 0);
            }
        }

        [TestMethod]
        public void CodeExplanationsHaveCodeQuote()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var explanation = step.Explanation;

                if (explanation == null) { continue; }

                foreach (var codeExplanation in explanation.CodeExplanations)
                {
                    Assert.IsTrue(codeExplanation.CodeQuote != null);
                }
            }
        }


        [TestMethod]
        public void CodeExplanationsHavePhrases()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                var explanation = step.Explanation;

                if (explanation == null) { continue; }

                foreach (var codeExplanation in explanation.CodeExplanations)
                {
                    Assert.IsTrue(codeExplanation.Phrases.Count > 0);
                }
            }
        }


        [TestMethod]
        public void StepsHaveASummary()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                Assert.IsNotNull(step.Summary);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(step.Summary.Content.Text));
            }
        }

        [TestMethod]
        public void ExplanationSectionMustHaveATestSection()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                if (step.Explanation != null && step.Test == null)
                {
                    Assert.Fail("An EXPLANATION section needs a TEST section");
                }
            }
        }

        [TestMethod]
        public void ExplanationHasExplanationParts()
        {
            var result = ParseSampleLesson();

            foreach (var step in result.Document.Steps)
            {
                if (step.Explanation != null)
                {
                    // test the explanation parts
                }
            }
        }

        #region General Document Integrity Tests

        [TestMethod]
        public void SkippedTextOnlyContainInsignificantText()
        {
            var result = ParseSampleLesson();
            var spans = result.FlattenSpans();

            foreach (var span in spans)
            {
                var skippedText = span.SkippedPreText.Text;
                var lines = skippedText.GetLines();

                foreach (var line in lines)
                {
                    Assert.IsTrue(
                        line.TrimStart() == span.StartMarker
                        || line.StartsWith("//")
                        || string.IsNullOrWhiteSpace(line),
                        "This line was not parsed: " + line
                        );
                }

            }
        }

        [TestMethod]
        public void NoSpansOverlap()
        {
            var result = ParseSampleLesson();
            var spans = result.FlattenSpans();
            LessonSpan lastSpan = null;

            foreach (var span in spans)
            {
                if (lastSpan != null)
                {
                    var iAfterLast = lastSpan.Content.GetIndexAfter();

                    if (iAfterLast > span.Content.Index)
                    {
                        Assert.Fail("Spans overlap");
                    }
                }

                lastSpan = span;
            }
        }

        [TestMethod]
        public void ContentBetweenFirstAndLastSpanIsCorrectForEachBlock()
        {
            var result = ParseSampleLesson();
            var block = result.Document;

            ContentBetweenFirstAndLastSpanIsCorrectForEachBlock_Inner(block);
        }

        private void ContentBetweenFirstAndLastSpanIsCorrectForEachBlock_Inner(LessonBlockBase block)
        {
            foreach (var c in block.Children)
            {
                if (c is LessonBlockBase)
                {
                    ContentBetweenFirstAndLastSpanIsCorrectForEachBlock_Inner(c as LessonBlockBase);
                }
            }

            var parsedContent = block.Content;
            var contentInSpans = block.ContentBetweenFirstAndLastSpan;

            if (contentInSpans.GetIndexAfter() > parsedContent.GetIndexAfter())
            {
                Assert.Fail("ContentBetweenFirstAndLastSpan is beyond the bounds of the end of the Content");
            }

        }

        [TestMethod]
        public void CanParseAndRebuildEachBlock()
        {
            var result = ParseSampleLesson();
            var block = result.Document;
            CanParseAndRebuildEachBlock_Inner(block);
        }

        private void CanParseAndRebuildEachBlock_Inner(LessonBlockBase block)
        {
            foreach (var c in block.Children)
            {
                if (c is LessonBlockBase)
                {
                    CanParseAndRebuildEachBlock_Inner(c as LessonBlockBase);
                }
            }

            var expected = block.ContentBetweenFirstAndLastSpan.Text;
            var actual = block.BuildTextFromSpans();

            AssertEqualWithDiff(expected, actual, block);
        }

        private void AssertEqualWithDiff(string expected, string actual, object debugData = null)
        {
            int? iFirstDiff = null;

            // Work from the start
            for (int i = 0; i < expected.Length && i < actual.Length; i++)
            {
                if (expected[i] != actual[i])
                {
                    iFirstDiff = i;
                    break;
                }
            }

            if (!iFirstDiff.HasValue)
            {
                if (expected.Length == actual.Length)
                {
                    return;
                }
                else
                {
                    iFirstDiff = Math.Min(expected.Length, actual.Length);
                }
            }

            int? iExpectedLastDiff = null;
            int? iActualLastDiff = null;

            for (int iRev = 0; iRev < expected.Length && iRev < actual.Length; iRev++)
            {
                var iExpected = expected.Length - 1 - iRev;
                var iActual = actual.Length - 1 - iRev;

                if (expected[iExpected] != actual[iActual])
                {
                    iExpectedLastDiff = iExpected;
                    iActualLastDiff = iActual;
                    break;
                }
            }

            var expectedDiff = iFirstDiff.Value < expected.Length ?
                expected.Substring(iFirstDiff.Value, iExpectedLastDiff.Value - iFirstDiff.Value) : "";
            var actualDiff = iFirstDiff.Value < actual.Length ?
                actual.Substring(iFirstDiff.Value, iActualLastDiff.Value - iFirstDiff.Value) : "";

            expectedDiff = expectedDiff.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
            actualDiff = actualDiff.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");

            // Extra variables for debugging
            var expectedFormatted = expected.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
            var actualFormatted = actual.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");

            var compareStr = expectedFormatted + "\r\n" + actualFormatted + "\r\n";
            var debugData2 = debugData;

            Assert.Fail("The string do not match: Expected: '" + expectedDiff + "' Actual:'" + actualDiff + "'");
        }

        [TestMethod]
        public void CanParseAndRebuildDocument()
        {
            var result = ParseSampleLesson();
            var resultStr = result.BuildTextFromSpans();

            var lesson = Lessons.LessonLoader.LoadSampleLesson();

            AssertEqualWithDiff(lesson, resultStr, result);
            Assert.AreEqual(lesson, resultStr);
        }



        #endregion
    }

}
