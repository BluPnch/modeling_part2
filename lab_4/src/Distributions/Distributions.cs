using System;
using MathNet.Numerics.Distributions;

namespace lab_4.Distributions
{
    public interface IDistribution
    {
        string Name { get; }
        double GenerateSample();
    }

    // 1. Uniform Distribution
    public class UniformDistribution : IDistribution
    {
        private readonly ContinuousUniform _distribution;
        public string Name => "Равномерное";
        
        public UniformDistribution(double min, double max)
        {
            _distribution = new ContinuousUniform(min, max);
        }
        
        public double GenerateSample()
        {
            return _distribution.Sample();
        }
    }

    // 2. Exponential Distribution
    public class ExponentialDistribution : IDistribution
    {
        private readonly Exponential _distribution;
        public string Name => "Экспоненциальное";
        
        public ExponentialDistribution(double lambda)
        {
            _distribution = new Exponential(lambda);
        }
        
        public double GenerateSample()
        {
            return _distribution.Sample();
        }
    }

    // 3. Normal Distribution
    public class NormalDistribution : IDistribution
    {
        private readonly Normal _distribution;
        public string Name => "Нормальное";
        
        public NormalDistribution(double mean, double stdDev)
        {
            _distribution = new Normal(mean, stdDev);
        }
        
        public double GenerateSample()
        {
            return Math.Max(0.1, _distribution.Sample()); // Минимум 0.1 чтобы избежать отрицательных значений
        }
    }

    // 4. Poisson Distribution
    public class PoissonDistribution : IDistribution
    {
        private readonly Poisson _distribution;
        public string Name => "Пуассона";
        
        public PoissonDistribution(double lambda)
        {
            _distribution = new Poisson(lambda);
        }
        
        public double GenerateSample()
        {
            return _distribution.Sample();
        }
    }

    // 5. Erlang Distribution
    public class ErlangDistribution : IDistribution
    {
        private readonly double _lambda;
        private readonly int _k;
        private readonly Random _random;
        public string Name => $"Эрланга (k={_k})";
        
        public ErlangDistribution(int k, double lambda)
        {
            _k = k;
            _lambda = lambda;
            _random = new Random();
        }
        
        public double GenerateSample()
        {
            // Erlang is sum of k exponential distributions
            double sum = 0;
            for (int i = 0; i < _k; i++)
            {
                sum += -Math.Log(1 - _random.NextDouble()) / _lambda;
            }
            return sum;
        }
    }
}