﻿namespace UASATE.Core;

public class Vector
{
    private readonly double[] _values;

    public double this[int i]
    {
        get => _values[i];
        set => _values[i] = value;
    }

    public int Dimension { get; }

    public Vector(int dimension)
    {
        if(dimension <= 0) throw new ArgumentOutOfRangeException(nameof(dimension), "Cant be negative");

        Dimension = dimension;

        _values = new double[dimension];
        for (int i = 0; i < dimension; i++)
            _values[i] = 0;
    }

    public Vector(int dimension, double[] values)
    {
        if (dimension <= 0) throw new ArgumentOutOfRangeException(nameof(dimension), "Cant be negative");
        if (values.Length != dimension) throw new ArgumentException(nameof(values), "values.length must be equal dimension");

        Dimension = dimension;

        _values = new double[dimension];
        for (int i = 0; i < dimension; i++)
            _values[i] = values[i];
    }

    public Vector(Vector vector)
    {
        Dimension = vector.Dimension;
        _values = new double[vector.Dimension];
        for (int i = 0; i < Dimension; i++)
            _values[i] = vector[i];
    }

    public override string ToString() => $"[{string.Join(", ", _values)}]";

    public static Vector operator+(Vector left, Vector right)
    {
        if (left.Dimension != right.Dimension) throw new ArgumentException("left and right dimension must be equal");

        Vector result = new Vector(left.Dimension);
        for(int i = 0; i < left.Dimension; i++)
            result[i] = left[i] + right[i];

        return result;
    }

    public static Vector operator-(Vector left, Vector right)
    {
        if (left.Dimension != right.Dimension) throw new ArgumentException("left and right dimension must be equal");

        Vector result = new Vector(left.Dimension);
        for (int i = 0; i < left.Dimension; i++)
            result[i] = left[i] - right[i];

        return result;
    }

    public static Vector operator*(double left, Vector right)
    {
        Vector result = new Vector(right.Dimension);
        for (int i = 0; i < right.Dimension; i++)
            result[i] = left * right[i];

        return result;
    }
}