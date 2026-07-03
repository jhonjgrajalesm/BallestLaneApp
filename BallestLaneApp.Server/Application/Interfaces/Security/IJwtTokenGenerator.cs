using BallestLaneApp.Server.Domain.Entities;

namespace BallestLaneApp.Server.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user);
}