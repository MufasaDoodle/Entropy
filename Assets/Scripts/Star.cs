using Entropy.Assets.Scripts;
using Godot;
using System;

public partial class Star : Orbital
{
	RandomNumberGenerator rng = new RandomNumberGenerator();

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

	private void SetUpStarZones()
	{
		var dist = new MinMaxStruct
		{
			Min = 0.1,
			Max = 40
		};

		if (MinHabitableRadius_m > DistanceMath.AuToMt(dist.Max) || MaxHabitableRadius_m < DistanceMath.AuToMt(dist.Min)) //check if habitable zone is too close or far to star. in that case we only generate inner and outer
		{
			// Habitable zone either too close or too far from star.
			// Only generating inner and outer zones.
			skipHabitableZone = true;

			innerZone_m = new MinMaxStruct(DistanceMath.AuToMt(dist.Min), DistanceMath.AuToMt(dist.Max * 0.5));
			habitableZone_m = new MinMaxStruct(MinHabitableRadius_m, MaxHabitableRadius_m); // Still need this for later.
			outerZone_m = new MinMaxStruct(DistanceMath.AuToMt(dist.Max * 0.5), DistanceMath.AuToMt(dist.Max));
		}
		else
		{
			innerZone_m = new MinMaxStruct(DistanceMath.AuToMt(dist.Min), MinHabitableRadius_m);
			habitableZone_m = new MinMaxStruct(MinHabitableRadius_m, MaxHabitableRadius_m);
			outerZone_m = new MinMaxStruct(MaxHabitableRadius_m, DistanceMath.AuToMt(dist.Max));
		}
	}
}

public enum SystemBand
{
	InnerBand,
	HabitableBand,
	OuterBand,
};

