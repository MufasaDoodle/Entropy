using Entropy.Assets.Scripts;
using Godot;
using System;

public static class OrbitalMath
{

	/// <summary>
	/// Calculates the orbital period of an orbital body from its radius and primary body's mass
	/// </summary>
	/// <param name="bodyRadius">Radius of body to calculate orbital period from</param>
	/// <param name="primaryBodyMass">Mass of the body which it is orbiting around</param>
	/// <returns></returns>
	public static UInt64 GetOrbitalPeriodCircularOrbitWithRadius(double bodyRadius, double primaryBodyMass)
	{
		UInt64 P = 0;

		double period = (bodyRadius * Math.Pow(4f * Math.PI * Math.PI / (primaryBodyMass * UniversalConstants.Science.GravitationalConstant), 1f / 3f));
		P = (UInt64)(bodyRadius * Math.Pow(4f * Math.PI * Math.PI / (primaryBodyMass * UniversalConstants.Science.GravitationalConstant), 1f / 3f));
		return P;
	}

	/// <summary>
	/// DOES NOT CURRENTLY WORK
	/// </summary>
	/// <param name="bodyRadius"></param>
	/// <param name="orbitingBodyMass"></param>
	/// <param name="primaryBodyMass"></param>
	/// <returns></returns>
	public static UInt64 GetOrbitalPeriodCircularOrbitWithRadius(double bodyRadius, double orbitingBodyMass, double primaryBodyMass)
	{
		double periodSquared = (4f * Math.PI * Math.PI * Math.Pow(bodyRadius, 3f)) / (UniversalConstants.Science.GravitationalConstant * (orbitingBodyMass + primaryBodyMass));
		double period = Math.Sqrt(periodSquared);
		return (UInt64)period;
	}

	public static UInt64 GetOrbitalPeriodWithDistance(UInt64 distance, double orbitingBodyMass, double primaryBodyMass)
	{
		double periodSquared = (4f * Math.PI * Math.PI * Math.Pow(distance, 3f)) / (UniversalConstants.Science.GravitationalConstant * (orbitingBodyMass + primaryBodyMass));
		double period = Math.Sqrt(periodSquared);
		return (UInt64)period;
	}
}
