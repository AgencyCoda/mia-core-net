using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MiaCore.Infrastructure.Persistence;
using MiaCore.Models;
using Microsoft.Extensions.Options;

namespace MiaCore.Infrastructure.Mail
{
    public class TemplateBuilder
    {
        private readonly IGenericRepository<MiaEmailTemplate> _templateRepository;
        private readonly MiaCoreOptions _options;
        public TemplateBuilder(IGenericRepository<MiaEmailTemplate> templateRepository, IOptions<MiaCoreOptions> options)
        {
            _templateRepository = templateRepository;
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task<(string, string)> BuildAsync(string templateSlug, object args)
        {
            var template = await _templateRepository.GetByAsync(new Where(nameof(MiaEmailTemplate.Slug), templateSlug));
            if (template is null)
                throw new Exception("template not found");

            string content_text = getContentTemplate(template.Title, template.ContentText, templateSlug);

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

        private string getContentTemplate(string title, string content_text, string template_slug)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string templateMail = Path.Combine(baseDir, "TemplateMailEN.txt");

            string lang = template_slug.Substring(template_slug.Length - 3);
            if (lang == "-es")
                templateMail = Path.Combine(baseDir, "TemplateMailES.txt");

            if (!File.Exists(templateMail))
                throw new Exception("file template not found");

            if (string.IsNullOrEmpty(_options.FontUrl))
                throw new Exception("font url not found");

            string str = File.ReadAllText(templateMail);

            str = str.Replace($"{{{{title}}}}", title, StringComparison.InvariantCultureIgnoreCase)
                     .Replace($"{{{{font_url}}}}", _options.FontUrl, StringComparison.InvariantCultureIgnoreCase)
                     .Replace($"{{{{content_text}}}}", content_text, StringComparison.InvariantCultureIgnoreCase);

            return str;
        }
    }
}