using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;

namespace MiaCore.Infrastructure.Mail
{
    public class TemplateBuilder
    {
        private readonly IGenericRepository<MiaEmailTemplate> _templateRepository;
        public TemplateBuilder(IGenericRepository<MiaEmailTemplate> templateRepository)
        {
            _templateRepository = templateRepository;
        }
        public async Task<(string, string)> BuildAsync(string email, string templateSlug, object args)
        {
            var template = await _templateRepository.GetByAsync(new Where(nameof(MiaEmailTemplate.Slug), templateSlug));
            if (template is null)
                throw new Exception("template not found");

            string content_text = getContentTemplate(template.Title, template.ContentText, templateSlug, email);

            string body = replace(content_text, args);
            return (template.Title, body);
        }

        private string replace(string str, object args)
        {
            var dictionary = convertToDictionary(args);

            foreach (var item in dictionary)
            {
                str = str.Replace($"{{{{{item.Key}}}}}", item.Value.ToString(), StringComparison.InvariantCultureIgnoreCase);
            }
            return str;
        }

        private Dictionary<string, object> convertToDictionary(object obj)
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

        private string getContentTemplate(string title, string content_text, string template_slug, string email)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templateMail = Path.Combine(baseDir, "TemplateMail.txt");

            if (!File.Exists(templateMail))
                throw new Exception("file template not found");

            string str = File.ReadAllText(templateMail);

            string lang = "<html lang=\"en\">";
            template_slug = template_slug.Substring(template_slug.Length - 3);
            if (template_slug == "-es")
                lang = "<html lang=\"es\">";

            str = str.Replace($"{{{{lang}}}}", lang, StringComparison.InvariantCultureIgnoreCase)
                     .Replace($"{{{{title}}}}", title, StringComparison.InvariantCultureIgnoreCase)
                     .Replace($"{{{{email}}}}", email, StringComparison.InvariantCultureIgnoreCase)
                     .Replace($"{{{{content_text}}}}", content_text, StringComparison.InvariantCultureIgnoreCase);

            return str;
        }
    }
}