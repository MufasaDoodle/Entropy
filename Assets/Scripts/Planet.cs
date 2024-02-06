using System;
using System.ComponentModel;
using Entropy.Assets.Scripts;
using Godot;

public class Planet : Orbital
{
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public BodyType BodyType { get; set; }

	public double Density { get; set; }

	public SystemBand SystemBand { get; set; }

	public double Volume { get; set; }

	public Planet()
	{
	}

	public void Generate(SolarSystem system, Orbital star, SystemBand systemBand, MinMaxStruct bandLimits_m)
	{
		// Randomize our values
		Parent = star;
		SystemBand = systemBand;
		BodyType = BodyType.Planet; //temporary. randomize at some point
		//SetBodyMassDensityRadiusByType();
		OrbitalDistance = (ulong)rng.RandfRange((float)bandLimits_m.Min, (float)bandLimits_m.Max);
		OrbitalPeriod = OrbitalMath.GetOrbitalPeriodWithDistance(OrbitalDistance, Mass, Parent.Mass);
		GraphicsName = "Planet.png";    // TODO: Make this not poop

		//for now we're not gonna generate any moons. will be implemented later
		int maxMoons = 0;
		int m = 0;
		//int m = rng.RandiRange(0, maxMoons + 1);
		for (int i = 0; i < m; i++)
		{
			Planet moon = new Planet();
			this.AddChild(moon);
			moon.BodyType = BodyType.Moon;
			//moon.SetBodyMassDensityRadiusByType();
			moon.OrbitalDistance = 100000000000; // FIXME: This makes no sense;
			OrbitalPeriod = OrbitalMath.GetOrbitalPeriodWithDistance(OrbitalDistance, moon.Mass, this.Mass);
		}
	}
}

public enum BodyType
{
	Planet,
	IceGiant,
	GasDwarf,
	Terrestrial,
	DwarfPlanet,
	GasGiant,
	Moon,
	Asteroid,
	Comet
}

public enum TectonicActivity
{
	[Description("?")]
	Unknown,

	[Description("Dead")]
	Dead,

	[Description("Minor")]
	Minor,

	[Description("Earth-like")]
	EarthLike,

	[Description("Major")]
	Major,

	[Description("Not-Applicable")]
	NA
}
