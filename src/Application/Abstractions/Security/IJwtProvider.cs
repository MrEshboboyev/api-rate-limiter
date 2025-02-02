using Domain.Entities;

namespace Application.Abstractions.Security;

public interface IJwtProvider
{
    string Generate(User user);
}