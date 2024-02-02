using System;
using Godot;

namespace Entropy.Assets.Scripts
{
	internal static class DistanceMath
	{
		//commented for now as Godot's vector2 cannot be created by a single double
		/*
		public static Vector3 AuToMt(Vector3 au)
		{
			Vector3 meters = au * UniversalConstants.Units.MetersPerAu;
			return meters;
		}
		public static Vector2 AuToMt(Vector2 au)
		{
			Vector2 meters = au * UniversalConstants.Units.MetersPerAu;
			return meters;
		}
		*/

		public static double AuToMt(double au)
		{
			return au * UniversalConstants.Units.MetersPerAu;
		}
	}
}
