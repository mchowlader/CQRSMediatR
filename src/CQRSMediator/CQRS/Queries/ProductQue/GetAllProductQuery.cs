using CQRSMediator.Context;
using CQRSMediator.Entities;
using CQRSMediator.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRSMediator.CQRS.Queries.ProductQue;

public record GetAllProductQuery(PaginationModel Pagination) : IRequest<PaginationResult<Product>>
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, PaginationResult<Product>>
    {
        private ProductContext _context;

        public GetAllProductQueryHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<PaginationResult<Product>> Handle(GetAllProductQuery query, CancellationToken cancellationToken)
        {
            var pagination = query.Pagination;
            var totalCount = await _context.Products
                                    .CountAsync(cancellationToken);
            var products = await _context.Products
                                    .AsNoTracking()
                                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                    .Take(pagination.PageSize)
                                    .ToListAsync(cancellationToken);

            return new PaginationResult<Product>
            {
                Items = products,
                TotalCount = totalCount
            };
        }
    }
}