using Entropy.Assets.Scripts;
using System;
using Godot;
using static BodyGeneration;
using System.Collections.Generic;
using static Godot.OpenXRInterface;

public class SolarSystem
{
	RandomNumberGenerator rng = new RandomNumberGenerator();
	Random random = new Random();

	public Star Star { get; set; }

	public List<Planet> systemBodies = new List<Planet>();


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
		Star = BodyGeneration.CreateStar(this);
		Star.GraphicsName = "sun.png";

		//calc number of planets
		int amount = DeterminePlanetAmount();

		//calculate zones

		if (amount != 0)
		{

		}

		systemBodies = new List<Planet>(amount);

		Star.SetUpStarZones();

		//generate zone planet numbers
		DeterminePlanetBands(amount);

		//generate bodies for each zone
		systemBodies.AddRange(CreateBodiesForBand(SystemBand.InnerBand, Star.innerZone_m, numInnerZoneBodies));
		systemBodies.AddRange(CreateBodiesForBand(SystemBand.HabitableBand, Star.habitableZone_m, numHabitableZoneBodies));
		systemBodies.AddRange(CreateBodiesForBand(SystemBand.OuterBand, Star.outerZone_m, numOuterZoneBodies));

		//finalize?
		int bodyCount = 1;
		foreach (var body in systemBodies)
		{
			Star.AddChild(body);
			body.Parent = Star;
			FinalizeBody(body, bodyCount);
			bodyCount++;
		}

		//asteroids?

		// Make a star with planets orbiting
		//Star = new Star();
		//Star.GraphicsName = "sun.png";
		//Star.Generate(this);

