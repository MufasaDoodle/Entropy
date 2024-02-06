using Entropy.Assets.Scripts;
using Godot;
using System;
using System.Collections.Generic;

public class BodyGeneration
{
	static Random random = new Random();

	public static Star CreateStar(SolarSystem system)
	{
		Star star = new Star();

		SpectralType starType = StarGenParameters.StarTypeDistributionForRealStars.Select(random.NextDouble());

		double randomSelection = random.NextDouble();

		var minMax = StarGenParameters.StarMassBySpectralType[starType];
		double mass = Mathf.Lerp(minMax.Min, minMax.Max, randomSelection);

		minMax = StarGenParameters.StarRadiusBySpectralType[starType];
		double radius = Mathf.Lerp(minMax.Min, minMax.Max, randomSelection);

		double age = (1 - mass / StarGenParameters.StarMassBySpectralType[starType].Max) * StarGenParameters.StarAgeBySpectralType[starType].Max;

		minMax = StarGenParameters.StarTemperatureBySpectralType[starType];
		ulong temperature = (ulong)Math.Round(Mathf.Lerp(minMax.Min, minMax.Max, randomSelection));

		minMax = StarGenParameters.StarLuminosityBySpectralType[starType];
		double luminosity = (double)Mathf.Lerp(minMax.Min, minMax.Max, randomSelection);

		double subDivF = temperature / StarGenParameters.StarTemperatureBySpectralType[starType].Max;
		ushort subDiv = (ushort)Mathf.Round((1 - subDivF) * 10); // 0-9

		LuminosityClass luminosityClass = LuminosityClass.V;

		string starClass = starType.ToString() + subDiv.ToString() + "-" + luminosityClass.ToString();

		star.solarSystem = system;
		star.SpectralType = starType;
		star.Mass = mass;
		star.Radius = radius;
		star.Age = age;
		star.Temperature = temperature;
		star.Luminosity = luminosity;
		star.SpectralSubDivision = subDiv;
		star.LuminosityClass = luminosityClass;
		star.StarClass = starClass;

		return star;
	}

	public static class BodyGenParameters
	{
		public static Dictionary<SpectralType, double> StarSpectralTypePlanetGenerationRatio = new Dictionary<SpectralType, double>()
		{
			{SpectralType.O, 0.6},
			{SpectralType.B, 0.7},
			{SpectralType.A, 0.9},
			{SpectralType.F, 0.9},
			{SpectralType.G, 2.1},
			{SpectralType.K, 2.4},
			{SpectralType.M, 1.8},
		};

		public static Dictionary<BodyType, MinMaxStruct> SystemBodyMassByType = new Dictionary<BodyType, MinMaxStruct>()
		{
			{BodyType.GasGiant, new MinMaxStruct
			{
				Min = 15 * UniversalConstants.Units.EarthMassInKG,
				Max = 500 * UniversalConstants.Units.EarthMassInKG
			}},
						{
			BodyType.IceGiant, new MinMaxStruct
			{
				Min = 5 * UniversalConstants.Units.EarthMassInKG,
				Max = 30 * UniversalConstants.Units.EarthMassInKG
			}},
						{
			BodyType.GasDwarf, new MinMaxStruct
			{
				Min = 1 * UniversalConstants.Units.EarthMassInKG,
				Max = 15 * UniversalConstants.Units.EarthMassInKG
			}},
						{
			BodyType.Terrestrial, new MinMaxStruct
			{
				Min = 0.05 * UniversalConstants.Units.EarthMassInKG,
				Max = 5 * UniversalConstants.Units.EarthMassInKG
			}},
						{
			BodyType.Moon, new MinMaxStruct
			{
				Min = 1E16,
				Max = 1 * UniversalConstants.Units.EarthMassInKG
			}}, // note 1E16 is 1 nano planet mass.
						{
			BodyType.DwarfPlanet, new MinMaxStruct
			{
				Min = 2E20,
				Max = 5E23
			}},
						{
			BodyType.Asteroid, new MinMaxStruct
			{
				Min = 1E15,
				Max = 9E19
			}},
						{
			BodyType.Comet, new MinMaxStruct
			{
				Min = 1E13,
				Max = 9E14
			}},
		};

