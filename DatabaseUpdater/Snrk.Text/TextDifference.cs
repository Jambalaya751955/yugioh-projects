using System;
using System.Collections.Generic;
using System.Linq;

namespace Snrk.Text
{
    public static class Text
    {
        public static string ReplaceWhitespaceblock(this string str, char newValue)
        {
            return str.ReplaceWhitespaceblock(newValue.ToString());
        }

        public static string ReplaceWhitespaceblock(this string str, string newValue)
        {
            string result = str;
            for (int i = result.Length - 1; i > 0; --i)
                if (Char.IsWhiteSpace(result[i]))
                    result = result.Remove(i, 1).Insert(i, Char.IsWhiteSpace(result[i - 1]) ? "" : newValue);
            return result;
        }

        public static int CountWhitespaceStart(this string str)
        {
            int count = 0;
            foreach (char c in str)
                if (!Char.IsWhiteSpace(c))
                    break;
                else
                    ++count;
            return count;
        }

        public static string GetBetween(this string str, string begin, string end, bool include = false)
        {
            if (String.IsNullOrEmpty(str))
                return "";

            string result = "";
            for (int iBegin = str.IndexOf(begin), iEnd = 0; iBegin >= 0 && iEnd >= 0; iBegin = str.IndexOf(begin))
            {
                string tmp = str.Substring(iBegin + begin.Length);
                iEnd = tmp.IndexOf(end);
                if (iEnd < 0)
                    break;

                tmp = tmp.Substring(0, iEnd);
                str = str.Remove(0, str.IndexOf(tmp) + tmp.Length);
                result += tmp;
            }

            return result;
        }
    }

