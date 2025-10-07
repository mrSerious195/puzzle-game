using Godot;
using System;
using System.Linq;

public partial class TeamSetupGui : CanvasLayer
{
	[Signal]
	public delegate void SelectedNewCreatureEventHandler(int CollectionIndex);
	[Signal]
	public delegate void GoBackToStartEventHandler();
	// Called when the node enters the scene tree for the first time.

	[Export]
	private PackedScene _edditorbutton;

	private CustomGridEdditorButton[] _creatureButtons = [];
	[Export]
	private GridContainer _buttonGrid;

	[Export]
	private PanelContainer victoryScreen;

	public Vector2I RosterSize { get; private set; } = new(5, 20);
	public Vector2 PieceSelectorSpace { get; private set; } = new(5, 20);

	public float SelectorTakesWhatpercentageOfTheScreen = 0.15f;
	public float MaxButtonVeroudinging = 4/3f;
	public override void _Ready()
	{
		GetViewport().SizeChanged += WindeowSizeChanged;
	}

	public void ChangeButtonRosterSize(Vector2I newRosterSize)
	{
		_buttonGrid.Columns = newRosterSize.X;
		RosterSize = newRosterSize;
		foreach (var button in _creatureButtons)
		{
			button.QueueFree();
		}
		int newButtonCount = newRosterSize.X * newRosterSize.Y;
		_creatureButtons = new CustomGridEdditorButton[newButtonCount];
		Vector2 windowsize = DisplayServer.WindowGetSize();
		Vector2 buttonSize = windowsize / (float)(RosterSize.Y+1);
		Vector2 seperators = buttonSize / (float)(RosterSize.Y-1);
		CustomGridEdditorButton b;
		_buttonGrid.AddThemeConstantOverride("h_separation", (int)seperators.X);
		_buttonGrid.AddThemeConstantOverride("v_separation", (int)seperators.Y);
		for (int i = 0; i < newButtonCount; i++)
		{
			int indx = i;
			b = _edditorbutton.Instantiate<CustomGridEdditorButton>();
			b.CustomMinimumSize = buttonSize;
			_creatureButtons[i] = b;
			_buttonGrid.AddChild(b);
			b.FocusMode = Control.FocusModeEnum.None;
			b.Pressed += () => ClickedButton(indx);
		}
		WindeowSizeChanged();
	}


	public void WindeowSizeChanged()
	{
		Vector2 windowsize = DisplayServer.WindowGetSize();
		PieceSelectorSpace =new Vector2(windowsize.X* SelectorTakesWhatpercentageOfTheScreen,windowsize.Y);
		Vector2 seperators = new Vector2(5,5);
		Vector2 buttonSize = (PieceSelectorSpace-seperators*(RosterSize.Y-1)) / (float)(RosterSize.Y);
		if (MaxButtonVeroudinging*buttonSize.X < buttonSize.Y)
		{
			buttonSize =new Vector2(buttonSize.X, MaxButtonVeroudinging*buttonSize.X);
		}
		
		GD.Print($"RosterSize={RosterSize} PieceSelectorSpace={PieceSelectorSpace} windowsize={windowsize} ");
		_buttonGrid.AddThemeConstantOverride("h_separation", (int)seperators.X);
		_buttonGrid.AddThemeConstantOverride("v_separation", (int)seperators.Y);
		for (int i = 0; i < _creatureButtons.Length; i++)
		{
			_creatureButtons[i].CustomMinimumSize = buttonSize;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetCreatureButtons((bool CanBeUsed, Color color, Vector2I outerbound, bool[] IsCellUsed)[] creatureInfo, int IndexSelected)
	{
		for (int i = 0; i < _creatureButtons.Length; i++)
		{
			if (i < creatureInfo.Length)
			{
				_creatureButtons[i].Visible = true;


				_creatureButtons[i].Setbutton(creatureInfo[i].color, creatureInfo[i].outerbound, creatureInfo[i].IsCellUsed, i == IndexSelected);
				//	_creatureButtons[i].Text = creatureNames[i];
				_creatureButtons[i].Enabled = creatureInfo[i].CanBeUsed;

			}
			else
			{
				//	_creatureButtons[i].Disabled = true;
				_creatureButtons[i].Visible = false;
				//	_creatureButtons[i].Text = "";
			}
		}
	}


	public void ClickedButton(int ButtonIndex)
	{
		EmitSignal(SignalName.SelectedNewCreature, ButtonIndex);
		GD.Print(ButtonIndex);
	}

	public void ShowVictory()
	{
		victoryScreen.Visible = true;
	}

	public void GoToStart()
	{
		EmitSignal(SignalName.GoBackToStart);
	}

	
}