		// note That these values are based on bodies in our solar system and discovered Exoplanets. Some adjustment can be made for game play.
		public static Dictionary<BodyType, MinMaxStruct> SystemBodyDensityByType = new Dictionary<BodyType, MinMaxStruct>()
		{
			{BodyType.GasGiant, new MinMaxStruct
			{
				Min = 0.5,
				Max = 10
			}},
			{
			BodyType.IceGiant, new MinMaxStruct
			{
				Min = 1,
				Max = 5
			}},
						{
			BodyType.GasDwarf, new MinMaxStruct
			{
				Min = 1,
				Max = 8
			}},
						{
			BodyType.Terrestrial, new MinMaxStruct
			{
				Min = 3,
				Max = 8
			}},
						{
			BodyType.Moon, new MinMaxStruct
			{
				Min = 1.4,
				Max = 5
			}},
						{
			BodyType.DwarfPlanet, new MinMaxStruct
			{
				Min = 1,
				Max = 6
			}},
						{
			BodyType.Asteroid, new MinMaxStruct
			{
				Min = 1,
				Max = 6
			}},
						{
			BodyType.Comet, new MinMaxStruct
			{
				Min = 0.25,
				Max = 0.7
			}},
		};

		// note These numbers, with the exception of G class stars, are based on habitable zone calculations. They could be tweaked for gameplay.
		public static Dictionary<SpectralType, MinMaxStruct> OrbitalDistanceByStarSpectralType = new Dictionary<SpectralType, MinMaxStruct>()
			{
				{SpectralType.O, DistanceMath.AuToMt(new MinMaxStruct
				{
					Min = 1,
					Max = 200
				})},
				{SpectralType.B, DistanceMath.AuToMt(new MinMaxStruct
				{
					Min = 0.5,
					Max = 100
				})},
				{SpectralType.A, DistanceMath.AuToMt(new MinMaxStruct
				{
					Min = 0.3,
					Max = 90
				})},
				{SpectralType.F, DistanceMath.AuToMt(new MinMaxStruct
				{
					Min = 0.2,
					Max = 60
				})},
				{SpectralType.G, DistanceMath.AuToMt(new MinMaxStruct
				{
					Min = 0.1,
					Max = 40
				})},
				{SpectralType.K, DistanceMath.AuToMt(new MinMaxStruct
				{
					Min = 0.01,
					Max = 18
				})},
				{SpectralType.M, DistanceMath.AuToMt(new MinMaxStruct
				{
					Min = 0.005,
					Max = 9
				})},
			};

		public static Dictionary<BodyType, MinMaxStruct> BodyEccentricityByType = new Dictionary<BodyType, MinMaxStruct>
			{
				{BodyType.Asteroid, new MinMaxStruct
				{
					Min = 0,
					Max = 0.5
				}},
				{BodyType.Comet, new MinMaxStruct
				{
					Min = 0.6,
					Max = 0.8
				}},
				{BodyType.DwarfPlanet, new MinMaxStruct
				{
					Min = 0,
					Max = 0.5
				}},
				{BodyType.GasDwarf, new MinMaxStruct
				{
					Min = 0,
					Max = 0.5
				}},
				{BodyType.GasGiant, new MinMaxStruct
				{
					Min = 0,
					Max = 0.5
				}},
				{BodyType.IceGiant, new MinMaxStruct
				{
					Min = 0,
					Max = 0.5
				}},
				{BodyType.Moon, new MinMaxStruct
				{
					Min = 0,
					Max = 0.5
				}},
				{BodyType.Terrestrial, new MinMaxStruct
				{
					Min = 0,
					Max = 0.5
				}}
			};

		// note These are WAGs roughly based on the albedo of bodies in our solar system. They could be tweak for gameplay.
		public static Dictionary<BodyType, MinMaxStruct> PlanetAlbedoByType = new Dictionary<BodyType, MinMaxStruct>
			{
				{BodyType.GasGiant, new MinMaxStruct
				{
					Min = 0.5,
					Max = 0.7
				}},
				{BodyType.IceGiant, new MinMaxStruct
				{
					Min = 0.5,
					Max = 0.7
				}},
				{BodyType.GasDwarf, new MinMaxStruct
				{
					Min = 0.3,
					Max = 0.7
				}},
				{BodyType.Terrestrial, new MinMaxStruct
				{
					Min = 0.05,
					Max = 0.5
				}},
				{BodyType.Moon, new MinMaxStruct
				{
					Min = 0.05,
					Max = 0.5
				}},
				{BodyType.DwarfPlanet, new MinMaxStruct
				{
					Min = 0.05,
					Max = 0.95
				}},
				{BodyType.Asteroid, new MinMaxStruct
				{
					Min = 0.05,
					Max = 0.15
				}},
				{BodyType.Comet, new MinMaxStruct
				{
					Min = 0.95,
					Max = 0.99
				}},
			};

