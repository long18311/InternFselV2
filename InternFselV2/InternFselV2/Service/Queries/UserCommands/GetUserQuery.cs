using AutoMapper;
using InternFselV2.Model.EnityModel;
using InternFselV2.Repositories.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternFselV2.Service.Queries.UserCommands
{
    public class GetUserQuery : IRequest<ObjectResult>
    {
        public Guid Id { get; set; }
    }
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ObjectResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ObjectResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            //var user = await _userRepository.GetbyId(request.Id);
            var user = await _userRepository.Queryable.AsNoTracking().Include(x => x.Products).FirstOrDefaultAsync(a => a.Id == request.Id);
            if (user == null)
            {
                return new ObjectResult(null){ StatusCode = StatusCodes.Status200OK };
            }

            var result = _mapper.Map<UserModel>(user);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
