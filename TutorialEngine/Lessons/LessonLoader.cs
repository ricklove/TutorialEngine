using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TutorialEngine.Lessons
{
    public class LessonLoader
    {
        // FROM: http://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
        public static string LoadSampleLesson()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "TutorialEngine.Lessons.Sample.md";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