		// note These are WAGs roughly based on the Magnetosphere of bodies in our solar system. They could be tweak for gameplay.
		public static Dictionary<BodyType, MinMaxStruct> PlanetMagneticFieldByType = new Dictionary<BodyType, MinMaxStruct>
			{
				{BodyType.GasGiant, new MinMaxStruct
				{
					Min = 10,
					Max = 2000
				}},
				{BodyType.IceGiant, new MinMaxStruct
				{
					Min = 5,
					Max = 50
				}},
				{BodyType.GasDwarf, new MinMaxStruct
				{
					Min = 0.1,
					Max = 20
				}},
				{BodyType.Terrestrial, new MinMaxStruct
				{
					Min = 0.0001,
					Max = 45
				}},
				{BodyType.Moon, new MinMaxStruct
				{
					Min = 0.0001,
					Max = 1
				}},
				{BodyType.DwarfPlanet, new MinMaxStruct
				{
					Min = 0.00001,
					Max = 0.0001
				}},
				{BodyType.Asteroid, new MinMaxStruct
				{
					Min = 0.000001,
					Max = 0.00001
				}},
				{BodyType.Comet, new MinMaxStruct
				{
					Min = 0.0000001,
					Max = 0.000001
				}},
			};

		// note These numbers can be tweaked as desired for gameplay. They effect the chances of atmosphere generation.
		public static Dictionary<BodyType, double> AtmosphereGenerationModifier = new Dictionary<BodyType, double>
			{
				{BodyType.GasGiant, 100000000},
				{BodyType.IceGiant, 100000000},
				{BodyType.GasDwarf, 100000000},
				{BodyType.Terrestrial, 1},
				{BodyType.Moon, 0.5},
				{BodyType.DwarfPlanet, 0},
				{BodyType.Asteroid, 0},
				{BodyType.Comet, 0},
			};

		// note that this number can be tweaked for gameplay. it affects the chance of venus like planets.
		public static double RunawayGreenhouseEffectChance = 0.25;

		public static MinMaxStruct MinMaxAtmosphericPressure = new MinMaxStruct(0.000000001, 200);

		// note that this number can be tweaked for gameplay. It affects the chance of venus like planets.
		public static double RunawayGreenhouseEffectMultiplyer = 10;

		// note These numbers can be tweaked as desired for gameplay. They effect the chances of a planet having moons.
		public static Dictionary<BodyType, double> MoonGenerationChanceByPlanetType = new Dictionary<BodyType, double>
			{
				{BodyType.GasGiant, 0.99999999},
				{BodyType.IceGiant, 0.99999999},
				{BodyType.GasDwarf, 0.99},
				{BodyType.Terrestrial, 0.5},
				{BodyType.DwarfPlanet, 0.0001},
				{BodyType.Moon, -1},
			};

		public static Dictionary<BodyType, double> MaxMoonOrbitDistanceByPlanetType = new Dictionary<BodyType, double>
			{
				{BodyType.GasGiant, DistanceMath.KmToM(60581692)}, // twice highest jupiter moon orbit
                {BodyType.IceGiant, DistanceMath.KmToM(49285000)}, // twice Neptune's highest moon orbit
                {BodyType.GasDwarf, DistanceMath.KmToM(6058169)}, // WAG
                {BodyType.Terrestrial, DistanceMath.KmToM(1923740)}, // 5 * luna orbit.
                {BodyType.DwarfPlanet, DistanceMath.KmToM(25000)}, // WAG
            };

		// note Given the way the calculation for max moons is done it is unlikely that any planet will ever have the maximum number of moon, so pad as desired.
		public static Dictionary<BodyType, double> MaxNoOfMoonsByPlanetType = new Dictionary<BodyType, double>
			{
				{BodyType.GasGiant, 20},
				{BodyType.IceGiant, 15},
				{BodyType.GasDwarf, 8},
				{BodyType.Terrestrial, 4},
				{BodyType.DwarfPlanet, 1},
			};

		public static Dictionary<TectonicActivity, double> BodyTectonicsThresholds = new Dictionary<TectonicActivity, double>
			{
				{TectonicActivity.Dead, 0.01},
				{TectonicActivity.Minor, 0.2},
				{TectonicActivity.EarthLike, 0.4},
				{TectonicActivity.Major, 1} // Not used, just here for completeness.
            };

