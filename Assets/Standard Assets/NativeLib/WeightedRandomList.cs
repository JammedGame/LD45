using System.Collections.Generic;
using Unity.Collections;

public readonly struct WeightedRandomList
{
    public readonly NativeArray<float> weights;

    public WeightedRandomList(IProbability[] input)
    {
        weights = new NativeArray<float>(input.Length, Allocator.TempJob);

        var totalWeight = 0f;
        for(int i = 0; i < input.Length; i++)
            totalWeight += input[i].Probability;

        var weight = 0f;
        for(int i = 0; i < input.Length; i++)
        {
            weights[i] = weight + input[i].Probability / totalWeight;
            weight = weights[i];
        }
    }

    public int NextIndex(ref Unity.Mathematics.Random rng)
    {
        return NextIndex(rng.NextFloat());
    }

    public int NextIndex(float seed)
    {
        for(int i = 0; i < weights.Length; i++)
        {
            if (seed <= weights[i])
            {
                return i;
            }
        }

        return weights.Length - 1;
    }

    public void Dispose()
    {
        weights.Dispose();
    }
}

public interface IProbability
{
    float Probability { get; }
}