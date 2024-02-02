using Godot;
using System;
using System.Collections.Generic;

public class Galaxy
{
	public Galaxy()
	{
		// Don't do procedural generation in any kind of constructor
		SolarSystems = new List<SolarSystem>();
		GD.Print($"Galaxy object created");
	}

	public List<SolarSystem> SolarSystems;


	/// <summary>
	/// Generate galaxy with given number of stars
	/// </summary>
	/// <param name="numStars">Number of stars to create in galaxy</param>
	public void Generate(int numStars)
	{
		GD.Print($"Generating {numStars} stars in galaxy");
		// We can set a seed for the random number generator, so that it
		// starts with the same systems every time

		for (int i = 0; i < numStars; i++)
		{
			SolarSystem ss = new SolarSystem();
			GD.Print($"Solar system {i} created");
			GD.Print($"Generating solar system {i}");
			ss.Generate();
			GD.Print($"Done generating solar system {i}");

			SolarSystems.Add(ss);
		}
	}

	/// <summary>
	/// NOT implemented
	/// </summary>
	/// <param name="fileName"></param>
	public void LoadFromFile(string fileName)
	{
		// We may want to load systems from a file instead
		// of randomly generating them
		// For saved games?
	}

	public void Update(UInt64 timeSinceStart)
	{
		// TODO: Consider only updating PART of the galaxy if you have a CRAAAAAAZY number of solar system

		foreach (SolarSystem ss in SolarSystems)
		{
			ss.Update(timeSinceStart);
		}
	}
}