		public static WeightedList<BodyType> GetBandBodyTypeWeight(SystemBand systemBand)
		{
			switch (systemBand)
			{
				case SystemBand.InnerBand:
					return InnerBandTypeWeights;
				case SystemBand.HabitableBand:
					return HabitableBandTypeWeights;
				case SystemBand.OuterBand:
				default:
					return OuterBandTypeWeights;
			}
		}

		public static WeightedList<SystemBand> BandBodyWeight = new WeightedList<SystemBand>
			{
				{0.3, SystemBand.InnerBand},
				{0.1, SystemBand.HabitableBand},
				{0.6, SystemBand.OuterBand},
			};

		public static WeightedList<BodyType> InnerBandTypeWeights = new WeightedList<BodyType>()
			{
				{35, BodyType.Asteroid},
				{10, BodyType.GasDwarf},
				{5, BodyType.GasGiant},
				{0, BodyType.IceGiant},
				{45, BodyType.Terrestrial},
				{5, BodyType.DwarfPlanet},
			};

		public static WeightedList<BodyType> HabitableBandTypeWeights = new WeightedList<BodyType>
			{
				{25, BodyType.Asteroid},
				{10, BodyType.GasDwarf},
				{5, BodyType.GasGiant},
				{0, BodyType.IceGiant},
				{55, BodyType.Terrestrial},
				{5, BodyType.DwarfPlanet},
			};

		public static WeightedList<BodyType> OuterBandTypeWeights = new WeightedList<BodyType>
			{
				{15, BodyType.Asteroid},
				{20, BodyType.GasDwarf},
				{25, BodyType.GasGiant},
				{20, BodyType.IceGiant},
				{7.5, BodyType.Terrestrial},
				{2.5, BodyType.DwarfPlanet},
			};
	}

	public static class StarGenParameters
	{
		public static WeightedList<SpectralType> StarTypeDistributionForRealStars = new WeightedList<SpectralType>
		{
			{ 0.00003, SpectralType.O},
			{ 0.13, SpectralType.B},
			{ 0.6, SpectralType.A},
			{ 3, SpectralType.F},
			{ 7.6, SpectralType.G},
			{ 12.1, SpectralType.K},
			{ 76.45, SpectralType.M},
			{ 0.11997, SpectralType.M} // reserved for more exotic star types
        };

		public static Dictionary<SpectralType, MinMaxStruct> StarRadiusBySpectralType = new Dictionary<SpectralType, MinMaxStruct>
		{
			{SpectralType.O, new MinMaxStruct
			{
				Min = 6.6 * UniversalConstants.Units.SolarRadiusInM,
				Max = 250 * UniversalConstants.Units.SolarRadiusInM
			}},
			{
			SpectralType.B, new MinMaxStruct
			{
				Min = 1.8 * UniversalConstants.Units.SolarRadiusInM,
				Max = 6.6 * UniversalConstants.Units.SolarRadiusInM
			}},
			{
			SpectralType.A, new MinMaxStruct
			{
				Min = 1.4 * UniversalConstants.Units.SolarRadiusInM,
				Max = 1.8 * UniversalConstants.Units.SolarRadiusInM
			}},
			{
			SpectralType.F, new MinMaxStruct
			{
				Min = 1.15 * UniversalConstants.Units.SolarRadiusInM,
				Max = 1.4 * UniversalConstants.Units.SolarRadiusInM
			}},
			{
			SpectralType.G, new MinMaxStruct
			{
				Min = 0.96 * UniversalConstants.Units.SolarRadiusInM,
				Max = 1.15 * UniversalConstants.Units.SolarRadiusInM
			}},
			{
			SpectralType.K, new MinMaxStruct
			{
				Min = 0.7 * UniversalConstants.Units.SolarRadiusInM,
				Max = 0.96 * UniversalConstants.Units.SolarRadiusInM
			}},
			{
			SpectralType.M, new MinMaxStruct
			{
				Min = 0.12 * UniversalConstants.Units.SolarRadiusInM,
				Max = 0.7 * UniversalConstants.Units.SolarRadiusInM
			}},
		};

