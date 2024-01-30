using Godot;
using System;

public partial class DatePanel : Panel
{
	private Label currentDate;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentDate = GetNode<Label>("DateLabel");
		currentDate.Text = $"Ticks: {GameManager.Instance.Ticks}";
		GameManager.Instance.OnTimeTick += SetDate;
	}

	public void SetDate(int ticks)
	{
		currentDate.Text = $"Ticks: {ticks}";
	}
}
