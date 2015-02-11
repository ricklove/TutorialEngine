using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Told.TutorialEngine.Lesson.Parsing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var document = Lessons.LessonLoader.LoadSampleLesson();

            var parser = new LessonParser();
            var lesson = parser.ParseLesson(document);

            var lessonStr = lesson.ToString();

        }

    }
}
