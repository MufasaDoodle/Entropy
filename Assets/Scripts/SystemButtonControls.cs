using Godot;
using System;

public partial class SystemButtonControls : Control
{
	public void OnPrevButtonPressed()
	{
		SolarSystemView.Instance.PrevSystem();
	}

	public void OnNextButtonPressed() 
	{ 
		SolarSystemView.Instance.NextSystem(); 
	}
}
