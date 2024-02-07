using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class GameManager : Node
{
	//called before _Ready()
	public override void _EnterTree()
	{
		Instance = this;
		Galaxy = new Galaxy();
		Galaxy.Generate(3);

		base._EnterTree(); 
	}

	//TODO: make into property with proper accessibility
	public Galaxy Galaxy { get; private set; }

	//seconds since game start
	//used to calculate precise locations of orbital bodies 
	ulong galacticTime = 0;

	/// <summary>
	/// Increment the galactic time by given number in seconds. Updates locations of every orbital body in every star system
	/// </summary>
	/// <param name="numSeconds">Number of seconds to increment time with. Cannot be negative</param>
	public void AdvanceTime(int numSeconds)
	{
		galacticTime = galacticTime + (uint)numSeconds;

		Galaxy.Update(galacticTime);
	}

	private double timePerTick;
	private double tickTimer = 0f;

	private bool isPaused;

	/// <summary>
	/// Whether the game is currently paused or not
	/// </summary>
	public bool IsPaused
		{
			get { return isPaused; }
			set { isPaused = value; }
		}	

	public double TimePerTick
	{
		get { return timePerTick; }
		private set { timePerTick = value; }
	}

	private int currentSpeed;

	public int CurrentSpeed
	{
		get { return currentSpeed; }
		private set { currentSpeed = value; }
	}

	public int MaxSpeed { get; private set; } = 10;
	public int MinSpeed { get; private set; } = 1;

	public delegate void TimeTickEventHandler(string date);
	public event TimeTickEventHandler OnTimeTick;

	public delegate void SpeedChangedEventHandler(int speed);
	public event SpeedChangedEventHandler OnSpeedChanged;

	public static GameManager Instance { get; private set; }

	public int Ticks { get; private set; }

	public DateOnly CurrentDate { get; private set; }

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TimePerTick = 2f;
		CurrentSpeed = 1;
		Ticks = 0;
		IsPaused = true;
		var today = DateTime.Now;
		CurrentDate = new DateOnly(today.Year, today.Month, today.Day);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		IncrementTickTimer(delta);
	}

	public void TogglePause()
	{
		IsPaused = !IsPaused;
	}

	public void DecreaseSpeed()
	{
		if (CurrentSpeed > MinSpeed)
		{
			CurrentSpeed -= 1;
			OnSpeedChanged?.Invoke(CurrentSpeed);
		}
	}

	public void IncreaseSpeed()
	{
		if (CurrentSpeed < MaxSpeed)
		{
			CurrentSpeed += 1;
			OnSpeedChanged?.Invoke(CurrentSpeed);
		}
	}

	private void IncrementTickTimer(double delta)
	{
		if (!IsPaused)
		{
			double modifiedTime = delta *= CurrentSpeed;

			if(modifiedTime > TimePerTick)
			{
				GD.Print("Game is running very slow. Game ticks may be missed");
			}

			tickTimer += modifiedTime;

			if (tickTimer >= timePerTick)
			{
				ProcessTimeTick();
				tickTimer = 0f;
			}
		}
	}

	private void ProcessTimeTick()
	{
		Ticks++;
		CurrentDate = CurrentDate.AddDays(1);
		AdvanceTime(24 * 60 * 60); 
		OnTimeTick?.Invoke(GetCurrentDateAsString());
	}

	public string GetCurrentDateAsString()
	{
		return $"{CurrentDate.Day}-{CurrentDate.Month}-{CurrentDate.Year}";
	}
}
