using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Codestellation.Galaxy.WebEnd.TagBuilder;
using Nancy.Helpers;
using Nancy.ViewEngines.Razor;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public static class ViewExtensions
    {
        public static IHtmlString LabeledTextBox<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        {
            Tag input = Tags.Input.Text();

            return BuildInput(htmlHelper, property, input);
        }

        public static IHtmlString LabeledNumberBox<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        {
            Tag input = Tags.Input.Number();

            return BuildInput(htmlHelper, property, input);
        }

        public static IHtmlString LabeledPasswordBox<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        {
            return BuildInput(htmlHelper, property, Tags.Input.Password());
        }

        public static IHtmlString DropDownList<TModel, TProperty, TDisplayValue>(
            this HtmlHelpers<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> property, 
            IEnumerable<KeyValuePair<TProperty, TDisplayValue>> values)
        {
            var currentValue = Reader.Read(htmlHelper.Model, property);

            var hasSelected = false;

            //TODO: Use id of feed instead of values
            var options = values.Select(item =>
                {
                    bool isSelected = item.Key.Equals(currentValue);
                    if (isSelected) hasSelected = true;
                    return Tags.Input.Option().Selected(isSelected).Content(item.Value).Value(item.Key);
                });

            if (!hasSelected)
            {
                //TODO: Instead of hardcoded string use placeholder from display attribute
                var placeholderOption = Tags.Input.Option().Disabled().Selected(true).Content(HttpUtility.HtmlEncode("<Select..>"));
                options = new[] { placeholderOption }.Union(options);
            }

            return BuildInput(htmlHelper, property, Tags.Input.Select().Content(options.ToArray()));
        }

        public static IHtmlString LabeledCheckBox<TModel>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, bool>> property)
        {
            var member = property.GetMember();
            var path = property.ToMemberPath();
            var display = member.GetDisplay();

            bool value = Reader.Read(htmlHelper.Model, property);

            var formGroup = Tags.Div().Classes(BootstrapClass.FormGroup).Content(
                Tags.Div().Classes(BootstrapClass.ColSmOffset2, BootstrapClass.ColSm10).Content(
                    Tags.Div().Classes(BootstrapClass.Checkbox).Content(
                        Tags.Label().Content(
                            Tags.Input.CheckBox().Checked(value).Name(path).Content(display.Name)
                            )
                        )
                    )
                );
            return formGroup;
        }

        private static IHtmlString BuildInput<TModel, TProperty>(HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property, Tag input)
        {
            var member = property.GetMember();
            var path = property.ToMemberPath();

            DisplayAttribute display;
            string placeholder = string.Empty;
            string name = member.Name;

            if (member.TryGetDisplay(out display))
            {
                placeholder = display.Prompt;
                name = display.Name;
            }

            TProperty value = Reader.Read(htmlHelper.Model, property);

            input.Classes(BootstrapClass.FormControl)
                .Id(path)
                .Name(path)
                .Value(value);

            if (!string.IsNullOrWhiteSpace(placeholder))
            {
                input.Placeholder(placeholder);
            }

            if(input is SelectTag)
                return BuildVerticalFormControl(path, name, input);
            else
                return BuildFormControl(path, name, input);
        }

        private static IHtmlString BuildFormControl(string path, string name, Tag input)
        {
            var formGroup = Tags.Div().Classes(BootstrapClass.FormGroup).Content(
                Tags.Label().For(path).Classes(BootstrapClass.ColSm2, BootstrapClass.ControlLabel).Content(name),
                Tags.Div().Classes(BootstrapClass.ColSm4).Content(input));

            return new NonEncodedHtmlString(formGroup.ToHtmlString());
        }
        private static IHtmlString BuildVerticalFormControl(string path, string name, Tag input)
        {
            var formGroup = Tags.Div().Classes(BootstrapClass.FormGroup).Content(
                Tags.Label().For(path).Content(name),
                Tags.Div().Content(input));

            return new NonEncodedHtmlString(formGroup.ToHtmlString());
        }
    }
}