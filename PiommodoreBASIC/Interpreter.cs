using System;
using System.Linq;

namespace PiommodoreBASIC
{
    public class Interpreter
    {
        private IStatement[] _statements;

        private List<Variable> variables = new List<Variable>();

        public Interpreter() => _statements = new IStatement[1];

        public Interpreter(params IStatement[] statements) => _statements = statements;

        public Interpreter(Parser parser) => _statements = parser.ParseStatements().ToArray();

        public void SetProgram(params string[] lines)
        {
            Parser parser = new Parser(lines);
            _statements = parser.ParseStatements().ToArray();
        }

        public void SetProgram(params IStatement[] lines)
        {
            _statements = lines;
        }

        private Dictionary<string, int> IndexLabels()
        {
            Dictionary<string, int> labels = new Dictionary<string, int>();

            foreach(IStatement statement in _statements)
            {
                if(statement is LabelStatement)
                {
                    var data = (statement as LabelStatement).GetLabelInfo();
                    labels.Add(data.LabelName, data.Index);
                }
            }

            return labels;
        }

        public Dictionary<int, int> IndexIfs()
        {
            Dictionary<int, int> ifs = new Dictionary<int, int>();

            Stack<int> temp = new Stack<int>();
            for (int i = 0; i < _statements.Length; i++)
            {
                if (_statements[i] is IfStatement)
                    temp.Push(i);
                else if (_statements[i] is EndIfStatement)
                    ifs.Add(temp.Pop(), i);
            }

            return ifs;
        }

        public void Run()
        {
            bool balanced_ifs = _statements.Count(x => x is IfStatement) == _statements.Count(x => x is EndIfStatement);
            bool balanced_fors = _statements.Count(x => x is ForStatement) == _statements.Count(x => x is NextStatement);

            if (!balanced_ifs)
                throw new Exception("One or more IF with missing ENDIF were detected");

            if (!balanced_fors)
                throw new Exception("One or more FOR with missing NEXT were detected");

            var labels = IndexLabels();
            var ifs = IndexIfs();

            Stack<int> callStack = new Stack<int>();

            for (int i = 0; i < _statements.Length; i++)
            {
                IStatement current = _statements[i];

                if (current is GotoStatement)
                {
                    GotoStatement @goto = (current as GotoStatement);
                    @goto.Execute(ref labels, ref i);
                } else if(current is IfStatement)
                {
                    IfStatement @if = (current as IfStatement);

                    @if.Execute(ref variables, ref i, ref ifs);
                } else if(current is NextStatement)
                {
                    NextStatement next = current as NextStatement;
                    next.Execute(ref variables, ref i);
                } else if(current is GoSubStatement)
                {
                    callStack.Push(i);

                    GoSubStatement gosub = current as GoSubStatement;
                    gosub.Execute(ref labels, ref i);
                } else if(current is ReturnStatement)
                {
                    ReturnStatement @return = current as ReturnStatement;
                    @return.Execute(ref callStack, ref i);
                }
                else if (current is StopStatement)
                {
                    return;
                }
                else
                    _statements[i].Execute(ref variables);
            }
        }
    }
}