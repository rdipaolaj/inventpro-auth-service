using invenpro.auth.domain.AggregatesModel.UserAggregate;

namespace invenpro.auth.repository.Security;

public interface IJwtTokenGenerator
{
    string Generate(User user, string roleString);
}