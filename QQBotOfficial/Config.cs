// #define Test

using Org.BouncyCastle.Asn1.X509;

namespace QQBotOfficial;

public static class Config
{
#if Test
    public const string BaseUrl = "https://sandbox.api.sgroup.qq.com";
# else
    public const string BaseUrl = "https://api.sgroup.qq.com";
#endif

    public const string AccessTokenBaseUrl = "https://bots.qq.com/app/getAppAccessToken";
    public const string Name = "和栗薰子";

    public const string ChatCommand = "/闲聊";
    public const string FortuneLookUp = "/今日运势";
    public const string GoodMorning = "/早安";
}