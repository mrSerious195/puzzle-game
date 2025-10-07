using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GridData : Node
{
	[Export]
	private Vector2I _gridSize = new Vector2I(5, 5);
	public Vector2I GridSize { get => _gridSize; set { _gridSize = new Vector2I(Math.Max(1, Math.Min(10, value.X)), Math.Max(1, Math.Min(10, value.Y))); _occupantByTile = new Creature[_gridSize.X * _gridSize.Y]; } }

	private Creature[] _occupantByTile = [null];
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GridSize = GridSize;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



	public bool CanCreatureBePlaced(Vector2I coordinates, Creature creature)
	{
		if (coordinates.X<0||coordinates.Y<0||coordinates.X + creature.UpperSize.X > GridSize.X || coordinates.Y + creature.UpperSize.Y > GridSize.Y)
		{
			return false;
		}
		bool[] coveredPos = creature.CoveredPositions;
		for (int x = 0; x < creature.UpperSize.X; x++)
		{
			for (int y = 0; y < creature.UpperSize.Y; y++)
			{

				GD.Print($"spot [{x + y * creature.UpperSize.X}]COVERPOS LENGHT = {coveredPos.Length},spot[{coordinates.X + x + (y + coordinates.Y) * GridSize.X}]_occupantByTile.lenght={_occupantByTile.Length} ");
				if (coveredPos[x + y * creature.UpperSize.X] && _occupantByTile[coordinates.X + x + (y + coordinates.Y) * GridSize.X] != null)
				{
					return false;
				}


			}
		}
		return true;
	}

	public void PlaceCreatureOnGrid(Vector2I coordinates, Creature creature)
	{
		bool[] coveredPos = creature.CoveredPositions;
		for (int x = 0; x < creature.UpperSize.X; x++)
		{
			for (int y = 0; y < creature.UpperSize.Y; y++)
			{

				if (coveredPos[x + y * creature.UpperSize.X])
				{
					_occupantByTile[coordinates.X + x + (y + coordinates.Y) * GridSize.X] = creature;
				}


			}
		}

	}

	public Color?[] GetGridColors()
	{
		Color?[] ret = new Color?[_occupantByTile.Length];
		for (int i = 0; i < _occupantByTile.Length; i++)
		{
			if (_occupantByTile[i] != null)
			{
				ret[i] = _occupantByTile[i].CreatureColor;
			}
		}
		return ret;

	}


	public bool HoldsCreature(Creature creature)
	{
		return -1 != Array.IndexOf(_occupantByTile, creature);
	}


	public Creature TryToPickUp(Vector2I position)
	{
		int posInArray = position.X + position.Y * GridSize.X;
		Creature ret = null;
		if (_occupantByTile[posInArray] != null)
		{
			ret = _occupantByTile[posInArray];
			_occupantByTile = _occupantByTile.Select(x => { if (x == ret) { return null; } else { return x; } }).ToArray();
		}


		return ret;

	}


	public bool IsGridFull()
	{
		foreach (var creature in _occupantByTile)
		{
			if (creature == null)
			{
				return false;
			}
		}
		return true;
        
    }


	
}
