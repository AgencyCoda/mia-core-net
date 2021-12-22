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
        public async Task<string> BuildAsync(string templateSlug, object args)
        {
            var template = await _templateRepository.GetByAsync(new Where(nameof(MiaEmailTemplate.Slug), templateSlug));
            if (template is null)
                throw new Exception("template not found");

            string html = replace(template.ContentText, args);
            return html;
        }

        private string replace(string str, object args)
        {
            var dictionary = convertToDictionary(args);

            foreach (var item in dictionary)
            {
                str = str.Replace($"{{{{{item.Key}}}}}", item.Value.ToString());
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
    }
}