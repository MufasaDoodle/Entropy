using Godot;
using System;
using System.Diagnostics;

public partial class GameManager : Node
{
	private bool isPaused;

	public int Ticks { get; set; }

	private double timePerTick;

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


	private double tickTimer = 0f;

	public delegate void TimeTickEventHandler(int ticks);
	public event TimeTickEventHandler OnTimeTick;

	public delegate void SpeedChangedEventHandler(int speed);
	public event SpeedChangedEventHandler OnSpeedChanged;

	public static GameManager Instance { get; private set; }

	/// <summary>
	/// Whether the game is currently paused or not
	/// </summary>
	public bool IsPaused
	{
		get { return isPaused; }
		set { isPaused = value; }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		TimePerTick = 2f;
		CurrentSpeed = 1;
		Ticks = 0;
		IsPaused = true;
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
		OnTimeTick?.Invoke(Ticks);
		GD.Print("Tick: " + Ticks);
	}
}
