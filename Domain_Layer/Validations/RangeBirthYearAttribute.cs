using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Domain_Layer.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RangeBirthYearAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessage = "{1} a {2}";
        private int _minYear = DateTime.Now.Year - 100;
        private int _maxYear = DateTime.Now.Year;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                int year = Convert.ToInt32(value);

                if(year <= _minYear || year >= _maxYear)
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
                ValidationType = "rangebirthyear",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };

            rule.ValidationParameters.Add("minyear", _minYear);
            rule.ValidationParameters.Add("maxyear", _maxYear);

            yield return rule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrWhiteSpace(base.ErrorMessage))            
                return string.Format(DefaultErrorMessage, name, _minYear, _maxYear);            

            return string.Format(base.ErrorMessage, name, _minYear, _maxYear);
        }
    }
}
