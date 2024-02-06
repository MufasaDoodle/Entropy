using Godot;
using System;

public static class MathHelper
{	public static double GetPercentage(double value, double min, double max)
	{
		if (min >= max)
		{
			throw new ArgumentOutOfRangeException("min", "Min value must be less than Max value.");
		}
		double adjustedMax = max - min;
		double adjustedValue = value - min;
		return adjustedValue / adjustedMax;
	}
}