    public class OldDifference
    {
        public static void Compute(ref string before, ref string after)
        {
            int bindex = 0;
            int aindex = 0;

            List<string> wordsBefore = before.Split(' ', '\n', '\r').Where(s => !String.IsNullOrWhiteSpace(s)).ToList();
            List<string> wordsAfter = after.Split(' ', '\n', '\r').Where(s => !String.IsNullOrWhiteSpace(s)).ToList();

            while (wordsBefore.Count > 0 && wordsAfter.Count > 0)
            {
                if (wordsBefore[0] == wordsAfter[0])
                {
                    int bj = bindex + wordsBefore[0].Length + 1;
                    string bsub = bj > before.Length ? "" : before.Substring(bj, before.Length - bindex - wordsBefore[0].Length - 1);
                    int bi = wordsBefore.Count > 1 ? bsub.TrimStart().IndexOf(wordsBefore[1]) : 0;
                    bindex += wordsBefore[0].Length + bi + 1 + bsub.CountWhitespaceStart();

                    int aj = aindex + wordsAfter[0].Length + 1;
                    string asub = aj > after.Length ? "" : after.Substring(aj, after.Length - aindex - wordsAfter[0].Length - 1);
                    int ai = wordsAfter.Count > 1 ? asub.TrimStart().IndexOf(wordsAfter[1]) : 0;
                    aindex += wordsAfter[0].Length + ai + 1 + asub.CountWhitespaceStart();

                    wordsBefore.RemoveAt(0);
                    wordsAfter.RemoveAt(0);
                }
                else
                {
                    var ntc = new List<string>(wordsAfter);
                    int tcnt = -1;
                    string twrd = "";
                    int tNumEqual = -1;
                    for (int i = 0; i < wordsBefore.Count; ++i)
                    {
                        int _tNumEqual = 0;
                        var _ntc = new List<string>(wordsAfter);
                        string _twrd = "";
                        while (_ntc.Count > 0)
                        {
                            if (wordsBefore[i] == _ntc[0])
                            {
                                int count = i;
                                for (int j = 0; j < _ntc.Count && count < wordsBefore.Count && wordsBefore[count] == _ntc[j]; ++count, ++j) ;
                                _tNumEqual = count - i;
                                break;
                            }
                            _twrd += ' ' + _ntc[0];
                            _ntc.RemoveAt(0);
                        }
                        _twrd = _twrd.TrimStart();
                        int _tcnt = wordsAfter.Count - _ntc.Count;
                        if (_tcnt <= 0 && tcnt <= 0)
                        {
                            ntc = new List<string>(wordsAfter);
                            tcnt = 0;
                            twrd = "";
                            break;
                        }
                        if (_tcnt > 0 && (tcnt < 0 || _tcnt < tcnt) && _tNumEqual > tNumEqual)
                        {
                            ntc = _ntc;
                            tcnt = _tcnt;
                            twrd = _twrd;
                            tNumEqual = _tNumEqual;
                        }
                        if (tcnt == 1)
                            break;
                    }

                    if (tcnt > 0)
                    {
                        if (twrd.Length > 0)
                        {
                            after = after.Remove(aindex, twrd.Length).Insert(aindex, string.Format("**`+{0}`**", twrd));
                        }
                        //if (ntc.Count > 0)
                        wordsAfter = new List<string>(ntc);
                    }

                    var nsc = new List<string>(wordsBefore);
                    int scnt = -1;
                    string swrd = "";
                    int sNumEqual = -1;
                    for (int i = 0; i < wordsAfter.Count; ++i)
                    {
                        int _sNumEqual = 0;
                        var _nsc = new List<string>(wordsBefore);
                        string _swrd = "";
                        while (_nsc.Count > 0)
                        {
                            if (_nsc[0] == wordsAfter[i])
                            {
                                int count = i;
                                for (int j = 0; j < _nsc.Count && count < wordsAfter.Count && wordsAfter[count] == _nsc[j]; ++count, ++j) ;
                                _sNumEqual = count - i;
                                break;
                            }
                            _swrd += ' ' + _nsc[0];
                            _nsc.RemoveAt(0);
                        }
                        _swrd = _swrd.TrimStart();
                        int _scnt = wordsBefore.Count - _nsc.Count;
                        if (_scnt <= 0 && scnt <= 0)
                        {
                            nsc = new List<string>(wordsBefore);
                            scnt = 0;
                            swrd = "";
                            break;
                        }
                        if (_scnt > 0 && (scnt < 0 || _scnt < scnt) && _sNumEqual > sNumEqual)
                        {
                            nsc = _nsc;
                            scnt = _scnt;
                            swrd = _swrd;
                            sNumEqual = _sNumEqual;
                        }
                        if (scnt <= tcnt)
                            break;
                    }

                    if (scnt > 0)
                    {
                        if (swrd.Length > 0)
                        {
                            before = before.Remove(bindex, swrd.Length).Insert(bindex, string.Format("**`-{0}`**", swrd));
                        }
                        wordsBefore = new List<string>(nsc);
                    }
                }

                if (wordsAfter.Count <= 0)
                {
                    string end = string.Join(" ", wordsBefore);
                    before = before.Remove(bindex, end.Length).Insert(bindex, string.Format("**`-{0}`**", end));
                }
                if (wordsBefore.Count <= 0)
                {
                    string end = string.Join(" ", wordsBefore);
                    after = after.Remove(aindex, end.Length).Insert(aindex, string.Format("**`+{0}`**", end));
                }
            }
        }
    }

