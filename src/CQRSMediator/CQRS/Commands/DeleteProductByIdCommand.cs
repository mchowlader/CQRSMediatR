using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRSMediator.CQRS.Commands;

public record DeleteProductByIdCommand : IRequest<int>
{
    public int Id { get; set; }
}

public class DeleteProductByIdCommandHandler : IRequestHandler<DeleteProductByIdCommand, int>
{
    private ProductContext _context;

    public DeleteProductByIdCommandHandler(ProductContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(DeleteProductByIdCommand command, CancellationToken cancellationToken)
    {
        var product = await _context.Products.Where(p => p.Id == command.Id).FirstOrDefaultAsync();
        if (product == null)
        {
            return default;    
        }
        else
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }
    }
}
