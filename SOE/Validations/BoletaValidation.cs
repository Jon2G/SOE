using System;
using System.ComponentModel.DataAnnotations;
using Xamarin.Forms.Internals;

namespace SOE.Validations
{
    [Preserve]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BoletaValidation : DataTypeAttribute
    {
        public BoletaValidation() : base(DataType.Custom)
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
                return new ValidationResult(this.ErrorMessage, MemberNames);
            }
            if (string.IsNullOrEmpty(_boleta))
            {
                return new ValidationResult("La boleta no puede estar vacia ", MemberNames);
            }
            if (!Models.Data.Validations.IsValidBoleta(_boleta))
            {
                return new ValidationResult(this.ErrorMessage, MemberNames);
            }
            return ValidationResult.Success;
        }
    }
}
