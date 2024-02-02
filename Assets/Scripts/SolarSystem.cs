using Entropy.Assets.Scripts;
using System;
using Godot;

public class SolarSystem
{
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public Orbital Star {  get; set; }


	//these are only for body generation
	int numInnerZoneBodies = 0;
	int numHabitableZoneBodies = 0;
	int numOuterZoneBodies = 0;

	public void Update(UInt64 timeSinceStart)
	{
		Star.Update(timeSinceStart);
	}

	public void Generate()
	{
		// Make a single star with planets orbiting

		Orbital myStar = new Orbital();
		myStar.GraphicID = 0;
		Star = myStar;

		int planetAmount = rng.RandiRange(1, 8);

		DeterminePlanetBands(planetAmount);

		CreateBodyForBand(SystemBand.InnerBand, Star.innerZone_m, numInnerZoneBodies);
		CreateBodyForBand(SystemBand.HabitableBand, Star.habitableZone_m, numHabitableZoneBodies);
		CreateBodyForBand(SystemBand.OuterBand, Star.outerZone_m, numOuterZoneBodies);

		
	}

	public void DeterminePlanetBands(int numberOfBodies)
	{
		numInnerZoneBodies = 0;
		numHabitableZoneBodies = 0;
		numOuterZoneBodies = 0;
				
		WeightedList<SystemBand> BandBodyWeight = new WeightedList<SystemBand>
		{
			{0.3, SystemBand.InnerBand},
			{0.1, SystemBand.HabitableBand},
			{0.6, SystemBand.OuterBand},
		};

		while (numberOfBodies > 0)
		{
			// Select a band to add a body to.
			float rngVal = rng.Randf();
			SystemBand selectedBand = BandBodyWeight.Select(rngVal);
			// Add a body to that band.
			switch (selectedBand)
			{
				case SystemBand.InnerBand:
					numInnerZoneBodies++;
					numberOfBodies--;
					break;
				case SystemBand.HabitableBand:
					if (Star.skipHabitableZone)
						break;
					numHabitableZoneBodies++;
					numberOfBodies--;
					break;
				case SystemBand.OuterBand:
					numOuterZoneBodies++;
					numberOfBodies--;
					break;
			}
		}
	}

	public void CreateBodyForBand(SystemBand band, MinMaxStruct bandLimits_m, int num)
	{
		for (int i = 0; i < num; i++)
		{
			Planet planet = new Planet();
			Star.AddChild(planet);
			planet.Generate(this, Star, band, bandLimits_m);
		}
	}
}

