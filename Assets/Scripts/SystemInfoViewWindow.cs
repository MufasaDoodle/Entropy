using Godot;
using System;

public partial class SystemInfoViewWindow : Window
{
	Panel starInfo;
	Panel planetInfo;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		starInfo = GetNode<Panel>("Star Info");
		planetInfo = GetNode<Panel>("Planets Panel");
	}	

	public void OpenSysView()
	{
		Show();
		var sysInfo = GetNode<SysStarInfo>("Star Info");
		sysInfo.SetSystemList(GameManager.Instance.Galaxy.SolarSystems);
	}

	public void UpdatePlanetViewer(SolarSystem system)
	{
		var planetView = GetNode<SysInfoPlanetViewer>("Planets Panel");
		planetView.Populate(system);
	}

	public void CloseRequested()
	{
		Hide();
	}
}