    public static class TextDifference
    {
        private class Info
        {
            public string Text
            {
                get { return m_Text; }
                set
                {
                    m_Text = value;
                    Words = m_Text.Split(new[] { ' ', '\n', '\r', '\'' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
            public string FormatStart { get; set; }
            public string FormatEnd { get; set; }
            public List<string> Words { get; private set; }

            private int m_Index = 0;
            private string m_Text;

            public Info(string text, TextDifference.Format format)
            {
                Text = text;
                FormatStart = format.Start;
                FormatEnd = format.End;
            }

            public void RemoveFirstWord()
            {
                int startIndex = m_Index + Words[0].Length + 1;
                string subString = startIndex > m_Text.Length ? "" : m_Text.Substring(startIndex, m_Text.Length - m_Index - Words[0].Length - 1);
                int nextWordIndex = Words.Count > 1 ? subString.TrimStart().IndexOf(Words[1]) : 0;
                m_Index += Words[0].Length + nextWordIndex + 1 + subString.CountWhitespaceStart();

                Words.RemoveAt(0);
            }

            public void Format(List<string> otherWords)
            {
                var ntc = new List<string>(Words);
                int tcnt = -1;
                List<string> twrd = null;
                int tNumEqual = -1;
                for (int i = 0; i < otherWords.Count; ++i)
                {
                    int _tNumEqual = 0;
                    var _ntc = new List<string>(Words);
                    var _twrd = new List<string>();
                    while (_ntc.Count > 0)
                    {
                        if (otherWords[i] == _ntc[0])
                        {
                            int count = i;
                            for (int j = 0; j < _ntc.Count && count < otherWords.Count && otherWords[count] == _ntc[j]; ++count, ++j) ;
                            _tNumEqual = count - i;
                            break;
                        }
                        _twrd.Add(_ntc[0]);
                        _ntc.RemoveAt(0);
                    }
                    int _tcnt = Words.Count - _ntc.Count;
                    if (_tcnt <= 0 && tcnt <= 0)
                    {
                        ntc = new List<string>(Words);
                        tcnt = 0;
                        twrd = null;
                        break;
                    }
                    if (_tcnt > 0 && (tcnt < 0 || _tcnt < tcnt) && _tNumEqual > tNumEqual)
                    {
                        ntc = _ntc;
                        tcnt = _tcnt;
                        twrd = _twrd;
                        tNumEqual = _tNumEqual;
                    }
                    if (tcnt == 1)
                        break;
                }

                if (tcnt > 0)
                {
                    if (twrd != null && twrd.Count > 0)
                    {
                        string text = m_Text.Substring(m_Index);

                        int i = m_Index;
                        foreach (string word in twrd)
                        {
                            int len = text.Length;
                            text = text.TrimStart();
                            len -= text.Length;
                            text = text.Substring(word.Length);
                            i += len + word.Length;
                        }

                        m_Text = m_Text.Insert(i, FormatEnd).Insert(m_Index, FormatStart);
                    }
                    Words = new List<string>(ntc);
                }
            }

            public void FormatEnding()
            {
                if (Words.Count == 0)
                    return;

                string end = string.Join(" ", Words);
                m_Text = m_Text.Remove(m_Index, end.Length).Insert(m_Index, FormatStart + end + FormatEnd);
            }
        }

        public class Format
        {
            public string Start { get; set; }
            public string End { get; set; }

            /// <summary>
            /// Formatting for text differences.
            /// </summary>
            /// <param name="start">String that is inserted at the beginning of a removed/added string.</param>
            /// <param name="end">String that is inserted at the end of a removed/added string.</param>
            public Format(string start, string end)
            {
                Start = start;
                End = end;
            }
        }
        
        /// <summary>
        /// Computes the differences between two strings and highlights them with a custom formatting.
        /// </summary>
        /// <param name="before">The old string.</param>
        /// <param name="after">The new string.</param>
        /// <param name="removed">Formatting for removed strings.</param>
        /// <param name="added">Formatting for added strings.</param>
        public static void Compute(ref string before, ref string after, TextDifference.Format removed, TextDifference.Format added)
        {
            var Before = new TextDifference.Info(before, removed);
            var After = new TextDifference.Info(after, added);
            
            while (Before.Words.Count > 0 && After.Words.Count > 0)
            {
                if (Before.Words[0] == After.Words[0])
                {
                    Before.RemoveFirstWord();
                    After.RemoveFirstWord();
                }
                else
                {
                    After.Format(Before.Words);
                    Before.Format(After.Words);
                }

                if (After.Words.Count <= 0)
                {
                    Before.FormatEnding();
                }

                if (Before.Words.Count <= 0)
                {
                    After.FormatEnding();
                }
            }

            before = Before.Text;
            after = After.Text;
        }
    }
}
