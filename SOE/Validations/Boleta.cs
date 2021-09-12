using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SOE.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class Boleta : DataTypeAttribute
    {
        public Boleta() : base(DataType.Custom)
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] MemberNames = new string[]
            {
                validationContext.MemberName
            };
            if (value is not string _boleta)
            {
                return new ValidationResult(this.ErrorMessage,MemberNames);
            }
            if (string.IsNullOrEmpty(_boleta))
            {
                return new ValidationResult("La boleta no puede estar vacia ", MemberNames);
            }
            if (!SOEWeb.Shared.Validations.IsValidBoleta(_boleta))
            {
                return new ValidationResult(this.ErrorMessage, MemberNames);
            }
            return ValidationResult.Success;
        }
    }
}
