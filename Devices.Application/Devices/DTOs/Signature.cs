﻿using System.Text;
using System.Text.Json;
using Devices.Application.Devices.Commands.RegisterDevice;
using Microsoft.IdentityModel.Tokens;

namespace Devices.Application.Devices.DTOs
{
    public class Signature
    {
        public Signature(SignatureAlgorithm algorithm, byte[] bytes)
        {
            Algorithm = algorithm;
            Bytes = bytes;
        }

        public SignatureAlgorithm Algorithm { get; set; }
        public byte[] Bytes { get; set; }

        public byte[] ToBytes()
        {
            var obj = new
            {
                sig = Bytes,
                alg = Algorithm
            };
            var signatureJsonString = JsonSerializer.Serialize(obj);
            var bytes = Encoding.UTF8.GetBytes(signatureJsonString);

            return bytes;
        }

        public static Signature FromBytes(byte[] bytes)
        {
            var signatureJsonString = Encoding.UTF8.GetString(bytes);
            var signatureObject = JsonSerializer.Deserialize<dynamic>(signatureJsonString, new JsonSerializerOptions {Converters = {new DynamicJsonConverter()}});
            var signature = Base64UrlEncoder.DecodeBytes((string) signatureObject.sig);
            var algorithm = (SignatureAlgorithm) signatureObject.alg;

            return new Signature(algorithm, signature);
        }
    }

    public enum SignatureAlgorithm
    {
        Ecdsa_P256 = 1,
        Ecdsa_P521 = 2,
        Ecdsa_Ed25519 = 3
    }
}
