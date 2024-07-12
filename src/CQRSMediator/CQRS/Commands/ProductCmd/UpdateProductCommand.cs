using CQRSMediator.Context;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CQRSMediator.CQRS.Commands.ProductCmd;

public record UpdateProductCommand : IRequest<int>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, int>
{
    private ProductContext _context;

    public UpdateProductCommandHandler(ProductContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _context.Products
                            .Where(p => p.Id == command.Id)
                            .FirstOrDefaultAsync();
        if (product == null)
        {
            return default;
        }
        else
        {
            if (command.Name != null)
            {
                product.Name = command.Name;
            }
            if (command.Price.HasValue)
            {
                product.Price = command.Price.Value;
            }

            await _context.SaveChangesAsync();
            return product.Id;
        }
    }
}