using FluentValidation;
using Restaurant.Application.DTOs.TableDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Validators;

public class CreateTableValidator : AbstractValidator<CreateTableDto>
{
    public CreateTableValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).WithMessage("Masa adı boş ola bilməz.");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("Tutum 0-dan böyük olmalıdır.");
    }
}






