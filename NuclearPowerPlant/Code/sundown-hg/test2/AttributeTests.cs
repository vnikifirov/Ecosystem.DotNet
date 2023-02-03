using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using corelib;
using NUnit.Framework;

namespace dsltools
{
    [TestFixture]
    public class AttributeTests
    {
        #region Example of usage lexer
        public class RTList
        {
            public string[] parameters;

            public RTList(string[] param)
            {
                parameters = param;
            }

            public static RTList init(LexemeStream lexs, out LexemeStream end)
            {
                end = lexs;
                if (!lexs.IsSatisfied("("))
                    return null;

#if !DOTNET_V11
                List<string> s = new List<string>();
#else
                ArrayList s = new ArrayList();
#endif
                
                while (lexs.IsMore)
                {
                    if (lexs.IsOr("word", "digit"))
                        s.Add(lexs[-1].Data);

                    if (lexs.IsSatisfied(")"))
                    {
                        end = lexs;
#if !DOTNET_V11
                        return new RTList(s.ToArray());
#else
                        return new RTList((string[])s.ToArray(typeof(string)));
#endif
                    }
                    else if (lexs.IsSatisfied(","))
                        continue;

                    break;
                }
                return null;
            }

        }

        public class RTRule
        {
            public string name;
            public string type;
            public RTList param;
            public string storage;

            public RTRule(string name, string type, RTList param, string storage)
            {
                this.name = name;
                this.type = type;
                this.param = param;
                this.storage = storage;
            }


            public static RTRule init(LexemeStream lexs, out LexemeStream end)
            {
                end = lexs;
                if (!lexs.IsSatisfied("word", "as", "word"))
                    return null;

                string name = lexs[-1].Data;
                string type = lexs[-3].Data;

                RTList list = RTList.init(lexs, out lexs);
                string storage = null;

                if (lexs.IsSatisfied("to", "word"))
                {
                    storage = lexs[-1].Data;
                }

                end = lexs;
                return new RTRule(name, type, list, storage);
            }

        }

        public class RTTupleRule
        {
            public string tupleName;
            public bool array;
            public RTRule[] rules;

            public RTTupleRule(string tupleName, bool array, RTRule[] rules)
            {
                this.tupleName = tupleName;
                this.array = array;
                this.rules = rules;              
            }

            public static RTTupleRule init(LexemeStream lexs, out LexemeStream end)
            {
                end = lexs;

                bool array;
                if (!lexs.IsSatisfied("word", "is", "("))
                {
                    if (!lexs.IsSatisfied("word", "[", "]", "is", "("))
                        return null;
                    else
                    {
                        array = true;
                    }
                }
                else                
                    array = false;               

                string name = end[0].Data;
#if !DOTNET_V11
                List<RTRule> s = new List<RTRule>();
#else
                ArrayList s = new ArrayList();
#endif

                while (lexs.IsMore)
                {
                    RTRule tup = RTRule.init(lexs, out lexs);
                    if (tup == null)
                        return null;

                    s.Add(tup);

                    if (lexs.IsSatisfied(")"))
                        break;
                    else if (lexs.IsSatisfied(","))
                        continue;

                    return null;
                }

                end = lexs;
                return new RTTupleRule(name, array, 
#if !DOTNET_V11
                    s.ToArray());
#else
                    (RTRule[])s.ToArray(typeof(RTRule)));
#endif
            }

            public static RTTupleRule initOld(LexemeStream lexs, out LexemeStream end)
            {
                end = lexs;

                bool array;
                if (!lexs.IsSatisfied("word", "contains"))
                {
                    if (!lexs.IsSatisfied("word", "[", "]", "contains"))
                        return null;
                    else
                    {
                        array = true;
                    }
                }
                else
                    array = false;

                RTList list = RTList.init(lexs, out lexs);
                if (list == null)
                    return null;

                if (!lexs.IsSatisfied("cast", "("))
                    return null;

                string name = end[0].Data;
#if !DOTNET_V11
                List<RTRule> s = new List<RTRule>();
#else
                ArrayList s = new ArrayList();
#endif
                while (lexs.IsMore)
                {
                    RTRule tup = RTRule.init(lexs, out lexs);
                    if (tup == null)
                        return null;

                    s.Add(tup);

                    if (lexs.IsSatisfied(")"))
                        break;
                    else if (lexs.IsSatisfied(","))
                        continue;

                    return null;
                }

                end = lexs;
                return new RTTupleRule(name, array,
#if !DOTNET_V11
                    s.ToArray());
#else
                    (RTRule[])s.ToArray(typeof(RTRule)));
#endif
            }
        }

        #endregion

        [Test]
        public void TestLexeme()
        {
            ILexerRule[] rules = {
                               new LexerRuleKeyword("to", true),
                               new LexerRuleKeyword("as", true),
                               new LexerRuleKeyword("is", true),
                               new LexerRuleKeyword("contains", true),
                               new LexerRuleKeyword("cast", true),
                               new LexerRuleKeyword("["),
                               new LexerRuleKeyword("]"),
                               new LexerRuleKeyword("("),
                               new LexerRuleKeyword(")"),
                               new LexerRuleKeyword(","),
                               new LexerRuleKeyword(";"),
                               new LexerRuleRegex("digit", @"\d+"),
                               new LexerRuleRegex("word", @"\w[\w|\d]*"),                               
                               new LexerRuleRegex("string", @"'[^']*'"),
                               new LexerRuleRegex("white", @"\s+", false)
                           };

            Lexer lex = new Lexer(rules, false);
            Lexer lex2 = new Lexer(rules);

            string testString1 = "A as Cart( a, 2, 3) to m";
            Lexeme[] lexemes1 = lex.Parse(testString1);
            Lexeme[] lex2emes1 = lex2.Parse(testString1);

            LexemeStream res;
            RTRule r = RTRule.init(new LexemeStream(lex2emes1, 0), out res);

            string testString2 = "M[] is (A as Cart( a, 2, 3) to m, D as Array)";
            Lexeme[] lexemes2 = lex2.Parse(testString2);

            RTTupleRule r2 = RTTupleRule.init(new LexemeStream(lexemes2, 0), out res);

            LexemeStream s = new LexemeStream(lexemes2);
            RTTupleRule r32 = RTTupleRule.init(s, out s);


            string rule3 = @"tozagr contains (zagr)  cast ( zagr as Cart(pvk,int,scale));
                          coeff[] contains (kpd, bet1, gamma) cast (kpd as Array, bet1 as Array, gamma as ParamTable); 
                          return[] contains (kpd, bet1, gamma) cast (kpd as Array, bet1 as Array, gamma as ParamTable)";

            Lexeme[] lexemes3 = lex2.Parse(rule3);
            RTTupleRule r33 = RTTupleRule.initOld(new LexemeStream(lexemes3, 0), out s);

            Assert.IsNotNull(r);
            Assert.IsNotNull(r2);
            Assert.IsNotNull(r32);
            Assert.IsNotNull(r33);
        }



        [Test]
        public void TestTupleMaps()
        {
            TupleMaps m = new TupleMaps("a as aa, b as bb, [0] as dd");

            string str, name;

            Assert.IsTrue(m.IsMapped(0, out str, out name));
            Assert.AreEqual("dd", name);
            Assert.IsTrue(m.IsMapped(null, "a", out str, out name));
            Assert.AreEqual("aa", name);
            Assert.IsTrue(m.IsMapped(null, "b", out str, out name));
            Assert.AreEqual("bb", name);
        }
    }
}
