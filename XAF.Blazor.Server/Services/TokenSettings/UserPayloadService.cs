using XAF.Blazor.Server.ModelsDTO;

namespace XAF.Blazor.Server.Services.TokenSettings;

public class UserPayloadService : IUserPayloadService
{
    public TokenPayload UserPayload { get; set; }

    public void SetUserDataPayload(TokenPayload payload) => UserPayload = payload;

    public TokenPayload GetUserDataPayload() => UserPayload;
}