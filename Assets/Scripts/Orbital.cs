using Entropy.Assets.Scripts;
using Godot;
using System;
using System.Collections.Generic;

public partial class Orbital
{
	RandomNumberGenerator rng = new RandomNumberGenerator();

	//we really need to move stars into its own class based on Orbital

	public Orbital()
	{
		SetStarMassRadiusByType();
		SetLuminosity();
		SetUpStarZones();
		OrbitalPeriod = 1;
		Children = new List<Orbital>();
		InitAngle = rng.RandfRange(0, Mathf.Pi * 2);
	}

	/// <summary>
	/// 
	/// </summary>
	public Orbital Parent;

	public List<Orbital> Children;

	/// <summary>
	/// starting angle of body at time of creation, in Radians.
	/// </summary>
	public float InitAngle;

	/// <summary>
	/// Angle around the parent, in Radians.
	/// </summary>
	public float OffsetAngle;

	/// <summary>
	/// From parent body, in meters.
	/// </summary>
	public UInt64 OrbitalDistance;
	// Max value is:  18,446,744,073,709,551,615
	// Pluto is:               4,000,000,000,000

	/// <summary>
	/// Time to orbit around primary body. In Seconds.
	/// </summary>
	public UInt64 OrbitalPeriod;

	public double Mass;

	public double Radius;

	public bool skipHabitableZone = false;


	//STAR SPECIFIC

	public double Luminosity { get; set; }

	public double MinHabitableRadius_AU => Math.Sqrt(Luminosity / 1.1);

	public double MinHabitableRadius_m => DistanceMath.AuToMt(MinHabitableRadius_AU);
	/// <summary>
	/// Maximum edge of the Habitable Zone (in AU)
	/// </summary>
	public double MaxHabitableRadius_AU => Math.Sqrt(Luminosity / 0.53);

	public double MaxHabitableRadius_m => DistanceMath.AuToMt(MaxHabitableRadius_AU);

	public MinMaxStruct innerZone_m;
	public MinMaxStruct habitableZone_m;
	public MinMaxStruct outerZone_m;




	public int GraphicID;

	// We need to be able to get an X, Y (and maybe Z) coordinate for our location
	// for the purpose of rendering the Oribtal on screen
	public Vector2 Position
	{
		get
		{
			// TODO: Convert our orbit info into a vector that we can use
			// to render something as a Unity GameObject

			// Consider whether or not we should be saving Vector3 in a 
			// private variable whenever we update our angle, or if it's
			// no slower to just calculate on demand like this.

			Vector2 myOffset = new Vector2(
				Mathf.Sin(InitAngle + OffsetAngle) * OrbitalDistance,
				-Mathf.Cos(InitAngle + OffsetAngle) * OrbitalDistance   // Z is locked to zero -- but consider adding Inclination if in 3D
			);

			if (Parent != null)
			{
				myOffset += Parent.Position;
			}

			return myOffset;
		}
	}

	public void Update(UInt64 timeSinceStart)
	{
		// Advance our angle by the correct amount of time.

		OffsetAngle = ((float)timeSinceStart / (float)OrbitalPeriod) * 2 * Mathf.Pi;

		// Update all of our children
		for (int i = 0; i < Children.Count; i++)
		{
			Children[i].Update(timeSinceStart);
		}
	}

	public ulong OrbitTimeForDistance()
	{
		// FIXME: Make real math!
		return 365 * 24 * 60 * 60;
	}


	public void AddChild(Orbital c)
	{
		c.Parent = this;
		Children.Add(c);
	}

	public void RemoveChild(Orbital c)
	{
		c.Parent = null;
		Children.Remove(c);
	}

	private void SetStarMassRadiusByType()
	{
		//mass
		double min = 0.8 * UniversalConstants.Units.SolarMassInKG;
		double max = 1.04 * UniversalConstants.Units.SolarMassInKG;

		Mass = Mathf.Lerp(min, max, rng.Randf());

		min = 0.96 * UniversalConstants.Units.SolarRadius;
		max = 1.15 * UniversalConstants.Units.SolarRadius;

		Radius = Mathf.Lerp(min, max, rng.Randf());
	}

	private void SetLuminosity()
	{
		double min = 1.5;
		double max = 5;

		Luminosity = Mathf.Lerp(min, max, rng.Randf());
	}

	private void SetUpStarZones()
	{
		

		var dist = new MinMaxStruct
		{
			Min = 0.1,
			Max = 40
		};

		

		if (MinHabitableRadius_m > dist.Max ||
				MaxHabitableRadius_m < dist.Min) //check if habitable zone is too close or far to star. in that case we only generate inner and outer
		{
			// Habitable zone either too close or too far from star.
			// Only generating inner and outer zones.
			skipHabitableZone = true;

			innerZone_m = new MinMaxStruct(DistanceMath.AuToMt(0.2), DistanceMath.AuToMt(2d));
			habitableZone_m = new MinMaxStruct(MinHabitableRadius_m, MaxHabitableRadius_m); // Still need this for later.
			outerZone_m = new MinMaxStruct(DistanceMath.AuToMt(5d), DistanceMath.AuToMt(40d));

			/*
			innerZone_m = new MinMaxStruct(DistanceMath.AuToMt(dist.Min), DistanceMath.AuToMt(dist.Max * 0.5));
			habitableZone_m = new MinMaxStruct(MinHabitableRadius_m, MaxHabitableRadius_m); // Still need this for later.
			outerZone_m = new MinMaxStruct(DistanceMath.AuToMt(dist.Max * 0.5), DistanceMath.AuToMt(dist.Max));
			*/
		}
		else
		{
			innerZone_m = new MinMaxStruct(DistanceMath.AuToMt(0.2), MinHabitableRadius_m);
			habitableZone_m = new MinMaxStruct(MinHabitableRadius_m, MaxHabitableRadius_m);
			outerZone_m = new MinMaxStruct(MaxHabitableRadius_m, DistanceMath.AuToMt(40d));

			/*
			innerZone_m = new MinMaxStruct(DistanceMath.AuToMt(dist.Min), MinHabitableRadius_m);
			habitableZone_m = new MinMaxStruct(MinHabitableRadius_m, MaxHabitableRadius_m);
			outerZone_m = new MinMaxStruct(MaxHabitableRadius_m, DistanceMath.AuToMt(dist.Max));
			*/
		}


		
	}
}

public struct MinMaxStruct
{
	public double Min, Max;

	public MinMaxStruct(double min, double max)
	{
		Min = min;
		Max = max;
	}
}

public enum SystemBand
{
	InnerBand,
	HabitableBand,
	OuterBand,
};
