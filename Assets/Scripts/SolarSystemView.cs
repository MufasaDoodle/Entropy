using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class SolarSystemView : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		GameManager.Instance.OnTimeTick += OnUpdateSprites;
		planetScene = GD.Load<PackedScene>("res://Assets/Prefabs/planet.tscn");
		planetTex = GD.Load<Texture2D>("res://Assets/Icons/planet.png");
		sunTex = GD.Load<Texture2D>("res://Assets/Icons/sun.png");
		orbitalToNodeMap = new Dictionary<Orbital, Node2D>();

		ShowSolarSystem(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (solarSystem != null)
		{
			//UpdateSprites(solarSystem.Star);
		}
	}

	//TODO figure this stuff out

	public static SolarSystemView Instance;

	SolarSystem solarSystem;

	//public Sprite[] Sprites;

	public ulong currentZoomLevel = 1500000000;

	PackedScene planetScene;
	Texture2D planetTex;
	Texture2D sunTex;


	ulong minZoomLevel = 10000;
	ulong maxZoomLevel = 150000000000;

	int currentID = 0;

	/// <summary>
	/// Dictionary mapping orbital model objects to their in-game Godot object counterparts
	/// </summary>
	Dictionary<Orbital, Node2D> orbitalToNodeMap;

	public void NextSystem()
	{
		int nextID = currentID +1;
		if (nextID < GameManager.Instance.galaxy.SolarSystems.Count)
		{
			ShowSolarSystem(nextID);
		}
	}

	public void PrevSystem()
	{
		int nextID = currentID -1;
		if (nextID >= 0)
		{
			ShowSolarSystem(nextID);
		}
	}

	public void ShowSolarSystem(int solarSystemID)
	{
		currentID = solarSystemID;
		//use the orbitalToNode dictionary to loop through and remove all old graphics
		//in order to not mix solar system graphics

		if (orbitalToNodeMap != null) //might be null on first instantiation
		{
			List<Node2D> bodies = new List<Node2D>();

			foreach (var pair in orbitalToNodeMap)
			{
				bodies.Add(pair.Value);
			}

			if (bodies.Count > 0)
				bodies[0].Free();

			bodies.Clear();

			orbitalToNodeMap.Clear(); //remove the Orbital references
		}

		solarSystem = GameManager.Instance.galaxy.SolarSystems[solarSystemID]; //retrieve the solar system to display at given ID

		//Create an in-game object with a sprite for the star of the chosen solar system
		//then recursively go through each child of the star, and each child's children
		//so every body gets an object
		CreateNodesForOrbital(this, solarSystem.Star);
		creatingStar = true;

		int nodeAmount = 0;

		foreach (var node in GetChildren())
		{
			nodeAmount++;
			foreach (var child in node.GetChildren())
			{
				nodeAmount++;
			}
		}
	}

	bool creatingStar = true;

	void CreateNodesForOrbital(Node parentNode, Orbital o)
	{
		var instance = planetScene.Instantiate<Node2D>(); //instantiate planet based on scene
		orbitalToNodeMap[o] = instance; //map to orbital in dictionary

		parentNode.AddChild(instance);

		/*
		if (instance.GetParent() != null)
			instance.Reparent(parentNode); //set the parent of the planet
		*/

		var spriteNode = instance.GetNode<Sprite2D>("Sprite2D");
		if (creatingStar)
		{
			//we makin a star
			//manually place the star in the middle of the screen
			var windowSize = DisplayServer.WindowGetSize();
			Vector2 pos = new Vector2(windowSize.X / 2f, windowSize.Y / 2f);
			instance.Position = pos;
			spriteNode.Texture = sunTex;

			creatingStar = false;
		}
		else
		{
			//we makin a planet
			instance.Position = o.Position / currentZoomLevel; //set our position based on the scaling level
			spriteNode.Texture = planetTex;
		}
		//now we need to set the texture of the instanced planet

		//recursively instantiate and add sprites to each child this body has
		for (int i = 0; i < o.Children.Count; i++)
		{
			CreateNodesForOrbital(instance, o.Children[i]);
		}
	}

	bool updatingStarPosition = true;

	public void OnUpdateSprites(string date)
	{
		UpdateSprites(solarSystem.Star);
		updatingStarPosition = true;
	}

	void UpdateSprites(Orbital o)
	{
		if (updatingStarPosition == false)
		{
			Node2D node = orbitalToNodeMap[o];
			Vector2 oldPos = node.Position;
			node.Position = o.Position / currentZoomLevel;
		}
		else
		{
			updatingStarPosition = false;
		}

		for (int i = 0; i < o.Children.Count; i++)
		{
			UpdateSprites(o.Children[i]);
		}
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt is InputEventMouseButton)
		{
			var mouseEvent = (InputEventMouseButton)evt;
			if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
			{
				if (currentZoomLevel < maxZoomLevel)
				{
					currentZoomLevel = (ulong)((double)currentZoomLevel * 1.05);
					Mathf.Clamp(currentZoomLevel, minZoomLevel, maxZoomLevel);
				}
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
			{
				if (currentZoomLevel > minZoomLevel)
				{
					currentZoomLevel = (ulong)((double)currentZoomLevel * 0.95);
					Mathf.Clamp(currentZoomLevel, minZoomLevel, maxZoomLevel);
				}
			}
		}

		OnUpdateSprites("");

		base._UnhandledInput(evt);
	}
}



