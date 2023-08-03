using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleArgs
{

    public class ArgumentParseAttribute : Attribute
    {
        public string name = string.Empty;
        public string help = string.Empty;
        public string defaults = string.Empty;

        public ArgumentParseAttribute(string name, string help = null)
        {
            this.name = name;
            this.help = help;
        }
    }

    public class SimpleParse
    {
        public static T Parse<T>(string[] args) where T : new()
        {
            T arg_obj = new T();
            Dictionary<string, string> arg_values = new Dictionary<string, string>();
            int len = args.Length;
            Type type = null;
            for (int i = 0; i < len; ++i)
            {
                var arg_name = args[i];
                if (arg_name.Equals("--help"))
                {
                    ShowHelp(arg_obj);
                    //Environment.Exit(0);
                    return default(T);
                    //return arg_obj;
                }

                if (arg_name.StartsWith("-") && i < len - 1)
                {
                    var value = args[i + 1];
                    if (value.StartsWith("-"))
                    {
                        arg_values[arg_name] = string.Empty;
                        continue;
                    }
                    arg_values.Add(arg_name, value);
                    ++i;
                }
            }

            if (type == null)
            {
                type = arg_obj.GetType();
            }

            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttributes(typeof(ArgumentParseAttribute), false).FirstOrDefault() as ArgumentParseAttribute;
                if (attr != null)
                {
                    var arg_key = attr.name;
                    string arg_value = string.Empty;
                    if (arg_values.TryGetValue(arg_key, out arg_value))
                    {
                        object val = null;
                        if (field.FieldType.IsEnum)
                        {
                            val = Enum.Parse(field.FieldType, arg_value, true);
                        }
                        else
                        {
                            val = Convert.ChangeType(arg_value, field.FieldType);
                        }
                        //Convert.
                        field.SetValue(arg_obj, val);
                    }
                }
            }
            //properties.att
            return arg_obj;
        }

        private static void ShowHelp<T>(T obj)
        {
            var type = obj.GetType();
            Console.Out.WriteLine("Help : < " + type.Name + " >");
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttributes(typeof(ArgumentParseAttribute), false).FirstOrDefault() as ArgumentParseAttribute;
                if (attr != null)
                {
                    var arg_key = attr.name;
                    Console.Out.WriteLine("    " + arg_key + "  " + field.Name);

                    if (!string.IsNullOrEmpty(attr.help))
                    {
                        Console.Out.WriteLine("      Help : " + attr.help);
                    }

                    var value = field.GetValue(obj);
                    var default_str = string.Empty;
                    if (value == null)
                    {
                        default_str = "None";
                    }
                    else
                    {
                        default_str = value.ToString();
                    }
                    Console.Out.WriteLine("      Default : " + default_str);

                    var fieldType = field.FieldType;
                    if (fieldType.IsEnum)
                    {
                        var enums = fieldType.GetEnumNames();
                        Console.Out.Write("      Options : [");
                        int cnt = enums.Length;
                        Console.Out.Write(enums[0]);

                        for (int i = 1; i < cnt; ++i)
                        {
                            Console.Out.Write(" | " + enums[i]);
                        }
                        Console.Out.WriteLine("]");
                    }
                }
            }
        }
    }

}
