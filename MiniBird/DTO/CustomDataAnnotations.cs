using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniBird.DTO
{
    public class CustomDataAnnotations : ValidationAttribute
    {
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public sealed class NotEqualToAttribute : ValidationAttribute
        {
            private const string DefaultErrorMessage = "{0} cannot be the same as {1}.";

            public string OtherProperty { get; private set; }

            public NotEqualToAttribute(string otherProperty)
              : base(DefaultErrorMessage)
            {
                if (string.IsNullOrEmpty(otherProperty))
                {
                    throw new ArgumentNullException("otherProperty");
                }

                OtherProperty = otherProperty;
            }

            public override string FormatErrorMessage(string name)
            {
                return string.Format(ErrorMessageString, name, OtherProperty);
            }

            protected override ValidationResult IsValid(object value,
                                  ValidationContext validationContext)
            {
                if (value != null)
                {
                    var otherProperty = validationContext.ObjectInstance.GetType()
                                       .GetProperty(OtherProperty);

                    var otherPropertyValue = otherProperty
                                  .GetValue(validationContext.ObjectInstance, null);

                    if (value.Equals(otherPropertyValue))
                    {
                        return new ValidationResult(
                          FormatErrorMessage(validationContext.DisplayName));
                    }
                }

                return ValidationResult.Success;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateAgeAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "Your age is invalid, your {0} should fall between {1} and {2}";

        public DateTime MinimumDateProperty { get; private set; }
        public DateTime MaximumDateProperty { get; private set; }

        public ValidateAgeAttribute(int minimumAgeProperty, int maximumAgeProperty) : base(DefaultErrorMessage)
        {
            MaximumDateProperty = DateTime.Now.AddYears(minimumAgeProperty * -1);
            MinimumDateProperty = DateTime.Now.AddYears(maximumAgeProperty * -1);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime parsedValue = (DateTime)value;

                if (parsedValue <= MinimumDateProperty || parsedValue >= MaximumDateProperty)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;

        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "validateage",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };

            rule.ValidationParameters.Add("minumumdate", MinimumDateProperty.ToShortDateString());
            rule.ValidationParameters.Add("maximumdate", MaximumDateProperty.ToShortDateString());

            yield return rule;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, MinimumDateProperty.ToShortDateString(), MaximumDateProperty.ToShortDateString());
        }
    }
}