		/*
		int planetAmount = rng.RandiRange(1, 8);

		
		
		DeterminePlanetBands(planetAmount);
		
		CreateBodiesForBand(SystemBand.InnerBand, Star.innerZone_m, numInnerZoneBodies);
		CreateBodiesForBand(SystemBand.HabitableBand, Star.habitableZone_m, numHabitableZoneBodies);
		CreateBodiesForBand(SystemBand.OuterBand, Star.outerZone_m, numOuterZoneBodies);
		*/
		GD.Print($"Generated star with following properties:\n" +
			$"Class: {Star.StarClass}\n" +
			$"Age: {Star.Age}\n" +
			$"Temperature: {Star.Temperature}\n" +
			$"Mass: {UniversalConstants.Units.SolarMassInKG / Star.Mass} solar masses\n" +
			$"Radius: {Star.Radius / UniversalConstants.Units.MetersPerKm} km\n" +
			$"Radius compared to Sol: {UniversalConstants.Units.SolarRadiusInM / Star.Radius} solar radiuses\n" +
			$"Luminosity: {Star.Luminosity}\n" +
			$"MinInner: {DistanceMath.MToAU(Star.innerZone_m.Min)} AU\n" +
			$"MaxInner: {DistanceMath.MToAU(Star.innerZone_m.Max)} AU\n" +
			$"MinHab: {Star.MinHabitableRadius_AU} AU\n" +
			$"MaxHab: {Star.MaxHabitableRadius_AU} AU\n" +
			$"MinOuter: {DistanceMath.MToAU(Star.outerZone_m.Min)} AU\n" +
			$"MaxOuter: {DistanceMath.MToAU(Star.outerZone_m.Max)} AU\n" +
			$"Habitable zone skipped: {Star.skipHabitableZone}\n" +
			$"Planet amounts:\n" +
			$"Inner: {numInnerZoneBodies}\n" +
			$"Hab: {numHabitableZoneBodies}\n" +
			$"Outer: {numOuterZoneBodies}");
	}

	public int DeterminePlanetAmount()
	{
		var minMax = StarGenParameters.StarMassBySpectralType[Star.SpectralType];
		double starMassRatio = MathHelper.GetPercentage(Star.Mass, minMax.Min, minMax.Max);

		double starSpectralTypeRatio = BodyGenParameters.StarSpectralTypePlanetGenerationRatio[Star.SpectralType];
		double randomMultiplier = random.NextDouble();

		double percentOfMax = Math.Clamp(starMassRatio * starSpectralTypeRatio * randomMultiplier, 0, 1);

		return (int)Math.Round(percentOfMax * 25); //percentOfMax multiplied by maximum number of planets allow in system generation
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

	public List<Planet> CreateBodiesForBand(SystemBand band, MinMaxStruct bandLimits_m, int num)
	{
		//TODO: also create asteroid belts
		List<Planet> planets = new List<Planet>();

		int numBodies = num;

		while (numBodies > 0)
		{
			Planet planet = new Planet();

			double massMultiplier = 1;

			planet.BodyType = BodyGenParameters.GetBandBodyTypeWeight(band).Select(random.NextDouble());

			if (planet.BodyType == BodyType.Asteroid)
			{
				//ASTEROID BELT SECTION TODO
				//for now we reroll

				while (planet.BodyType == BodyType.Asteroid)
					planet.BodyType = BodyGenParameters.GetBandBodyTypeWeight(band).Select(random.NextDouble());
			}

			double density;
			if (planet.BodyType == BodyType.Asteroid)
			{
				//TODO when implementing asteroids
				density = 2; //TEMP
			}
			else
			{
				var minMax = BodyGenParameters.SystemBodyMassByType[planet.BodyType];
				massMultiplier *= Mathf.Lerp(minMax.Min, minMax.Max, Mathf.Pow(rng.Randf(), 3));
				minMax = BodyGenParameters.SystemBodyDensityByType[planet.BodyType];
				density = Mathf.Lerp(minMax.Min, minMax.Max, rng.Randf());
			}
			double volumeInCm3 = massMultiplier / density;

			// convert to Km^3
			double volume = volumeInCm3 * 1.0e-15;

			double radius = Mathf.Pow((3 * massMultiplier) / (4 * Mathf.Pi * (density / 1000)), 0.3333333333);
			radius = radius / 100; //convert from cm to m

			planet.Mass = massMultiplier;
			planet.Radius = radius;
			planet.Density = density;
			planet.Volume = volume;
			planet.Guid = Guid.NewGuid();

			planets.Add(planet);
			numBodies--;
		}

		CreateOrbitsForBodies(this, Star, ref planets, bandLimits_m, systemBodies);
		return planets;

		/*
		for (int i = 0; i < num; i++)
		{
			Planet planet = new Planet();
			Star.AddChild(planet);
			planet.Generate(this, Star, band, bandLimits_m);
		}
		*/
	}

	private void CreateOrbitsForBodies(SolarSystem system, Orbital parent, ref List<Planet> bodies, MinMaxStruct bandLimits_m, List<Planet> systemBodies)
	{
		double totalBandMass = 0;

		foreach (var body in bodies)
		{
			totalBandMass += body.Mass;
		}

		double remainingBandMass = totalBandMass;

		double insideApoapsis_m = double.MinValue; // Apoapsis of the orbit that is inside of the next body.
		double outsidePeriapsis_m = double.MaxValue; // Periapsis of the orbit that is outside of the next body.
		double minDistance_m = bandLimits_m.Min; // The minimum distance we can place a body.
		double remainingDistance_m = bandLimits_m.Max - minDistance_m; // distance remaining that we can place bodies into.

		double insideMass = 0; // Mass of the object that is inside of the next body.
		double outsideMass = 0; // Mass of the object that is outside of the next body.

		//this is a convuluted way of finding out which (if any) body is closest to either the inner or outer band
		foreach (var systemBody in systemBodies)
		{
			if (systemBody.OrbitalDistance <= bandLimits_m.Min && systemBody.OrbitalDistance > insideApoapsis_m)
			{
				insideApoapsis_m = systemBody.OrbitalDistance;
				insideMass = systemBody.Mass;
			}
			else if (systemBody.OrbitalDistance >= bandLimits_m.Max && systemBody.OrbitalDistance < outsidePeriapsis_m)
			{
				outsidePeriapsis_m = systemBody.OrbitalDistance;
				outsideMass = systemBody.Mass;
			}
		}

		for (int i = 0; i < bodies.Count; i++)
		{
			Planet currentBody = bodies[i];

			double massRatio = currentBody.Mass / remainingBandMass;
			double maxDistance_m = remainingDistance_m * massRatio + minDistance_m;

			remainingBandMass -= currentBody.Mass;

			var (couldFindStableOrbit, orbitalDistance) = FindStableOrbit(this, parent, currentBody, insideApoapsis_m, outsidePeriapsis_m, insideMass, outsideMass, minDistance_m, maxDistance_m);

			if (couldFindStableOrbit == false)
			{
				bodies.RemoveAt(i);
				i--;
				continue;
			}

			currentBody.OrbitalDistance = (ulong)orbitalDistance;
			insideMass = currentBody.Mass;
			insideApoapsis_m = currentBody.OrbitalDistance; //if this line is making trouble, convert orbitaldistance to AU and then assign to insideApoapsis_m
			currentBody.OrbitalPeriod = OrbitalMath.GetOrbitalPeriodWithDistance(currentBody.OrbitalDistance, currentBody.Mass, parent.Mass);
		}
	}

	private double OrbitGravityFactor = 20;

	private (bool couldFindStableOrbit, double orbitalDistance) FindStableOrbit(SolarSystem system, Orbital parent, Planet body, double insideApoapsis, double outsidePeriapsis, double insideMass, double outsideMass, double minDistance, double maxDistance)
	{
		double parentMass = parent.Mass;
		double myMass = body.Mass;

		// Adjust minDistance
		double gravAttractionInsiderNumerator = UniversalConstants.Science.GravitationalConstant * myMass * insideMass;
		double gravAttractionOutsideNumerator = UniversalConstants.Science.GravitationalConstant * myMass * outsideMass;
		double gravAttractionParentNumerator = UniversalConstants.Science.GravitationalConstant * myMass * parentMass;
		double gravAttractionToInsideOrbit = gravAttractionInsiderNumerator / ((minDistance - insideApoapsis) * (minDistance - insideApoapsis));
		double gravAttractionToOutsideOrbit = gravAttractionOutsideNumerator / ((outsidePeriapsis - maxDistance) * (outsidePeriapsis - maxDistance));
		double gravAttractionToParent = gravAttractionParentNumerator / (minDistance * minDistance);

		while (gravAttractionToInsideOrbit * OrbitGravityFactor > gravAttractionToParent)
		{
			// We're too attracted to our inside neighbor, increase minDistance by 1%.
			// Assuming our parent is more massive than our inside neighbor, then this will "tip" us to be more attracted to parent.
			minDistance += minDistance * 0.01;

			// Reevaluate our gravitational attractions with new minDistance.
			gravAttractionToInsideOrbit = gravAttractionInsiderNumerator / ((minDistance - insideApoapsis) * (minDistance - insideApoapsis));
			gravAttractionToOutsideOrbit = gravAttractionOutsideNumerator / ((outsidePeriapsis - maxDistance) * (outsidePeriapsis - maxDistance));
			gravAttractionToParent = gravAttractionParentNumerator / (minDistance * minDistance);
		}

		if (gravAttractionToOutsideOrbit * OrbitGravityFactor > gravAttractionToParent || minDistance > maxDistance)
		{
			// Unable to find suitable orbit. This body is rejected.
			GD.Print("Planet rejected");
			return (false, 0f);
		}

		double sma_m = Mathf.Lerp(minDistance, maxDistance, random.NextDouble());

		return (true, sma_m);
	}

	private void FinalizeBody(Planet body, int bodyCount)
	{
		//many things here are unused until we implement all the stuff from the comments in the method

		if (body.BodyType == BodyType.Asteroid)
		{
			//do asteroid specific stuff
			return;
		}

		if (body.Parent == null)
		{
			throw new InvalidOperationException("Body cannot be finalized without a parent.");
		}

		double parentDistance = 0;

		Star star;
		if (body.Parent.GetType() == typeof(Star))
		{
			star = body.Parent as Star;
		}
		else
		{
			parentDistance += body.Parent.OrbitalDistance;
			if (body.Parent.Parent == null)
			{
				throw new InvalidOperationException("Body cannot be finalized without a root star.");
			}
			star = body.Parent.Parent as Star;
		}

		//check if planet bodyType supports populations, and set to true here

		//axial tilt generation
		//length of day generation
		//base temperature generation
		//generate tectonic activity
		//generate magnetic field
		//generate atmosphere
		//set radiation level
		//generate minerals
		//generate ruins


		body.BodyName = $"{body.Parent.BodyName} - {bodyCount}"; 
		body.GraphicsName = "";

		if (body.BodyType != BodyType.Moon)
		{
			//generate moons
			GenerateMoons(body);
		}

		if(body.Children.Count > 0)
		{
			int numChildren = body.Children.Count;
			int recursiveBodyCount = 1;
			for (int i = 0; i < numChildren; i++)
			{
				FinalizeBody(body.Children[i] as Planet, recursiveBodyCount);
				recursiveBodyCount++;
			}
		}
	}

	private void GenerateMoons(Planet parent)
	{
		//check if we generate moons
		if (random.NextDouble() > BodyGenParameters.MoonGenerationChanceByPlanetType[parent.BodyType])
			return; //no moons

		double massRatioOfParent = parent.Mass / BodyGenParameters.SystemBodyMassByType[parent.BodyType].Max;
		double moonGenChance = massRatioOfParent * random.NextDouble() * BodyGenParameters.MaxNoOfMoonsByPlanetType[parent.BodyType];
		moonGenChance = Math.Clamp(moonGenChance, 1, BodyGenParameters.MaxNoOfMoonsByPlanetType[parent.BodyType]);
		int numMoons = (int)Math.Round(moonGenChance);

		var moons = new List<Planet>(numMoons);

		while (numMoons > 0)
		{
			Planet newMoon = new Planet();
			newMoon.BodyType = BodyType.Moon;

			var moonMassMinMax = BodyGenParameters.SystemBodyMassByType[newMoon.BodyType];
			double maxRelativeMass = parent.Mass * 0.4d;

			if (maxRelativeMass < moonMassMinMax.Max)
			{
				moonMassMinMax.Max = maxRelativeMass;
			}
			double massMultiplier = 1;

			massMultiplier *= Mathf.Lerp(moonMassMinMax.Min, moonMassMinMax.Max, Mathf.Pow(rng.Randf(), 3));
			moonMassMinMax = BodyGenParameters.SystemBodyDensityByType[newMoon.BodyType];
			double density = Mathf.Lerp(moonMassMinMax.Min, moonMassMinMax.Max, rng.Randf());

			double volumeInCm3 = massMultiplier / density;

			// convert to Km^3
			double volume = volumeInCm3 * 1.0e-15;

			double radius = Mathf.Pow((3 * massMultiplier) / (4 * Mathf.Pi * (density / 1000)), 0.3333333333);
			radius = radius / 100; //convert from cm to m

			newMoon.Mass = massMultiplier;
			newMoon.Radius = radius;
			newMoon.Density = density;
			newMoon.Volume = volume;
			newMoon.Guid = Guid.NewGuid();

			moons.Add(newMoon);
			numMoons--;
		}

		double minMoonOrbitDist = parent.Radius * 2.5d;
		double maxMoonDistance = BodyGenParameters.MaxMoonOrbitDistanceByPlanetType[parent.BodyType] * massRatioOfParent;

		CreateOrbitsForBodies(this, parent, ref moons, new MinMaxStruct { Min =  minMoonOrbitDist, Max = maxMoonDistance }, new List<Planet>());

		foreach (var moon in moons)
		{
			moon.Parent = parent;
			parent.AddChild(moon);
		}
	}
}

