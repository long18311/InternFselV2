using AutoMapper;
using InternFselV2.Entities;
using InternFselV2.Model.CommandModel.UserCmd;
using InternFselV2.Model.EnityModel;
using InternFselV2.Repositories.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternFselV2.Service.Command.UserCommands
{
    public class UpdateUserCommand : UpdateUserCommandModel, IRequest<ObjectResult>
    {
    }
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ObjectResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ObjectResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            
            var user = await _userRepository.Queryable.FirstOrDefaultAsync(a => a.Id != request.Id && a.UserName == request.UserName);
            if (user != null)
            {
                return new ObjectResult(new { Error = "UserName đã tồn tại" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            user = await _userRepository.GetbyId(request.Id);
            if (user == null)
            {
                return new ObjectResult(new { Error = "User không tồn tại" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            _mapper.Map(request, user);
            user = await _userRepository.UpdateAsync(user);
            if (user == null)
            {
                return new ObjectResult(new { Error = "Lỗi hệ thống" }) { StatusCode = StatusCodes.Status400BadRequest };
            }
            var result = _mapper.Map<UserModel>(user);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
