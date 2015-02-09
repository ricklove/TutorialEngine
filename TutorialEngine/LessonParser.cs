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



            //// TODO: LOW PRIORITY Decorate the Nodes with their comments

            return document;
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

            // Add instructions - the first section (no header)
            var instructionsPart = headerParts[1];
            step.Children.Add(ParseInstructions(instructionsPart));

            // Add other parts

            return step;
        }

        private LessonInstructions ParseInstructions(StringWithIndex text)
        {
            var lesson = new LessonInstructions(text);

            return lesson;
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
