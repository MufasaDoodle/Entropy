using Entropy.Assets.Scripts;
using Godot;
using System;

public static class GenerateSol
{
	static SolarSystem solSys;
	public static SolarSystem CreateSolSystem()
	{
		solSys = new SolarSystem();

		solSys.Star = CreateSol();
		solSys.Star.solarSystem = solSys;
		Planet mercury = CreateMercury();
		solSys.Star.AddChild(mercury);
		Planet venus = CreateVenus();
		solSys.Star.AddChild(venus);
		Planet earth = CreateEarthAndLuna();
		solSys.Star.AddChild(earth);
		Planet mars = CreateMars();
		solSys.Star.AddChild(mars);
		Planet ceres = CreateCeres();
		solSys.Star.AddChild(ceres);
		Planet jupiter = CreateJupiter();
		solSys.Star.AddChild(jupiter);
		Planet saturn = CreateSaturn();
		solSys.Star.AddChild(saturn);
		Planet uranus = CreateUranus();
		solSys.Star.AddChild(uranus);
		Planet neptune = CreateNeptune();
		solSys.Star.AddChild(neptune);
		Planet pluto = CreatePluto();
		solSys.Star.AddChild(pluto);

		return solSys;
	}

	private static Planet CreatePluto()
	{
		Planet pluto = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.DwarfPlanet,
			Density = 1.854,
			SystemBand = SystemBand.OuterBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(39.482),
			OrbitalPeriod = (ulong)(90560 * 24d * 60d * 60d),
			Mass = 1.303E22,
			Radius = 1188.3 * 1000,
			BodyName = "Pluto",
			InitAngle = 2.63545f,
			GraphicsName = "pluto.png"
		};

		return pluto;
	}

	private static Planet CreateNeptune()
	{
		Planet neptune = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.IceGiant,
			Density = 1.638,
			SystemBand = SystemBand.OuterBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(30.07),
			OrbitalPeriod = (ulong)(60195 * 24d * 60d * 60d),
			Mass = 1.02413E26,
			Radius = 24622 * 1000,
			BodyName = "Neptune",
			InitAngle = 1.62316f,
			GraphicsName = "neptune.png"
		};

		return neptune;
	}

	private static Planet CreateUranus()
	{
		Planet uranus = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.IceGiant,
			Density = 1.27,
			SystemBand = SystemBand.OuterBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(19.19126),
			OrbitalPeriod = (ulong)(30688.5 * 24d * 60d * 60d),
			Mass = 8.6810E25,
			Radius = 25362 * 1000,
			BodyName = "Uranus",
			InitAngle = 0.663225f,
			GraphicsName = "uranus.png"
		};

		return uranus;
	}

	private static Planet CreateSaturn()
	{
		Planet saturn = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.GasGiant,
			Density = 0.687,
			SystemBand = SystemBand.OuterBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(9.5826),
			OrbitalPeriod = (ulong)(10755.70 * 24d * 60d * 60d),
			Mass = 5.6834E26,
			Radius = 58232 * 1000,
			BodyName = "Saturn",
			InitAngle = 1.95040544f,
			GraphicsName = "saturn.png"
		};

		return saturn;
	}

	private static Planet CreateJupiter()
	{
		Planet jupiter = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.GasGiant,
			Density = 1.326,
			SystemBand = SystemBand.OuterBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(5.2038),
			OrbitalPeriod = (ulong)(4332.59 * 24d * 60d * 60d),
			Mass = 1.8982E27,
			Radius = 69911 * 1000,
			BodyName = "Jupiter",
			InitAngle = 0.7243116f,
			GraphicsName = "jupiter.png"
		};

		return jupiter;
	}

	private static Planet CreateCeres()
	{
		Planet ceres = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.DwarfPlanet,
			Density = 2.1616,
			SystemBand = SystemBand.OuterBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(2.77),
			OrbitalPeriod = (ulong)(1680 * 24d * 60d * 60d),
			Mass = 9.38392E20,
			Radius = 469.7d * 1000d,
			BodyName = "Ceres",
			InitAngle = 3.429572f,
			GraphicsName = "ceres.png"
		};

		return ceres;
	}

	private static Planet CreateMars()
	{
		Planet mars = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.Terrestrial,
			Density = 3.9335,
			SystemBand = SystemBand.HabitableBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(1.52368055),
			OrbitalPeriod = (ulong)(686.980 * 24d * 60d * 60d),
			Mass = 6.4171E23,
			Radius = 3389.5 * 1000,
			BodyName = "Mars",
			InitAngle = 3.01942f,
			GraphicsName = "mars.png"
		};

		return mars;
	}

	private static Planet CreateEarthAndLuna()
	{
		Planet earth = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.Terrestrial,
			Density = 5.5134,
			SystemBand = SystemBand.HabitableBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(1d),
			OrbitalPeriod = (ulong)(365.256363004 * 24d * 60d * 60d),
			Mass = UniversalConstants.Units.EarthMassInKG,
			Radius = 6371 * 1000,
			BodyName = "Earth",
			InitAngle = 5.51524f,
			GraphicsName = "earth.png"
		};

		Planet luna = new Planet
		{
			Parent = earth,
			BodyType = BodyType.Moon,
			Density = 3.344,
			SystemBand = SystemBand.HabitableBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(0.00257),
			OrbitalPeriod = (ulong)(27.321661 * 24d * 60d * 60d),
			Mass = 7.342E22,
			Radius = 1737.4 * 1000,
			BodyName = "Luna",
			InitAngle = 3.429572f,
			GraphicsName = "luna.png"
		};
		earth.AddChild(luna);

		return earth;
	}

	private static Planet CreateVenus()
	{
		Planet venus = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.Terrestrial,
			Density = 5.243,
			SystemBand = SystemBand.InnerBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(0.723332),
			OrbitalPeriod = (ulong)(224.701 * 24d * 60d * 60d),
			Mass = 4.8675E24,
			Radius = 6051.8 * 1000,
			BodyName = "Venus",
			InitAngle = 3.6564648f,
			GraphicsName = "venus.png"
		};

		return venus;
	}

	private static Planet CreateMercury()
	{
		Planet mercury = new Planet
		{
			Parent = solSys.Star,
			BodyType = BodyType.Terrestrial,
			Density = 5.427,
			SystemBand = SystemBand.InnerBand,
			OrbitalDistance = (ulong)DistanceMath.AuToMt(0.387098),
			OrbitalPeriod = (ulong)(87.9691 * 24d * 60d * 60d),
			Mass = 3.3011E23,
			Radius = 2439 * 1000,
			BodyName = "Mercury",
			InitAngle = 3.29867f,
			GraphicsName = "mercury.png"
		};

		return mercury;
	}

	private static Star CreateSol()
	{
		Star sol = new Star
		{
			Mass = UniversalConstants.Units.SolarMassInKG,
			Radius = UniversalConstants.Units.SolarRadiusInM,
			BodyName = "Sol",
			Luminosity = 1,
			OrbitalPeriod = 1,
			innerZone_m = new MinMaxStruct { Min = 0.1, Max = 0.9},
			outerZone_m = new MinMaxStruct { Min = 1.88679, Max = 40}
		};

		return sol;
	}
}
