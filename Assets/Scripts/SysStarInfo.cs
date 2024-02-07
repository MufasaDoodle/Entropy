using Entropy.Assets.Scripts;
using Godot;
using System;
using System.Collections.Generic;

public partial class SysStarInfo : Panel
{
	Label classLabel;
	Label ageLabel;
	Label tempLabel;
	Label luminLabel;
	Label massLabel;
	Label diameterLabel;
	OptionButton systemSelect;

	List<SolarSystem> systems;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		classLabel = GetNode<Label>("ClassLabel");
		ageLabel = GetNode<Label>("AgeLabel");
		tempLabel = GetNode<Label>("TempLabel");
		luminLabel = GetNode<Label>("LuminLabel");
		massLabel = GetNode<Label>("MassLabel");
		diameterLabel = GetNode<Label>("DiameterLabel");
		systemSelect = GetNode<OptionButton>("SystemSelectDropdown");
	}

	public void SetSystemList(List<SolarSystem> systems)
	{
		systemSelect.Clear();
		this.systems = systems;
		foreach (SolarSystem system in systems)
		{
			systemSelect.AddItem(system.Star.BodyName);
		}

		Guid currentSysViewSystemID = SolarSystemView.Instance.SolarSystem.Star.Guid;

		int idToShow = -1;
		for (int i = 0; i < systems.Count; i++)
		{
			if (systems[i].Star.Guid == currentSysViewSystemID)
			{
				idToShow = i;
				break; //get out of loop
			}
		}

		if(idToShow == -1)
		{
			GD.PrintErr($"StarInfo could not find currently viewed system in system view");
		}

		systemSelect.Selected = idToShow;

		OnSystemSelectChanged(idToShow);
	}

	private void Populate(SolarSystem system)
	{
		if(system == null)
		{
			return;
		}

		string ageText = "";
		if(system.Star.Age > 1000000000) //if more than 1 bil, display in billions of years
		{
			ageText = $"{Math.Round(system.Star.Age / 1000000000d, 2, MidpointRounding.AwayFromZero)}B years";
		}
		else //else we show millions of years
		{
			ageText = $"{Math.Round(system.Star.Age / 1000000d, 2, MidpointRounding.AwayFromZero)}M years";
		}

		classLabel.Text = $"{system.Star.StarClass}";
		ageLabel.Text = $"{ageText}";
		tempLabel.Text = $"{system.Star.Temperature} degrees";
		luminLabel.Text = $"{Math.Round(system.Star.Luminosity, 6, MidpointRounding.AwayFromZero)} L";
		massLabel.Text = $"{Math.Round(system.Star.Mass / UniversalConstants.Units.SolarMassInKG, 3, MidpointRounding.AwayFromZero)} sols";
		diameterLabel.Text = $"{Math.Round(((system.Star.Radius) / 1000) / UniversalConstants.Units.SolarRadiusInKm, 4, MidpointRounding.AwayFromZero)} solar radii";
	}

	public void OnSystemSelectChanged(int index)
	{
		Populate(systems[index]);
		var window = GetParent<SystemInfoViewWindow>();
		window.UpdatePlanetViewer(systems[index]);
	}
}
