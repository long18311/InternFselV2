using AutoMapper;
using InternFselV2.Entities;
using InternFselV2.Helpers;
using InternFselV2.Model.CommandModel.ProductCmd;
using InternFselV2.Repositories.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InternFselV2.Service.Command.ProductCommands
{
    public class CreateProductSQLCommand : CreateProductCommandModel, IRequest<ObjectResult>
    {
    }
    public class CreateProductCommandSQLHandle : IRequestHandler<CreateProductSQLCommand, ObjectResult>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly InternV2DbContext _internV2DbContext;

        public CreateProductCommandSQLHandle(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository, IUserRepository userRepository, IMapper mapper, InternV2DbContext internV2DbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _internV2DbContext = internV2DbContext;
        }

        public async Task<ObjectResult> Handle(CreateProductSQLCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var isProduct = await _productRepository.Queryable.AnyAsync(a => a.Name == request.Name);
            if (isProduct)
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
                    product.CreatedUser = user;
                }
            }
            var sqlStr = _userRepository.GetSQLCreateEntity(product);
            //var sqlStr = SQLHelper.GetSQLCreateEntity(product, _productRepository.Queryable);
            var sqlBytes = Encoding.UTF8.GetBytes(sqlStr);
            return new ObjectResult(sqlBytes) { StatusCode = StatusCodes.Status200OK };
        }

        
    }
}
