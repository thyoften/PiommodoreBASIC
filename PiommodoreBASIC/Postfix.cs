using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiommodoreBASIC
{
    public class Postfix
    {
        TokenizedExpression infix;

        enum PostfixParserState { EXPECTED_OPERATOR, EXPECTED_OPERAND }

        public Postfix(params string[] tokens)
        {
            infix = new ExpressionParser(tokens).ParseExpression();
        }

        private int GetPrecedence(string s)
        {
            if (s == "+" || s == "-" || s == "<" || s == "=" || s == ">" || s == "#")
                return 0;
            else if (s == "&" || s == "|")
                return -1;
            else if (s == "^")
                return 4;
            else if (s == "*" || s == "/" || s == "%")
                return 2;
            else
                return -1;
        }

        private int GetAssociativity(string s)
        {
            if (s == "+" || s == "-" || s == "*" || s == "/")
                return -1;
            else
                return 1;
        }

        public TokenizedExpression GetPostfix()
        {
            TokenizedExpression postfix = new TokenizedExpression();
            Stack<(ExpressionTokenType @Type, string Value)> operatorStack = new Stack<(ExpressionTokenType Type, string Value)>();
            PostfixParserState state = PostfixParserState.EXPECTED_OPERAND;

            foreach((ExpressionTokenType @Type, string Value) token in infix)
            {
                if(token.Type == ExpressionTokenType.OPERAND)
                {
                    if (state != PostfixParserState.EXPECTED_OPERAND)
                        throw new Exception("Expected operand, found operator " + token.Value);

                    postfix.Add(token);

                    state = PostfixParserState.EXPECTED_OPERATOR;
                } else if(token.Type == ExpressionTokenType.UNARY_OPERATOR)
                {
                    if (state != PostfixParserState.EXPECTED_OPERAND)
                        throw new Exception("Expected operand, found operator");

                    operatorStack.Push(token);
                    state = PostfixParserState.EXPECTED_OPERAND; 
                } else if(token.Type == ExpressionTokenType.BINARY_OPERATOR)
                {
                    if (state != PostfixParserState.EXPECTED_OPERATOR)
                        throw new Exception("Expected operator, found operand");

                    //Get operators with left assoc. && less or eq precedence and operators with r. assoc. and less prec
                    while (operatorStack.Count > 0 && ((GetAssociativity(token.Value) == -1
                        && GetPrecedence(token.Value) <= GetPrecedence(operatorStack.Peek().Value))
                        || GetAssociativity(token.Value) == 1 && GetPrecedence(token.Value) < GetPrecedence(operatorStack.Peek().Value)))
                    {
                        postfix.Add(operatorStack.Pop());
                    }

                    operatorStack.Push(token);
                    state = PostfixParserState.EXPECTED_OPERAND;
                } else if(token.Type == ExpressionTokenType.FUNCTION)
                {
                    if (state != PostfixParserState.EXPECTED_OPERAND)
                        throw new Exception("Expected operand, found operator");

                    operatorStack.Push(token);
                    state = PostfixParserState.EXPECTED_OPERAND;
                } else if(token.Type == ExpressionTokenType.OPEN_BRACKET)
                {
                    if (state != PostfixParserState.EXPECTED_OPERAND)
                        throw new Exception("Unexpected bracket found");

                    operatorStack.Push(token);
                    state = PostfixParserState.EXPECTED_OPERAND;
                } else if(token.Type == ExpressionTokenType.CLOSED_BRACKET)
                {
                    if (state != PostfixParserState.EXPECTED_OPERATOR)
                        throw new Exception("Expected closed bracket, found operator");

                    while (operatorStack.Count > 0 && operatorStack.Peek().Value != "(")
                    {
                        postfix.Add(operatorStack.Pop());
                    }
                    operatorStack.Pop(); //Discard the (

                    if (operatorStack.Count > 0 && operatorStack.Peek().Type == ExpressionTokenType.FUNCTION)
                    {
                        postfix.Add(operatorStack.Pop());
                    }

                    state = PostfixParserState.EXPECTED_OPERATOR;
                }
            }

            if (!operatorStack.Any(x => x.Type == ExpressionTokenType.OPEN_BRACKET))
                postfix.AddRange(operatorStack.ToArray());
            else
                throw new Exception("Mismatched brackets!");

            return postfix;
        }
    }
}
