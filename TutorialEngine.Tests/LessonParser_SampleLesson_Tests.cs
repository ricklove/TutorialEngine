using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialEngine.Tests
{
    [TestClass]
    public class LessonParser_SampleLesson_Tests
    {
        [TestMethod]
        public void CanParse()
        {
            var result = ParseSampleLesson();
            var resultStr = result.ToString();
        }

        private static LessonSyntaxTree ParseSampleLesson()
        {
            var lesson = Lessons.LessonLoader.LoadSampleLesson();
            var parser = new LessonParser();
            var result = parser.ParseLesson(lesson);
            return result;
        }

        [TestMethod]
        public void HasTitle()
        {
            var result = ParseSampleLesson();
            Assert.IsNotNull(result.Document.Title);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.Document.Title.Content.Text));
        }

        [TestMethod]
        public void HasSteps()
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


        //[TestMethod]
        //public void CanParseAndRebuildDocument()
        //{
        //    var result = ParseSampleLesson();
        //    var resultStr = result.ToString();

        //    var lesson = Lessons.LessonLoader.LoadSampleLesson();

        //    Assert.AreEqual(lesson, resultStr);
        //}

    }

}
