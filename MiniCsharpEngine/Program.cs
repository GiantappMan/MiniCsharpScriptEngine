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
    //https://msdn.microsoft.com/zh-cn/library/6a71f45d(VS.80).aspx
    /* Xaml中使用的特殊字符串
    &lt;    <!-- Less than symbol -->
    &gt;    <!-- Greater than symbol -->
    &amp;   <!-- Ampersand symbol -->
    &quot;  <!-- Double quote symbol -->
    */

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
        readonly string groupOperato = @"\((.*)\)";
        readonly string[] returnValues = new string[] { "bool", "int", "double", "Visibility" };
        //new string[] { ".*?||.*?", "?.*:.*" }
        readonly List<Func<string, string>> operators = new List<Func<string, string>>()
        {
              EvalUnary,//1
              EvalPlusAndMinus,//3
              EvalGtLt, //4
              EvalEqual,//5
              EvalBool,  //9-10
              EvalConditional, //11
        };

        private static string EvalPlusAndMinus(string input)
        {
            //"((\d+(\.\d+)?)+?)(\+{1}|\-{1})(((?!\+|\-)(\d+(\.\d+)?))+)"
            string pattern = @"((\d+(\.\d+)?)+?)(\+{1}|\-{1})(((?!\+|\-)(\d+(\.\d+)?))+)";
            Regex regex = new Regex(pattern);
            var m = regex.Match(input);
            if (m.Success)
            {
                var matched = m.Groups[0].Value;
                var left = float.Parse(m.Groups[1].Value);
                var opr = m.Groups[4].Value;
                var right = float.Parse(m.Groups[5].Value);
                float result = 0;
                switch (opr)
                {
                    case "+":
                        result = left + right;
                        break;
                    case "-":
                        result = left - right;
                        break;
                }
                input = input.Replace(matched, result.ToString());
            }

            return input;
        }

        private static string EvalUnary(string input)
        {
            //"(\!{1})(((?!\!).)+)"
            string pattern = @"(\!{1})(((?!\!|\&|\?|\:).)+)";
            Regex regex = new Regex(pattern);
            var m = regex.Match(input);
            if (m.Success)
            {
                var matched = m.Groups[0].Value;
                //var left = m.Groups[1].Value;
                var opr = m.Groups[1].Value;
                var right = bool.Parse(m.Groups[2].Value);
                bool result = false;
                switch (opr)
                {
                    case "!":
                        result = !right;
                        break;
                }
                input = input.Replace(matched, result.ToString());
            }

            return input;
        }

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
            //"(((?!\||\&).)+)(\|{2}|\&{2})(((?!\||\&).)+)"
            string pattern = @"(((?!\||\&).)+)(\|{2}|\&{2})(((?!\||\&).)+)";
            Regex regex = new Regex(pattern, RegexOptions.RightToLeft);
            var mes = regex.Matches(input);
            foreach (Match m in mes)
            {
                if (m.Success)
                {
                    var matched = m.Groups[0].Value;
                    var left = m.Groups[1].Value;
                    var opr = m.Groups[3].Value;
                    var right = m.Groups[4].Value;

                    bool isBool;
                    bool bleft;
                    isBool = bool.TryParse(left, out bleft);
                    if (!isBool)
                        return input;

                    bool bright;
                    isBool = bool.TryParse(right, out bright);
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
        static string EvalEqual(string input)
        {
            //"(.+?)(\=\={1}|\!\={1})(((?!\||\&|\?|\:).)+)"
            string pattern = @"(.+?)(\=\={1}|\!\={1})(((?!\||\&|\?|\:).)+)";
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
                    case "==":
                        result = left == right;
                        break;
                    case "!=":
                        result = left != right;
                        break;
                }
                input = input.Replace(matched, result.ToString());
            }

            return input;
        }

        public object TryEval(string pattern)
        {
            try
            {
                return Eval(pattern);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
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

            //解析括号，优先执行
            pattern = GroupCommand(pattern);

            //解析操作符
            pattern = ResolveCommand(pattern);

            //按要求返回指定类型
            switch (returnType)
            {
                case "(bool)": return bool.Parse(pattern);
                case "(int)": return int.Parse(pattern);
                case "(double)": return double.Parse(pattern);
                case "(Visibility)":
                    bool b;
                    bool isBool = bool.TryParse(pattern, out b);
                    if (isBool)
                    {
                        return b ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        Visibility v;
                        bool isVisibility = Enum.TryParse<Visibility>(pattern, out v);
                        if (isVisibility)
                            return v;
                    }
                    return Visibility.Collapsed;
                default:
                    return pattern;
            }
        }

        //private string GroupCommand(string pattern)
        //{
        //    string temp = null;
        //    do
        //    {
        //        temp = pattern;
        //        var matched = Regex.Match(pattern, groupOperato);
        //        if (matched.Success)
        //        {
        //            string value = matched.Groups[1].Value;
        //            //解析多重内嵌括号 ((1==2))
        //            value = GroupCommand(value);
        //            var result = ResolveCommand(value);
        //            pattern = pattern.Replace(matched.Value, result);
        //        }
        //    }
        //    while (temp != pattern);
        //    return pattern;
        //}

        private string GroupCommand(string pattern)
        {
            string temp = null;
            do
            {
                temp = pattern;
                bool findLeft = true, findRinght = false;
                int leftCount = 0, rightCount = 0;
                int leftIndex = -1, rightIndex = -1;
                for (int index = 0; index < pattern.Length; index++)
                {
                    var item = pattern[index];
                    if (item == '(')
                    {
                        leftCount++;
                        if (findLeft)
                        {
                            leftIndex = index;
                            findLeft = false;
                            findRinght = true;
                        }
                    }

                    if (item == ')')
                    {
                        rightCount++;
                        if (findRinght && leftCount == rightCount)
                        {
                            rightIndex = index;
                            findRinght = false;
                            break;
                        }
                    }
                }

                var left = pattern.IndexOf("(");
                if (leftIndex < 0)
                    break;

                var content = pattern.Substring(leftIndex, rightIndex - leftIndex + 1);
                //解析多重内嵌括号 ((1==2))
                var result = GroupCommand(content.Substring(1, content.Length - 2));
                //var result = content.Substring(1, content.Length - 2);
                result = ResolveCommand(result);
                pattern = pattern.Replace(content, result);
            }
            while (temp != pattern);
            return pattern;
        }

        private string ResolveCommand(string pattern)
        {
            foreach (var item in operators)
            {
                string temp = null;
                do
                {
                    temp = pattern;
                    var result = item(pattern);
                    pattern = result;
                }
                while (temp != pattern);
            }
            return pattern;
        }
    }
    class Program
    {
        static MiniCsharpEngine engine = new MiniCsharpEngine();
        static void Main(string[] args)
        {
            Debug.Assert(Eval("True||False").ToString() == "True");
            Debug.Assert(Eval("True?111:222").ToString() == "111");

            Debug.Assert(Eval("(bool)True||False").ToString() == true.ToString());
            Debug.Assert(Eval("(int)True?111:222").ToString() == 111.ToString());
            Debug.Assert(Eval("(bool)1>2").ToString() == false.ToString());
            Debug.Assert(Eval("(bool)1<2").ToString() == true.ToString());
            Debug.Assert(Eval("(bool)False||False||1<2").ToString() == true.ToString());
            Debug.Assert(Eval("(bool)True||False||1<2").ToString() == true.ToString());
            Debug.Assert(Eval("(bool)False||False||1>2").ToString() == false.ToString());

            Debug.Assert(Eval("(bool)False||False||(2>1&&1<2)").ToString() == true.ToString());
            Debug.Assert(Eval("(bool)1<2&&True&&(True==False||1<2)").ToString() == true.ToString());
            Debug.Assert(Eval("(bool)1<2&&True&&((True==False)||1<2)").ToString() == true.ToString());
            Debug.Assert(Eval("(bool)1<2&&True&&((True==False)||1>2)").ToString() == false.ToString());

            Debug.Assert(Eval("(bool)!True").ToString() == false.ToString());
            Debug.Assert(Eval("(bool)!False").ToString() == true.ToString());
            Debug.Assert(Eval("(Visibility)!False?Visible:Collapsed").ToString() == Visibility.Visible.ToString());
            Debug.Assert(Eval("(Visibility)False?Visible:Collapsed").ToString() == Visibility.Collapsed.ToString());

            Debug.Assert(Eval("(int)1+1").ToString() == 2.ToString());
            Debug.Assert(Eval("(int)2-1").ToString() == 1.ToString());

            Debug.Assert(Eval("(False||False)&&!(False||False)?0.5:1").ToString() == "1");
            Debug.Assert(Eval("((False||False)&&!(False||False))?0.5:1").ToString() == "1");

            Debug.Assert(Eval("(False||False)||!(False||False)?0.5:1").ToString() == "0.5");
            Debug.Assert(Eval("((False||False)||(False||True))?0.5:1").ToString() == "0.5");
        }

        private static object Eval(string script)
        {
            var result = engine.Eval(script);
            Console.WriteLine($"test:{script}, result:{result}");
            return result;
        }
    }
}
