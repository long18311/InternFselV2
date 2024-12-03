using InternFselV2.Entities;
using InternFselV2.Model.QueryModel;
using InternFselV2.Repositories.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InternFselV2.Service.Queries.UserCommands
{
    public class LoginQuery : LoginQueryModel, IRequest<ObjectResult>
    {
    }
    public class LoginQueryHandler : IRequestHandler<LoginQuery, ObjectResult>
    {
        private readonly IUserRepository _userRepository;

        public LoginQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ObjectResult> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var user = await _userRepository.GetUserbyLoginmodel(request.Username ?? string.Empty, request.Password ?? string.Empty);
            if (user == null)
            {
                return new ObjectResult("User không tồn tại") { StatusCode = StatusCodes.Status400BadRequest };
            }
            return new ObjectResult(new {Token = CreateToken(user) }) { StatusCode = StatusCodes.Status200OK };
        }
        private string CreateToken(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.FullName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role , "User")

            };
            var authenkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecurityKey0123456789InternFselV2"));
            var token = new JwtSecurityToken(
            issuer: "https://localhost:7131",
            audience: "InternFselV2",
            expires: DateTime.Now.AddMinutes(30),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authenkey, SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
