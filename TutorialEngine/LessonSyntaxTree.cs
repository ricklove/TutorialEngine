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

    public class LessonTitle : LessonSpan
    {
        public LessonTitle(StringWithIndex content) : base(content) { }

        public override string ToString()
        {
            return "% TITLE = " + Content + "\r\n";
        }
    }
}
