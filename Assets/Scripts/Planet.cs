using System;
using Entropy.Assets.Scripts;
using Godot;

public class Planet : Orbital
{
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public BodyType BodyType { get; set; }

	public double Density { get; set; }

	public SystemBand SystemBand { get; set; }

	public Planet()
	{
		GD.Print($"Empty planet object created");
	}

	public void Generate(SolarSystem system, Orbital star, SystemBand systemBand, MinMaxStruct bandLimits_m)
	{
		GD.Print($"Generating planet...");
		// Randomize our values
		Parent = star;
		SystemBand = systemBand;
		BodyType = BodyType.Planet; //temporary. randomize at some point
		SetBodyMassDensityRadiusByType();
		OrbitalDistance = (ulong)rng.RandfRange((float)bandLimits_m.Min, (float)bandLimits_m.Max);
		OrbitalPeriod = OrbitalMath.GetOrbitalPeriodWithDistance(OrbitalDistance, Mass, Parent.Mass);
		ulong rad1 = OrbitalMath.GetOrbitalPeriodCircularOrbitWithRadius(Radius, Parent.Mass);
		ulong rad2 = OrbitalMath.GetOrbitalPeriodCircularOrbitWithRadius(Radius, Mass, Parent.Mass);
		ulong dista = OrbitalMath.GetOrbitalPeriodWithDistance(OrbitalDistance, Mass, Parent.Mass);
		//OrbitalPeriod = OrbitalMath.GetOrbitalPeriodWithDistance(OrbitalDistance, Mass, Parent.Mass);
		GD.Print($"With Radius: {rad1}");
		GD.Print($"With Radius2: {rad2}");
		GD.Print($"With Distance: {dista}");
		GraphicID = rng.RandiRange(1, 12);    // TODO: Make this not poop

		//for now we're not gonna generate any moons. will be implemented later
		int maxMoons = 0;
		int m = 0;
		//int m = rng.RandiRange(0, maxMoons + 1);
		for (int i = 0; i < m; i++)
		{
			Planet moon = new Planet();
			this.AddChild(moon);
			moon.BodyType = BodyType.Moon;
			moon.SetBodyMassDensityRadiusByType();
			moon.OrbitalDistance = 100000000000; // FIXME: This makes no sense;
			OrbitalPeriod = OrbitalMath.GetOrbitalPeriodWithDistance(OrbitalDistance, moon.Mass, this.Mass);
		}

		GD.Print($"	Done generating planet");
	}

	public void MakeEarth()
	{
		OffsetAngle = 0;  // "North" of the sun
		OrbitalDistance = 150000000000; // 150 million KM
		OrbitalPeriod = 365 * 24 * 60 * 60;
	}

	private void SetBodyMassDensityRadiusByType()
	{
		GD.Print($"	setting mass, density, radius");
		double min = 0f;
		double max = 0f;

		//mass calc
		if (BodyType == BodyType.Planet)
		{
			min = 0.05 * UniversalConstants.Units.EarthMassInKG;
			max = 5 * UniversalConstants.Units.EarthMassInKG;
		}
		else if (BodyType == BodyType.Moon)
		{
			min = 1E16;
			max = 1 * UniversalConstants.Units.EarthMassInKG;
		}
		else
		{
			min = 1E15;
			max = 9E19;
		}

		RandomNumberGenerator rng = new RandomNumberGenerator();
		double mass = Mathf.Lerp(min, max, Mathf.Pow(rng.Randf(), 3));

		//density calc
		if (BodyType == BodyType.Planet)
		{
			min = 3;
			max = 8;
		}
		else if (BodyType == BodyType.Moon)
		{
			min = 1.4f;
			max = 5;
		}
		else
		{
			min = 1;
			max = 6;
		}

		double density = Mathf.Lerp(min, max, rng.Randf());

		//calc radius
		double radius = Mathf.Pow((3 * mass) / (4 * Mathf.Pi * (density / 1000)), 0.3333333333);
		radius = radius / 1000 / 100; //convert from cm to km

		Mass = mass;
		Radius = radius;
		Density = density;
	}

	public static double KmToM(double kilometers)
	{
		return kilometers * 1000.0;
	}
}

public enum BodyType
{
	Planet,
	GasGiant,
	Moon,
	Asteroid,
	Comet
}
