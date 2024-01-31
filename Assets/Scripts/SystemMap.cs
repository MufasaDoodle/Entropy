using Godot;
using System;

public partial class SystemMap : MeshInstance2D
{
	private bool isMouseOver = false;

	bool isDragging = false;
	Vector2 offset = new Vector2(0, 0);

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isDragging)
		{
			Position = GetGlobalMousePosition() - offset;
		}
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
		}
	}
}
