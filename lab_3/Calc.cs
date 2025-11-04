using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

public interface IDistribution
{
    string Name { get; }
    double PDF(double x);
    double CDF(double x);
    double Mean();
    double Variance();
    double[] GenerateSamples(int count);
}

// 1. Uniform Distribution
public class UniformDistribution : IDistribution
{
    private double _a, _b;
    
    public UniformDistribution(double a = 0, double b = 1)
    {
        _a = a;
        _b = b;
    }
    
    public string Name => "Uniform Distribution";
    
    public double PDF(double x)
    {
        if (x < _a || x > _b) return 0;
        return 1.0 / (_b - _a);
    }
    
    public double CDF(double x)
    {
        if (x < _a) return 0;
        if (x > _b) return 1;
        return (x - _a) / (_b - _a);
    }
    
    public double Mean() => (_a + _b) / 2;
    public double Variance() => Math.Pow(_b - _a, 2) / 12;
    
    public double[] GenerateSamples(int count)
    {
        var uniform = new ContinuousUniform(_a, _b);
        double[] samples = new double[count];
        for (int i = 0; i < count; i++)
        {
            samples[i] = uniform.Sample();
        }
        return samples;
    }
}

// 2. Poisson Distribution
public class PoissonDistribution : IDistribution
{
    private double _lambda;
    
    public PoissonDistribution(double lambda = 3)
    {
        _lambda = lambda;
    }
    
    public string Name => "Poisson Distribution";
    
    public double PDF(double x)
    {
        int k = (int)Math.Round(x);
        if (k < 0) return 0;
        return Math.Exp(-_lambda) * Math.Pow(_lambda, k) / Factorial(k);
    }
    
    public double CDF(double x)
    {
        int k = (int)Math.Floor(x);
        if (k < 0) return 0;
        
        double sum = 0;
        for (int i = 0; i <= k; i++)
        {
            sum += Math.Exp(-_lambda) * Math.Pow(_lambda, i) / Factorial(i);
        }
        return sum;
    }
    
    public double Mean() => _lambda;
    public double Variance() => _lambda;
    
    public double[] GenerateSamples(int count)
    {
        var poisson = new Poisson(_lambda);
        double[] samples = new double[count];
        for (int i = 0; i < count; i++)
        {
            samples[i] = poisson.Sample();
        }
        return samples;
    }
    
    private double Factorial(int n)
    {
        double result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }
}

// 3. Exponential Distribution
public class ExponentialDistribution : IDistribution
{
    private double _lambda;
    
    public ExponentialDistribution(double lambda = 1)
    {
        _lambda = lambda;
    }
    
    public string Name => "Exponential Distribution";
    
    public double PDF(double x)
    {
        if (x < 0) return 0;
        return _lambda * Math.Exp(-_lambda * x);
    }
    
    public double CDF(double x)
    {
        if (x < 0) return 0;
        return 1 - Math.Exp(-_lambda * x);
    }
    
    public double Mean() => 1.0 / _lambda;
    public double Variance() => 1.0 / (_lambda * _lambda);
    
    public double[] GenerateSamples(int count)
    {
        double[] samples = new double[count];
        var exp = new Exponential(_lambda);
    
        for (int i = 0; i < count; i++)
        {
            samples[i] = exp.Sample();
        }
        return samples;
    }
}

// 4. Normal Distribution (Gaussian)
public class NormalDistribution : IDistribution
{
    private double _mu, _sigma;
    
    public NormalDistribution(double mu = 0, double sigma = 1)
    {
        _mu = mu;
        _sigma = sigma;
    }
    
    public string Name => "Normal Distribution (Gaussian)";
    
    public double PDF(double x)
    {
        return (1.0 / (_sigma * Math.Sqrt(2 * Math.PI))) * 
               Math.Exp(-0.5 * Math.Pow((x - _mu) / _sigma, 2));
    }
    
    public double CDF(double x)
    {
        return 0.5 * (1 + Erf((x - _mu) / (_sigma * Math.Sqrt(2))));
    }
    
    public double Mean() => _mu;
    public double Variance() => _sigma * _sigma;
    
    public double[] GenerateSamples(int count)
    {
        var normal = new Normal(_mu, _sigma);
        double[] samples = new double[count];
        for (int i = 0; i < count; i++)
        {
            samples[i] = normal.Sample();
        }
        return samples;
    }
    
    // Error function for Normal CDF
    private double Erf(double x)
    {
        // approximation
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;

        int sign = x < 0 ? -1 : 1;
        x = Math.Abs(x);

        double t = 1.0 / (1.0 + p * x);
        double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

        return sign * y;
    }
}

// 5. Erlang Distribution
public class ErlangDistribution : IDistribution
{
    private int _k;
    private double _lambda;
    
    public ErlangDistribution(int k = 2, double lambda = 1)
    {
        _k = k;
        _lambda = lambda;
    }
    
    public string Name => $"Erlang Distribution (k={_k})";
    
    public double PDF(double x)
    {
        if (x < 0) return 0;
        return (Math.Pow(_lambda, _k) * Math.Pow(x, _k - 1) * Math.Exp(-_lambda * x)) / Factorial(_k - 1);
    }
    
    public double CDF(double x)
    {
        if (x < 0) return 0;
        double sum = 0;
        for (int i = 0; i < _k; i++)
        {
            sum += Math.Exp(-_lambda * x) * Math.Pow(_lambda * x, i) / Factorial(i);
        }
        return 1 - sum;
    }
    
    public double Mean() => _k / _lambda;
    public double Variance() => _k / (_lambda * _lambda);
    
    public double[] GenerateSamples(int count)
    {
        // Erlang distribution is sum of k exponential distributions
        double[] samples = new double[count];
        var exp = new Exponential(_lambda);
        
        for (int i = 0; i < count; i++)
        {
            double sum = 0;
            for (int j = 0; j < _k; j++)
            {
                sum += exp.Sample();
            }
            samples[i] = sum;
        }
        return samples;
    }
    
    private double Factorial(int n)
    {
        double result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }
}