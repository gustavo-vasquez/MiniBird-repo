﻿using System;
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
    public class MultipleFilesMaxSizeAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _maxSize;
        private string _defaultErrorMessage = "El peso total de los archivos supera los {1}kb";

        public MultipleFilesMaxSizeAttribute(int maxSize)
        {
            this._maxSize = maxSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<HttpPostedFileBase> files = value as IEnumerable<HttpPostedFileBase>;
            int totalSize = 0;

            if (files != null)
            {
                foreach(HttpPostedFileBase file in files)
                {
                    if(file != null)
                    {
                        if (file.ContentLength > _maxSize)
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

                        totalSize = totalSize + file.ContentLength;
                    }                    
                }

                if(totalSize > _maxSize)                                    
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));                

            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "multiplefilesmaxsize",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
            };

            rule.ValidationParameters.Add("maxsize", _maxSize);

            yield return rule;
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrWhiteSpace(base.ErrorMessage))
                return string.Format(_defaultErrorMessage, name, _maxSize / 1024);

            return string.Format(base.ErrorMessage, name);
        }
    }
}
