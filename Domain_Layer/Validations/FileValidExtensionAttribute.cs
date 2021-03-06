﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Domain_Layer.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileValidExtensionAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string[] _extensions;
        private const string _defaultErrorMessage = "Sólo archivos {1}";

        public FileValidExtensionAttribute(params string[] extensions)
        {
            this._extensions = extensions.Select(s => s.ToLower()).ToArray();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string fileName = (value as HttpPostedFileBase).FileName;
                string extension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();

                if (!_extensions.Contains(extension))
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "filevalidextension",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };
            
            rule.ValidationParameters.Add("extensions", new JavaScriptSerializer().Serialize(_extensions));

            yield return rule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrWhiteSpace(base.ErrorMessage))
                return string.Format(_defaultErrorMessage, name, string.Join(",", _extensions));

            return string.Format(base.ErrorMessage, name);
        }
    }
}
