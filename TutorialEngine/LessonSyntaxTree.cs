using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialEngine
{
    public class LessonSyntaxTree
    {
        public LessonDocument Document { get; set; }
    }

    public abstract class LessonNode
    {
        public StringWithIndex Content { get; private set; }

        public LessonNode(StringWithIndex content)
        {
            Content = content;
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", GetType().Name, Content.Text.Replace("\r", "\\r").Replace("\n", "\\n"));
        }
    }


    public abstract class LessonSpan : LessonNode
    {
        public LessonSpan(StringWithIndex content) : base(content) { }
    }

    public class LessonComment : LessonSpan
    {
        public LessonComment(StringWithIndex content) : base(content) { }

        public override string ToString()
        {
            return "// " + Content + "\r\n";
        }
    }

    public abstract class LessonBlockBase : LessonNode
    {
        public List<LessonNode> Children { get; private set; }
        public LessonBlockBase(StringWithIndex content)
            : base(content)
        {
            Children = new List<LessonNode>();
        }

        public override string ToString()
        {
            return Children.Aggregate(new StringBuilder(), (sb, c) => sb.Append(c.ToString())).ToString();
        }
    }

    public class LessonBlock : LessonBlockBase
    {
        public LessonBlock(StringWithIndex content) : base(content) { }
    }

    public class LessonDocument : LessonBlockBase
    {
        public LessonDocument(StringWithIndex content) : base(content) { }
        public LessonTitle Title
        {
            get
            {
                return Children.Where(c => c is LessonTitle).Cast<LessonTitle>().FirstOrDefault();
            }
        }

        public List<LessonStep> Steps
        {
            get
            {
                return Children.Where(c => c is LessonStep).Cast<LessonStep>().ToList();
            }
        }
    }

    public class LessonTitle : LessonSpan
    {
        public LessonTitle(StringWithIndex content) : base(content) { }

        public override string ToString()
        {
            return "% TITLE = " + Content + "\r\n";
        }
    }

    public class LessonStep : LessonBlockBase
    {
        public LessonStep(StringWithIndex content) : base(content) { }

        public LessonTitle Title
        {
            get
            {
                return Children.Where(c => c is LessonTitle).Cast<LessonTitle>().FirstOrDefault();
            }
        }

        public LessonInstructions Instructions
        {
            get
            {
                return Children.Where(c => c is LessonInstructions).Cast<LessonInstructions>().FirstOrDefault();
            }
        }

        public LessonGoal Goal
        {
            get
            {
                return Children.Where(c => c is LessonGoal).Cast<LessonGoal>().FirstOrDefault();
            }
        }
    }

    public class LessonInstructions : LessonBlockBase
    {
        public LessonInstructions(StringWithIndex content) : base(content) { }

        public List<LessonParagraph> Paragraphs
        {
            get
            {
                return Children.Where(c => c is LessonParagraph).Cast<LessonParagraph>().ToList();
            }
        }
    }

    public class LessonGoal : LessonBlockBase
    {
        public LessonGoal(StringWithIndex content) : base(content) { }

        public List<LessonParagraph> Paragraphs
        {
            get
            {
                return Children.Where(c => c is LessonParagraph).Cast<LessonParagraph>().ToList();
            }
        }
    }

    public class LessonParagraph : LessonBlockBase
    {
        public LessonParagraph(StringWithIndex content) : base(content) { }

        public List<LessonPhrase> Phrases
        {
            get
            {
                return Children.Where(c => c is LessonPhrase).Cast<LessonPhrase>().ToList();
            }
        }

        public LessonCode Code
        {
            get
            {
                return Children.Where(c => c is LessonCode).Cast<LessonCode>().FirstOrDefault();
            }
        }
    }

    public class LessonPhrase : LessonSpan
    {
        public LessonPhrase(StringWithIndex content) : base(content) { }

        public override string ToString()
        {
            return "- " + Content + "\r\n";
        }
    }

    public class LessonCode : LessonSpan
    {
        public LessonCode(StringWithIndex content) : base(content) { }

        public override string ToString()
        {
            var lines = Content.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None).Select(l => "\t" + l + "\r\n");
            return lines.Aggregate(new StringBuilder(), (sb, l) => sb.Append(l)).ToString();
        }
    }



}
