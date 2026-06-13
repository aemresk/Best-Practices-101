namespace ECommerce.Application.UseCases.PlaceOrder;

public record PlaceOrderCommand(string CustomerName, string CustomerEmail, decimal Amount);
public record PlaceOrderResult(int OrderId, decimal FinalAmount, decimal DiscountRate);
