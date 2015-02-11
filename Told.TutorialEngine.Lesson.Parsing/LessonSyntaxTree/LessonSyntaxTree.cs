using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Told.TutorialEngine.Lesson.Parsing.LessonSyntaxTree
{
    public partial class LessonTree
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

    public partial class LessonEnd : LessonSpan
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

    public partial class LessonComment : LessonSpan
    {
        public LessonComment(StringWithIndex content) : base(content, "// ", "\r\n") { }
    }

    public abstract class LessonBlockBase : LessonNode
    {
        public List<LessonNode> Children { get; private set; }

        public void AddChildren(System.Collections.IEnumerable nodes)
        {
            Children.AddRange(nodes.Cast<LessonNode>());
        }

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

    public partial class LessonBlock : LessonBlockBase
    {
        public LessonBlock(StringWithIndex content) : base(content) { }
    }

    public partial class LessonDocument : LessonBlockBase
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

    public partial class LessonDocumentTitle : LessonSpan
    {
        public LessonDocumentTitle(StringWithIndex content) : base(content, "% TITLE = ", "\r\n") { }
    }

    public partial class LessonStepTitle : LessonSpan
    {
        public LessonStepTitle(StringWithIndex content) : base(content, "# STEP = ", "\r\n") { }
    }

    public partial class LessonBlankTitlePlaceholder : LessonSpan
    {
        public LessonBlankTitlePlaceholder(StringWithIndex content, string startMarker, string endMarker) : base(content, startMarker, endMarker) { }
    }

    public partial class LessonFileMethodReference : LessonSpan
    {
        public LessonFileMethodReference(StringWithIndex content) : base(content, "## FILE = ", "\r\n") { }
    }

    public partial class LessonStep : LessonBlockBase
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

        public LessonSummary Summary
        {
            get
            {
                return Children.Where(c => c is LessonSummary).Cast<LessonSummary>().FirstOrDefault();
            }
        }

        public LessonTest Test
        {
            get
            {
                return Children.Where(c => c is LessonTest).Cast<LessonTest>().FirstOrDefault();
            }
        }


        public LessonExplanation Explanation
        {
            get
            {
                return Children.Where(c => c is LessonExplanation).Cast<LessonExplanation>().FirstOrDefault();
            }
        }


        public LessonFile File
        {
            get
            {
                return Children.Where(c => c is LessonFile).Cast<LessonFile>().FirstOrDefault();
            }
        }
    }

    public partial class LessonInstructions : LessonBlockBase
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

    public partial class LessonGoal : LessonBlockBase
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

    public partial class LessonSummary : LessonBlockBase
    {
        public LessonSummary(StringWithIndex content) : base(content) { }

        public List<LessonParagraph> Paragraphs
        {
            get
            {
                return Children.Where(c => c is LessonParagraph).Cast<LessonParagraph>().ToList();
            }
        }
    }

    public partial class LessonFile : LessonBlockBase
    {
        public LessonFile(StringWithIndex content) : base(content) { }

        public LessonFileMethodReference FileMethodReference
        {
            get
            {
                return Children.Where(c => c is LessonFileMethodReference).Cast<LessonFileMethodReference>().FirstOrDefault();
            }
        }

        public string Path
        {
            get
            {
                // TODO: Parse this correctly
                var t = FileMethodReference.Content.Text;
                var i = t.IndexOf(">");
                return t.Substring(0, i).Trim();
            }
        }
        public string Context
        {
            get
            {
                var t = FileMethodReference.Content.Text;
                var i = t.IndexOf(">");
                return t.Substring(i + 1).Trim();
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

    public partial class LessonTest : LessonBlockBase
    {
        public LessonTest(StringWithIndex content) : base(content) { }

        public LessonCode Code
        {
            get
            {
                return Children.Where(c => c is LessonCode).Cast<LessonCode>().FirstOrDefault();
            }
        }
    }

    public partial class LessonParagraph : LessonBlockBase
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

    public partial class LessonPhrase : LessonSpan
    {
        public LessonPhrase(StringWithIndex content) : base(content, "- ", "\r\n") { }
    }

    public partial class LessonCode : LessonSpan
    {
        public LessonCode(StringWithIndex content) : base(content, "", "") { }

        //public override string ToString()
        //{
        //    var lines = Content.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None).Select(l => StartMarker + l + "\r\n");
        //    return lines.Aggregate(new StringBuilder(), (sb, l) => sb.Append(l)).ToString();
        //}
    }


    public partial class LessonExplanation : LessonBlockBase
    {
        public LessonExplanation(StringWithIndex content) : base(content) { }

        public List<LessonCodeExplanation> CodeExplanations
        {
            get
            {
                return Children.Where(c => c is LessonCodeExplanation).Cast<LessonCodeExplanation>().ToList();
            }
        }
    }


    public partial class LessonCodeExplanation : LessonBlockBase
    {
        public LessonCodeExplanation(StringWithIndex content) : base(content) { }

        public LessonCodeExplanationQuote CodeQuote
        {
            get
            {
                return Children.Where(c => c is LessonCodeExplanationQuote).Cast<LessonCodeExplanationQuote>().FirstOrDefault();
            }
        }

        public List<LessonPhrase> Phrases
        {
            get
            {
                return Children.Where(c => c is LessonPhrase).Cast<LessonPhrase>().ToList();
            }
        }
    }

    public partial class LessonCodeExplanationQuote : LessonSpan
    {
        public LessonCodeExplanationQuote(StringWithIndex content) : base(content, "* ", "\r\n") { }
    }

}
