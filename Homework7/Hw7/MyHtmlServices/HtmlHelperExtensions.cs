using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Hw7.ErrorMessages;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hw7.MyHtmlServices;

public static class HtmlHelperExtensions
{
    public static IHtmlContent MyEditorForModel(this IHtmlHelper helper)
    {
        var type = helper.ViewData.ModelMetadata.ModelType;
        var result = new HtmlContentBuilder();
        
        foreach (var property in type.GetProperties())
        {
            result.AppendHtml(HandleProperty(property, helper.ViewData.Model));
        }
        
        return result;
    }

    private static IHtmlContent HandleProperty(this PropertyInfo propertyInfo, object? model)
    {
        var html = new TagBuilder("form");
        
        html.InnerHtml.AppendHtml(propertyInfo.GetPropertyLabel());
        html.InnerHtml.AppendFormat("<p>");
        html.InnerHtml.AppendHtml(propertyInfo.GetPropertyInput());
        html.InnerHtml.AppendFormat("</p>");
        html.InnerHtml.AppendHtml(propertyInfo.HandlePropertyValidation(model));
        
        return html;
    }

    private static IHtmlContent GetPropertyLabel(this PropertyInfo propertyInfo)
    {
        var html = new TagBuilder("label");

        var display = (DisplayAttribute) propertyInfo.GetCustomAttribute(typeof(DisplayAttribute));
        
        html.Attributes.Add("for", propertyInfo.Name);
        html.InnerHtml.AppendHtmlLine(display == null || display.Name == String.Empty ? 
            propertyInfo.Name.SeparateByCamelCase() : display.Name!);
        
        
        return html;
    }
    private static IHtmlContent GetPropertyInput(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsEnum ? GetDropDown(propertyInfo) : GetInput(propertyInfo);
    }

    private static IHtmlContent GetDropDown(PropertyInfo propertyInfo)
    {
        var html = new TagBuilder("select");
        
        html.Attributes.Add("id", propertyInfo.Name);
        foreach (var item in Enum.GetValues(propertyInfo.PropertyType))
        {
            var innerHtml = new TagBuilder("option");
            innerHtml.Attributes.Add("value",item.ToString());
            innerHtml.InnerHtml.AppendFormat(item.ToString()!);
            html.InnerHtml.AppendHtml(innerHtml);
        }
        
        return html;
    }
    private static IHtmlContent GetInput(PropertyInfo propertyInfo)
    {
        var html = new TagBuilder("input");
        
        html.Attributes.Add("type", propertyInfo.PropertyType.IsValueType ? "number" : "text");
        html.Attributes.Add("id", propertyInfo.Name);
        
        return html;
    }

    private static IHtmlContent? HandlePropertyValidation(this PropertyInfo propertyInfo, object? model)
    {
        if (model == null || propertyInfo.PropertyType.IsEnum) 
            return null;

        var value = propertyInfo.GetValue(model);
        Validate(propertyInfo, value, out var message);
        return GetSpan(message);
    }
    private static IHtmlContent? GetSpan(string errorMessage)
    {
        var html = new TagBuilder("span");
        
        html.InnerHtml.SetContent(errorMessage);
        
        return html;
    }

    private static string SeparateByCamelCase(this string str)
    {
        return String.Join(" ", Regex.Split(str, "(?=\\p{Lu})")).TrimStart();
    }

    private static bool Validate(PropertyInfo propertyInfo, object? value, out string message)
    {
        message = String.Empty;
        
        if (value == null || value.ToString() == String.Empty)
        {
            message = Messages.RequiredMessage;
        }
        else if (value is int val && (val < 10 || val > 100))
        {
            message = $"{propertyInfo.Name.SeparateByCamelCase()} {Messages.RangeMessage}";
        }
        else if (value.ToString()!.Length > 30)
        {
            message = $"{propertyInfo.Name.SeparateByCamelCase()} {Messages.MaxLengthMessage}";
        }

        return message == String.Empty;
    }
} 