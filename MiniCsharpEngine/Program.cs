using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MiniCsharpEngine
{

    /*执行优先级
!      ++     --                    1
*     /    %                        2
 +     -                            3
 <   >    <=    >=    is            4
==    !=                            5
&                                   6
^                                   7
|                                   8
&&                                  9
||                                  10
?:                                  11
   */
    public class MiniCsharpEngine
    {
        readonly string groupOperato = @"\(.*?\)";
        readonly string[] returnValues = new string[] { "bool", "int", "double", "Visibility" };
        //new string[] { ".*?||.*?", "?.*:.*" }
        readonly List<Func<string, string>> operators = new List<Func<string, string>>()
        {
              EvalGtLt, //4
              EvalBool,  //9-10
              EvalConditional, //11
        };

        static string EvalConditional(string input)
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
                while (true)
                {
                    var newRight = EvalBool(right);
                    if (newRight == right)
                        break;
                    else
                        right = newRight;
                }
                bool isBool;
                isBool = bool.TryParse(left, out bool bleft);
                if (!isBool)
                    return input;
                isBool = bool.TryParse(right, out bool bright);
                if (!isBool)
                    return input;
                bool result = false;
                switch (opr)
                {
                    case "&&":
                        result = bleft && bright;
                        break;
                    case "||":
                        result = bleft || bright;
                        break;
                }
                input = input.Replace(matched, result.ToString());
            }

            return input;
        }

        static string EvalGtLt(string input)
        {
            //"(\d+?)(\>{1}|\<{1})(\d+)"
            string pattern = @"(\d+?)(\>{1}|\<{1})(\d+)";
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
                    case ">":
                        result = float.Parse(left) > float.Parse(right);
                        break;
                    case "<":
                        result = float.Parse(left) < float.Parse(right);
                        break;
                }
                input = input.Replace(matched, result.ToString());
            }

            return input;
        }

        public object Eval(string pattern)
        {
            //解析返回类型
            string returnType = null;
            foreach (var item in returnValues)
            {
                var matched = Regex.Match(pattern, $"\\({item}\\)");
                if (matched.Success)
                {
                    returnType = matched.Value;
                    pattern = pattern.Replace(matched.Value, "");
                    break;
                }
            }

            //解析操作符
            foreach (var item in operators)
            {
                pattern = item(pattern);
            }

            //按要求返回指定类型
            switch (returnType)
            {
                case "(bool)": return bool.Parse(pattern);
                case "(int)": return int.Parse(pattern);
                case "(double)": return double.Parse(pattern);
                case "(Visibility)":
                    var b = bool.Parse(pattern);
                    return b ? Visibility.Visible : Visibility.Collapsed;
                default:
                    return pattern;
            }
        }
    }

    class Program
    {
        static MiniCsharpEngine engine = new MiniCsharpEngine();
        static void Main(string[] args)
        {
            Debug.Assert(Test("True||False").ToString() == "True");
            Debug.Assert(Test("True?111:222").ToString() == "111");

            Debug.Assert(Test("(bool)True||False").ToString() == true.ToString());
            Debug.Assert(Test("(int)True?111:222").ToString() == 111.ToString());
            Debug.Assert(Test("(bool)1>2").ToString() == false.ToString());
            Debug.Assert(Test("(bool)1<2").ToString() == true.ToString());
            Debug.Assert(Test("(bool)True||False||1<2").ToString() == true.ToString());
            Debug.Assert(Test("(bool)False||False||1>2").ToString() == false.ToString());
            //Test("(11==22)?111:222");
        }

        private static object Test(string script)
        {
            var result = engine.Eval(script);
            Console.WriteLine($"test:{script}, result:{result}");
            return result;
        }
    }
}
