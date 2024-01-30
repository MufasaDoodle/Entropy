using Godot;
using System;

public partial class PauseButton : Button
{
	private Texture2D pauseIcon;
	private Texture2D unpauseIcon;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pauseIcon = GD.Load<Texture2D>("res://Assets/Icons/pause-button.svg");
		unpauseIcon = GD.Load<Texture2D>("res://Assets/Icons/play-button.svg");
	}

	public void OnPauseButtonPressed()
	{
		GameManager.Instance.TogglePause();
		var spriteNode = GetNode<Sprite2D>("PauseSprite");

		if (GameManager.Instance.IsPaused)
		{
			spriteNode.Texture = unpauseIcon;
		}
		else
		{
			spriteNode.Texture = pauseIcon;
		}		

		GD.Print("Pause Pressed. Paused = " + GameManager.Instance.IsPaused);
	}
}
