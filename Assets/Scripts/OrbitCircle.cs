using Godot;
using System;

public partial class OrbitCircle : Node2D
{
	[Export]
	public int Segments { get; set; } = 100;
	[Export]
	public int Width { get; set; } = 3;
	[Export]
	public Color Color { get; set; } = Colors.Green;
	[Export]
	public bool AntiAliasing { get; set; } = false;

	[Export]
	public Vector2 center = new Vector2(0f,0f);
	[Export]
	public float radius = 1f;

	[Export]
	public bool isStar = true;

	public void DrawOrbitCircle(Vector2 origin, double radius)
	{
		center = origin;
		this.radius = (float)radius;
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (!isStar) //don't draw orbit circles if it's a star
		{
			float startAngle = 0f;
			float endAngle = Mathf.Tau;

			// Finally, draw the arc.
			DrawArc(center, radius, startAngle, endAngle, Segments, Color,
					Width, AntiAliasing);
		}		

		base._Draw();
	}
}
