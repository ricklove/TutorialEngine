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
        public void CanParseAndRebuildDocument()
        {
            var result = ParseSampleLesson();
            var resultStr = result.ToString();

            var lesson = Lessons.LessonLoader.LoadSampleLesson();

            Assert.AreEqual(lesson, resultStr);
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
    }

}
