using Godot;
using System;
using System.Collections.Generic;


public partial class TeamGridVisualizer : Node2D
{
	[Signal]
	public delegate void TileClickedOnEventHandler(bool OnExistingTile,Vector2I position);
	[Signal]
	public delegate void HoverOverChangedEventHandler(bool OnExistingTile, Vector2I position);
	
	[Export]
	private TileMapLayer _groundTiles;

	public bool MustCheckHover = false;

	public Vector2I? CurrentHover = null;
	private Vector2I _gridSize = new(3, 3);

	private static Color _WHITE = Color.Color8(255, 255, 255);
	[Export]
	public Vector2I GridSize { get => _gridSize; set { _gridSize = new Vector2I(Math.Max(1, Math.Min(10, value.X)), Math.Max(1, Math.Min(10, value.Y))); UpdateGrid(); } }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateGrid();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(MustCheckHover)
		CheckHover();
	}

	public void UpdateGrid()
	{
		if (!IsInstanceValid(_groundTiles))
		{

			GD.PrintErr("TeamGrid.UpdateGrid():_groundTiles is invalid");
			return;
		}
		GD.Print("TeamGrid.UpdateGrid()");
		Vector2I cellPos;


		_groundTiles.Clear();
		for (int IndexX = 0; IndexX < GridSize.X; IndexX++)
		{

			for (int IndexY = 0; IndexY < GridSize.Y; IndexY++)
			{
				cellPos = new(IndexX, IndexY);

				_groundTiles.SetCell(cellPos, 0, cellPos);

			}
		}
	}

	public void SetGridColours( Color?[] colors)
	{
		Vector2I ColoursPosition;
		//	GD.Print("SetGridColours");
		for (int i = 0; i < colors.Length; i++)
		{
			ColoursPosition = new(i%GridSize.X, i / GridSize.X);

			//	GD.Print($"SetGridColour of {pos}");
			if (colors[i] == null)
			{
			_groundTiles.GetCellTileData(ColoursPosition).Modulate = _WHITE;
			}
			else
			{
				_groundTiles.GetCellTileData(ColoursPosition).Modulate = colors[i].Value;
			}


		}
	}

	public void WhipeColours()
	{
		Vector2I cellPos;
		for (int IndexX = 0; IndexX < GridSize.X; IndexX++)
		{

			for (int IndexY = 0; IndexY < GridSize.Y; IndexY++)
			{
				cellPos = new(IndexX, IndexY);
				_groundTiles.GetCellTileData(cellPos).Modulate = _WHITE;

			}
		}
	}


	public void CheckHover()
	{


		Vector2I HoverCell = _groundTiles.LocalToMap(_groundTiles.GetLocalMousePosition());

		if (HoverCell.X < 0 || HoverCell.X >= GridSize.X || HoverCell.Y < 0 || HoverCell.Y >= GridSize.Y)
		{
			if (CurrentHover != null)
			{
				EmitSignal(SignalName.HoverOverChanged, false, HoverCell);
				CurrentHover = null;
				//GD.Print($"HoverCell = {HoverCell} = NULL");
			}
		}
		else
		{
			if (CurrentHover != HoverCell)
			{
				EmitSignal(SignalName.HoverOverChanged, true, HoverCell);
				CurrentHover = HoverCell;
				//GD.Print($"HoverCell = {HoverCell}");
			}
		}
		//GD.Print($"{HoverCell}");
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
			{
				//Vector2 local = _groundTiles.ToLocal(GetViewport().GetMousePosition());
				//Vector2I clickedCell = _groundTiles.LocalToMap(local);
				Vector2I clickedCell = _groundTiles.LocalToMap(_groundTiles.GetLocalMousePosition());
				if (clickedCell.X >= 0 && clickedCell.X < GridSize.X && clickedCell.Y >= 0 && clickedCell.Y < GridSize.Y)
				{
					//	GD.Print($"clickedCell = {clickedCell}");
					EmitSignal(SignalName.TileClickedOn, true, clickedCell);
				}
				else
				{
					EmitSignal(SignalName.TileClickedOn,false, clickedCell);
				}
			}


		}
	}

	





}
