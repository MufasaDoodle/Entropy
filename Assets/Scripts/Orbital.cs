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
		BodyName = "Unnamed";
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
	// Max uint64 value is:		   18,446,744,073,709,551,615m -> ~123,308,867 AU
	// Pluto distance from sun is: ~4,000,000,000,000m -> ~27 AU
	// uint64 (ulong) is completely fine to use

	/// <summary>
	/// Time to orbit around primary body. In Seconds.
	/// </summary>
	public UInt64 OrbitalPeriod;

	public double Mass;

	public double Radius;

	public string GraphicsName { get; set; }

	public string BodyName {  get; set; }

	public Guid Guid { get; set; }

	
	/// <summary>
	/// The body's position relative to the system's star (in meters)
	/// </summary>
	public Vector2 Position
	{
		get
		{
			// Consider whether or not we should be saving Vector3 in a 
			// private variable whenever we update our angle, or if it's
			// no slower to just calculate on demand like this.

			Vector2 myOffset = new Vector2(
				Mathf.Sin(InitAngle + OffsetAngle) * OrbitalDistance,
				-Mathf.Cos(InitAngle + OffsetAngle) * OrbitalDistance
			);

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
}