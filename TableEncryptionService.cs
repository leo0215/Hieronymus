using System;
using System.Collections.Generic;
using System.Text;

public static class TableEncryptionService
{
    public static byte[] CreateKey(string name, int size = 8)
    {
        ulong seed = XXHashService.CalculateHash(name);
        MersenneTwister mt = new MersenneTwister((long)seed);
        return mt.NextBytes(size);
    }

    public static string NewZipPassword(string key)
    {
        byte[] keyBytes = CreateKey(key, 15);
        return Convert.ToBase64String(keyBytes);
    }

    public static byte[] XOR(string name, byte[] data)
    {
        if (data.Length == 0)
            return data;

        byte[] mask = CreateKey(name, data.Length);
        return _XOR(data, mask);
    }

    private static byte[] _XOR(byte[] value, byte[] key)
    {
        if (value.Length == key.Length)
            return XORBytes(value, key);
        else if (value.Length < key.Length)
            return XORBytes(value, key.AsSpan(0, value.Length).ToArray());
        else
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < value.Length / key.Length; i++)
            {
                byte[] segment = value[(i * key.Length)..((i + 1) * key.Length)];
                Array.Copy(value, i * key.Length, segment, 0, key.Length);
                result.AddRange(XORBytes(segment, key));
            }
            byte[] remainingBytes = new byte[value.Length % key.Length];
            Array.Copy(value, value.Length - remainingBytes.Length, remainingBytes, 0, remainingBytes.Length);
            result.AddRange(XORBytes(remainingBytes, key.AsSpan(0, remainingBytes.Length).ToArray()));
            return result.ToArray();
        }
    }

    private static byte[] XORBytes(byte[] value, byte[] key)
    {
        byte[] result = new byte[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            result[i] = (byte)(value[i] ^ key[i]);
        }
        return result;
    }

    public static T XOR_Struct<T>(T value, byte[] key, Func<byte[], T> structConverter)
    {
        byte[] bytes;
        if (typeof(T) == typeof(byte[]))
        {
            bytes = value as byte[];
        }
        else if (typeof(T) == typeof(string))
        {
            bytes = Encoding.Unicode.GetBytes(value as string);
        }
        else
        {
            throw new ArgumentException("Unsupported type for value");
        }
        byte[] result = _XOR(bytes, key);
        Dictionary<Type, Func<byte[], int, T>> converters = new Dictionary<Type, Func<byte[], int, T>>()
        {
            { typeof(sbyte), (data, offset) => (T)(object)data[offset] },
            { typeof(byte), (data, offset) => (T)(object)data[offset] },
            { typeof(short), (data, offset) => (T)(object)BitConverter.ToInt16(data, offset) },
            { typeof(ushort), (data, offset) => (T)(object)BitConverter.ToUInt16(data, offset) },
            { typeof(int), (data, offset) => (T)(object)BitConverter.ToInt32(data, offset) },
            { typeof(uint), (data, offset) => (T)(object)BitConverter.ToUInt32(data, offset) },
            { typeof(long), (data, offset) => (T)(object)BitConverter.ToInt64(data, offset) },
            { typeof(ulong), (data, offset) => (T)(object)BitConverter.ToUInt64(data, offset) },
            { typeof(float), (data, offset) => (T)(object)BitConverter.ToSingle(data, offset) },
            { typeof(double), (data, offset) => (T)(object)BitConverter.ToDouble(data, offset) },
        };
        if (converters.ContainsKey(typeof(T)))
        {
            return converters[typeof(T)](result, 0);
        }
        else if (typeof(T) == typeof(string))
        {
            return structConverter(result);
        }
        else
        {
            throw new ArgumentException("Unsupported type for value");
        }

    }

    public static sbyte ConvertSbyte(sbyte value, byte[] key)
    {
        return XOR_Struct(value, key, (byte[] bytes) => Convert.ToSByte(bytes[0]));
    }

    public static int ConvertInt(int value, byte[] key)
    {
        return XOR_Struct(value, key, (byte[] bytes) => BitConverter.ToInt32(bytes, 0));
    }

    public static long ConvertLong(long value, byte[] key)
    {
        return XOR_Struct(value, key, (byte[] bytes) => BitConverter.ToInt64(bytes, 0));
    }

    public static uint ConvertUInt(uint value, byte[] key)
    {
        return XOR_Struct(value, key, (byte[] bytes) => BitConverter.ToUInt32(bytes, 0));
    }

    public static ulong ConvertULong(ulong value, byte[] key)
    {
        return XOR_Struct(value, key, (byte[] bytes) => BitConverter.ToUInt64(bytes, 0));
    }

    public static float ConvertFloat(float value, byte[] key)
    {
        return ConvertInt(BitConverter.ToInt32(BitConverter.GetBytes(value), 0), key) * 0.00001f;
    }

    public static double ConvertDouble(double value, byte[] key)
    {
        return ConvertLong(BitConverter.ToInt64(BitConverter.GetBytes(value), 0), key) * 0.00001;
    }

    public static float EncryptFloat(float value, byte[] key)
    {
        return ConvertInt((int)(value * 100000), key);
    }

    public static double EncryptDouble(double value, byte[] key)
    {
        return ConvertLong((long)(value * 100000), key);
    }

    public static string ConvertString(object value, byte[] key)
    {
        if (value == null || value.ToString() == "")
        {
            return "";
        }
        try
        {
            string stringValue = value as string;
            byte[] raw = Convert.FromBase64String(stringValue);
            byte[] decryptedBytes = _XOR(raw, key);
            return Encoding.Unicode.GetString(decryptedBytes);
        }
        catch (FormatException)
        {
            if (value is byte[] byteArrayValue)
            {
                return Encoding.UTF8.GetString(_XOR(byteArrayValue, key));
            }
            else
            {
                throw new ArgumentException("Unsupported data type. Only base64 encoded string or byte array is supported.");
            }
        
        }


    }


    public static string EncryptString(string value, byte[] key)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        byte[] utf16Bytes = Encoding.Unicode.GetBytes(value);
        byte[] encryptedBytes = _XOR(utf16Bytes, key);
        StringBuilder hex = new StringBuilder(encryptedBytes.Length * 2);
        foreach (byte b in encryptedBytes)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }
}
