using Godot;
using System;
using System.Data;

public partial class HoverTile : TileMapLayer
{

	[Signal]
	public delegate void HoverOverChangedEventHandler( Vector2I position);

	public Color HoverTileColour { get => Modulate; set { Modulate = value; } }

	public Vector2I CurrentHover;


	public override void _Process(double delta)
	{
		CheckHover();
	}

	public void PlaceTiles(Vector2I hoverPosition, Vector2I upperSize, bool[] filledTiles)
	{
		Clear();
		Vector2I cellPos;

		for (int x = 0; x < upperSize.X; x++)
		{
			for (int y = 0; y < upperSize.Y; y++)
			{
				if (filledTiles[x + y * upperSize.X])
				{
					cellPos = hoverPosition + new Vector2I(x, y);
					SetCell(cellPos, 0, Vector2I.Zero);
				}
			}


		}

	}

	public void CheckHover()
	{


		Vector2I HoverCell = LocalToMap(GetLocalMousePosition());


		if (CurrentHover != HoverCell)
		{
			EmitSignal(SignalName.HoverOverChanged, HoverCell);
			CurrentHover = HoverCell;
			//GD.Print($"HoverCell = {HoverCell}");
		}
			
	}
		
	
}
