using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Domain_Layer.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RangeBirthMonthAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "{1} a {2}";
        private const int _minMonth = 1;
        private const int _maxMonth = 12;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                int year = Convert.ToInt32(value);

                if (year <= _minMonth || year >= _maxMonth)
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
                ValidationType = "rangebirthmonth",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };

            rule.ValidationParameters.Add("minmonth", _minMonth);
            rule.ValidationParameters.Add("maxmonth", _maxMonth);

            yield return rule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrWhiteSpace(base.ErrorMessage))
                return string.Format(DefaultErrorMessage, name, _minMonth, _maxMonth);

            return string.Format(base.ErrorMessage, name, _minMonth, _maxMonth);
        }
    }
}
