using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Told.TutorialEngine.Lesson;

namespace Told.TutorialEngine
{
    public class TutorialController : ITutorialController
    {
        private ILessonTree _lesson;
        private int? _stepIndex;

        private ILessonStep _step;
        private StepState _stepState;

        private List<IInstructionPresenter> _instructionPresenters = new List<IInstructionPresenter>();
        private List<IFileSystemPresenter> _fileSystemPresenters = new List<IFileSystemPresenter>();


        public bool IsLessonLoaded
        {
            get { return _lesson != null; }
        }


        public void AddPresenter(ITutorialPresenter presenter)
        {
            var wasAdded = false;

            if (presenter is IInstructionPresenter)
            {
                var p = presenter as IInstructionPresenter;
                _instructionPresenters.Add(p);

                p.Next += instructionPresenter_Next;
                p.ResetCode += instructionPresenter_ResetCode;
                wasAdded = true;
            }

            if (presenter is IFileSystemPresenter)
            {
                var p = presenter as IFileSystemPresenter;
                _fileSystemPresenters.Add(p);
                wasAdded = true;
            }

            if (!wasAdded)
            {
                throw new NotImplementedException();
            }
        }

        void instructionPresenter_ResetCode(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void instructionPresenter_Next(object sender, EventArgs e)
        {
            GotoNextState();
        }

        public void LoadLesson(ILessonTree lesson)
        {
            _lesson = lesson;
            LoadNextStep();
        }

        private void LoadNextStep()
        {
            if (!_stepIndex.HasValue)
            {
                _stepIndex = 0;
            }
            else { _stepIndex++; }

            if (_stepIndex > _lesson.Document.Steps.Count - 1) { _stepIndex = 0; }

            _step = _lesson.Document.Steps[_stepIndex.Value];

            // Load File
            LoadFileSystemState();

            ShowInstructions();
            _stepState = StepState.Instructions;
        }

        public class FileState
        {
            public string FileName { get; set; }
            public string MethodName { get; set; }
            public string Content { get; set; }
        }

        private void LoadFileSystemState()
        {
            var fileStates = new List<FileState>();

            // Get file states up to this point
            for (int i = 0; i <= _stepIndex; i++)
            {
                var s = _lesson.Document.Steps[i];

                if (i < _stepIndex)
                {
                    // Use FILE
                    if (s.File != null)
                    {
                        var fRef = s.File.FileMethodReference;
                        fileStates.Add(new FileState() { FileName = fRef.FileName.Text, MethodName = fRef.MethodName.Text, Content = s.File.Code.Text });
                    }

                    // Use TEST
                    if (s.Test != null)
                    {
                        var lastFileState = fileStates.Last();
                        fileStates.Add(new FileState() { FileName = lastFileState.FileName, MethodName = lastFileState.MethodName, Content = s.Test.Code.Text });
                    }

                }
                else
                {
                    // Use FILE 
                    if (s.File != null)
                    {
                        var fRef = s.File.FileMethodReference;
                        fileStates.Add(new FileState() { FileName = fRef.FileName.Text, MethodName = fRef.MethodName.Text, Content = s.File.Code.Text });
                    }
                }
            }

            // Load the file state
            foreach (var f in _fileSystemPresenters)
            {
                f.CreateDefaultProject();

                foreach (var state in fileStates)
                {
                    var fileText = f.GetFile(state.FileName);
                    var fileContent = SetContentsAtContext(fileText, state);
                    f.SetFile(state.FileName, fileContent);
                }
            }
        }

        private string SetContentsAtContext(string fileText, FileState state)
        {
            throw new NotImplementedException();
        }

        private void GotoNextState()
        {
            if (_stepState == StepState.Instructions)
            {
                ShowGoal();
                _stepState = StepState.Testing;
                return;
            }
            else if (_stepState == StepState.Testing)
            {
                var didPass = VerifyTest();

                if (!didPass)
                {
                    ShowNotification(CreateTestFailNotification());
                    return;
                }

                ShowSummary();
                _stepState = StepState.Summary;
                return;
            }
            else //if (_stepState == StepState.Summary)
            {
                LoadNextStep();
                _stepState = StepState.Instructions;
                return;
            }
        }

        private Paragraph CreateTestFailNotification()
        {
            throw new NotImplementedException();
        }

        private bool VerifyTest()
        {
            // Verfiy if the test if correct
            if (_step.Test == null)
            {
                return true;
            }


            throw new NotImplementedException();
            return false;
        }

        private void ShowInstructions()
        {
            var instructions = _step.Instructions;

            foreach (var presenter in _instructionPresenters)
            {
                presenter.ShowInstructions(CreateParagraph(instructions.Paragraphs));
                presenter.EnableNext(true);
            }
        }

        private void ShowGoal()
        {
            var goal = _step.Goal;

            foreach (var presenter in _instructionPresenters)
            {
                presenter.ShowGoal(CreateParagraph(goal.Paragraphs));
                presenter.EnableNext(true);
                presenter.EnableResetCode(true);
            }
        }

        private void ShowSummary()
        {
            var summary = _step.Summary;

            foreach (var presenter in _instructionPresenters)
            {
                presenter.ShowInstructions(CreateParagraph(summary.Paragraphs));
                presenter.EnableNext(true);
                presenter.EnableResetCode(false);
            }
        }

        private void ShowNotification(Paragraph message)
        {
            foreach (var presenter in _instructionPresenters)
            {
                presenter.ShowNotification(message);
            }
        }

        private Paragraph CreateParagraph(IList<ILessonParagraph> source)
        {
            var p = new Paragraph();

            foreach (var pNode in source)
            {
                if (pNode.Code != null)
                {
                    p.Items.Add(new ParagraphItem(pNode.Code.Text, ParagraphItemKind.Code));
                }

                foreach (var phrase in pNode.Phrases)
                {
                    p.Items.Add(new ParagraphItem(phrase.Text, ParagraphItemKind.Text));
                }

                p.Items.Add(new ParagraphItem("", ParagraphItemKind.BlankLine));
            }

            return p;
        }
    }

    public enum StepState
    {
        Instructions,
        Testing,
        Summary
    }
}
