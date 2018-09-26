using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Domain_Layer.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ImageSizeAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _maxSize;
        private const string defaultErrorMessage = "La imágen es superior a {1}kb";

        public ImageSizeAttribute(int maxSize)
        {
            this._maxSize = maxSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value != null)
            {
                if ((value as HttpPostedFileBase).ContentLength <= _maxSize)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "imagesize",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };

            rule.ValidationParameters.Add("maxsize", _maxSize);            

            yield return rule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrWhiteSpace(base.ErrorMessage))
                return string.Format(defaultErrorMessage, name, _maxSize/1024);

            return string.Format(base.ErrorMessage, name);
        }
    }
}
