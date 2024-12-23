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
    public class UpdateProductCommand : UpdateProductCommandModel, IRequest<ObjectResult>
    {
    }
    public class UpdateProductCommandHandle : IRequestHandler<UpdateProductCommand, ObjectResult>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandle(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository, IUserRepository userRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ObjectResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var isProduct = await _productRepository.Queryable.AnyAsync(a => a.Id != request.Id && a.Name == request.Name);
            if (isProduct)
            {
                return new ObjectResult(new { Error = "Name đã tồn tại" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            var product = await _productRepository.GetbyId(request.Id);
            if (product == null)
            {
                return new ObjectResult(new { Error = "product không tồn tại" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            _mapper.Map(request, product);
            if (product.CreatedUserId == null)
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                if (Guid.TryParse(userIdStr, out Guid userId))
                {
                    var user = await _userRepository.GetbyId(userId);
                    if (user != null)
                    {
                        product.CreatedUserId = user.Id;
                    }
                }
            }
            product = await _productRepository.UpdateAsync(product);
            var result = _mapper.Map<ProductModel>(product);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
