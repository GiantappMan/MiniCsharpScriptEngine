using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace MiniCsharpEngine
{
    public class MiniCsharpEngine
    {
        readonly string groupOperato = @"\(.*?\)";
        //new string[] { ".*?||.*?", "?.*:.*" }
        readonly List<Func<string, string>> operators = new List<Func<string, string>>()
        {
               EvalBool,
              EvalConditional,
        };

        private static string EvalConditional(string input)
        {
            //"(.*?)\?(.*?)\:(.*)"
            string pattern = @"(.*?)\?(.*?)\:(.*)";
            Regex regex = new Regex(pattern);
            var m = regex.Match(input);
            if (m.Success)
            {
                var matched = m.Groups[0].Value;
                var condition = m.Groups[1].Value;
                var value1 = m.Groups[2].Value;
                var value2 = m.Groups[3].Value;

                var result = bool.Parse(EvalBool(condition));
                input = input.Replace(matched, result ? value1 : value2);
            }

            return input;
        }

        static string EvalBool(string input)
        {
            //"(.*?)(\|{2}|\&{2})(.*)"
            string pattern = @"(.*?)(\|{2}|\&{2})(.*)";
            Regex regex = new Regex(pattern);
            var m = regex.Match(input);
            if (m.Success)
            {
                var matched = m.Groups[0].Value;
                var left = m.Groups[1].Value;
                var opr = m.Groups[2].Value;
                var right = m.Groups[3].Value;
                bool result = false;
                switch (opr)
                {
                    case "&&":
                        result = bool.Parse(left) && bool.Parse(right);
                        break;
                    case "||":
                        result = bool.Parse(left) || bool.Parse(right);
                        break;
                }
                input = input.Replace(matched, result.ToString());

            }

            return input;
        }

        public object Eval(string pattern)
        {
            foreach (var item in operators)
            {
                pattern = item(pattern);
            }

            return pattern;
        }
    }

    class Program
    {
        static MiniCsharpEngine engine = new MiniCsharpEngine();
        static void Main(string[] args)
        {
            Test("True||False");
            Test("True?111:222");
            //Test("(11==22)?111:222");
        }

        private static void Test(string script)
        {
            var result = engine.Eval(script);
            Console.WriteLine($"test:{script}, result:{result}");
        }
    }
}
