using Godot;
using System;

public partial class DatePanel : Panel
{
	private Label currentDate;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentDate = GetNode<Label>("DateLabel");
		currentDate.Text = GameManager.Instance.GetCurrentDateAsString();
		GameManager.Instance.OnTimeTick += SetDate;
	}

	public void SetDate(string date)
	{
		GD.Print(date);
		currentDate.Text = date;
	}
}
