namespace XAF.Blazor.Server.ModelsDTO;

public class TokenDataDTO
{
    public TokenHeader Header { get; set; }
    public TokenPayload Payload { get; set; }
}

public class TokenPayload
{
    public string Ver { get; set; }
    public string Iss { get; set; }
    public string Sub { get; set; }
    public string Aud { get; set; }
    public int Exp { get; set; }
    public string Nonce { get; set; }
    public int Iat { get; set; }
    public int AuthTime { get; set; }
    public string Oid { get; set; }
    public List<string> Emails { get; set; }
    public bool NewUser { get; set; }
    public string Tfp { get; set; }
    public int Nbf { get; set; }
}

public class TokenHeader
{
    public string Alg { get; set; }
    public string Kid { get; set; }
    public string Typ { get; set; }
}
