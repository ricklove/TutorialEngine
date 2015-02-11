using System.Collections.Generic;
using System.Linq;

namespace Told.TutorialEngine.Lesson
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

