using Godot;
using System;

public partial class SpeedControls : Panel
{
	private Label speedLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		speedLabel = GetNode<Label>("SpeedLabel");
		UpdateSpeedLabel(GameManager.Instance.CurrentSpeed);
		GameManager.Instance.OnSpeedChanged += UpdateSpeedLabel;
	}	

	public void UpdateSpeedLabel(int newSpeed)
	{
		if(newSpeed < GameManager.Instance.MinSpeed || newSpeed > GameManager.Instance.MaxSpeed)
		{
			GD.PrintErr("Tried setting game speed UI to an illegal amount: " +  newSpeed);
		}

		string speedText = string.Empty;
		for (int i = 0; i < newSpeed; i++)
		{
			speedText += ">";
		}
		speedLabel.Text = speedText;
	}

	public void OnDecreaseSpeedPressed()
	{
		GameManager.Instance.DecreaseSpeed();
	}

	public void OnIncreaseSpeedPressed()
	{
		GameManager.Instance.IncreaseSpeed();
	}
}
