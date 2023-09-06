using XAF.Blazor.Server.ModelsDTO;
namespace XAF.Blazor.Server.Services.TokenSettings;

public interface ITokenService
{
    TokenDataDTO TokenDecoder(string token);
}