using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using FluentValidation;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Core.CrossCuttingConcerns.Validation.FluentValidation
{
    public static class ValidationTool
    {
        public static void Validate<T>(IValidator<T> validator,T entity)
        {
            var result = validator.Validate(entity);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors.ToString());
            }
        }
}
}
