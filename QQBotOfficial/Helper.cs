using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

namespace QQBotOfficial;

public static class GoEd25519Compatible
{
    public static string GenerateSignature(string plainToken, string botSecret, string eventTs)
    {
        // 完全复制Go的种子处理
        string seed = botSecret;
        while (seed.Length < 32)
        {
            seed += seed;
        }

        seed = seed.Substring(0, 32);

        // 使用Go的确定性随机数生成方式
        byte[] seedBytes = Encoding.UTF8.GetBytes(seed);
        using (var rng = new DeterministicRandom(seedBytes))
        {
            // 生成密钥对（模拟Go的ed25519.GenerateKey）
            var privateKey = new byte[64];
            rng.NextBytes(privateKey, 0, 32); // 前32字节是私钥种子

            // 计算公钥
            var publicKey = GetPublicKey(privateKey);
            Buffer.BlockCopy(publicKey, 0, privateKey, 32, 32);

            // 签名
            string message = eventTs + plainToken;
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] signature = SignMessage(messageBytes, privateKey);

            return BytesToHex(signature);
        }
    }

    private static byte[] GetPublicKey(byte[] privateKey)
    {
        byte[] seed = new byte[32];
        Buffer.BlockCopy(privateKey, 0, seed, 0, 32);

        var privateKeyParam = new Ed25519PrivateKeyParameters(seed, 0);
        return privateKeyParam.GeneratePublicKey().GetEncoded();
    }

    private static byte[] SignMessage(byte[] message, byte[] privateKey)
    {
        byte[] seed = new byte[32];
        Buffer.BlockCopy(privateKey, 0, seed, 0, 32);

        var privateKeyParam = new Ed25519PrivateKeyParameters(seed, 0);
        var signer = new Ed25519Signer();

        signer.Init(true, privateKeyParam);
        signer.BlockUpdate(message, 0, message.Length);

        return signer.GenerateSignature();
    }

    private static string BytesToHex(byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}

// 确定性随机数生成器（模拟Go的strings.Reader行为）
public class DeterministicRandom : IDisposable
{
    private readonly byte[] _seed;
    private int _position;

    public DeterministicRandom(byte[] seed)
    {
        _seed = seed;
        _position = 0;
    }

    public void NextBytes(byte[] buffer, int offset, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (_position >= _seed.Length)
                _position = 0;

            buffer[offset + i] = _seed[_position++];
        }
    }

    public void Dispose()
    {
    }
}