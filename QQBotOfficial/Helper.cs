using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace QQBotOfficial;

public static class Ed25519SignatureGenerator
{
    /// <summary>
    /// 生成ED25519签名
    /// </summary>
    /// <param name="plainToken">明文令牌</param>
    /// <param name="botSecret">机器人密钥</param>
    /// <param name="eventTs">事件时间戳</param>
    /// <returns>十六进制格式的签名</returns>
    public static string GenerateSignature(string plainToken, string botSecret, string eventTs)
    {
        if (string.IsNullOrEmpty(plainToken))
            throw new ArgumentException("PlainToken cannot be null or empty");
        if (string.IsNullOrEmpty(botSecret))
            throw new ArgumentException("BotSecret cannot be null or empty");
        if (string.IsNullOrEmpty(eventTs))
            throw new ArgumentException("EventTs cannot be null or empty");

        try
        {
            // 1. 从botSecret生成确定性私钥
            byte[] privateKey = GeneratePrivateKeyFromSeed(botSecret);

            // 2. 构建签名消息：eventTs + plainToken
            string message = eventTs + plainToken;
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // 3. 使用ED25519签名
            byte[] signatureBytes = SignMessage(messageBytes, privateKey);

            // 4. 返回十六进制格式的签名
            return BytesToHex(signatureBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate signature", ex);
        }
    }

    /// <summary>
    /// 从种子生成ED25519私钥
    /// </summary>
    private static byte[] GeneratePrivateKeyFromSeed(string seed)
    {
        // 扩展种子到至少32字节（与Go代码行为一致）
        string expandedSeed = ExpandSeed(seed, 32);
        byte[] seedBytes = Encoding.UTF8.GetBytes(expandedSeed);

        // 使用SHA256哈希确保种子长度正确
        using (var sha256 = SHA256.Create())
        {
            byte[] hashedSeed = sha256.ComputeHash(seedBytes);

            // 使用确定性随机数生成器生成密钥对
            var random = new FixedSecureRandom(hashedSeed);
            var keyPairGenerator = new Ed25519KeyPairGenerator();
            keyPairGenerator.Init(new KeyGenerationParameters(random, 255));

            AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();
            Ed25519PrivateKeyParameters privateKey = (Ed25519PrivateKeyParameters)keyPair.Private;

            return privateKey.GetEncoded();
        }
    }

    /// <summary>
    /// 使用ED25519对消息进行签名
    /// </summary>
    private static byte[] SignMessage(byte[] message, byte[] privateKeyBytes)
    {
        Ed25519PrivateKeyParameters privateKey = new Ed25519PrivateKeyParameters(privateKeyBytes, 0);

        var signer = new Org.BouncyCastle.Crypto.Signers.Ed25519Signer();
        signer.Init(true, privateKey);
        signer.BlockUpdate(message, 0, message.Length);

        return signer.GenerateSignature();
    }

    /// <summary>
    /// 扩展种子到指定长度（复制Go代码逻辑）
    /// </summary>
    private static string ExpandSeed(string seed, int targetLength)
    {
        if (seed.Length >= targetLength)
            return seed.Substring(0, targetLength);

        // 与Go代码相同的逻辑：重复种子直到达到目标长度
        StringBuilder expanded = new StringBuilder(seed);
        while (expanded.Length < targetLength)
        {
            expanded.Append(seed);
        }

        return expanded.ToString(0, targetLength);
    }

    /// <summary>
    /// 字节数组转十六进制字符串
    /// </summary>
    private static string BytesToHex(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}

/// <summary>
/// 固定种子的安全随机数生成器（用于确定性密钥生成）
/// </summary>
internal class FixedSecureRandom : SecureRandom
{
    private readonly byte[] _seed;
    private int _position;

    public FixedSecureRandom(byte[] seed)
    {
        _seed = seed ?? throw new ArgumentNullException(nameof(seed));
        _position = 0;
    }

    public override void NextBytes(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            if (_position >= _seed.Length)
                _position = 0;

            bytes[i] = _seed[_position++];
        }
    }
}