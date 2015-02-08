using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var document = System.IO.File.ReadAllText(@"D:\UserData\Projects\Products\Frameworks\TutorialEngine\TutorialEngine\Lessons\Lesson.md");

            var parser = new LessonParser();
            var lesson = parser.ParseLesson(document);

            var lessonStr = lesson.ToString();

        }
    }
}
