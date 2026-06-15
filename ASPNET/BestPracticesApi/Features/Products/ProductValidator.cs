using FluentValidation;

namespace BestPracticesApi.Features.Products;

// 02. FluentValidation — kural bazlı model doğrulama
public sealed class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunlu")
            .MaximumLength(100).WithMessage("Ürün adı 100 karakteri aşamaz");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalı");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz");
    }
}

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunlu")
            .MaximumLength(100).WithMessage("Ürün adı 100 karakteri aşamaz");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalı");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz");
    }
}
