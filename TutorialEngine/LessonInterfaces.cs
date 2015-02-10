using System.Collections.Generic;
using System.Linq;

namespace TutorialEngine
{
    public interface ILesson
    {
        ILessonDocument Document { get; }
    }

    public interface ILessonDocument
    {
        ILessonDocumentTitle Title { get; }
        IList<ILessonStep> Steps { get; }
    }

    public interface ILessonDocumentTitle
    {
        string Text { get; }
    }

    public interface ILessonStep
    {
        ILessonStepTitle Title { get; }
        ILessonInstructions Instructions { get; }
        ILessonGoal Goal { get; }
        ILessonSummary Summary { get; }
        ILessonTest Test { get; }
        ILessonExplanation Explanation { get; }
        ILessonFile File { get; }
    }

    public interface ILessonStepTitle
    {
        string Text { get; }
    }

    public interface ILessonInstructions
    {
        IList<ILessonParagraph> Paragraphs { get; }
    }

    public interface ILessonParagraph
    {
        IList<ILessonPhrase> Phrases { get; }
        ILessonCode Code { get; }
    }

    public interface ILessonPhrase
    {
        string Text { get; }
    }

    public interface ILessonCode
    {
        string Text { get; }
    }

    public interface ILessonGoal
    {
        IList<ILessonParagraph> Paragraphs { get; }
    }

    public interface ILessonSummary
    {
        IList<ILessonParagraph> Paragraphs { get; }
    }

    public interface ILessonTest
    {
        ILessonCode Code { get; }
    }

    public interface ILessonExplanation
    {
        IList<ILessonCodeExplanation> CodeExplanations { get; }
    }

    public interface ILessonCodeExplanation
    {
        ILessonCodeExplanationQuote CodeQuote { get; }
        IList<ILessonPhrase> Phrases { get; }
    }

    public interface ILessonCodeExplanationQuote
    {
        string Text { get; }
    }

    public interface ILessonFile
    {
        ILessonFileMethodReference FileMethodReference { get; }
        ILessonCode Code { get; }

        string Path { get; }
        string Context { get; }
    }

    public interface ILessonFileMethodReference
    {
        string Text { get; }
    }

}


namespace TutorialEngine.LessonSyntaxTree
{
    public partial class Lesson : ILesson
    {
        ILessonDocument ILesson.Document { get { return Document; } }
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
        string ILessonFileMethodReference.Text { get { return Content.Text; } }
    }

}
