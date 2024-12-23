using AutoMapper;
using InternFselV2.Model.CommandModel.ProductCmd;
using InternFselV2.Model.EnityModel;
using InternFselV2.Repositories.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternFselV2.Service.Queries.ProductCommands
{
    public class GetProductQuery : IRequest<ObjectResult>
    {
        public Guid Id { get; set; }
    }
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ObjectResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ObjectResult> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            //var user = await _userRepository.GetbyId(request.Id);
            var user = await _productRepository.Queryable.Include(a => a.CreatedUser).FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (user == null)
            {
                return new ObjectResult(null){ StatusCode = StatusCodes.Status200OK };
            }

            var result = _mapper.Map<ProductModel>(user);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
