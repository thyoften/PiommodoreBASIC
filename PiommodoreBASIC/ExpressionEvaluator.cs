using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiommodoreBASIC
{
    public class ExpressionEvaluator
    {

        public double EvaluateExpression(string[] tokens, List<Variable> vars)
        {
            Stack<double> data = new Stack<double>();

            Postfix postfix = new Postfix(tokens);

            foreach (var rpnToken in postfix.GetPostfix())
            {
                if (rpnToken.TokenType == ExpressionTokenType.OPERAND)
                {
                    if (double.TryParse(rpnToken.TokenValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) == true)
                    {
                        data.Push(value);
                    }
                    else
                    {
                        var variable = vars.Find(x => x.Name == rpnToken.TokenValue);
                        if (variable != null)
                            data.Push(variable.Value);
                    }
                }
                else if (rpnToken.TokenType == ExpressionTokenType.UNARY_OPERATOR)
                {
                    if (rpnToken.TokenValue == "-")
                    {
                        data.Push(-1.0 * data.Pop());
                    }
                    else
                    {
                        bool val = !Convert.ToBoolean(data.Pop());
                        data.Push(Convert.ToDouble(val));
                    }
                } else if(rpnToken.TokenType == ExpressionTokenType.BINARY_OPERATOR)
                {
                    switch(rpnToken.TokenValue)
                    {
                        case "+": data.Push(data.Pop()+ data.Pop()); break;
                        case "-":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();
                                data.Push(n2 - n1);
                            }
                            break;
                        case "*": data.Push(data.Pop() * data.Pop()); break;
                        case "/":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();

                                if (n1 == 0.0)
                                    throw new Exception("Division by zero!");

                                data.Push(n2 / n1);
                            } break;
                        case "%":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();
                                data.Push(n2 % n1);
                            }
                            break;
                        case "^":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();
                                data.Push(Math.Pow(n2, n1));
                            } break;
                        case ">":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();
                                data.Push(Convert.ToDouble(n2 > n1));
                            }
                            break;
                        case "<":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();
                                data.Push(Convert.ToDouble(n2 < n1));
                            }
                            break;
                        case "=":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();
                                data.Push(Convert.ToDouble(n2 == n1));
                            }
                            break;
                        case "#":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();
                                data.Push(Convert.ToDouble(n2 != n1));
                            }
                            break;
                        case "&":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop();

                                bool b1 = Convert.ToBoolean(n1);
                                bool b2 = Convert.ToBoolean(n2);
                                data.Push(Convert.ToDouble(b2 && b1));
                            }
                            break;
                        case "|":
                            {
                                double n1 = data.Pop();
                                double n2 = data.Pop(); 

                                bool b1 = Convert.ToBoolean(n1);
                                bool b2 = Convert.ToBoolean(n2);
                                data.Push(Convert.ToDouble(b2 || b1));
                            }
                            break;
                    }
                } else if(rpnToken.TokenType == ExpressionTokenType.FUNCTION)
                {
                    switch (rpnToken.TokenValue)
                    {
                        case "SIN": data.Push(Math.Sin(data.Pop())); break;
                        case "COS": data.Push(Math.Cos(data.Pop())); break;
                        case "ATN": data.Push(Math.Atan(data.Pop())); break;
                        case "PEEK": data.Push(Scratchpad.Read(Convert.ToInt32(data.Pop()))); break;
                    }
                }
            }

            return data.Pop();
        }
    }
}
