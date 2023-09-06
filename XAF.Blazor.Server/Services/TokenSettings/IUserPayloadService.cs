using XAF.Blazor.Server.ModelsDTO;
namespace XAF.Blazor.Server.Services.TokenSettings;

public interface IUserPayloadService
{
    public TokenPayload UserPayload { get; set; }
    void SetUserDataPayload(TokenPayload payload);
    TokenPayload GetUserDataPayload();
}