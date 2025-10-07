using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class GameData : Node
{
    public GridData TeamGridData;

    private int _softMinimum = 2;
    public int HardMinimum { get => _softMinimum; set { _softMinimum = Math.Max(1, Math.Min(SoftMaximum, value));SoftMaximum = SoftMaximum; } }

    private int _softMaximum=5;
    public int SoftMaximum { get => _softMaximum; set { _softMaximum = Math.Max(HardMinimum, value); } }
    public GameData()
    {
        TeamGridData = new GridData();
        AddChild(TeamGridData);
    }

}
