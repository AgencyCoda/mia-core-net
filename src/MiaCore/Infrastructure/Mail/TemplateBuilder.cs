using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MiaCore.Infrastructure.Mail
{
    internal class TemplateBuilder
    {
        public static async Task<string> BuildAsync(string fileName, object args)
        {
            var html = await readFromFileAsync(fileName);
            html = replace(html, args);
            return html;
        }

        private static async Task<string> readFromFileAsync(string fileName)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"EmailTemplates/{fileName}");
            if (!File.Exists(path))
            {
                throw new System.Exception("html template not found");
            }
            var html = await File.ReadAllTextAsync(path);
            return html;
        }

        private static string replace(string str, object args)
        {
            var dictionary = convertToDictionary(args);

            foreach (var item in dictionary)
            {
                str = str.Replace($"{{{{{item.Key}}}}}", item.Value.ToString());
            }
            return str;
        }

        private static Dictionary<string, object> convertToDictionary(object obj)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(obj))
            {
                object val = propertyDescriptor.GetValue(obj);
                if (val != null)
                    dictionary.Add(propertyDescriptor.Name, val);
            }
            return dictionary;
        }
    }
}