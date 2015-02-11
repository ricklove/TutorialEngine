using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Told.TutorialEngine.Lesson;



namespace Told.TutorialEngine.Lesson.Parsing.LessonSyntaxTree
{
    public partial class LessonTree : ILessonTree
    {
        ILessonDocument ILessonTree.Document { get { return Document; } }
    }

    public partial class LessonDocument : ILessonDocument
    {
        ILessonDocumentTitle ILessonDocument.Title { get { return Title; } }
        IList<ILessonStep> ILessonDocument.Steps { get { return Steps.Cast<ILessonStep>().ToList(); } }
    }

    public partial class LessonDocumentTitle : ILessonDocumentTitle
    {
        string ILessonDocumentTitle.Text { get { return Content.Text; } }
    }

    public partial class LessonStep : ILessonStep
    {
        ILessonStepTitle ILessonStep.Title { get { return Title; } }
        ILessonInstructions ILessonStep.Instructions { get { return Instructions; } }
        ILessonGoal ILessonStep.Goal { get { return Goal; } }
        ILessonSummary ILessonStep.Summary { get { return Summary; } }
        ILessonTest ILessonStep.Test { get { return Test; } }
        ILessonExplanation ILessonStep.Explanation { get { return Explanation; } }
        ILessonFile ILessonStep.File { get { return File; } }
    }

    public partial class LessonStepTitle : ILessonStepTitle
    {
        string ILessonStepTitle.Text { get { return Content.Text; } }
    }

    public partial class LessonInstructions : ILessonInstructions
    {
        IList<ILessonParagraph> ILessonInstructions.Paragraphs { get { return Paragraphs.Cast<ILessonParagraph>().ToList(); } }
    }

    public partial class LessonParagraph : ILessonParagraph
    {
        IList<ILessonPhrase> ILessonParagraph.Phrases { get { return Phrases.Cast<ILessonPhrase>().ToList(); } }
        ILessonCode ILessonParagraph.Code { get { return Code; } }
    }

    public partial class LessonPhrase : ILessonPhrase
    {
        string ILessonPhrase.Text { get { return Content.Text; } }
    }

    public partial class LessonCode : ILessonCode
    {
        string ILessonCode.Text { get { return Content.Text; } }
    }

    public partial class LessonGoal : ILessonGoal
    {
        IList<ILessonParagraph> ILessonGoal.Paragraphs { get { return Paragraphs.Cast<ILessonParagraph>().ToList(); } }
    }

    public partial class LessonSummary : ILessonSummary
    {
        IList<ILessonParagraph> ILessonSummary.Paragraphs { get { return Paragraphs.Cast<ILessonParagraph>().ToList(); } }
    }

    public partial class LessonTest : ILessonTest
    {
        ILessonCode ILessonTest.Code { get { return Code; } }
    }

    public partial class LessonExplanation : ILessonExplanation
    {
        IList<ILessonCodeExplanation> ILessonExplanation.CodeExplanations { get { return CodeExplanations.Cast<ILessonCodeExplanation>().ToList(); } }
    }

    public partial class LessonCodeExplanation : ILessonCodeExplanation
    {
        ILessonCodeExplanationQuote ILessonCodeExplanation.CodeQuote { get { return CodeQuote; } }
        IList<ILessonPhrase> ILessonCodeExplanation.Phrases { get { return Phrases.Cast<ILessonPhrase>().ToList(); } }
    }

    public partial class LessonCodeExplanationQuote : ILessonCodeExplanationQuote
    {
        string ILessonCodeExplanationQuote.Text { get { return Content.Text; } }
    }

    public partial class LessonFile : ILessonFile
    {
        ILessonFileMethodReference ILessonFile.FileMethodReference { get { return FileMethodReference; } }
        ILessonCode ILessonFile.Code { get { return Code; } }
    }

    public partial class LessonFileMethodReference : ILessonFileMethodReference
    {
        ILessonFileName ILessonFileMethodReference.FileName { get { return FileName; } }
        ILessonMethodName ILessonFileMethodReference.MethodName { get { return MethodName; } }
    }

    public partial class LessonFileName : ILessonFileName
    {
        string ILessonFileName.Text { get { return Content.Text; } }
    }

    public partial class LessonMethodName : ILessonMethodName
    {
        string ILessonMethodName.Text { get { return Content.Text; } }
    }

}

