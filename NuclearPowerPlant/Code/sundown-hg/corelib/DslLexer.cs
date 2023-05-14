using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace dsltools
{
    public struct Lexeme
    {
        public readonly int StartIdx;
        public readonly int EndIdx;

        public readonly int Line;
        public readonly int Pos;

        public readonly string Source;
        public readonly string Data;

        public readonly ILexerRule Rule;

        public Lexeme(ILexerRule rule, string s, int startIdx, int endIdx)
            : this(rule, s, startIdx, endIdx, "[unknown]")
        {
        }
        public Lexeme(ILexerRule rule, string s, int startIdx, int endIdx, string source)
            : this(rule, s, startIdx, endIdx, source, -1, -1)
        {
        }
        public Lexeme(ILexerRule rule, string s, int startIdx, int endIdx, string source, int line, int pos)
        {
            StartIdx = startIdx;
            EndIdx = endIdx;
            Line = line;
            Pos = pos;
            Source = source;
            Data = s;

            Rule = rule;
        }

        public override string ToString()
        {
            return String.Format("{1}: [{0}]", Data, Rule.Id);
        }
    }

    public struct LexemeStream
    {
        Lexeme[] _lexems;
        int _pos;

        public LexemeStream(Lexeme[] lexemes, int pos)
        {
            _lexems = lexemes;
            _pos = pos;
        }

        public LexemeStream(Lexeme[] lexemes)
            : this (lexemes, 0)
        {

        }

        public LexemeStream(LexemeStream s)
            : this (s._lexems, s._pos)
        {
        }

        public bool IsSatisfied(string id1)
        {
            if ((IsMore) && (Current.Rule.Id == id1))
            {
                MoveNext();
                return true;
            }
            return false;
        }

        public bool IsSatisfied(string id1, string id2)
        {
            int pos = _pos;
            if (!(IsSatisfied(id1) && IsSatisfied(id2)))
            {
                _pos = pos;
                return false;
            }
            return true;
        }

        public bool IsSatisfied(string id1, string id2, string id3)
        {
            int pos = _pos;
            if (!(IsSatisfied(id1) && IsSatisfied(id2) && IsSatisfied(id3)))
            {
                _pos = pos;
                return false;
            }
            return true;
        }

        public bool IsSatisfied(params string[] ids)
        {
            int pos = _pos;
            bool s = true;
            for (int i = 0; i < ids.Length; i++)
            {
                s = s && IsSatisfied(ids[i]);
            }
            if (!s)
                _pos = pos;
            return s;
        }

        public bool IsOr(string id1, string id2)
        {
            return IsSatisfied(id1) || IsSatisfied(id2);
        }

        public Lexeme this[int i]
        {
            get { return _lexems[_pos + i]; }
        }

        public Lexeme Current
        {
            get { return _lexems[_pos]; }
        }

        public bool MoveNext()
        {
            _pos++;
            return IsMore;
        }

        public bool IsMore
        {
            get { return _lexems.Length > _pos; }
        }

        public void SetToFirst()
        {
            _pos = 0;
        }        

    }

    public interface ILexerRule
    {
        bool Include { get; }
        string Id { get; }
        bool Check(string data, int startIndex, out int lengthLex);
    }


    public class Lexer
    {
        ILexerRule[] _rules;
        bool _skipWhite;

        public Lexer(ILexerRule[] rules)
            : this(rules, true)
        {

        }

        public Lexer(ILexerRule[] rules, bool skip)
        {
            _rules = rules;
            _skipWhite = skip;
        }

        public Lexeme[] Parse(string st)
        {
#if !DOTNET_V11
            List<Lexeme> _lexemes = new List<Lexeme>();
#else
            ArrayList _lexemes = new ArrayList();
#endif
            int startPos = 0;
            int lengthLex = 0;
            int end = 0;

            while (end < st.Length)
            {
                for (int i = 0; i < _rules.Length; i++)
                {
                    if (_rules[i].Check(st, startPos, out lengthLex))
                    {
                        if (lengthLex < 1)
                            throw new ArgumentException("Error in rule");

                        end = startPos + lengthLex;
                        if ((!_skipWhite) || (_rules[i].Include))
                            _lexemes.Add(new Lexeme(
                                _rules[i], st.Substring(startPos, lengthLex), startPos, end));

                        startPos = end;
                        goto next;
                    }
                }

                throw new ArgumentException(String.Format("Unknown lexeme: {0}", st.Substring(startPos)));
            next: ;
            }

#if !DOTNET_V11
            return _lexemes.ToArray();
#else
            return (Lexeme[])_lexemes.ToArray(typeof(Lexeme));
#endif
        }
    }

    public class LexerRuleKeyword : ILexerRule
    {
        string _cmp;
        bool _checkNextLetter;
        static public implicit operator LexerRuleKeyword(string s)
        {
            return new LexerRuleKeyword(s);
        }

        public LexerRuleKeyword(string str)
            : this (str, false)
        {

        }
        public LexerRuleKeyword(string str, bool checkNextLetter)
        {
            _checkNextLetter = checkNextLetter;
            _cmp = str;
        }

        public bool Check(string array, int startIndex, out int lengthLex)
        {
            int j = 0;
            lengthLex = 0;
            for (int i = startIndex; i < array.Length && j < _cmp.Length; i++, j++)
                if (array[i] != _cmp[j])
                    return false;

            if (j == _cmp.Length)
            {
                if (_checkNextLetter)
                {
                    // Если задана проверка на `is` то исключить совпадение с `isa`
                    if (startIndex + lengthLex < array.Length)
                    {
                        if (Char.IsLetterOrDigit((array[startIndex + j])))
                            return false;
                    }
                }
                lengthLex = j;                
                return true;
            }
            return false;
        }

        public bool Include { get { return true; } }
        public string Id { get { return _cmp; } }

        public override string ToString()
        {
            return String.Format("LexerRuleKeyword: {0}", _cmp);
        }
    }

    public class LexerRuleRegex : ILexerRule
    {
        Regex _check;
        bool _include;
        string _id;

        public LexerRuleRegex(string id, string regex)
            : this (id, regex, true)
        {

        }

        public LexerRuleRegex(string id, string regex, bool include)
        {
            _id = id;
            _check = new Regex(regex);
            _include = include;
        }

        public bool Check(string array, int startIndex, out int lengthLex)
        {
            lengthLex = 0;
            Match m = _check.Match(array, startIndex);
            if (!m.Success)
                return false;

            if (m.Index != startIndex)
                return false;

            lengthLex =  m.Length;
            return true;
        }

        public bool Include { get { return _include; } }
        public string Id { get { return _id; } }

        public override string ToString()
        {
            return String.Format("LexerRuleRegex: {0} [{1}]", Id, _check.ToString());
        }

    }
}
