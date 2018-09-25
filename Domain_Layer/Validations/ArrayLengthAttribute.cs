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
    public class ArrayLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _maxLength;        
        private const string DefaultErrorMessage = "Máximo: {1} imágenes";
        
        public ArrayLengthAttribute(int maxLength)
        {
            this._maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (((System.Collections.ICollection)value).Count <= _maxLength)            
                return ValidationResult.Success;
            else            
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));                        
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "arraylength",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };

            rule.ValidationParameters.Add("maxlength", _maxLength);

            yield return rule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrWhiteSpace(base.ErrorMessage))
                return string.Format(DefaultErrorMessage, name, _maxLength);            

            return string.Format(base.ErrorMessage, name, _maxLength);
        }
    }
}
