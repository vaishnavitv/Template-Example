using Mustache;
using System;
using System.Collections.Generic;

namespace Template_Example
{

    interface ITemplateRenderer
    {
        String Render(String templateString, Dictionary<String, object> parameters);
    }

    class SimpleStringRenderer : ITemplateRenderer
    {

        public string Render(String templateString, Dictionary<String, object> parameters)
        {
            String returnValue = templateString;
            foreach(String key in parameters.Keys)
            {
                String patternToReplace = "{{"+key+"}}";
                String toReplaceWith = parameters[key].ToString();

                returnValue = returnValue.Replace(patternToReplace, toReplaceWith);
            }
            return returnValue;
        }

    }

    class SQLStringRenderer : SimpleStringRenderer
    {

        public string Render(String templateString, Dictionary<String, object> parameters)
        {
            Dictionary<String, object> newParameters = new Dictionary<string, object>();
            foreach (String key in parameters.Keys)
            {
                Object value = parameters[key];
                if(value is string)
                {
                    value = "`" + value.ToString() + "`"; //TODO: SQL Injections, escaping should be handled here.
                }
                newParameters[key] = value;
            }
            return base.Render(templateString, newParameters);
        }

    }

    class MustacheRenderer : ITemplateRenderer
    {
        public string Render(String templateString, Dictionary<String, object> parameters)
        {
            if (!generators.ContainsKey(templateString))
                generators[templateString] = compiler.Compile(templateString);
            var generator = generators[templateString];
            return generator.Render(parameters);
        }

        static Dictionary<String, Generator> generators = new Dictionary<String, Generator>();
        static FormatCompiler compiler = new FormatCompiler();
    }


    class Program
    {
        static void Main(string[] args)
        {
            var parameters = new Dictionary<string, object>(){
                    { "name", "Vaishnavi" },
                    { "age", 16 }
                };
            Console.WriteLine(new SimpleStringRenderer().Render("Hello {{name}} of age {{age}}!", parameters));
            Console.WriteLine(new SQLStringRenderer().Render("SELECT name, age from Student where name={{name}} and age={{age}}", parameters));
            Console.WriteLine(new MustacheRenderer().Render("<html><body><h1>Hello {{name}}</h1>. {{name}}'s Age is <b>{{age}}</b></body></html>", parameters));
            Console.ReadLine();
        }
    }
}
