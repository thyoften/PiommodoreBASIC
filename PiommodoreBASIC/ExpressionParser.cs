using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiommodoreBASIC
{
    public enum ExpressionTokenType
    {
        OPERAND, FUNCTION, UNARY_OPERATOR, BINARY_OPERATOR, OPEN_BRACKET, CLOSED_BRACKET
    }

    public class TokenizedExpression : List<(ExpressionTokenType TokenType, string TokenValue)> { }

    public class ExpressionParser
    {

        string[] exprTokens;

        public readonly static string[] Operators = { "+", "-", "*", "/", "^", "%",  ">", "<", "=", "#", "&", "|", "~" };
        public readonly static string[] Functions = { "SIN", "COS", "ATN", "PEEK", "INT"};


        private bool IsNumber(string token)
        {
            
            bool integer = token.All(Char.IsDigit);
            bool @float = new Regex("[0-9]+\\.[0-9]+").IsMatch(token);

            return integer || @float;
        }

        private bool IsOperand(string token)
        {
            return IsNumber(token) || ((new Regex("[A-Za-z0-9]+")).IsMatch(token) && !Functions.Contains(token));
        }

        public ExpressionParser(params string[] tokens)
        {
            exprTokens = tokens;
        }

        public TokenizedExpression ParseExpression()
        {
            TokenizedExpression expression = new TokenizedExpression();
           
            for (int i = 0; i < exprTokens.Length; i++)
            {
                if(IsOperand(exprTokens[i]))
                {
                    expression.Add((ExpressionTokenType.OPERAND, exprTokens[i]));
                } else if(Operators.Contains(exprTokens[i]))
                {
                    //Check if unary or binary
                    if(i == 0)
                    {
                        expression.Add((ExpressionTokenType.UNARY_OPERATOR, exprTokens[i]));
                    }
                    else if (i > 0 && exprTokens[i - 1] == "(")
                    {
                        expression.Add((ExpressionTokenType.UNARY_OPERATOR, exprTokens[i]));
                    }
                    else if (i > 0 && Operators.Contains(exprTokens[i - 1]))
                    {
                        expression.Add((ExpressionTokenType.UNARY_OPERATOR, exprTokens[i]));
                    }
                    else
                    {
                        expression.Add((ExpressionTokenType.BINARY_OPERATOR, exprTokens[i]));
                    }
                } else if(exprTokens[i] == "(")
                {
                    expression.Add((ExpressionTokenType.OPEN_BRACKET, exprTokens[i]));
                }
                else if (exprTokens[i] == ")")
                {
                    expression.Add((ExpressionTokenType.CLOSED_BRACKET, exprTokens[i]));
                } else if(Functions.Contains(exprTokens[i]))
                {
                    expression.Add((ExpressionTokenType.FUNCTION, exprTokens[i]));
                }
            }
            
            return expression;
        }
    }
}
