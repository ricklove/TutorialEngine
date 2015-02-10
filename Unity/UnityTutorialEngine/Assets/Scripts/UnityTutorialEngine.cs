using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using TutorialEngine;
using System.Linq;

public class UnityTutorialEngine
{
    private TutorialController _tutorialController;

    public UnityTutorialEngine(UnityInstructionPresenterViewModel instructionPresenter)
    {
        _tutorialController = new TutorialController();

        // Set up presenters
        _tutorialController.AddPresenter(instructionPresenter);

        // Load sample lesson
        var sampleLessonText = TutorialEngine.Lessons.LessonLoader.LoadSampleLesson();
        var parser = new TutorialEngine.LessonParser();
        var sampleLesson = parser.ParseLesson(sampleLessonText);
        _tutorialController.LoadLesson(sampleLesson);
    }
}

public class UnityTutorialEngineEditorWindow : EditorWindow
{
    [MenuItem("Tutorial/Show Tutorial Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UnityTutorialEngineEditorWindow), true, "Tutorial");
    }

    private UnityInstructionPresenterViewModel _viewModel;
    private UnityTutorialEngine _engine;

    public UnityTutorialEngineEditorWindow()
    {
        _viewModel = new UnityInstructionPresenterViewModel();
        _engine = new UnityTutorialEngine(_viewModel);
    }

    private Vector2 _scrollPosition;
    public List<ChatLine> _lines { get; private set; }
    private float chatDelay = 3.0f;
    private float pauseTime = 0.0f;
    private double timeAtLastLine = 0f;

    public double GetTimeAbsolute()
    {
        return (System.DateTime.Now - new System.DateTime(2000, 1, 1)).TotalSeconds;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        //Debug.Log("EditorWindow.OnGUI called: " + GetTimeAbsolute());

        if (_lines == null) { _lines = new List<ChatLine>(); }

        // If it is time to update the chat
        if (GetTimeAbsolute() > timeAtLastLine + chatDelay + pauseTime)
        {
            //Debug.Log("Chat Delay Passed");

            timeAtLastLine = GetTimeAbsolute();
            pauseTime = 0;

            if (_lines.Count != _viewModel.Lines.Count)
            {
                // Remove any unwanted lines
                for (int i = 0; i < _lines.Count && i < _viewModel.Lines.Count; i++)
                {
                    if (_lines[i] != _viewModel.Lines[i])
                    {
                        _lines.RemoveAt(i);
                        i--;
                    }
                }

                // Add a missing line
                if (_lines.Count < _viewModel.Lines.Count)
                {
                    _lines.Add(_viewModel.Lines[_lines.Count]);

                    if (_lines.Last().Type == ChatLineType.Pause)
                    {
                        pauseTime = chatDelay * 2;
                    }
                }
            }
        }


        // Scrolling Comments (with history)
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        // Add lines
        foreach (var l in _lines)
        {
            if (l.Type == ChatLineType.Pause) { continue; }
            
            if (l.Type == ChatLineType.Divider) {
                GUILayout.Label("----------------");
            }

            GUILayout.Label(l.Text);
        }

        GUILayout.EndScrollView();


        // Show control buttons
        if (_viewModel.IsNextEnabled)
        {
            if (GUILayout.Button("Next"))
            {
                _viewModel.OnNext();
            }
        }

        if (_viewModel.IsResetCodeEnabled)
        {
            if (GUILayout.Button("Reset Code"))
            {
                _viewModel.OnResetCode();
            }
        }
    }
}

public class UnityInstructionPresenterViewModel : IInstructionPresenter
{
    // View Model
    public bool IsNextEnabled { get; private set; }
    public bool IsResetCodeEnabled { get; private set; }
    public Paragraph Goal { get; private set; }
    public Paragraph Instructions { get; private set; }
    public Paragraph Notification { get; private set; }

    public List<ChatLine> Lines { get; private set; }

    public UnityInstructionPresenterViewModel()
    {
        Lines = new List<ChatLine>();
    }

    private void SetInstructions(Paragraph p)
    {
        AddParagraph(p, ChatLineType.InstructionCommentLine, ChatLineType.InstructionCodeLine);
        Instructions = p;
    }

    private void SetGoal(Paragraph p)
    {
        AddParagraph(p, ChatLineType.GoalCommentLine, ChatLineType.GoalCodeLine);
        Goal = p;
    }

    private void SetNotification(Paragraph p)
    {
        AddParagraph(p, ChatLineType.NotificationCommentLine, ChatLineType.NotificationCodeLine);
        Notification = p;
    }

    private void AddParagraph(Paragraph p, ChatLineType commentType, ChatLineType codeType)
    {
        foreach (var item in p.Items)
        {
            if (item.Kind == ParagraphItemKind.Text)
            {
                Lines.AddRange(item.Text.GetLines()
                    .Select(l => new ChatLine(commentType, l)));
            }
            else if( item.Kind == ParagraphItemKind.Code)
            {
                Lines.AddRange(item.Code.GetLines()
                    .Select(l => new ChatLine(codeType, l)));
            }
            else
            {
                Lines.Add(ChatLine.BlankLine);
                Lines.Add(ChatLine.Pause);
            }
        }

        Lines.Add(ChatLine.Divider);
        Lines.Add(ChatLine.Pause);
    }


    public void OnNext()
    {
        if (_next != null) { _next(this, new System.EventArgs()); }
    }

    public void OnResetCode()
    {
        if (_resetCode != null) { _resetCode(this, new System.EventArgs()); }
    }

    // Interface Implementation
    private event System.EventHandler _next;
    private event System.EventHandler _resetCode;

    event System.EventHandler IInstructionPresenter.Next
    {
        add { _next += value; }
        remove { _next -= value; }
    }

    event System.EventHandler IInstructionPresenter.ResetCode
    {
        add { _resetCode += value; }
        remove { _resetCode -= value; }
    }

    void IInstructionPresenter.EnableNext(bool isEnabled)
    {
        IsNextEnabled = isEnabled;
    }

    void IInstructionPresenter.EnableResetCode(bool isEnabled)
    {
        IsResetCodeEnabled = isEnabled;
    }

    void IInstructionPresenter.ShowGoal(Paragraph paragraph)
    {
        SetGoal(paragraph);
    }

    void IInstructionPresenter.ShowInstructions(Paragraph paragraph)
    {
        SetInstructions(paragraph);
    }

    void IInstructionPresenter.ShowNotification(Paragraph paragraph)
    {
        SetNotification(paragraph);
    }



}

public class ChatLine
{
    public static ChatLine Pause = new ChatLine(ChatLineType.Pause, "");
    public static ChatLine BlankLine = new ChatLine(ChatLineType.BlankLine, "");
    public static ChatLine Divider = new ChatLine(ChatLineType.Divider, "");

    public ChatLineType Type { get; private set; }
    public string Text { get; private set; }

    public ChatLine(ChatLineType type, string text)
    {
        Type = type;
        Text = text;
    }
}

public enum ChatLineType
{
    BlankLine,
    Divider,
    Pause,

    InstructionCommentLine,
    InstructionCodeLine,

    GoalCommentLine,
    GoalCodeLine,

    NotificationCommentLine,
    NotificationCodeLine
}