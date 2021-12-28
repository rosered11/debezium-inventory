using InventoryManagement.Domain.Utilities;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.DTO.Validation
{
    public enum ValidateEnumType { Request, Receive };
    public class ValidateEnumAttribute : ValidationAttribute
    {
        private string _validateEnumType;
        public ValidateEnumAttribute(string type = null)
        {
            _validateEnumType = type;
        }
        public override bool IsValid(object value)
        {
            bool validate = false;
            InputTransactionType inputType;

            // Check transaction type
            if (value != null && Enum.TryParse<Utilities.InputTransactionType>((string)value, out inputType))
            {
                validate = true;

                if (_validateEnumType != null)
                {
                    if (Enum.TryParse<ValidateEnumType>(_validateEnumType, out ValidateEnumType result))
                    {
                        switch (result)
                        {
                            case ValidateEnumType.Receive:
                                validate = inputType.IsReceive();
                                break;
                            case ValidateEnumType.Request:
                                validate = inputType.IsRequest();
                                break;
                        }
                    }
                    else
                    {
                        validate = false;
                    }
                }
            }           

            return validate;
        }
    }

    public class ValidateEnumReceiveAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool validate = false;

            // Check transaction type
            if (value != null && Enum.IsDefined(typeof(Utilities.InputTransactionType), value))
            {
                validate = true;
            }

            return validate;
        }
    }

    public class ValidateListAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool validate = false;

            // Check empty list
            if (value != null && ((IList)value).Count > 0)
            {
                validate = true;
            }

            return validate;
        }
    }
}
