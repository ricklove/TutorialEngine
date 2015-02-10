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

    private Texture _headIcon;
    private Texture _codeIcon;
    private Texture _goalIcon;
    private Texture _notificationIcon;

    private GUIStyle _codeStyle;

    private Vector2 _scrollPosition;
    private Vector2 _lastMaxScrollPosition;

    public List<ChatLine> _lines { get; private set; }
    public List<ChatLineGroup> _groupedLines { get; private set; }
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

        if (_headIcon == null)
        {
            _headIcon = Resources.Load<Texture>("dgi-monkey");
            _codeIcon = Resources.Load<Texture>("code");
            _goalIcon = Resources.Load<Texture>("goal");
            _notificationIcon = Resources.Load<Texture>("alert");

            _codeStyle = new GUIStyle();
        }

        if (_lines == null) { _lines = new List<ChatLine>(); }
        if (_groupedLines == null) { _groupedLines = new List<ChatLineGroup>(); }

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
                        pauseTime = chatDelay;
                    }

                    if (_lastMaxScrollPosition.y - _scrollPosition.y < 0.1f)
                    {
                        _scrollPosition.y = float.MaxValue;
                    }

                    // Regroup lines
                    _groupedLines = GroupLines(_lines);
                }
            }
        }


        // Scrolling Comments (with history)
        var isAtMax = false;
        if (_scrollPosition.y > float.MaxValue / 2) { isAtMax = true; }

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        if (isAtMax) { _lastMaxScrollPosition = _scrollPosition; }

        // Add lines
        var isFirst = true;

        foreach (var g in _groupedLines)
        {
            // New section
            GUILayout.BeginHorizontal();
            GUILayout.Label(g.Icon, GUILayout.Width(48), GUILayout.Height(48));
            GUILayout.BeginVertical();

            isFirst = false;

            foreach (var l in g.Lines)
            {
                if (l.Type == ChatLineType.Pause)
                {
                    continue;
                }
                if (l.Type == ChatLineType.Divider)
                {
                    continue;
                }

                if (l.Type == ChatLineType.BlankLine)
                {
                    GUILayout.Label("");
                    continue;
                }

                if (l.Type == ChatLineType.InstructionCommentLine
                    || l.Type == ChatLineType.GoalCommentLine
                    || l.Type == ChatLineType.NotificationCommentLine)
                {
                    GUILayout.Label(l.Text);
                }

                if (l.Type == ChatLineType.InstructionCodeLine
                    || l.Type == ChatLineType.GoalCodeLine
                    || l.Type == ChatLineType.NotificationCodeLine)
                {
                    GUILayout.Label(l.Text, _codeStyle);
                }

            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
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

    private List<ChatLineGroup> GroupLines(List<ChatLine> lines)
    {
        var groups = lines.GroupBy(l => l.Type).Select(g => new ChatLineGroup() { Lines = g.ToList() }).ToList();

        // Add pause and blanks back into groups
        for (int i = 0; i < groups.Count; i++)
        {
            if (i == 0) { continue; }

            var g = groups[i];
            var t = g.Lines.First().Type;
            var lastT = groups[i - 1].Lines.First().Type;

            if (t == ChatLineType.BlankLine
                || t == ChatLineType.Pause
                || t == lastT
                )
            {
                groups[i - 1].Lines.AddRange(g.Lines);
                groups.RemoveAt(i);
            }
        }

        foreach (var g in groups)
        {
            var t = g.Lines.First().Type;

            if (t == ChatLineType.InstructionCommentLine)
            {
                g.Icon = _headIcon;
            }
            else if (t == ChatLineType.InstructionCodeLine)
            {
                g.Icon = _codeIcon;
            }
            else if (t == ChatLineType.GoalCommentLine
                || t == ChatLineType.GoalCodeLine)
            {
                g.Icon = _goalIcon;
            }
            else if (t == ChatLineType.NotificationCommentLine
                || t == ChatLineType.NotificationCodeLine)
            {
                g.Icon = _notificationIcon;
            }
        }

        return groups;
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
            else if (item.Kind == ParagraphItemKind.Code)
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

public class ChatLineGroup
{
    public List<ChatLine> Lines { get; set; }
    public Texture Icon { get; set; }
}