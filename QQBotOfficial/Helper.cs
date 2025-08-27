using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace QQBotOfficial;

public class Ed25519Signer : IDisposable
{
    /// <summary>
    /// 从种子生成ED25519密钥对
    /// </summary>
    public static (byte[] privateKey, byte[] publicKey) GenerateKeyPairFromSeed(string seed)
    {
        string expandedSeed = ExpandSeed(seed, 32); // Ed25519种子需要32字节
        byte[] seedBytes = Encoding.UTF8.GetBytes(expandedSeed);

        // 使用SHA256哈希确保种子长度正确
        using (var sha256 = SHA256.Create())
        {
            byte[] hashedSeed = sha256.ComputeHash(seedBytes);

            // 使用确定性随机数生成器
            var random = new FixedSecureRandom(hashedSeed);
            var keyPairGenerator = new Ed25519KeyPairGenerator();
            keyPairGenerator.Init(new KeyGenerationParameters(random, 255));

            AsymmetricCipherKeyPair keyPair = keyPairGenerator.GenerateKeyPair();
            Ed25519PrivateKeyParameters privateKey = (Ed25519PrivateKeyParameters)keyPair.Private;
            Ed25519PublicKeyParameters publicKey = (Ed25519PublicKeyParameters)keyPair.Public;

            return (privateKey.GetEncoded(), publicKey.GetEncoded());
        }
    }

    /// <summary>
    /// 从私钥字节数组创建私钥参数
    /// </summary>
    public static Ed25519PrivateKeyParameters CreatePrivateKey(byte[] privateKeyBytes)
    {
        return new Ed25519PrivateKeyParameters(privateKeyBytes, 0);
    }

    /// <summary>
    /// 从公钥字节数组创建公钥参数
    /// </summary>
    public static Ed25519PublicKeyParameters CreatePublicKey(byte[] publicKeyBytes)
    {
        return new Ed25519PublicKeyParameters(publicKeyBytes, 0);
    }

    /// <summary>
    /// 对消息进行签名
    /// </summary>
    public static byte[] Sign(byte[] message, byte[] privateKeyBytes)
    {
        try
        {
            Ed25519PrivateKeyParameters privateKey = CreatePrivateKey(privateKeyBytes);

            // 使用Ed25519Signer进行签名
            var signer = new Org.BouncyCastle.Crypto.Signers.Ed25519Signer();
            signer.Init(true, privateKey);
            signer.BlockUpdate(message, 0, message.Length);

            return signer.GenerateSignature();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate signature", ex);
        }
    }

    /// <summary>
    /// 对字符串消息进行签名并返回十六进制字符串
    /// </summary>
    public static string Sign(string message, byte[] privateKeyBytes)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        byte[] signature = Sign(messageBytes, privateKeyBytes);
        return BytesToHex(signature);
    }

    /// <summary>
    /// 验证签名
    /// </summary>
    public static bool Verify(byte[] message, byte[] signature, byte[] publicKeyBytes)
    {
        try
        {
            Ed25519PublicKeyParameters publicKey = CreatePublicKey(publicKeyBytes);

            var signer = new Org.BouncyCastle.Crypto.Signers.Ed25519Signer();
            signer.Init(false, publicKey);
            signer.BlockUpdate(message, 0, message.Length);

            return signer.VerifySignature(signature);
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 从私钥获取公钥
    /// </summary>
    public static byte[] GetPublicKeyFromPrivate(byte[] privateKeyBytes)
    {
        Ed25519PrivateKeyParameters privateKey = CreatePrivateKey(privateKeyBytes);
        return privateKey.GeneratePublicKey().GetEncoded();
    }

    /// <summary>
    /// 扩展种子到指定长度
    /// </summary>
    private static string ExpandSeed(string seed, int targetLength)
    {
        if (string.IsNullOrEmpty(seed))
            throw new ArgumentException("Seed cannot be null or empty");

        if (seed.Length >= targetLength)
            return seed.Substring(0, targetLength);

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

    /// <summary>
    /// 十六进制字符串转字节数组
    /// </summary>
    private static byte[] HexToBytes(string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Hex string must have even length");

        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        return bytes;
    }

    public void Dispose()
    {
        // 清理资源
    }
}

/// <summary>
/// 固定种子的安全随机数生成器（用于确定性密钥生成）
/// </summary>
public class FixedSecureRandom : SecureRandom
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

    public override void NextBytes(byte[] buf, int off, int len)
    {
        for (int i = 0; i < len; i++)
        {
            if (_position >= _seed.Length)
                _position = 0;

            buf[off + i] = _seed[_position++];
        }
    }

    public byte NextByte()
    {
        if (_position >= _seed.Length)
            _position = 0;

        return _seed[_position++];
    }
}