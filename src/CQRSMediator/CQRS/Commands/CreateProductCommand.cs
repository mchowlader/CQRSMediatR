using CQRSMediator.Models;
using MediatR;

namespace CQRSMediator.CQRS.Commands
{
    public record CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private ProductContext _context;

        public ProductCommandHandler(ProductContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product();
            product.Name = command.Name;
            product.Price = command.Price;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }
    }
}