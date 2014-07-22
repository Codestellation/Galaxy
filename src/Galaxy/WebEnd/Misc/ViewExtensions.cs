﻿using System;
using System.Collections.Generic;
using System.Linq;
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

            return BuildFormControlInput(htmlHelper, property, input);
        }

        public static IHtmlString LabeledTextArea<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        {
            Tag input = Tags.TextArea();

            return BuildFormControlInput(htmlHelper, property, input);
        }

        public static IHtmlString LabeledNumberBox<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        {
            Tag input = Tags.Input.Number();

            return BuildFormControlInput(htmlHelper, property, input);
        }

        public static IHtmlString LabeledPasswordBox<TModel, TProperty>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property)
        {
            return BuildFormControlInput(htmlHelper, property, Tags.Input.Password());
        }

        public static IHtmlString LabeledDropDown<TModel, TProperty, TDisplayValue>(
            this HtmlHelpers<TModel> htmlHelper, 
            Expression<Func<TModel, TProperty>> property, 
            IEnumerable<KeyValuePair<TProperty, TDisplayValue>> values)
        {
            var selectTag = BuildSelectTag(htmlHelper, property, values);

            return BuildFormControlInput(htmlHelper, property, selectTag);
        }

        public static IHtmlString NoLabelDropDown<TModel, TProperty, TDisplayValue>(
            this HtmlHelpers<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> property,
            IEnumerable<KeyValuePair<TProperty, TDisplayValue>> values)
        {

            var input = BuildSelectTag(htmlHelper, property, values);
            input.Classes(BootstrapClass.FormControl);

            return new NonEncodedHtmlString(input.ToHtmlString());
        }

        private static Tag BuildSelectTag<TModel, TProperty, TDisplayValue>(HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property,
            IEnumerable<KeyValuePair<TProperty, TDisplayValue>> values)
        {
            
            var currentValue = htmlHelper.Model.Read(property);
            var inputProperties = InputProperties.Get(property);


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
                var placeholderOption =
                    Tags.Input.Option().Disabled().Selected(true).Content(HttpUtility.HtmlEncode(inputProperties.Placeholder));
                options = new[] {placeholderOption}.Union(options);
            }

            var finalOption = options.ToArray();
            var selectTag = Tags.Input.Select().Content(finalOption).Name(inputProperties.Path);
            return selectTag;
        }

        public static IHtmlString LabeledCheckBox<TModel>(this HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, bool>> property)
        {
            var member = property.GetMember();
            var path = property.ToMemberPath();
            var display = member.GetDisplay();

            bool value = htmlHelper.Model.Read(property);

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

        private static IHtmlString BuildFormControlInput<TModel, TProperty>(HtmlHelpers<TModel> htmlHelper, Expression<Func<TModel, TProperty>> property, Tag input)
        {
            var inputProperties = InputProperties.Get(property);
            
            TProperty value = htmlHelper.Model.Read(property);

            input.Classes(BootstrapClass.FormControl)
                .Id(inputProperties.Path)
                .Name(inputProperties.Path)
                .Value(value);

            if (!string.IsNullOrWhiteSpace(inputProperties.Placeholder))
            {
                input.Placeholder(inputProperties.Placeholder);
            }

            return BuildFormControl(inputProperties.Path, inputProperties.Name, input);
        }

        private static IHtmlString BuildFormControl(string path, string name, Tag input)
        {
            var formGroup = Tags.Div().Classes(BootstrapClass.FormGroup).Content(
                Tags.Label().For(path).Classes(BootstrapClass.ColSm2, BootstrapClass.ControlLabel).Content(name),
                Tags.Div().Classes(BootstrapClass.ColSm4).Content(input));

            return new NonEncodedHtmlString(formGroup.ToHtmlString());
        }

        
    }
}