using AutoMapper;
using InternFselV2.Entities;
using InternFselV2.Model.CommandModel.ProductCmd;
using InternFselV2.Repositories.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text;

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

        public CreateProductCommandSQLHandle(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository, IUserRepository userRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
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
            var sqlStr = GetSQLCreateEntity(product, "Products");
            var sqlBytes = Encoding.UTF8.GetBytes(sqlStr);
            return new ObjectResult(sqlBytes) { StatusCode = StatusCodes.Status200OK };
        }
        private static readonly Type[] NumericType = [typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), 
            typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)];
        private string GetSQLCreateEntity<T>(T entity,string tableName) where T : Entity
        {
            string sql = $"INSERT INTO [dbo].[{tableName}]";
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !p.IsDefined(typeof(NotMappedAttribute), true) && ( !typeof(IEnumerable).IsAssignableFrom(p.PropertyType) || p != typeof(string)) && !typeof(Entity).IsAssignableFrom(p.PropertyType));
            List<string> fields = new List<string>();
            List<string> values = new List<string>();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);
                if (value != null)
                {
                    fields.Add($"[{prop.Name}]");
                    if (NumericType.Contains(prop.PropertyType))
                    {
                        values.Add(value.ToString()!);
                    }
                    else if (prop.PropertyType == typeof(string) || prop.PropertyType.IsEnum)
                    {
                        value = value.ToString()?.Replace("'", "''");
                        values.Add($"N'{value}'");
                    }
                    else
                    {
                        values.Add($"'{value}'");
                    }

                }
            }
            sql = sql + '(' + string.Join(',', fields.Where(x => !string.IsNullOrEmpty(x)).ToList()) + ") VALUES ("+ string.Join(',', values.Where(x => !string.IsNullOrEmpty(x)).ToList()) + ")";
            return sql;
        }        
        /*private static readonly HashSet<Type> NumericTypeSet = new HashSet<Type>
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
            typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        };
        private bool IsNumericType(Type type)
        {
            return NumericTypeSet.Contains(type);
        }*/
    }
}
