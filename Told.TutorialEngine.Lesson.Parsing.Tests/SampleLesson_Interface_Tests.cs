using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Told.TutorialEngine.Lesson.Parsing.LessonSyntaxTree;

namespace Told.TutorialEngine.Lesson.Parsing.Tests
{
    [TestClass]
    public class SampleLesson_Interface_Tests
    {
        private static LessonTree ParseSampleLesson()
        {
            var lesson = Lessons.LessonLoader.LoadSampleLesson();
            var parser = new LessonParser();
            var result = parser.ParseLesson(lesson);
            return result;
        }

        [TestMethod]
        public void CanReachAllChildrenThroughInterfaces()
        {
            var result = ParseSampleLesson();
            CanReachAllChildrenThroughInterfaces_Inner(result.Document);
        }

        private void CanReachAllChildrenThroughInterfaces_Inner(LessonBlockBase block)
        {
            var remainingChildren = block.Children.ToList();

            var interfaces = block.GetType().GetInterfaces();

            foreach (var z in interfaces)
            {
                foreach (var prop in z.GetProperties())
                {
                    var val = prop.GetValue(block);

                    if (val is System.Collections.IEnumerable)
                    {
                        foreach (var item in (val as System.Collections.IEnumerable))
                        {
                            remainingChildren.Remove(item as LessonNode);
                        }
                    }
                    else
                    {
                        remainingChildren.Remove(val as LessonNode);
                    }
                }
            }

            // Ignore empty children (which may be placeholders)
            for (int i = 0; i < remainingChildren.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(remainingChildren[i].Content.Text))
                {
                    remainingChildren.RemoveAt(i);
                    i--;
                }
            }

            if (remainingChildren.Count > 0)
            {
                Assert.Fail("Some children are not accessible through the interfaces: Count:" + remainingChildren.Count + " first:" + remainingChildren.First());
            }

            // Go deeper
            foreach (var c in block.Children)
            {
                if (c is LessonBlockBase)
                {
                    CanReachAllChildrenThroughInterfaces_Inner(c as LessonBlockBase);
                }
            }
        }
    }
}
