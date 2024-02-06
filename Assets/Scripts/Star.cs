using Entropy.Assets.Scripts;
using Godot;
using System;
using System.ComponentModel;

public partial class Star : Orbital
{
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public SpectralType SpectralType { get; set; }

	public double Age { get; set; }

	public ulong Temperature { get; set; }

	public ushort SpectralSubDivision { get; set; }

	public LuminosityClass LuminosityClass { get; set; }

	public string StarClass { get; set; }

	/// <summary>
	/// The luminosity of this star (joules per second or watts)
	/// </summary>
	public double Luminosity { get; set; }

	/// <summary>
	/// Minimum edge of the Habitable Zone (in AU)
	/// </summary>
	public double MinHabitableRadius_AU => Math.Sqrt(Luminosity / 1.1);

	/// <summary>
	/// Minimum edge of the Habitable Zone (in meters)
	/// </summary>
	public double MinHabitableRadius_m => DistanceMath.AuToMt(MinHabitableRadius_AU);

	/// <summary>
	/// Maximum edge of the Habitable Zone (in AU)
	/// </summary>
	public double MaxHabitableRadius_AU => Math.Sqrt(Luminosity / 0.53);

	/// <summary>
	/// Maximum edge of the Habitable Zone (in meters)
	/// </summary>
	public double MaxHabitableRadius_m => DistanceMath.AuToMt(MaxHabitableRadius_AU);

	public MinMaxStruct innerZone_m;
	public MinMaxStruct habitableZone_m;
	public MinMaxStruct outerZone_m;

	public bool skipHabitableZone = false;

	public SolarSystem solarSystem;

	public Star()
	{
		OrbitalPeriod = 1;
	}

	public void Generate(SolarSystem system)
	{
		solarSystem = system;
		SetStarMassRadiusByType();
		SetLuminosity();
		SetUpStarZones();
		OrbitalPeriod = 1;
	}

	private void SetStarMassRadiusByType()
	{
		//mass
		double min = 0.8 * UniversalConstants.Units.SolarMassInKG;
		double max = 1.04 * UniversalConstants.Units.SolarMassInKG;

		Mass = Mathf.Lerp(min, max, rng.Randf());

		min = 0.96 * UniversalConstants.Units.SolarRadiusInM;
		max = 1.15 * UniversalConstants.Units.SolarRadiusInM;

		Radius = Mathf.Lerp(min, max, rng.Randf());
	}

	private void SetLuminosity()
	{
		double min = 0.6;
		double max = 1.5;

		Luminosity = Mathf.Lerp(min, max, rng.Randf());
	}

	public void SetUpStarZones()
	{
		var dist = BodyGeneration.BodyGenParameters.OrbitalDistanceByStarSpectralType[SpectralType];

		if (MinHabitableRadius_m > dist.Max || MaxHabitableRadius_m < dist.Min) //check if habitable zone is too close or far to star. in that case we only generate inner and outer
		{
			// Habitable zone either too close or too far from star.
			// Only generating inner and outer zones.
			skipHabitableZone = true;

			innerZone_m = new MinMaxStruct(dist.Min, dist.Max * 0.5);
			habitableZone_m = new MinMaxStruct(MinHabitableRadius_m, MaxHabitableRadius_m); // Still need this for later.
			outerZone_m = new MinMaxStruct(dist.Max * 0.5, dist.Max);
		}
		else
		{
			innerZone_m = new MinMaxStruct(dist.Min, MinHabitableRadius_m);
			habitableZone_m = new MinMaxStruct(MinHabitableRadius_m, MaxHabitableRadius_m);
			outerZone_m = new MinMaxStruct(MaxHabitableRadius_m, dist.Max);
		}
	}
}

public enum SystemBand
{
	InnerBand,
	HabitableBand,
	OuterBand,
};

public enum SpectralType
{
	O,
	B,
	A,
	F,
	G,
	K,
	M,
	D,
	C
}

public enum LuminosityClass
{
	[Description("Hypergiant")]
	O,

	[Description("Luminous Supergiant")]
	Ia,

	[Description("Intermediate Supergiant")]
	Iab,

	[Description("Less Luminous Supergiant")]
	Ib,

	[Description("Bright Giant")]
	II,

	[Description("Giant")]
	III,

	[Description("Subgiant")]
	IV,

	[Description("Main-Sequence")]
	V,

	[Description("Sub-Dwarf")]
	sd,

	[Description("White Dwarf")]
	D,
}