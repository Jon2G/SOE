using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SOE.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
   public class UserValidation : DataTypeAttribute
    {
        public UserValidation() : base(DataType.Custom)
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] MemberNames = new string[]
            {
                validationContext.MemberName
            };
            if (value is not string _User)
            {
                return new ValidationResult(this.ErrorMessage, MemberNames);
            }
            if (string.IsNullOrEmpty(_User))
            {
                return new ValidationResult("El usuario no puede estar vacio ", MemberNames);
            }
            if (!Models.Data.Validations.IsValidUser(_User))
            {
                return new ValidationResult(this.ErrorMessage, MemberNames);
            }
            return ValidationResult.Success;
        }
    }
}
