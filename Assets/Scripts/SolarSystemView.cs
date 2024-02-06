using Entropy.Assets.Scripts;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public partial class SolarSystemView : Node2D
{
	public override void _Ready()
	{
		Instance = this;
		GameManager.Instance.OnTimeTick += OnUpdateSprites;
		planetScene = GD.Load<PackedScene>("res://Assets/Prefabs/planet.tscn");
		planetTex = GD.Load<Texture2D>("res://Assets/Icons/planet.png");
		sunTex = GD.Load<Texture2D>("res://Assets/Icons/sun.png");
		distanceIndicator = GetNode<Line2D>("../CanvasLayer/DistanceIndicator");
		RecalculateDistanceIndicator();
		orbitalToNodeMap = new Dictionary<Orbital, Node2D>();

		ShowSolarSystem(0);
	}

	public override void _Process(double delta)
	{
		if (isDragging)
		{
			DoDrag();
		}

		if (solarSystem != null)
		{
			//if we ever want to force a UI update of the solar system we can un-comment the below line
			//UpdateSprites(solarSystem.Star);
		}
	}

	/// <summary>
	/// The singleton instance of the solar system UI handler
	/// </summary>
	public static SolarSystemView Instance;

	SolarSystem solarSystem;

	// CLASS TEMPORARY FIELDS
	bool isDragging = false;
	Vector2 offset = new Vector2(0, 0);
	int currentID = 0;

	// GODOT RELATED FIELD
	PackedScene planetScene;
	Texture2D planetTex;
	Texture2D sunTex;
	Line2D distanceIndicator;

	// ZOOM RELATED FIELDS
	public ulong currentZoomLevel = 1500000000;
	ulong minZoomLevel = 10000;
	ulong maxZoomLevel = 150000000000;

	/// <summary>
	/// Dictionary mapping orbital model objects to their in-game Godot object counterparts
	/// </summary>
	Dictionary<Orbital, Node2D> orbitalToNodeMap;

	/// <summary>
	/// Loads the next solar system in order they were created
	/// </summary>
	public void NextSystem()
	{
		int nextID = currentID + 1;
		if (nextID < GameManager.Instance.galaxy.SolarSystems.Count)
		{
			ShowSolarSystem(nextID);
		}
	}

	/// <summary>
	/// Loads the previous solar system in order they were created
	/// </summary>
	public void PrevSystem()
	{
		int nextID = currentID - 1;
		if (nextID >= 0)
		{
			ShowSolarSystem(nextID);
		}
	}

	/// <summary>
	/// Display the solar system at the specified ID
	/// </summary>
	/// <param name="solarSystemID">ID of solar system to display</param>
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
	}

	/// <summary>
	/// Recursively creates nodes on screen for given body, its children and children's children 
	/// </summary>
	/// <param name="parentNode"></param>
	/// <param name="o"></param>
	void CreateNodesForOrbital(Node2D parentNode, Orbital o)
	{
		var instance = planetScene.Instantiate<Node2D>(); //instantiate planet based on scene
		orbitalToNodeMap[o] = instance; //map to orbital in dictionary

		parentNode.AddChild(instance);

		var spriteNode = instance.GetNode<Sprite2D>("Sprite2D");
		var labelNode = instance.GetNode<Label>("BodyNameLabel");

		if (o.GetType() == typeof(Star)) //star properties have to be set differently to other bodies, like planets and moon
		{
			//we makin a star
			//manually place the star in the middle of the screen
			var windowSize = DisplayServer.WindowGetSize();
			Vector2 pos = new Vector2(windowSize.X / 2f, windowSize.Y / 2f); //middle of screen
			instance.Position = pos; //set star's position to middle of screen
			spriteNode.Texture = sunTex;			
			var orbitCircleNode = instance.GetNode<OrbitCircle>("OrbitCircle");
			orbitCircleNode.isStar = true; //this ensures that stars don't have orbital circles drawn
			instance.Name = o.BodyName;
			labelNode.Text = o.BodyName;
			DrawPlanetDot(o, instance);
			CheckIfDisableSprites(instance, o);
		}
		else
		{
			//we makin a planet
			instance.Position = o.Position / currentZoomLevel; //set our position based on the scaling level and in relation to parent body's position

			if (o.GraphicsName == string.Empty)
			{
				spriteNode.Texture = planetTex;
				instance.Name = o.BodyName;
				labelNode.Text = o.BodyName;
			}
			else
			{
				Texture2D planetTexture = GD.Load<Texture2D>($"res://Assets/Icons/SolIcons/{o.GraphicsName}");
				spriteNode.Texture = planetTexture;
				instance.Name = o.BodyName;
				labelNode.Text = o.BodyName;
			}

			if (o.GetType() == typeof(Planet))
			{
				var planet = (Planet)o;
				if (planet.BodyType == BodyType.Moon) //if this is a moon, we want to render the sprite behind the parent body's sprite
				{
					instance.ShowBehindParent = true;
				}
			}

			DrawPlanetDot(o, instance);
			var orbitCircleNode = instance.GetNode<OrbitCircle>("OrbitCircle");
			orbitCircleNode.isStar = false; //ensures that this body will have orbital lines drawn
			DrawOrbitCircle(o, instance, parentNode); //force an initial redraw of the orbital circle
			CheckIfDisableSprites(instance, o);
		}

		//recursively instantiate and add sprites to each child this body has
		for (int i = 0; i < o.Children.Count; i++)
		{
			CreateNodesForOrbital(instance, o.Children[i]);
		}
	}

	public void OnUpdateSprites(string date)
	{
		UpdateSprites(solarSystem.Star);
	}

	void UpdateSprites(Orbital o)
	{
		if (o.GetType() != typeof(Star)) //if this isn't a star. We don't need to update star sprites
		{
			Node2D node = orbitalToNodeMap[o];
			node.Position = o.Position / currentZoomLevel; // set our position based on the scaling level and in relation to parent body's position
			DrawPlanetDot(o, node);
			DrawOrbitCircle(o, node, (Node2D)node.GetParent()); //redraw orbital circle
			CheckIfDisableSprites(node, o);
		}
		else
		{
			Node2D node = orbitalToNodeMap[o];
			CheckIfDisableSprites(node, o);
		}

		//update any children this orbital body may have
		for (int i = 0; i < o.Children.Count; i++)
		{
			UpdateSprites(o.Children[i]);
		}
	}

	private void DrawPlanetDot(Orbital o, Node2D bodyToDrawFor)
	{
		var planetDotNode = bodyToDrawFor.GetNode<PlanetDot>("PlanetDot");

		Color color;

        if (o.GetType() == typeof(Planet))
        {
			var p = (Planet) o;
            if (p.BodyType == BodyType.Moon)
			{
				color = Colors.Yellow;
			}
			else
			{
				color = Colors.Blue;
			}
        }
		else
		{
			color = Colors.Orange;
		}        

		planetDotNode.DrawPlanetDot(color);
	}

	private void DrawOrbitCircle(Orbital o, Node2D bodyToDrawFor, Node2D bodyParentNode)
	{
		var orbitCircleNode = bodyToDrawFor.GetNode<OrbitCircle>("OrbitCircle");
		if (orbitCircleNode.isStar == true) return; //don't draw orbital circle if this orbital is a star

		//get the vector for the parent body, relative to this body
		Vector2 posToDrawAt = new Vector2(-(bodyToDrawFor.Position.X), -(bodyToDrawFor.Position.Y));

		//draw the orbital circle centered at the parent body, with a radius depending on orbital distance and the current zoom scale
		orbitCircleNode.DrawOrbitCircle(posToDrawAt, o.OrbitalDistance / currentZoomLevel);
	}

	private void DoDrag()
	{
		Position = GetGlobalMousePosition() - offset;
		OnUpdateSprites("");
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt is InputEventMouseButton)
		{
			var mouseEvent = (InputEventMouseButton)evt;
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (mouseEvent.Pressed) //check if we're dragging the map
				{
					isDragging = true;
					offset = GetGlobalMousePosition() - GlobalPosition;
				}
				else
				{
					isDragging = false;
				}
			}

			//even if we are dragging, we also want to handle other mouse events, like zooming in/out
			if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
			{
				if (currentZoomLevel < maxZoomLevel)
				{
					currentZoomLevel = (ulong)((double)currentZoomLevel * 1.05);
					Mathf.Clamp(currentZoomLevel, minZoomLevel, maxZoomLevel);

					ZoomCamOffset(false);

					RecalculateDistanceIndicator();
					OnUpdateSprites("");
				}
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
			{
				if (currentZoomLevel > minZoomLevel)
				{
					currentZoomLevel = (ulong)((double)currentZoomLevel * 0.95);
					Mathf.Clamp(currentZoomLevel, minZoomLevel, maxZoomLevel);

					ZoomCamOffset(true);

					RecalculateDistanceIndicator();
					OnUpdateSprites("");
				}
			}
		}

		base._UnhandledInput(evt);
	}

	/// <summary>
	/// Attempt to move the 'camera' to follow the mouse's location when zooming in or out
	/// </summary>
	/// <param name="isZoomIn"></param>
	private void ZoomCamOffset(bool isZoomIn)
	{
		double zoomSpeed = 0.95;

		var mousePos = GetLocalMousePosition();

		float mouseX = mousePos.X;
		float mouseY = mousePos.Y;

		var windowSize = DisplayServer.WindowGetSize();
		Vector2 viewportCenter = new Vector2(windowSize.X / 2f, windowSize.Y / 2f);

		double xOffset = mouseX - viewportCenter.X - (mouseX - viewportCenter.X) * zoomSpeed;
		double yOffset = mouseY - viewportCenter.Y - (mouseY - viewportCenter.Y) * zoomSpeed;

		if (isZoomIn)
		{
			var newPos = new Vector2(Position.X + (float)-xOffset, Position.Y + (float)-yOffset);
			Position = newPos;
		}
		else
		{
			var newPos = new Vector2(Position.X + (float)+xOffset, Position.Y + (float)+yOffset);
			Position = newPos;
		}
	}

	private void RecalculateDistanceIndicator()
	{
		Label distLabel = distanceIndicator.GetNode<Label>("DistanceLabel");
		var points = distanceIndicator.Points;
		double pointLength = points[1].X - points[0].X;
		ulong distanceInMeters = (ulong)pointLength * currentZoomLevel;

		double distInAU = Math.Round((double)(distanceInMeters / UniversalConstants.Units.MetersPerAu), 3, MidpointRounding.AwayFromZero);
		double distInThousandsKM = Math.Round((double)(distanceInMeters / 1000d / 1000d), 1, MidpointRounding.AwayFromZero);

		string distText = $"{distInThousandsKM}k km ({distInAU} AU). System scale: 1:{currentZoomLevel}";

		distLabel.Text = distText;
	}

	private void CheckIfDisableSprites(Node2D node, Orbital o)
	{
		if (o.GetType() == typeof(Planet))
		{
			if (currentZoomLevel > 100000000)
			{
				//disable sprites
				var spriteNode = node.GetNode<Sprite2D>("Sprite2D");
				spriteNode.Visible = false;
				
				if(((Planet)o).BodyType == BodyType.Moon)
				{
					var labelNode = node.GetNode<Label>("BodyNameLabel");
					labelNode.Visible = false;
				}
			}
			else
			{
				//enable sprites
				var spriteNode = node.GetNode<Sprite2D>("Sprite2D");
				spriteNode.Visible = true;

				if (((Planet)o).BodyType == BodyType.Moon)
				{
					var labelNode = node.GetNode<Label>("BodyNameLabel");
					labelNode.Visible = true;
				}
			}
		}
		else if (o.GetType() == typeof(Star))
		{
			if (currentZoomLevel > 750000000)
			{
				//disable sprites
				var spriteNode = node.GetNode<Sprite2D>("Sprite2D");
				spriteNode.Visible = false;
			}
			else
			{
				//enable sprites
				var spriteNode = node.GetNode<Sprite2D>("Sprite2D");
				spriteNode.Visible = true;
			}
		}		
	}
}