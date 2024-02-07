using Entropy.Assets.Scripts;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SysInfoPlanetViewer : Panel
{
	OptionButton sortingMode;
	VBoxContainer container;
	PackedScene planetRowPrefab;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sortingMode = GetNode<OptionButton>("SortSelect");
		container = GetNode<VBoxContainer>("PlanetViewer/VBoxContainer");
		planetRowPrefab = GD.Load<PackedScene>("res://Assets/Prefabs/UIComponents/planet_row.tscn");
	}

	SolarSystem system;

	public void Populate(SolarSystem system)
	{
		this.system = system;
		RemoveAllPlanetRows();
		int sortMode = sortingMode.Selected;
		PlanetSortingMode mode = (PlanetSortingMode)sortMode;

		List<Planet> bodyList = system.Star.Children.Cast<Planet>().ToList();
		List<Planet> sortedList = SortPlanetListByMode(mode, bodyList);

		foreach (Planet planet in sortedList)
		{
			AddPlanetRow(planet);

			List<Planet> moons = planet.Children.Cast<Planet>().ToList();
			List<Planet> sortedMoons = SortPlanetListByMode(mode, moons);

			foreach (Planet child in sortedMoons) 
			{
				AddPlanetRow(child, true);
			}
		}
	}

	private List<Planet> SortPlanetListByMode(PlanetSortingMode mode, List<Planet> bodyList)
	{
		List<Planet> sortedList;
		if (mode == PlanetSortingMode.Distance)
		{
			sortedList = bodyList.OrderBy(o => o.OrbitalDistance).ToList();
		}
		else if (mode == PlanetSortingMode.Name)
		{
			sortedList = bodyList.OrderBy(o => o.BodyName).ToList();
		}
		else if (mode == PlanetSortingMode.BodyType)
		{
			sortedList = bodyList.OrderBy(o => o.BodyType).ToList();
		}
		else if (mode == PlanetSortingMode.Population)
		{
			sortedList = bodyList;
			//sortedList = bodyList.OrderBy(o => o.Population).ToList(); //not implemented
		}
		else if (mode == PlanetSortingMode.ColonyCost)
		{
			sortedList = bodyList;
			//sortedList = bodyList.OrderBy(o => o.OrbitalDistance).ToList(); //not implemented
		}
		else if (mode == PlanetSortingMode.Diameter)
		{
			sortedList = bodyList.OrderBy(o => o.Radius).ToList();
		}
		else if (mode == PlanetSortingMode.Period)
		{
			sortedList = bodyList.OrderBy(o => o.OrbitalPeriod).ToList();
		}
		else if (mode == PlanetSortingMode.Mass)
		{
			sortedList = bodyList.OrderBy(o => o.Mass).ToList();
		}
		else
		{
			sortedList = bodyList;
		}

		return sortedList;
	}

	private void AddPlanetRow(Planet planet, bool isMoon = false)
	{
		HBoxContainer planetRow = planetRowPrefab.Instantiate<HBoxContainer>();

		//distText is here if we want to display meter distance instead of AU
		string distText = string.Empty;
		if(planet.OrbitalDistance > 1000000000)
		{
			distText = $"{Math.Round(planet.OrbitalDistance / 1000d / 1000d / 1000d, 2, MidpointRounding.AwayFromZero)}Bm";
		}
		else if(planet.OrbitalDistance > 1000000)
		{
			distText = $"{Math.Round(planet.OrbitalDistance / 1000d / 1000d, 2, MidpointRounding.AwayFromZero)}Mm";
		}
		else
		{
			distText = $"{Math.Round(planet.OrbitalDistance / 1000d, 2, MidpointRounding.AwayFromZero)}km";
		}

		double diameter = Math.Round(planet.Radius * 2d / 1000d, 5, MidpointRounding.AwayFromZero);
		
		string diameterText = string.Empty;
		if(diameter <= 100000)
		{
			diameterText = $"{Math.Round(planet.Radius * 2d / 1000d, 2, MidpointRounding.AwayFromZero)}km";
		}
		else
		{
			diameterText = $"{Math.Round(planet.Radius * 2d / 1000d / 1000d, 2, MidpointRounding.AwayFromZero)}k km";
		}

		double orbitTime = Math.Round(planet.OrbitalPeriod / 60d / 60d / 24d / 365d, 2, MidpointRounding.AwayFromZero);
		string timeText = string.Empty;
		if(orbitTime <= 0.35d)
		{
			timeText = $"{Math.Round(planet.OrbitalPeriod / 60d / 60d / 24d, 2, MidpointRounding.AwayFromZero)} days";
		}
		else
		{
			timeText = $"{Math.Round(planet.OrbitalPeriod / 60d / 60d / 24d / 365d, 2, MidpointRounding.AwayFromZero)} years";
		}


		if (isMoon)
		{
			planetRow.GetNode<Label>("Name").Text = $"  {planet.BodyName}";
		}
		else
		{
			planetRow.GetNode<Label>("Name").Text = $"{planet.BodyName}";
		}

		planetRow.GetNode<Label>("BodyType").Text = $"{planet.BodyType}";
		planetRow.GetNode<Label>("Population").Text = $"N/A";
		planetRow.GetNode<Label>("ColonyCost").Text = $"N/A";
		planetRow.GetNode<Label>("Diameter").Text = $"{diameterText}";
		planetRow.GetNode<Label>("Orbital Distance").Text = $"{Math.Round(planet.OrbitalDistance / UniversalConstants.Units.MetersPerAu, 4, MidpointRounding.AwayFromZero)}AU";
		planetRow.GetNode<Label>("Orbital Period").Text = $"{timeText}";
		planetRow.GetNode<Label>("Mass").Text = $"{Math.Round((double)planet.Mass / (double)UniversalConstants.Units.EarthMassInKG, 3, MidpointRounding.AwayFromZero)} earths";
		planetRow.GetNode<Button>("ViewButton").Pressed += () => OpenPlanetView(planet.Guid);
		container.AddChild(planetRow);
	}

	private void OpenPlanetView(Guid guid)
	{
		GD.Print($"Open planet view with ID: {guid}");
	}

	private enum PlanetSortingMode
	{
		Name,
		BodyType,
		Population,
		ColonyCost,
		Diameter,
		Distance,
		Period,
		Mass
	}

	public void OnSortSelectChanged (int index)
	{
		RemoveAllPlanetRows();
		Populate(system);
	}

	private void RemoveAllPlanetRows()
	{
		var children = container.GetChildren();

		if(children.Count > 1 ) 
		{
			for(int i = 1; i < children.Count; i++) //delete everything but the top object (which is the table description)
			{
				children[i].QueueFree();
			}
		}
	}
}

