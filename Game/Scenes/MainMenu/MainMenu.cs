using Godot;
using System;

public partial class MainMenu : SubScene
{
	private GameData _data;
	[Export]
	private SpinBox minpieceSize;
	[Export]
	private SpinBox maxpieceSize;
	public void GoToTeamSetupEditor()
	{
		EmitSignal(SignalName.GotoSubScene, (int)SubSceneIndex.TeamSetupEditor);
	}

	public override void GiveData(GameData data)
	{
		_data = data;
	}

	public void SetGridX(float newX)
	{
		_data.TeamGridData.GridSize = new Vector2I((int)newX, _data.TeamGridData.GridSize.Y);
	}

	public void SetGridY(float newY)
	{
		_data.TeamGridData.GridSize = new Vector2I(_data.TeamGridData.GridSize.X, (int)newY);
	}

	public void SetMinPieceSize(float newMin)
	{
		_data.HardMinimum = (int)newMin;
		minpieceSize.Value = _data.HardMinimum;
		maxpieceSize.Value = _data.SoftMaximum;
	}
	
	public void SetMaxPieceSize(float newMax)
	{
			_data.SoftMaximum = (int)newMax;
		maxpieceSize.Value = _data.SoftMaximum;
	}

}
