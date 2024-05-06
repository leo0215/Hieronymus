using Standart.Hash.xxHash;

public class XXHashService
{
    public static uint CalculateHash(string str, uint seed = 0)
    {
        try
        {
            return xxHash32.ComputeHash(str, seed);
        }
        catch (Exception)
        {
            return 0; // 或者根據需要返回一個預設值
        }
    }

    public static uint CalculateHash(byte[] data)
    {
        try
        {
            return (uint)xxHash32.ComputeHash(data);
        }
        catch (Exception)
        {
            return 0; // 或者根據需要返回一個預設值
        }
    }
}
