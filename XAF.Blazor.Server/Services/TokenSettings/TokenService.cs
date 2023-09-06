using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using XAF.Blazor.Server.ModelsDTO;

namespace XAF.Blazor.Server.Services.TokenSettings;

public class TokenService : ITokenService
{
    public TokenDataDTO TokenDecoder(string token)
    {
        try
        {
            var tokenData = new TokenDataDTO();
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);
            
            var jsonPayload = jwtToken.Payload.SerializeToJson();
            var resultPayload = JsonConvert.DeserializeObject<TokenPayload>(jsonPayload);
            tokenData.Payload = resultPayload;
            
            return tokenData;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error decoding token: " + ex.Message);
            //throw an error exception
            throw;
        }
    }
}