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

        public List<LessonSpan> FlattenSpans()
        {
            return Document.FlattenSpans();
        }

        public string BuildTextFromSpans()
        {
            return Document.BuildTextFromSpans();
        }
    }

    public abstract class LessonNode
    {
        public StringWithIndex Content { get; private set; }
        public LessonBlockBase Parent { get; internal set; }

        internal void OverrideContent(StringWithIndex content)
        {
            Content = content;
        }

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
        public StringWithIndex SkippedPreText { get; internal set; }
        public string StartMarker { get; protected set; }
        public string EndMarker { get; protected set; }

        public LessonSpan(StringWithIndex content, string startMarker, string endMarker)
            : base(content)
        {
            StartMarker = startMarker;
            EndMarker = endMarker;
            OverrideContent(content.TrimStart(startMarker).TrimEnd(endMarker));
        }

        public override string ToString()
        {
            return StartMarker + Content.Text + EndMarker;
        }

    }

    public class LessonEnd : LessonSpan
    {
        public LessonEnd(StringWithIndex content)
            : base(content, "", "")
        {
            if (content.Length != 0)
            {
                throw new ArgumentException("LessonEnd cannot contain content");
            }
        }
    }

    public class LessonComment : LessonSpan
    {
        public LessonComment(StringWithIndex content) : base(content, "// ", "\r\n") { }
    }

    public abstract class LessonBlockBase : LessonNode
    {
        public List<LessonNode> Children { get; private set; }
        //public StringWithIndex SkippedPreText
        //{
        //    get
        //    {
        //        var firstSpanSkippedIndex = FirstSpan().SkippedPreText.Index;
        //        var skippedPreText = new StringWithIndex(Content.Source, firstSpanSkippedIndex, Content.Index - firstSpanSkippedIndex);
        //        return skippedPreText;
        //    }
        //}

        public StringWithIndex ContentBetweenFirstAndLastSpan
        {
            get
            {
                var firstSpan = FirstSpan();
                var lastSpan = LastSpan();

                var firstSpanSkippedIndex = firstSpan.SkippedPreText.Index;
                var lastSpanAfterIndex = lastSpan.Content.GetIndexAfter();

                return new StringWithIndex(Content.Source, firstSpanSkippedIndex, lastSpanAfterIndex - firstSpanSkippedIndex);
            }
        }

        public LessonBlockBase(StringWithIndex content)
            : base(content)
        {
            Children = new List<LessonNode>();
        }

        private LessonSpan FirstSpan()
        {
            foreach (var c in Children)
            {
                if (c is LessonBlockBase)
                {
                    var firstSpan = (c as LessonBlockBase).FirstSpan();
                    if (firstSpan != null)
                    {
                        return firstSpan;
                    }
                }
                else
                {
                    return c as LessonSpan;
                }
            }

            // This shouldn't happen unless this is a completely empty block
            return null;
        }

        private LessonSpan LastSpan()
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                var c = Children[i];

                if (c is LessonBlockBase)
                {
                    var firstSpan = (c as LessonBlockBase).LastSpan();
                    if (firstSpan != null)
                    {
                        return firstSpan;
                    }
                }
                else
                {
                    return c as LessonSpan;
                }
            }

            // This shouldn't happen unless this is a completely empty block
            return null;
        }

        public List<LessonSpan> FlattenSpans()
        {
            var spans = new List<LessonSpan>();
            FlattenSpans(spans);
            return spans;
        }

        private void FlattenSpans(List<LessonSpan> spans)
        {
            foreach (var c in Children)
            {
                if (c is LessonBlockBase)
                {
                    (c as LessonBlockBase).FlattenSpans(spans);
                }
                else
                {
                    spans.Add(c as LessonSpan);
                }
            }
        }

        public string BuildTextFromSpans()
        {
            var spans = FlattenSpans();

            var sb = new StringBuilder();

            foreach (var span in spans)
            {
                sb.Append(span.SkippedPreText.Text + span.Content.Text);
            }

            return sb.ToString();
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
        public LessonDocumentTitle Title
        {
            get
            {
                return Children.Where(c => c is LessonDocumentTitle).Cast<LessonDocumentTitle>().FirstOrDefault();
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

    public class LessonDocumentTitle : LessonSpan
    {
        public LessonDocumentTitle(StringWithIndex content) : base(content, "% TITLE = ", "\r\n") { }
    }

    public class LessonStepTitle : LessonSpan
    {
        public LessonStepTitle(StringWithIndex content) : base(content, "# STEP = ", "\r\n") { }
    }

    public class LessonFileMethodReference : LessonSpan
    {
        public LessonFileMethodReference(StringWithIndex content) : base(content, "# FILE = ", "\r\n") { }
    }

    public class LessonStep : LessonBlockBase
    {
        public LessonStep(StringWithIndex content) : base(content) { }

        public LessonStepTitle Title
        {
            get
            {
                return Children.Where(c => c is LessonStepTitle).Cast<LessonStepTitle>().FirstOrDefault();
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

    public class LessonFile : LessonBlockBase
    {
        public LessonFile(StringWithIndex content) : base(content) { }

        public List<LessonParagraph> Paragraphs
        {
            get
            {
                return Children.Where(c => c is LessonParagraph).Cast<LessonParagraph>().ToList();
            }
        }
    }

    public class LessonTest : LessonBlockBase
    {
        public LessonTest(StringWithIndex content) : base(content) { }

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
        public LessonPhrase(StringWithIndex content) : base(content, "- ", "\r\n") { }
    }

    public class LessonCode : LessonSpan
    {
        public LessonCode(StringWithIndex content) : base(content, "", "") { }

        //public override string ToString()
        //{
        //    var lines = Content.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None).Select(l => StartMarker + l + "\r\n");
        //    return lines.Aggregate(new StringBuilder(), (sb, l) => sb.Append(l)).ToString();
        //}
    }



}
