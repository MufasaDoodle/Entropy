using Godot;
using System;

public partial class GameManager : Node
{
	private static bool isPaused;

	/// <summary>
	/// Whether the game is currently paused or not
	/// </summary>
	public static bool IsPaused
	{
		get { return isPaused; }
		set { isPaused = value; }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsPaused = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
