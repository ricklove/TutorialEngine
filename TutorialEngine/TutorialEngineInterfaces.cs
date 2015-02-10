using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorialEngine
{
    public interface ITutorialController
    {
        void AddPresenter(ITutorialPresenter presenter);
    }

    public interface ITutorialPresenter { }

    public interface ICodeEditorPresenter : ITutorialPresenter
    {
        event EventHandler<string> MethodBodyChanged;

        void ShowContext(string filename, string methodName);
        void SetMethodBody(string filename, string methodName, string body);
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

        public ParagraphItem(string text, string code)
        {
            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(code)) { throw new ArgumentException("Paragraph Item must have either code or text"); }
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(code)) { throw new ArgumentException("Paragraph Item cannot have both code and text"); }

            Text = text ?? "";
            Code = code ?? "";
            Kind = string.IsNullOrEmpty(Code) ? ParagraphItemKind.Text : ParagraphItemKind.Code;
        }
    }

    public enum ParagraphItemKind
    {
        Text,
        Code
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
        void SetGameState(string gameStateName);
    }

    public class Project
    {
        public IList<ProjectFile> Files { get; private set; }

        public Project(IList<ProjectFile> files = null)
        {
            Files = files ?? new List<ProjectFile>();
        }
    }

    public class ProjectFile
    {
        public string FilePath { get; private set; }
        public string Contents { get; private set; }

        public ProjectFile(string filePath, string contents)
        {
            FilePath = filePath;
            Contents = contents;
        }
    }

    public interface IFileSystemPresenter : ITutorialPresenter
    {
        void OverwriteProjectContents(Project project);
    }
}
