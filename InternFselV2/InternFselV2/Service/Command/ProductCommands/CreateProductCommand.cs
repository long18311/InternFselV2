using AutoMapper;
using InternFselV2.Entities;
using InternFselV2.Model.CommandModel.ProductCmd;
using InternFselV2.Repositories.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InternFselV2.Service.Command.ProductCommands
{
    public class CreateProductCommand : CreateProductCommandModel, IRequest<ObjectResult>
    {
    }
    public class CreateProductCommandHandle : IRequestHandler<CreateProductCommand, ObjectResult>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandle(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository, IUserRepository userRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ObjectResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var isProduct = await _productRepository.Queryable.AnyAsync(a => a.Name == request.Name);
            if(isProduct)
            {
                return new ObjectResult(new { Error = "Name đã tồn tại" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            var product = _mapper.Map<Product>(request);
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (Guid.TryParse(userIdStr, out Guid userId))
            {
                var user = await _userRepository.GetbyId(userId);
                if (user != null)
                {
                    product.CreatedUserId = user.Id;
                }
            }
                  
            product = await _productRepository.Create(product);
            var result = _mapper.Map<ProductModel>(product);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
