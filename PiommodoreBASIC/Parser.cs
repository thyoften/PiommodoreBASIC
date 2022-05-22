using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiommodoreBASIC
{

    public class Parser
    {
        readonly string[] _statement_keywords = { "PRINT", "INPUT", "FOR", "IF", "GOTO", "GOSUB", "RETURN", "STOP", "POKE" };

        string[] _program;

        Stack<(ForStatement, int)> ForLoops = new Stack<(ForStatement, int)>();

        public Parser(params string[] lines)
        {
            _program = lines.Where(x => x.Length > 0 && !x.StartsWith("REM")).ToArray(); //remove empty lines and comments
        }

        public List<IStatement> ParseStatements()
        {
            List<IStatement> statements = new List<IStatement>();

            for (int i = 0; i < _program.Length; i++)
            { 

                var stmt = ParseSingleStatement(_program[i], i);

                if (stmt == null) //If somehow something got past all checks
                    throw new Exception("Unrecognized statement: " + _program[i]);

                statements.Add(stmt);
                if (stmt is ForStatement)
                    ForLoops.Push((stmt as ForStatement, i));
            }

            return statements;
        }

        private IStatement ParseSingleStatement(string line, int lineIndex = 0)
        {
            
            Tokenizer tokenizer = new Tokenizer(line);

            string[] tokens = tokenizer.GetTokens();

            if(tokens.Length >= 2) //Avoid PRINT = 2
            {
                if (_statement_keywords.Contains(tokens[0]) && tokens[1] == "=")
                    throw new Exception("Reserved keywords cannot be used as lvalue or rvalue");
            }

            int i = 0;
            IStatement partial_statement = null;

            while (i < tokens.Length)
            {
                if (tokens[0] == "PRINT")
                {

                    if (tokens[1].StartsWith("$_"))
                        partial_statement = new PrintStringStatement(tokens[1]);
                    else
                        partial_statement = new PrintExpressionStatement(tokens.Skip(1).ToArray());
                } else if(tokens[0] == "INPUT")
                {
                    partial_statement = new InputStatement(tokens[1]);
                } else if(tokens[0] == "IF")
                {
                    var lastToken = tokens.Last();
                    
                    if (lastToken != "THEN")
                        throw new Exception("Missing THEN in IF statement!");

                    partial_statement = new IfStatement(tokens.Skip(1).SkipLast(1).ToArray());
                } else if(tokens[0] == "ENDIF")
                {
                    partial_statement = new EndIfStatement();
                }
                else if (tokens[0] == "FOR")
                {
                    if (!tokens.Contains("TO"))
                        throw new Exception("FOR: Missing TO");

                    string counter = tokens[1];
                    if (tokens[2] != "=")
                        throw new Exception("FOR: Expected '=' after counter identifier");

                    string[] initExpr = tokens.Skip(3).TakeWhile(x => x != "TO").ToArray();
                    string[] endExpr = tokens.Skip(initExpr.Length + 4).ToArray();

                    ForDataFrame data = new ForDataFrame();
                    data.CounterIdent = counter;
                    data.InitExpr = initExpr;
                    data.FinalExpr = endExpr;

                    partial_statement = new ForStatement(data);
                    
                    //ForLoops.Push((partial_statement as ForStatement, i));

                } else if( tokens[0] == "NEXT")
                {
                    var forData = ForLoops.Pop();
                    partial_statement = new NextStatement(forData.Item1, forData.Item2);
                } else if(tokens[0] == "GOTO")
                {
                    string label = tokens[1];
                    partial_statement = new GotoStatement(label);
                }
                else if (tokens[0] == "GOSUB")
                {
                    string label = tokens[1];
                    partial_statement = new GoSubStatement(label);
                }
                else if (tokens[0] == "RETURN")
                {
                    partial_statement = new ReturnStatement();
                }
                else if(tokens[0] == "STOP")
                {
                    partial_statement = new StopStatement();
                }
                else if (tokens[0] == "POKE")
                {
                    string[] addrExpr = tokens.Skip(1).TakeWhile(x => x != ",").ToArray();
                    string[] valueExpr = tokens.Skip(addrExpr.Length + 2).ToArray();
                    partial_statement = new PokeStatement(addrExpr, valueExpr);
                }
                else
                {
                    if((tokens.Length <= 1 && !tokens[0].EndsWith(":")) || (tokens.Length > 2 && tokens[1] != "="))
                        throw new Exception("Invalid command: " + line);
                    else if(tokens[0].EndsWith(":"))
                    {
                        partial_statement = new LabelStatement(tokens[0].TrimEnd(':'), lineIndex);
                    }
                    else
                    {
                        partial_statement = new AssignStatement(tokens[0], tokens.Skip(2).ToArray());
                    }
                }
                    

                i++;
            }

            return partial_statement;
        }

    }
}
