using AutoMapper;
using InternFselV2.Entities;
using InternFselV2.Model.CommandModel.UserCmd;
using InternFselV2.Model.EnityModel;
using InternFselV2.Repositories.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace InternFselV2.Service.Command.UserCommands
{
    public class CreateUserCommand : CreateUserCommandModel, IRequest<ObjectResult>
    {
    }
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ObjectResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ObjectResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var user = await _userRepository.GetByUserName(request.UserName!);
            if (user != null) {
                return new ObjectResult(new {Error = "UserName đã tồn tại" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            user = _mapper.Map<User>(request);
            user = await _userRepository.Create(user);
            if(user == null) {
                return new ObjectResult(new { Error = "Lỗi hệ thống" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            var result = _mapper.Map<UserModel>(user);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

    }
}
