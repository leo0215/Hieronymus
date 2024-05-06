using System.Diagnostics;

public class MersenneTwister
{
    private const int N = 624;
    private const int M = 397;
    private const uint MATRIX_A = 0x9908B0DF;
    private const uint UPPER_MASK = 0x80000000;
    private const uint LOWER_MASK = 0x7FFFFFFF;

    private uint[] mt = new uint[N];
    private int mti = N + 1;

    public MersenneTwister(long seed = 0)
    {
        if (seed == 0)
        {
            seed = GetNanoTimestamp();
        }

        InitGenRand((uint)seed);
    }
    private long GetNanoTimestamp()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        long timestamp = stopwatch.ElapsedTicks * 1000000000 / Stopwatch.Frequency;
        return timestamp;
    }
    public void InitGenRand(uint s)
    {
        mt[0] = s;
        for (mti = 1; mti < N; mti++)
        {
            mt[mti] = (uint)((1812433253 * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti) & 0xFFFFFFFF);
        }
        mti = N;
    }

    public byte[] NextBytes(int length)
    {
        var buffer = new byte[length];
        for (int i = 0; i < length; i++)
        {
            buffer[i] = (byte)(GenRandInt32() & 0xFF);
        }
        return buffer;
    }

    public int Next(int minValue = 0, int maxValue = int.MaxValue)
    {
        if (minValue == maxValue)
            return minValue;
        if (minValue > maxValue)
            throw new ArgumentException("minValue must be less than or equal to maxValue");
        if (minValue == 0 && maxValue == int.MaxValue)
            return (int)(GenRandReal1() * 2147483647);
        long range = (long)maxValue - minValue + 1;
        if (range <= 0)
            throw new ArgumentException("maxValue must be greater than minValue");
        long unsignedValue = (long)(GenRandInt32() & 0x7FFFFFFF);
        long remainder = unsignedValue % range;
        long result = remainder + minValue;
        return (int)result;
    }

    public ulong GenRandInt32()
    {
        uint y;
        if (mti >= N)
        {
            for (int kk = 0; kk < N - M; kk++)
            {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + M] ^ (y >> 1) ^ ((y & 0x1) * MATRIX_A);
            }
            for (int kk = N - M; kk < N - 1; kk++)
            {
                y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ ((y & 0x1) * MATRIX_A);
            }
            y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
            mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ ((y & 0x1) * MATRIX_A);
            mti = 0;
        }

        y = mt[mti++];
        y ^= y >> 11;
        y ^= (y << 7) & 0x9D2C5680;
        y ^= (y << 15) & 0xEFC60000;
        y ^= y >> 18;

        return y;
    }

    public float NextFloat(bool includeOne = false)
    {
        if (includeOne)
            return (float)GenRandReal1();
        return (float)GenRandReal2();
    }

    public float NextFloatPositive()
    {
        return (float)GenRandReal3();
    }

    public double NextDouble(bool includeOne = false)
    {
        if (includeOne)
            return GenRandReal1();
        return GenRandReal2();
    }

    public double NextDoublePositive()
    {
        return GenRandReal3();
    }

    public double Next53BitRes()
    {
        return GenRandRes53();
    }

    public double GenRandReal1()
    {
        return GenRandInt32() * (1.0 / 4294967295.0);
    }

    public double GenRandReal2()
    {
        return GenRandInt32() * (1.0 / 4294967296.0);
    }

    public double GenRandReal3()
    {
        return (GenRandInt32() + 0.5) * (1.0 / 4294967296.0);
    }

    public double GenRandRes53()
    {
        uint a = (uint)(GenRandInt32() >> 5);
        uint b = (uint)(GenRandInt32() >> 6);
        return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
    }

}