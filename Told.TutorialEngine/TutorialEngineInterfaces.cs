using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Told.TutorialEngine.Lesson;

namespace Told.TutorialEngine
{
    public interface ITutorialController
    {
        void AddPresenter(ITutorialPresenter presenter);
        void LoadLesson(ILesson lesson);
    }

    public interface ITutorialPresenter { }

    public interface ICodeEditorPresenter : ITutorialPresenter
    {
        void ShowContext(string filename, string methodName);


        //event EventHandler<StringEventArgs> MethodBodyChanged;

        //void SetMethodBody(string filename, string methodName, string body);
    }

    public class StringEventArgs : EventArgs
    {
        public string Text { get; private set; }
        public StringEventArgs(string text)
        {
            Text = text;
        }
    }

    public class Paragraph
    {
        public IList<ParagraphItem> Items { get; private set; }

        public Paragraph(IList<ParagraphItem> items = null)
        {
            Items = items ?? new List<ParagraphItem>();
        }
    }

    public class ParagraphItem
    {
        public ParagraphItemKind Kind { get; private set; }
        public string Text { get; private set; }
        public string Code { get; private set; }

        public ParagraphItem(string text, ParagraphItemKind kind)
        {
            Text = kind == ParagraphItemKind.Text ? text : "";
            Code = kind == ParagraphItemKind.Code ? text : "";
            Kind = kind;
        }
    }

    public enum ParagraphItemKind
    {
        Text,
        Code,
        BlankLine
    }

    public interface IInstructionPresenter : ITutorialPresenter
    {
        event EventHandler Next;
        event EventHandler ResetCode;

        void EnableNext(bool isEnabled);
        void EnableResetCode(bool isEnabled);

        void ShowInstructions(Paragraph paragraph);
        void ShowGoal(Paragraph paragraph);
        void ShowNotification(Paragraph paragraph);
    }

    public interface IGamePreviewPresenter : ITutorialPresenter
    {
        void ShowGameState(string gameStateName);
    }

    //public class Project
    //{
    //    public IList<ProjectFile> Files { get; private set; }

    //    public Project(IList<ProjectFile> files = null)
    //    {
    //        Files = files ?? new List<ProjectFile>();
    //    }
    //}

    //public class ProjectFile
    //{
    //    public string FilePath { get; private set; }
    //    public string Contents { get; private set; }

    //    public ProjectFile(string filePath, string contents)
    //    {
    //        FilePath = filePath;
    //        Contents = contents;
    //    }
    //}

    public interface IFileSystemPresenter : ITutorialPresenter
    {
        void CreateDefaultProject();
        void SetFile(string filePath, string contents);
        string GetFile(string filePath);

        //void OverwriteProjectContents(Project project);
    }
}
