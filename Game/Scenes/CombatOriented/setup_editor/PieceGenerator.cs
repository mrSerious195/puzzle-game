using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

public partial class PieceGenerator : Node
{
    private Vector2I[] _posDif = [new(0, 1), new(0, -1), new(1, 0), new(-1, 0)];
    private int MaxTryAddTiles = 10;
    public Creature[] GeneratePieces(Vector2I gridsize, int looseMaxSize,int MinCellsToCover)
    {
        GD.Print($"making pieces for gridsize ({gridsize}) with looseMaxSize {looseMaxSize} and MinCellsToCover {MinCellsToCover} ");
        List<Creature> ret = new();
        List<List<Vector2I>> partitions = [];

        Random rnd = new();

        bool[][] covered = new bool[gridsize.Y][];
        for (int i = 0; i < covered.Length; i++)
        {
            covered[i] = new bool[gridsize.X];
        }


        int tilesToPlace = gridsize.X * gridsize.Y;
        int placedTiles = 0;
        Vector2I? startPos = null;
        int startPosInt;
        while (placedTiles < tilesToPlace)
        {
            startPos = null;
            startPosInt = rnd.Next(tilesToPlace - placedTiles);
            GD.Print($"startPosInt={startPosInt}");
            int collumn;
            for (int row = 0; row < gridsize.Y; row++)
            {
                int occupiedSpotsInRow = covered[row].Count(IsSpotOccupied => { return !IsSpotOccupied; });
                if (startPosInt < occupiedSpotsInRow)
                {
                    GD.Print($"In row startPosInt={startPosInt}");
                    collumn = 0;
                    foreach (bool spotCovered in covered[row])
                    {
                        
                        if (startPosInt == 0&&collumn < gridsize.X)
                        {
                            if (!spotCovered)
                            {
                                break;
                            }
                            else
                            {
                                 collumn++;
                            }
                        }
                            else if (startPosInt > 0)
                            {
                                if (!spotCovered)
                                {
                                    startPosInt--;
                                }
                                collumn++;
                            }
                        if (collumn > gridsize.X || startPosInt < 0)
                        {
                            GD.PushError($"PieceGenerator.GeneratePieces gridsize={gridsize} Collumn = {collumn} occupiedSpotsInRow ={occupiedSpotsInRow} ");
                        }
                    }
                    startPos = new Vector2I(collumn, row);
                    placedTiles++;
                    GD.Print($"         NEW PIECE WITH Random StartPos={startPos}");
                    break;
                }
                else
                {
                    startPosInt -= occupiedSpotsInRow;
                }
            }

            if (startPos == null)
            {
                GD.PushError($"startingPos is null; ");
            }

            covered[startPos.Value.Y][startPos.Value.X] = true;
            foreach (var row in covered)
            {
                GD.Print(string.Join('|',row));
            }
            List<Vector2I> CovereredByPiece = [startPos.Value];
            partitions.Add(CovereredByPiece);
            bool cancontinue = true;
            int pieceSize = 1;
            while (cancontinue && pieceSize < looseMaxSize&&placedTiles < tilesToPlace)
            {
               

                for (int tries = 0; tries < MaxTryAddTiles + 1; tries++)
                {
                    Vector2I posDif = _posDif[rnd.Next(_posDif.Length)];
                    Vector2I tryCover = CovereredByPiece[rnd.Next(CovereredByPiece.Count)] + posDif;
                    if (tryCover.X >= 0 && tryCover.X < gridsize.X && tryCover.Y >= 0 && tryCover.Y < gridsize.Y && !covered[tryCover.Y][tryCover.X])
                    {
                        covered[tryCover.Y][tryCover.X] = true;
                        
                        CovereredByPiece.Add(tryCover);
                        GD.Print($"for startpos {startPos} added pos {tryCover}, posdif ({posDif})");
                        foreach (var row in covered)
            {
                            GD.Print(string.Join('|', row));
            }
                        placedTiles++;
                        pieceSize++;
                    }

                    if (tries >= MaxTryAddTiles)
                    {
                        cancontinue = false;
                        break;
                    }
                }

            }

        }

        for (int pieceIndex=partitions.Count-1;pieceIndex>=0 ; pieceIndex--)
        {
            List<Vector2I> CovereredByPiece = partitions[pieceIndex];
            if (CovereredByPiece.Count < MinCellsToCover)
            {
                 GD.Print($"PIECE ON {CovereredByPiece[0]} is smaller ({CovereredByPiece.Count}) than {MinCellsToCover}");
                for (int cellId = CovereredByPiece.Count - 1; cellId >= 0; cellId--)
                {
                    Vector2I cell = CovereredByPiece[cellId];

                    int posdifIndex = rnd.Next(_posDif.Length);
                    bool breakPosDifLoop = false;
                    for (int i = 0; i < _posDif.Length; i++)
                    {
                     
                        Vector2I tryCover = cell + _posDif[(posdifIndex + i) % _posDif.Length];
                        if (tryCover.X >= 0 && tryCover.X < gridsize.X && tryCover.Y >= 0 && tryCover.Y < gridsize.Y)
                        {

                            for (int pieceToMakeBiggerIndex = partitions.Count - 1; pieceToMakeBiggerIndex >= 0; pieceToMakeBiggerIndex--)
                            {
                                List<Vector2I> pieceThatMightGrows = partitions[(pieceIndex + pieceToMakeBiggerIndex) % partitions.Count];
                                if (pieceThatMightGrows != CovereredByPiece && pieceThatMightGrows.Contains(tryCover))
                                {
                                    pieceThatMightGrows.Add(cell);
                                    CovereredByPiece.Remove(cell);
                                    GD.Print($"Assigned cellc {cell}, which is adjecent to {tryCover}, to piece on {pieceThatMightGrows[0]}");
                                    breakPosDifLoop = true;
                                    break;
                                }
                            }

                        }
                        if (breakPosDifLoop) { break; }
                    }
                    if (!breakPosDifLoop)
                    {
                        GD.PrintErr($"Couldn't assign {cell}");
                    }
                }
                if (CovereredByPiece.Count == 0)
                {
                    partitions.Remove(CovereredByPiece);
                }
                else
                {
                    GD.Print("Small piece still had a cell and can't be removed");
                }
            }
        }


        foreach (List<Vector2I> CovereredByPiece in partitions)
        {
            //calculate the outer bounds;
            int minY = gridsize.Y, maxY = 0, minX = gridsize.X, maxX = 0;
            foreach (Vector2I pos in CovereredByPiece)
            {
                minY = Math.Min(minY, pos.Y);
                minX = Math.Min(minX, pos.X);

                maxY = Math.Max(maxY, pos.Y);
                maxX = Math.Max(maxX, pos.X);
            }
            Vector2I firstPos = new Vector2I(minX, minY);
            Vector2I outersize = new Vector2I(maxX - minX + 1, maxY - minY + 1);
            bool[] pieceCover = new bool[outersize.Y * outersize.X];
            for (int yMove = 0; yMove < outersize.Y; yMove++)
            {
                for (int xMove = 0; xMove < outersize.X; xMove++)
                {
                    if (CovereredByPiece.Contains(firstPos + new Vector2I(xMove, yMove)))
                    {
                        pieceCover[outersize.X * yMove + xMove] = true;
                    }
                    else
                    {
                        GD.Print($"piece on {CovereredByPiece[0]} DOES NOT haspos {firstPos + new Vector2I(xMove, yMove)}");
                    }
                }
            }

            GD.Print($"Added piece with outerSize = {outersize} haspos:");
            foreach (var b in CovereredByPiece)
            {
                GD.Print($"End piece on {CovereredByPiece[0]} haspos {b}");
            }

            ret.Add(new Creature("O", outersize, pieceCover));
        }

        foreach(var piece in ret)
        {
            piece.GridRotation = rnd.Next(4);
        }


        return ret.ToArray(); ;
    }
    
}