		public static Dictionary<SpectralType, MinMaxStruct> StarTemperatureBySpectralType = new Dictionary<SpectralType, MinMaxStruct>
		{
			{SpectralType.O, new MinMaxStruct
			{
				Min = 30000,
				Max = 60000
			}},
			{
			SpectralType.B, new MinMaxStruct
			{
				Min = 10000,
				Max = 30000
			}},
			{
			SpectralType.A, new MinMaxStruct
			{
				Min = 7500,
				Max = 10000
			}},
			{
			SpectralType.F, new MinMaxStruct
			{
				Min = 6000,
				Max = 7500
			}},
			{
			SpectralType.G, new MinMaxStruct
			{
				Min = 5200,
				Max = 6000
			}},
			{
			SpectralType.K, new MinMaxStruct
			{
				Min = 3700,
				Max = 5200
			}},
			{
			SpectralType.M, new MinMaxStruct
			{
				Min = 2400,
				Max = 3700
			}},
		};

		public static Dictionary<SpectralType, MinMaxStruct> StarLuminosityBySpectralType = new Dictionary<SpectralType, MinMaxStruct>
		{
			{SpectralType.O, new MinMaxStruct
			{
				Min = 30000,
				Max = 1000000
			}},
			{SpectralType.B, new MinMaxStruct
			{
				Min = 25,
				Max = 30000
			}},
			{SpectralType.A, new MinMaxStruct
			{
				Min = 5,
				Max = 25
			}},
			{SpectralType.F, new MinMaxStruct
			{
				Min = 1.5,
				Max = 5
			}},
			{SpectralType.G, new MinMaxStruct
			{
				Min = 0.6,
				Max = 1.5
			}},
			{SpectralType.K, new MinMaxStruct
			{
				Min = 0.08,
				Max = 0.6
			}},
			{SpectralType.M, new MinMaxStruct
			{
				Min = 0.0001,
				Max = 0.08
			}},
		};

		public static Dictionary<SpectralType, MinMaxStruct> StarMassBySpectralType = new Dictionary<SpectralType, MinMaxStruct>()
		{
			{SpectralType.O, new MinMaxStruct
			{
				Min = 16 * UniversalConstants.Units.SolarMassInKG,
				Max = 265 * UniversalConstants.Units.SolarMassInKG
			}},
			{SpectralType.B, new MinMaxStruct
			{
				Min = 2.1 * UniversalConstants.Units.SolarMassInKG,
				Max = 16 * UniversalConstants.Units.SolarMassInKG
			}},
			{SpectralType.A, new MinMaxStruct
			{
				Min = 1.4 * UniversalConstants.Units.SolarMassInKG,
				Max = 2.1 * UniversalConstants.Units.SolarMassInKG
			}},
			{SpectralType.F, new MinMaxStruct
			{
				Min = 1.04 * UniversalConstants.Units.SolarMassInKG,
				Max = 1.4 * UniversalConstants.Units.SolarMassInKG
			}},
			{SpectralType.G, new MinMaxStruct
			{
				Min = 0.8 * UniversalConstants.Units.SolarMassInKG,
				Max = 1.04 * UniversalConstants.Units.SolarMassInKG
			}},
			{SpectralType.K, new MinMaxStruct
			{
				Min = 0.45 * UniversalConstants.Units.SolarMassInKG,
				Max = 0.8 * UniversalConstants.Units.SolarMassInKG
			}},
			{SpectralType.M, new MinMaxStruct
			{
				Min = 0.08 * UniversalConstants.Units.SolarMassInKG,
				Max = 0.45 * UniversalConstants.Units.SolarMassInKG
			}},
		};

		public static Dictionary<SpectralType, MinMaxStruct> StarAgeBySpectralType = new Dictionary<SpectralType, MinMaxStruct>()
		{
			{SpectralType.O, new MinMaxStruct
			{
				Min = 0,
				Max = 6000000
			}}, // after 6 million years O types either go nova or become B type stars.
            {SpectralType.B, new MinMaxStruct
			{
				Min = 0,
				Max = 100000000
			}}, // could not find any info on B type ages, so i made it between O and A (100 million).
            {SpectralType.A, new MinMaxStruct
			{
				Min = 0,
				Max = 350000000
			}}, // A type stars are always young, typical a few hundred million years..
            {SpectralType.F, new MinMaxStruct
			{
				Min = 0,
				Max = 3000000000
			}}, // Could not find any info again, chose a number between B and G stars (3 billion)
            {SpectralType.G, new MinMaxStruct
			{
				Min = 0,
				Max = 10000000000
			}}, // The life of a G class star is about 10 billion years.
            {SpectralType.K, new MinMaxStruct
			{
				Min = 0,
				Max = 13200000000
			}},
			{SpectralType.M, new MinMaxStruct
			{
				Min = 0,
				Max = 13200000000
			}},
		};
	}

}

