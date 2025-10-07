using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

public partial class TeamSetupEditor : SubScene
{
	[Export]
	private TeamGridVisualizer _teamSetupVisualiser;

	[Export]
	private PieceGenerator _pieceGen;
	[Export]
	private HoverTile _hoverTile;

	[Export]
	private GridData _gridData;
	[Export]
	private TeamSetupGui _gui;

	private Creature _holdingCreature = null;

	private Creature[] _creatures;
	public override void _Ready()
	{
		_gui.GoBackToStart += () => { EmitSignal(SignalName.GotoSubScene, 0); };
	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void WindeowSizeChanged()
	{
		Vector2 tileSize = new Vector2(32, 16);
		Vector2 GridPixelSize = tileSize * (_gridData.GridSize.X+_gridData.GridSize.Y)/2;
		Vector2 windowSize = DisplayServer.WindowGetSize();
		float grows = Math.Min((windowSize.X-_gui.PieceSelectorSpace.X) / GridPixelSize.X, windowSize.Y / GridPixelSize.Y);
		grows *= 0.8f;
		grows = ((int)(grows*5))/5.0f;// to prevent it from creating thin lines in the cardinal directions
		GD.Print(new Vector2(grows, grows));
		_teamSetupVisualiser.Position = new Vector2(_gui.PieceSelectorSpace.X-GridPixelSize.X*grows/2, -0.25f*tileSize.Y*grows*(1+_gridData.GridSize.Y-_gridData.GridSize.X));
		_teamSetupVisualiser.Scale = new Vector2(grows, grows);

	}



	public void HoverHighLight(Vector2I hoverPos)
	{
		if (_holdingCreature != null)
		{
			if (!_gridData.CanCreatureBePlaced(hoverPos, _holdingCreature))
			{
				_hoverTile.HoverTileColour = Color.Color8(10, 10, 10);
			}
			else
			{
				_hoverTile.HoverTileColour = Color.Color8(20, 240, 20); ;
			}
			_hoverTile.PlaceTiles(hoverPos, _holdingCreature.UpperSize, _holdingCreature.CoveredPositions);

		}
		else
		{
			_hoverTile.PlaceTiles(hoverPos, Vector2I.Zero, []);

		}
	}

	public void ClickedTile(bool hoverOverCell, Vector2I clickPos)
	{
		if (_holdingCreature != null)
		{
			if (hoverOverCell)
			{
				if (_gridData.CanCreatureBePlaced(clickPos, _holdingCreature))
				{
					_gridData.PlaceCreatureOnGrid(clickPos, _holdingCreature);
					_holdingCreature = null;
					UpdateRoster();
					_teamSetupVisualiser.SetGridColours(_gridData.GetGridColors());
					HoverHighLight(clickPos);
					if (_gridData.IsGridFull())
					{
						_gui.ShowVictory();
					}
				}
			}
			else
			{
				_holdingCreature = null;
				UpdateRoster();
				_teamSetupVisualiser.SetGridColours(_gridData.GetGridColors());
				HoverHighLight(clickPos);
			}
		}
		else
		{
			if (hoverOverCell)
			{
				_holdingCreature = _gridData.TryToPickUp(clickPos);
				UpdateRoster();
				_teamSetupVisualiser.SetGridColours(_gridData.GetGridColors());
				HoverHighLight(clickPos);
			}
		}

	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.ButtonIndex == MouseButton.Right && mouseButtonEvent.Pressed)
			{
				//Vector2 local = _groundTiles.ToLocal(GetViewport().GetMousePosition());
				//Vector2I clickedCell = _groundTiles.LocalToMap(local);
				if (_holdingCreature != null)
				{
					_holdingCreature.GridRotation++;
					GD.Print($"_holdingCreature.GridRotation ={_holdingCreature.GridRotation}");
					HoverHighLight(_hoverTile.CurrentHover);
					UpdateRoster();
				}
			}


		}
	}

	public void UpdateRoster()
	{
		var infoCreatures = _creatures.Select(x => (!_gridData.HoldsCreature(x),x.CreatureColor,x.UpperSize,x.CoveredPositions)).ToArray();
		GD.Print($"Array.IndexOf(infoCreatures,_holdingCreature) ={Array.IndexOf(_creatures,_holdingCreature)}");
		_gui.SetCreatureButtons(infoCreatures,Array.IndexOf(_creatures,_holdingCreature));
	}


	public void TryToSelectCreature(int creatureIndex)
	{
		if (creatureIndex < 0 || creatureIndex >= _creatures.Length)
		{
			GD.PrintErr($"TeamSetupEditor.TryToSelectCreature:creatureIndex {creatureIndex} is invalid");
		}
		else
		{
			if (!_gridData.HoldsCreature(_creatures[creatureIndex]))
			{
				_holdingCreature = _creatures[creatureIndex];
			}
		}
		UpdateRoster();
	}


	public override void GiveData(GameData data)
	{
		_gridData = data.TeamGridData;
		_creatures = _pieceGen.GeneratePieces(_gridData.GridSize, data.SoftMaximum, data.HardMinimum);
		int columns = 3;
		_gui.ChangeButtonRosterSize(new Vector2I(columns, 1 + _creatures.Length / columns));
		_teamSetupVisualiser.GridSize = _gridData.GridSize;
		_hoverTile.HoverOverChanged += HoverHighLight;
		_teamSetupVisualiser.TileClickedOn += ClickedTile;
		GetViewport().SizeChanged += WindeowSizeChanged;
		WindeowSizeChanged();

		_gui.SelectedNewCreature += TryToSelectCreature;
		UpdateRoster();
		_teamSetupVisualiser.SetGridColours(_gridData.GetGridColors());
		WindeowSizeChanged();
	}


	
}
