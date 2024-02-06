using Godot;
using System;

public partial class PlanetDot : Node2D
{
	[Export]
	public Color Color { get; set; } = Colors.Green;
	[Export]
	public Vector2 center = new Vector2(0f, 0f);
	[Export]
	public float radius = 30f;

	public void DrawPlanetDot(Color color)
	{
		this.Color = color;
		QueueRedraw();
	}

	public override void _Draw()
	{
		DrawCircle(center, radius, Color);

		base._Draw();
	}
}
