using CQRSMediator.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRSMediator.CQRS.Queries;

public record GetProductByIdQuery : IRequest<Product>
{
    public int Id { get; set; }
}
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    private ProductContext _context;

    public GetProductByIdQueryHandler(ProductContext context)
    {
        _context = context;
    }

    public async Task<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await _context.Products
                                .Where(p => p.Id == query.Id)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

        return product;
    }
}