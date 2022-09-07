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

        public string GetTemplateSlug(string template, string language)
        {
            string lang = "en";
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
            string context_text = "<!DOCTYPE html>" +
                                    "<html lang=\"en\">" +

                                    "<head>" +
                                      "<meta charset=\"UTF-8\">" +
                                      "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                                      "<title>{{title}}</title>" +
                                    "</head>" +

                                    "<body style=\"height: 100%;background-color: #000;\" bgcolor=\"#000\">" +
                                      "<div style=\"font-family:'Roboto',-apple-system,BlinkMacSystemFont,sans-serif;color:#FFF;font-weight:300\" bgcolor=\"#000\">" +
                                        "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:100%;height:100%;padding: 4em 0; background-color: #000;border-radius: 20px;\" bgcolor=\"#EDF0F4\">" +
                                          "<tbody>" +
                                            "<tr>" +
                                              "<th align=\"center\" style=\"padding-bottom: 40px;\">" +
                                                "<img src=\"https://ewire-dev.web.app/assets/img/logos/logo.png\" height=\"100px\">" +
                                                "</img>" +
                                              "</th>" +
                                            "</tr>" +

                                            "<tr>" +
                                              "<td align=\"center\">" +
                                                "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width:540px;border-radius: 1em;\" bgcolor=\"#262645\">" +
                                                  "<tbody>" +
                                                    "<tr>" +
                                                      "<td>" +
                                                        "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-color:#ebecee;border-style:solid;border-width:0px 0px 0px;\">" +
                                                          "<tbody>" +
                                                            "<tr>" +
                                                              "<td style=\"padding:2em 3em 0.5em\">" +
                                                                     "{{content_text}}" +
                                                              "</td>" +
                                                            "</tr>" +
                                                            "<tr>" +
                                                              "<td align=\"left\" style=\"font-size:14px;padding:0 3em 1em\">" +
                                                                "<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"text-transform:capitalize;margin:0 0 24px\">" +
                                                                  "<tbody>" +
                                                                    "<tr>" +
                                                                      "<td valign=\"middle\" align=\"left\">" +
                                                                        "<div>" +
                                                                          "<div>" +

                                                                          "</div>" +
                                                                        "</div>" +
                                                                      "</td>" +
                                                                    "</tr>" +
                                                                  "</tbody>" +
                                                                "</table>" +
                                                              "</td>" +
                                                            "</tr>" +
                       
                                                          "</tbody>" +
                                                        "</table>" +
                                                      "</td>" +
                                                    "</tr>" +
                                                  "</tbody>" +
                                                "</table>" +
                                                "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width:460px;margin-top:32px\">" +
                                                  "<tbody>" +
                                                    "<tr>" +
                                                      "<td align=\"center\">" +
                                                         "<h3 style=\"font-weight:300;color:#6a6a6b;font-size:12px;letter-spacing:.02em;line-height:20px;margin:0;padding:0\">                          Spontaneous coverages, reliable news<br>" +
                                    "eWire | <a href=\"mailto:notificaciones@ewire.news\">notificaciones@ewire.news</a> <br>" +
                                                                                      "<a href=\"{{font_url}}/legals/privacy-policy\">Privacy policy</a> | Argentina <br>" +
                                    "Copyright © 2022 eWire | All rights reserved" +
                                    "<br>" +
                                    "<a href=\"{{font_url}}/legals/terms-and-conditions\">Terms and conditions</a>" +
                                                        "</h3>" +
                                                        "<h2 style=\"font-weight:500;margin:0;margin-top:1em;margin-bottom:2em;padding:0;font-size:12px;line-height:1.5em;letter-spacing:.02em;color:#203389\">" +
                                                          "Poweredy by <a href=\"https://agencycoda.com/\" target=\"_blank\" data-saferedirecturl=\"\" style=\"color:#203389;\"><u>Agencycoda</u></a>" +
                                                        "</h2>" +
                                                      "</td>" +
                                                    "</tr>" +
                                                  "</tbody>" +
                                                "</table>" +
                                              "</td>" +
                                            "</tr>" +
                                          "</tbody>" +
                                        "</table>" +
                                      "</div>" +
                                    "</body>" +

                                    "</html>";

            string lang = template_slug.Substring(template_slug.Length - 3);
            if (lang == "-es")
            {
                context_text = "<!DOCTYPE html>" +
                                    "<html lang=\"es\">" +

                                    "<head>" +
                                      "<meta charset=\"UTF-8\">" +
                                      "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                                      "<title>{{title}}</title>" +
                                    "</head>" +

                                    "<body style=\"height: 100%;background-color: #000;\" bgcolor=\"#000\">" +
                                      "<div style=\"font-family:'Roboto',-apple-system,BlinkMacSystemFont,sans-serif;color:#FFF;font-weight:300\" bgcolor=\"#000\">" +
                                        "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:100%;height:100%;padding: 4em 0; background-color: #000;border-radius: 20px;\" bgcolor=\"#EDF0F4\">" +
                                          "<tbody>" +
                                            "<tr>" +
                                              "<th align=\"center\" style=\"padding-bottom: 40px;\">" +
                                                "<img src=\"https://ewire-dev.web.app/assets/img/logos/logo.png\" height=\"100px\">" +
                                                "</img>" +
                                              "</th>" +
                                            "</tr>" +

                                            "<tr>" +
                                              "<td align=\"center\">" +
                                                "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width:540px;border-radius: 1em;\" bgcolor=\"#262645\">" +
                                                  "<tbody>" +
                                                    "<tr>" +
                                                      "<td>" +
                                                        "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-color:#ebecee;border-style:solid;border-width:0px 0px 0px;\">" +
                                                          "<tbody>" +
                                                            "<tr>" +
                                                              "<td style=\"padding:2em 3em 0.5em\">" +
                                                                     "{{content_text}}" +
                                                              "</td>" +
                                                            "</tr>" +
                                                            "<tr>" +
                                                              "<td align=\"left\" style=\"font-size:14px;padding:0 3em 1em\">" +
                                                                "<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"text-transform:capitalize;margin:0 0 24px\">" +
                                                                  "<tbody>" +
                                                                    "<tr>" +
                                                                      "<td valign=\"middle\" align=\"left\">" +
                                                                        "<div>" +
                                                                          "<div>" +

                                                                          "</div>" +
                                                                        "</div>" +
                                                                      "</td>" +
                                                                    "</tr>" +
                                                                  "</tbody>" +
                                                                "</table>" +
                                                              "</td>" +
                                                            "</tr>" +

                                                          "</tbody>" +
                                                        "</table>" +
                                                      "</td>" +
                                                    "</tr>" +
                                                  "</tbody>" +
                                                "</table>" +
                                                "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width:460px;margin-top:32px\">" +
                                                  "<tbody>" +
                                                    "<tr>" +
                                                      "<td align=\"center\">" +
                                                         "<h3 style=\"font-weight:300;color:#6a6a6b;font-size:12px;letter-spacing:.02em;line-height:20px;margin:0;padding:0\">                          Coberturas espontáneas, noticias confiable<br>" +
                                    "eWire | <a href=\"mailto:notificaciones@ewire.news\">notificaciones@ewire.news</a> <br>" +
                                                                                      "<a href=\"{{font_url}}/legals/privacy-policy\">Políticas y privacidad</a> | Argentina <br>" +
                                    "Copyright © 2022 eWire | Todos los derechos reservados" +
                                    "<br>" +
                                    "<a href=\"{{font_url}}/legals/terms-and-conditions\">Terms and conditions</a>" +
                                                        "</h3>" +
                                                        "<h2 style=\"font-weight:500;margin:0;margin-top:1em;margin-bottom:2em;padding:0;font-size:12px;line-height:1.5em;letter-spacing:.02em;color:#203389\">" +
                                                          "Poweredy by <a href=\"https://agencycoda.com/\" target=\"_blank\" data-saferedirecturl=\"\" style=\"color:#203389;\"><u>Agencycoda</u></a>" +
                                                        "</h2>" +
                                                      "</td>" +
                                                    "</tr>" +
                                                  "</tbody>" +
                                                "</table>" +
                                              "</td>" +
                                            "</tr>" +
                                          "</tbody>" +
                                        "</table>" +
                                      "</div>" +
                                    "</body>" +

                                    "</html>";
            }

            return context_text;
        }
    }
}