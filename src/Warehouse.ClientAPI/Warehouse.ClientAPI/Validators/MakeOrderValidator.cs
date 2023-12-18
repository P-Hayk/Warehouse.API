using FluentValidation;
using Warehouse.ClientAPI.ApiModels;

namespace Warehouse.ClientAPI.Validators
{
    public class MakeOrderValidator : AbstractValidator<MakeOrderRequest>
    {
        public MakeOrderValidator()
        {
            RuleFor(r => r.ProductId).NotNull().GreaterThan(0);
            RuleFor(r => r.Count).NotNull().GreaterThan(0);
        }
    }
}
