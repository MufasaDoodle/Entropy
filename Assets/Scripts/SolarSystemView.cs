using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class SolarSystemView : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print($"Initializing SolarSystemView");
		GameManager.Instance.OnTimeTick += OnUpdateSprites;
		planetScene = GD.Load<PackedScene>("res://Assets/Prefabs/planet.tscn");
		planetTex = GD.Load<Texture2D>("res://Assets/Icons/planet.png");
		GD.Print($"Loaded scene and texture");
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

	SolarSystem solarSystem;

	//public Sprite[] Sprites;

	public ulong zoomLevels = 1500000000;

	PackedScene planetScene;
	Texture2D planetTex;

	/// <summary>
	/// Dictionary mapping orbital model objects to their in-game Godot object counterparts
	/// </summary>
	Dictionary<Orbital, Node2D> orbitalToNodeMap;

	public void ShowSolarSystem(int solarSystemID)
	{
		GD.Print($"Rendering solar system {solarSystemID}");
		//use the orbitalToNode dictionary to loop through and remove all old graphics
		//in order to not mix solar system graphics

		if (orbitalToNodeMap != null) //might be null on first instantiation
		{
			GD.Print($"Cleaning up dictionary");
			foreach (var pair in orbitalToNodeMap)
			{
				GD.Print($"dictionary iteration");
				pair.Value.Reparent(null);
				var children = pair.Value.GetChildren();

				foreach (var child in children)
				{
					child.Reparent(null); //setting the parent of the child node to null to avoid destroying more nodes than intended
				}

				GD.Print($"Destroying node");
				pair.Value.Free(); //destroy the node
			}

			orbitalToNodeMap.Clear(); //remove the Orbital references
		}
		/*

		// First, clean up the solar system by deleting any old graphics.
		while (transform.childCount > 0)
		{
			Transform c = transform.GetChild(0);
			c.SetParent(null);  // Become Batman
			Destroy(c.gameObject);
		}
		*/

		//orbitalToNodeMap = new Dictionary<Orbital, Node2D>();

		solarSystem = GameManager.Instance.galaxy.SolarSystems[solarSystemID]; //retrieve the solar system to display at given ID
		GD.Print($"Retrieved solar system from GameManager {solarSystem}");

		//Create an in-game object with a sprite for the star of the chosen solar system
		//then recursively go through each child of the star, and each child's children
		//so every body gets an object
		GD.Print($"Creating nodes for star system...");
		CreateNodesForOrbital(this, solarSystem.Star);


		//MakeSpritesForOrbital(this.Transform, solarSystem.Children[i]);

		/*
		for (int i = 0; i < solarSystem.Children.Count; i++)
		{
			//Orbital o = solarSystem.Children[i];
			MakeSpritesForOrbital(this.transform, solarSystem.Children[i]);
		}
		*/

		int nodeAmount = 0;

		foreach (var node in GetChildren())
		{
			nodeAmount++;
			foreach (var child in node.GetChildren())
			{
				nodeAmount++;
			}
		}

		GD.Print($"Star system render complete. Displaying {nodeAmount-1} nodes"); //minus 1 because it'll count a Texture2D 
	}

	bool creatingStar = true;

	void CreateNodesForOrbital(Node parentNode, Orbital o)
	{
		if(creatingStar)
		{
			GD.Print("   Creating star...");
		}
		else
		{
			GD.Print($"  Creating node...");
		}

		var instance = planetScene.Instantiate<Node2D>(); //instantiate planet based on scene
		orbitalToNodeMap[o] = instance; //map to orbital in dictionary

		parentNode.AddChild(instance);

		/*
		if (instance.GetParent() != null)
			instance.Reparent(parentNode); //set the parent of the planet
		*/

		if (creatingStar)
		{
			//manually place the star in the middle of the screen
			var windowSize = DisplayServer.WindowGetSize();
			Vector2 pos = new Vector2(windowSize.X / 2f, windowSize.Y / 2f);
			instance.Position = pos;

			creatingStar = false;
		}
		else
		{
			instance.Position = o.Position / zoomLevels; //set our position based on the scaling level
		}

		GD.Print($"Set node position to ({instance.Position.X}, {instance.Position.Y})");

		GD.Print($"Setting texture...");
		//now we need to set the texture of the instanced planet
		var spriteNode = instance.GetNode<Sprite2D>("Sprite2D");
		spriteNode.Texture = planetTex;
		GD.Print($"Texture set");

		GD.Print($"Generating for {o.Children.Count} children");
		//recursively instantiate and add sprites to each child this body has
		for (int i = 0; i < o.Children.Count; i++)
		{
			CreateNodesForOrbital(instance, o.Children[i]);
		}
	}

	/*
	void MakeSpritesForOrbital(Transform transformParent, Orbital o)
	{
		GameObject go = new GameObject();
		orbitalToNodeMap[o] = go;
		go.transform.SetParent(transformParent);

		// Set our position.
		go.transform.position = o.Position / zoomLevels;

		SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
		sr.sprite = Sprites[o.GraphicID];

		for (int i = 0; i < o.Children.Count; i++)
		{
			MakeSpritesForOrbital(go.transform, o.Children[i]);
		}
	}
	*/

	bool updatingStarPosition = true;

	public void OnUpdateSprites(string date)
	{
		UpdateSprites(solarSystem.Star);
		updatingStarPosition = true;
	}

	void UpdateSprites(Orbital o)
	{
		if(updatingStarPosition == false)
		{
			Node2D node = orbitalToNodeMap[o];
			Vector2 oldPos = node.Position;
			node.Position = o.Position / zoomLevels;
			Vector2 newPos = node.Position;

			Vector2 diff = oldPos - newPos;
			//GD.Print($"Planet position difference: ({diff.X}, {diff.Y})");
			//Debug.WriteLine($"Planet position difference: ({diff.X}, {diff.Y})");
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

	public void SetZoomLevel(ulong zl)
	{
		zoomLevels = zl;
		// Update planet positions
		// Also consider scaling the graphics up/down -- but keep a minimum size
		// figure out what scale means that each planet will always at least be a 
		// few pixels big, no matter the zoom level.
	}

}



