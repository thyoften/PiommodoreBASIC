using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiommodoreBASIC
{
    public interface IStatement { public void Execute(ref List<Variable> vars);  }

    public class PrintStringStatement : IStatement
    {
        string _str = "";
        public PrintStringStatement(string tokenizedString)
        {
            _str = tokenizedString;
        }

        public void Execute(ref List<Variable> vars)
        {
            Console.WriteLine(_str.Substring(2));
        }
    }

    public class PrintExpressionStatement : IStatement
    {
        string[] _expr;
        public PrintExpressionStatement(params string[] tokenizedExpr)
        {
            _expr = tokenizedExpr;
        }

        public void Execute(ref List<Variable> vars)
        {
            ExpressionEvaluator eval = new ExpressionEvaluator();
            Console.WriteLine(eval.EvaluateExpression(_expr, vars).ToString(CultureInfo.InvariantCulture));
        }
    }

    public class InputStatement : IStatement
    {
        string _ident = "";
        public InputStatement(string ident)
        {
            _ident = ident;
        }

        public void Execute(ref List<Variable> vars)
        {
            var target = vars.Find(v => v.Name == _ident);

            string input = Console.ReadLine();
            double value;

            if (input != "")
                value = double.Parse(input, CultureInfo.InvariantCulture);
            else
                value = 0.0;            

            if(target == null)
            {
                Variable v = new Variable()
                {
                    Name = _ident,
                    Value = value
                };

                vars.Add(v);
            } else
            {
                vars[vars.IndexOf(target)].Value = value;
            }
        }
    }

    public class AssignStatement : IStatement
    {
        string _variableIdent;
        string[] assignExpr;

        public AssignStatement(string variableIdent, params string[] exprTokens)
        {
            _variableIdent = variableIdent;
            assignExpr = exprTokens;
        }

        public void Execute(ref List<Variable> vars)
        {
            var target = vars.Find(v => v.Name == _variableIdent);

            var value = (new ExpressionEvaluator()).EvaluateExpression(assignExpr, vars);

            if (target == null) //TODO: Export in own method, duplicate
            {
                Variable v = new Variable()
                {
                    Name = _variableIdent,
                    Value = value
                };

                vars.Add(v);
            }
            else
            {
                vars[vars.IndexOf(target)].Value = value;
            }
        }

    }

    public class LabelStatement : IStatement
    {
        (string name, int index) _labelInfo;

        public LabelStatement(string labelName, int index)
        {
            _labelInfo = (labelName, index);
        }

        public (string LabelName, int Index) GetLabelInfo() => _labelInfo;

        public void Execute(ref List<Variable> vars) { } //Work as a NOP
    }

    public class GotoStatement : IStatement
    {
        string _dest;
        public GotoStatement(string label)
        { _dest = label; }

        public string DestinationLabel => _dest;

        [Obsolete("Use the other version, this is unimplemented for this statement", true)]
        public void Execute(ref List<Variable> vars)
        {
            throw new NotImplementedException();
        }

        public void Execute(ref Dictionary<string, int> labels, ref int loopIndex)
        {
            loopIndex = labels[DestinationLabel];
        }
    }

    public class GoSubStatement : IStatement
    {
        string _dest;
        public GoSubStatement(string label)
        { _dest = label; }

        public string DestinationLabel => _dest;

        [Obsolete("Use the other version, this is unimplemented for this statement", true)]
        public void Execute(ref List<Variable> vars)
        {
            throw new NotImplementedException();
        }

        public void Execute(ref Dictionary<string, int> labels, ref int loopIndex)
        {
            loopIndex = labels[DestinationLabel];
        }
    }

    public class ReturnStatement : IStatement
    {
        public void Execute(ref List<Variable> vars)
        {
            throw new NotImplementedException();
        }

        public void Execute(ref Stack<int> callstack, ref int index)
        {
            if (callstack.Count <= 0)
                throw new Exception("Trying to RETURN from no subroutine, stack empty!");

            index = callstack.Pop();
        }
    }

    public class IfStatement : IStatement
    {
        string[] _expr;
        public IfStatement(params string[] tokenizedExpr)
        {
            _expr = tokenizedExpr;
        }

        [Obsolete("Do not call this directly", true)]
        public void Execute(ref List<Variable> vars) => throw new NotImplementedException();

        public void Execute(ref List<Variable> vars, ref int i, ref Dictionary<int, int> endIfIndex)
        {
            ExpressionEvaluator expr = new ExpressionEvaluator();
            bool result = Convert.ToBoolean((expr.EvaluateExpression(_expr, vars)));
            if(result != true)
                i = endIfIndex[i];
        }
    }

    public class EndIfStatement : IStatement
    {
        public void Execute(ref List<Variable> vars) { } //Behave like a NOP
    }

    public class ForStatement : IStatement
    {
        ForDataFrame _df;

        public string[] FinalValueExpression => _df.FinalExpr;

        public string Counter => _df.CounterIdent;

        public ForDataFrame GetData() => _df;

        public ForStatement(ForDataFrame forData)
        {
            _df = forData;
        }

        public void Execute(ref List<Variable> vars)
        {
            //Create counter var, evaluate final value of counter

            var target = vars.Find(v => v.Name == _df.CounterIdent);

            var value = (new ExpressionEvaluator()).EvaluateExpression(_df.InitExpr, vars);

            if (target == null) //TODO: Export in own method, duplicate (i copied it now, nobody can stop me heh heh heh)
            {
                Variable v = new Variable()
                {
                    Name = _df.CounterIdent,
                    Value = value
                };

                vars.Add(v);
            }
            else
            {
                vars[vars.IndexOf(target)].Value = value;
            }

            //Console.WriteLine($"'{_df.CounterIdent}' {value}");

        }
    }

    public class NextStatement : IStatement
    {
        string _forCounter;
        string[] _endExpr;
        double? _endValue = null;
        int _forIndex;

        public NextStatement(ForStatement forData, int forIndex)
        {
            _forCounter = forData.Counter;
            _endExpr = forData.FinalValueExpression;
            _forIndex = forIndex;
        }

        [Obsolete("Use the other version, this is unimplemented for this statement", true)]
        public void Execute(ref List<Variable> vars) => throw new NotImplementedException();

        public void Execute(ref List<Variable> vars, ref int i) {
            var target = vars.Find(v => v.Name == _forCounter);

            if (_endValue == null)
                _endValue = (new ExpressionEvaluator().EvaluateExpression(_endExpr, vars));

            if (target != null)
            {
                if (target.Value < _endValue)
                {
                    //Console.WriteLine("Cur: {0}, Target: {1}", target.Value, _endValue);
                    vars[vars.IndexOf(target)].Value += 1.0;
                    i = _forIndex; //Next statement after FOR
                }
            }
            else
                throw new Exception("NEXT: Counter variable is not defined");

        }

    }

    public class PokeStatement : IStatement
    {
        string[] addrExpr, _valueExpr;
        public PokeStatement(string[] addressExpr, string[] valueExpr)
        {
            addrExpr = addressExpr;
            _valueExpr = valueExpr;
        }

        public void Execute(ref List<Variable> vars)
        {
            ExpressionEvaluator eval = new ExpressionEvaluator();
            int index = Convert.ToInt32(eval.EvaluateExpression(addrExpr, vars));

            double value = eval.EvaluateExpression(_valueExpr, vars);

            Scratchpad.InsertAt(index, value);
        }
    }

    public class StopStatement : IStatement
    {
        [Obsolete("Do not call this directly", true)]
        public void Execute(ref List<Variable> vars) => throw new NotImplementedException();
    }

}
