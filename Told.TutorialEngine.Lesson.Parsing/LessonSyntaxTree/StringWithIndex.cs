﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Told.TutorialEngine.Lesson.Parsing.LessonSyntaxTree
{
    public class StringWithIndex
    {
        public string Source { get; private set; }
        public int Index { get; private set; }
        public int Length { get; private set; }

        public string Text { get { return Source.Substring(Index, Length); } }

        public StringWithIndex Substring(int index, int? length = null)
        {
            return new StringWithIndex(Source, Index + index, length ?? (Length - index));
        }

        public StringWithIndex(string source, int index, int length)
        {
            Source = source;
            Index = index;
            Length = length;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public static class ParseHelper
    {
        public static int GetIndexAfter(this StringWithIndex text)
        {
            return text.Index + text.Length;
        }

        public static StringWithIndex Trim(this StringWithIndex text, params char[] chars)
        {
            return text.TrimStart(chars).TrimEnd(chars);
        }

        public static StringWithIndex TrimEnd(this StringWithIndex text)
        {
            return TrimEnd(text, new char[0]);
        }

        public static StringWithIndex TrimEnd(this StringWithIndex text, params char[] chars)
        {
            var trimmed = text.Text.TrimEnd(chars);
            var lengthDiff = text.Length - trimmed.Length;

            return text.Substring(0, text.Length - lengthDiff);
        }

        public static StringWithIndex TrimEnd(this StringWithIndex text, params string[] starts)
        {
            var trimmed = text.Text;

            foreach (var s in starts.OrderByDescending(s => s.Length))
            {
                if (trimmed.EndsWith(s))
                {
                    trimmed = trimmed.ReplaceEnd(s, "");
                    break;
                }
            }

            var lengthDiff = text.Length - trimmed.Length;

            return text.Substring(0, text.Length - lengthDiff);
        }

        public static StringWithIndex TrimStart(this StringWithIndex text, params char[] chars)
        {
            var trimmed = text.Text.TrimStart(chars);
            var lengthDiff = text.Length - trimmed.Length;

            return text.Substring(lengthDiff, text.Length - lengthDiff);
        }

        public static StringWithIndex TrimStart(this StringWithIndex text, params string[] starts)
        {
            var trimmed = text.Text;

            foreach (var s in starts.OrderByDescending(s => s.Length))
            {
                if (trimmed.StartsWith(s))
                {
                    trimmed = trimmed.ReplaceStart(s, "");
                    break;
                }
            }

            var lengthDiff = text.Length - trimmed.Length;

            return text.Substring(lengthDiff, text.Length - lengthDiff);
        }

        public static List<StringWithIndex> Split(this StringWithIndex text, params string[] separators)
        {
            var parts = SplitWithoutModification(text, separators);
            var trimmed = parts.Select(p => p.TrimStart(separators)).ToList();

            return trimmed;
        }

        public static List<StringWithIndex> SplitLines(this StringWithIndex text)
        {
            return text.Split("\r\n");
        }

        public static List<StringWithIndex> SplitAfterFirstLine(this StringWithIndex text)
        {
            var area = text.Text;
            var lineBreakIndex = area.IndexOf("\r\n");

            return new List<StringWithIndex>() {
                text.Substring(0, lineBreakIndex),
                text.Substring(lineBreakIndex+2)
            };
        }

        public static List<StringWithIndex> SplitWithoutModificationRegex(this StringWithIndex text, string regexStr)
        {
            var parts = new List<StringWithIndex>();

            var regex = new Regex(regexStr);
            var area = text.Text;

            var prevIndex = 0;
            var m = regex.Match(area);

            while (m.Success)
            {
                var index = m.Index;

                parts.Add(text.Substring(prevIndex, index - prevIndex));

                prevIndex = index;
                m = regex.Match(area, index + 1);
            }

            // Add last part
            parts.Add(text.Substring(prevIndex, area.Length - prevIndex));

            return parts;
        }

        public static List<StringWithIndex> SplitWithoutModification(this StringWithIndex text, params string[] separatorStarts)
        {
            var parts = new List<StringWithIndex>();

            var area = text.Text;

            var prevIndex = 0;
            var nextIndex = -1;
            nextIndex = area.IndexOfAny(separatorStarts);

            while (nextIndex >= 0)
            {
                // Add previous part
                parts.Add(new StringWithIndex(text.Source, text.Index + prevIndex, nextIndex - prevIndex));

                prevIndex = nextIndex;
                nextIndex = area.IndexOfAny(separatorStarts, nextIndex + 1);
            }

            // Add last part
            parts.Add(new StringWithIndex(text.Source, text.Index + prevIndex, text.Length - prevIndex));

            return parts;
        }

        public static int IndexOfAny(this string text, string[] values, int startIndex = 0)
        {
            // NOTE: For many values, this could perform poorly
            var indices = values.Select(v => text.IndexOf(v, startIndex)).Where(index => index >= 0);

            if (!indices.Any()) { return -1; }

            return indices.Min();
        }

        public static List<StringWithIndex> GetLines(this StringWithIndex text)
        {
            return Split(text, "\n").Where(l => !StringHelper.IsNullOrWhiteSpace(l.Text)).Select(l => l.TrimEnd('\r')).ToList();
        }

        public static string[] GetLines(this string text)
        {
            return text.Split('\n').Where(l => !StringHelper.IsNullOrWhiteSpace(l)).Select(l => l.TrimEnd('\r')).ToArray();
        }

        public static StringWithIndex GetFirstLineValue(this IEnumerable<StringWithIndex> lines, string lineStartMatch)
        {
            return lines.Where(l => l.Text.StartsWith(lineStartMatch)).Select(l => l.GetValueAfter(lineStartMatch)).First();
        }

        public static string GetFirstLineValue(this IEnumerable<string> lines, string lineStartMatch)
        {
            return lines.Where(l => l.StartsWith(lineStartMatch)).Select(l => l.GetValueAfter(lineStartMatch)).First();
        }

        public static StringWithIndex GetValueAfter(this StringWithIndex text, string lineStartMatch)
        {
            if (!text.Text.StartsWith(lineStartMatch))
            {
                return null;
            }
            else
            {
                return text.ReplaceStart(lineStartMatch, "");
            }
        }

        public static string GetValueAfter(this string text, string lineStartMatch)
        {
            if (!text.StartsWith(lineStartMatch))
            {
                return "";
            }
            else
            {
                return text.ReplaceStart(lineStartMatch, "");
            }
        }

        public static StringWithIndex ReplaceStart(this StringWithIndex text, string match, string replacement)
        {
            if (text.Text.StartsWith(match))
            {
                return text.Substring(match.Length);
            }
            else
            {
                return text;
            }
        }

        public static string ReplaceStart(this string text, string match, string replacement)
        {
            if (text.StartsWith(match))
            {
                return text.Substring(match.Length);
            }
            else
            {
                return text;
            }
        }


        public static StringWithIndex ReplaceEnd(this StringWithIndex text, string match, string replacement)
        {
            if (text.Text.EndsWith(match))
            {
                return text.Substring(0, text.Text.Length - match.Length);
            }
            else
            {
                return text;
            }
        }

        public static string ReplaceEnd(this string text, string match, string replacement)
        {
            if (text.EndsWith(match))
            {
                return text.Substring(0, text.Length - match.Length);
            }
            else
            {
                return text;
            }
        }
    }

    public static class StringHelper
    {
        public static bool IsNullOrWhiteSpace(string text)
        {
            return string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text.Trim());
        }
    }

}
