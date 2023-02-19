using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MiaCore.Exceptions;
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

        public async Task<(string, string)> BuildAsync(int templateId, object args)
        {
            var template = await _templateRepository.GetAsync(templateId);
            if (template is null)
                throw new Exception("template not found");

            string content_text = getContentTemplate(template.Title, template.ContentText, template.Slug);

            string body = replace(content_text, args);
            return (template.Title, body);
        }

        public string GetTemplateSlug(string template, string language)
        {
            string lang = "es";
            if (!string.IsNullOrEmpty(language))
                lang = language;
            return string.Concat(template, "-", lang);
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
            string str = getContextText(template_slug);

            str = str.Replace($"{{{{title}}}}", title, StringComparison.InvariantCultureIgnoreCase)
                     .Replace($"{{{{content_text}}}}", content_text, StringComparison.InvariantCultureIgnoreCase);

            if (!string.IsNullOrEmpty(_options.FontUrl))
                str = str.Replace($"{{{{font_url}}}}", _options.FontUrl, StringComparison.InvariantCultureIgnoreCase);

            return str;
        }

        private string getContextText(string template_slug)
        {
            string context_text = @"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <meta name='x-apple-disable-message-reformatting'>
                <meta name='color-scheme' content='light'>
                <meta name='supported-color-schemes' content='light'>
                <title>{{title}}</title>
            </head>
            <body style='height: 100%; background-color: #1F1D2B;' bgcolor='#1F1D2B!important'>
                <div style='font-family:'Baloo 2','Roboto',-apple-system,BlinkMacSystemFont,sans-serif;color:#FFFFFF;font-weight:300' bgcolor='#1F1D2B'>
                  <table cellpadding='0' cellspacing='0' style='width:540px; max-width: 100%; border-radius: 1em; margin: 2em auto; background-color: #1F1D2B!important;' bgcolor='#1F1D2B'>
                    <tbody>
                        <tr>
                            <td align='center' style='font-size:14px; padding:3em'>
                              <div>
                                <img alt='eWire' width='74' height='68' style='display:block;line-height:1px;border:0' src='{{font_url}}/assets/img/logos/logo_white.png'>
                              </div>
                              {{content_text}}
                              <div style='background-color: #383838; height: 1px; margin: 2em 0;'></div>
                              <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'>Spontaneous coverages, reliable news</p>
                              <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'>eWire &nbsp; | &nbsp; <a href='mailto:notificaciones@ewire.news' target='_blank' data-saferedirecturl='' style='color: blue;'>notificaciones@ewire.news</a></p>
                              <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'><a href='{{font_url}}/legals/privacy-policy' target='_blank' data-saferedirecturl='' style='color: blue;'>Privacy policy</a> &nbsp; | &nbsp; <a href='{{font_url}}/legals/terms-and-conditions' target='_blank' data-saferedirecturl='' style='color: blue;'>Terms and conditions</a></p>
                              <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'>Copyright � 2022 eWire &nbsp; | &nbsp; All rights reserved</p>
                              <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'><a href='https://agencycoda.com/' target='_blank' data-saferedirecturl='' style='color: blue;'>Poweredy by Agencycoda</a></p>
                            </td>
                        </tr>
                    </tbody>
                  </table>
                </div>
            </body>
            </html>
            ";

            string lang = template_slug.Substring(template_slug.Length - 3);
            if (lang == "-es")
            {
                context_text = @"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <meta name='x-apple-disable-message-reformatting'>
                    <meta name='color-scheme' content='light'>
                    <meta name='supported-color-schemes' content='light'>
                    <title>{{title}}</title>
                </head>
                <body style='height: 100%; background-color: #1F1D2B;' bgcolor='#1F1D2B!important'>
                    <div style='font-family:'Baloo 2','Roboto',-apple-system,BlinkMacSystemFont,sans-serif;color:#FFFFFF;font-weight:300' bgcolor='#1F1D2B'>
                      <table cellpadding='0' cellspacing='0' style='width:540px; max-width: 100%; border-radius: 1em; margin: 2em auto; background-color: #1F1D2B!important;' bgcolor='#1F1D2B'>
                        <tbody>
                            <tr>
                                <td align='center' style='font-size:14px; padding:3em'>
                                  <div>
                                    <img alt='eWire' width='74' height='68' style='display:block;line-height:1px;border:0' src='{{font_url}}/assets/img/logos/logo_white.png'>
                                  </div>
                                  {{content_text}}
                                  <div style='background-color: #383838; height: 1px; margin: 2em 0;'></div>
                                  <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'>Coberturas espont�neas, noticias confiables</p>
                                  <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'>eWire &nbsp; | &nbsp; <a href='mailto:notificaciones@ewire.news' target='_blank' data-saferedirecturl='' style='color: blue;'>notificaciones@ewire.news</a></p>
                                  <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'><a href='{{font_url}}/legals/privacy-policy' target='_blank' data-saferedirecturl='' style='color: blue;'>Pol�ticas y privacidad</a> &nbsp; | &nbsp; <a href='{{font_url}}/legals/terms-and-conditions' target='_blank' data-saferedirecturl='' style='color: blue;'>T�rminos y condiciones</a></p>
                                  <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'>Copyright � 2022 eWire &nbsp; | &nbsp; All rights reserved</p>
                                  <p style='font-weight:300; color:#6a6a6b; font-size:12px; line-height:1.5em; margin: 0.25em;'><a href='https://agencycoda.com/' target='_blank' data-saferedirecturl='' style='color: blue;'>Poweredy by Agencycoda</a></p>
                                </td>
                            </tr>
                        </tbody>
                      </table>
                    </div>
                </body>
                </html>
                ";
            }

            return context_text;
        }
    }
}