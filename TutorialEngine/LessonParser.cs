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
            var document = ParseDocument(lessonDoc, 0, lessonDoc.Length);
            return new LessonSyntaxTree() { Document = document };
        }

        private LessonDocument ParseDocument(string lessonDoc, int start, int length)
        {
            var text = new StringWithIndex(lessonDoc, start, length);

            var sections = text.SplitWithoutModification(new string[] { "\n# STEP = " });

            var header = sections[0];
            var steps = sections.Skip(1).ToList();

            var document = new LessonDocument(text);

            // Get the header metadata
            var headerLines = header.GetLines();
            var title = headerLines.GetFirstLineValue("% TITLE =").Trim();
            document.Children.Add(new LessonTitle(title));

            //// Process the steps


            //// TODO: LOW PRIORITY Decorate the Nodes with their comments

            throw new NotImplementedException();
        }



        private LessonBlockBase ParseBlock(string lessonDoc, int start, int length)
        {
            // Separate the major block elements
            throw new NotImplementedException();

        }

        private LessonNode ParseNode(string lessonDoc, int start, int length)
        {
            throw new NotImplementedException();
        }

    }


}
