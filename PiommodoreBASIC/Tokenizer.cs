using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiommodoreBASIC
{
    public class Tokenizer
    {
        string _data;

        string[] operators = ExpressionParser.Operators;

        char[] ignore_characters =
        { ' ', '\n', '\t', (char)13, (char)10 };

        public Tokenizer(string data) => _data = data;

        public string[] GetTokens()
        {
            List<string> tokens = new List<string>();
            string buffer = "";
            int i = 0;
            while(i < _data.Length)
            {
                if(ignore_characters.Contains(_data[i]))
                {
                    if (buffer != "")
                    {
                        tokens.Add(buffer);
                        buffer = "";
                    }
                    i++;
                    continue;
                }
                if (Char.IsDigit(_data[i]) || _data[i] == '.')
                {
                    if (buffer != "")
                    {
                        tokens.Add(buffer);
                        buffer = "";
                    }

                    do
                    {
                        buffer += _data[i];
                        i++;
                    } while (i < _data.Length && (Char.IsDigit(_data[i]) || _data[i] == '.'));

                    tokens.Add(buffer);
                    buffer = "";
                    i--;
                }
                else if (_data[i] == ',')
                {
                    if (buffer != "")
                    {
                        tokens.Add(buffer);
                        buffer = "";
                    }
                    tokens.Add(",");
                }
                else if (_data[i] == '\"')
                {
                    if (buffer != "")
                    {
                        tokens.Add(buffer);
                        buffer = "";
                    }
                    i++;
                    while (i < _data.Length && _data[i] != '\"')
                    {
                        buffer += _data[i];
                        i++;
                    }

                    tokens.Add("$_" + buffer);
                    buffer = "";
                }
                else if (operators.Contains(_data[i].ToString()) || _data[i] == '(' || _data[i] == ')')
                {
                    if (buffer != "")
                    {
                        tokens.Add(buffer);
                        buffer = "";
                    }
                    tokens.Add(_data[i].ToString());
                }
                else
                {
                    buffer += _data[i];
                }
                i++;
            }

            if (buffer != "")
                tokens.Add(buffer);

            return tokens.ToArray();
        }
    }
}
