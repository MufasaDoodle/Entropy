using Godot;
using System;

public partial class SystemMap : MeshInstance2D
{
	private bool isMouseOver = false;

	bool isDragging = false;
	Vector2 offset = new Vector2(0, 0);
	int currentZoomLevel;
	int minZoomLevel = 1;
	int maxZoomLevel = 100;

	public override void _Ready()
	{
		currentZoomLevel = minZoomLevel;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isDragging)
		{
			DoDrag();
		}
	}

	private void DoDrag()
	{
		//trying to clamp the map to the edges of the screen
		/*
		var screenSize = GetViewportRect().Size;
		var mapSize = Texture.GetSize();

		Vector2 newPos = GetGlobalMousePosition() - offset;

		newPos.X = Mathf.Clamp(newPos.X, mapSize.X, screenSize.X - mapSize.X);
		newPos.Y = Mathf.Clamp(newPos.Y, mapSize.Y, screenSize.Y - mapSize.Y);

		Position = newPos;
		*/

		Position = GetGlobalMousePosition() - offset; 
	}

	public void OnInputEvent(Node viewport, InputEvent evt, int shape_idx)
	{
		if (evt is InputEventMouseButton)
		{
			var mouseEvent = (InputEventMouseButton)evt;
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (mouseEvent.Pressed)
				{
					isDragging = true;
					offset = GetGlobalMousePosition() - GlobalPosition;
				}
				else
				{
					isDragging = false;
				}
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
			{
				if(currentZoomLevel > minZoomLevel)
				{
					currentZoomLevel--;
					Vector2 scale = Scale;
					scale.X -= 0.1f;
					scale.Y -= 0.1f;
					Scale = scale;
				}
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
			{
				if (currentZoomLevel < maxZoomLevel)
				{
					currentZoomLevel++;
					Vector2 scale = Scale;
					scale.X += 0.1f;
					scale.Y += 0.1f;
					Scale = scale;
				}
			}
		}
	}
}
