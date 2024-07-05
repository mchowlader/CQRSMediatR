using CQRSMediator.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRSMediator.CQRS.Queries;

public record GetAllProductQuery : IRequest<IEnumerable<Product>>
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, IEnumerable<Product>>
    {
        private ProductContext _context;

        public GetAllProductQueryHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> Handle(GetAllProductQuery query, CancellationToken cancellationToken)
        {
            var productList = await _context
                                    .Products
                                    .AsNoTracking()
                                    .ToListAsync();

            return productList;
        }
    }
}