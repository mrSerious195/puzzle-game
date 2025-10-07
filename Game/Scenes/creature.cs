using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class Creature : Node2D
{
    private static Random RANDOM = new();

    public String CreatureName { get; private set; }

    public Color CreatureColor = Color.Color8(255, 20, 255);

    public Vector2I? _upperSize;
    public Vector2I UpperSize{
        get { if (GridRotation % 2 == 0)
            {
                return _upperSize.Value;
            }
            else {
                return new(_upperSize.Value.Y,_upperSize.Value.X);
            } } set{ _upperSize ??= value; }}

    protected bool[] _coveredPositions;

    public bool[] CoveredPositions{ get
        {
            List<bool> ret=[];
            switch (GridRotation)
            {
                case 1:for(int y = 0; y <_upperSize.Value.X; y++)
                    {
                        for (int x = _upperSize.Value.Y-1; x >= 0; x--)
                        {
                          //  GD.Print($"i ={x}, j= {y}, f(x,y)={(_upperSize.Value.X)*x+y}={_coveredPositions[(_upperSize.Value.X)*x+y]}");
                            ret.Add(_coveredPositions[(_upperSize.Value.X)*x+y]);
                        }
                }
                    return ret.ToArray();
                case 2: for (int i = _coveredPositions.Length - 1; i >= 0; i--)
                    {
                        ret.Add(_coveredPositions[i]);
                }
                    return ret.ToArray();
                     case 3:for(int y = _upperSize.Value.X-1; y >=0; y--)
                    {
                        for (int x = 0; x <_upperSize.Value.Y; x++)
                        {
                          //  GD.Print($"i ={x}, j= {y}, f(x,y)={(_upperSize.Value.X)*x+y}={_coveredPositions[(_upperSize.Value.X)*x+y]}");
                            ret.Add(_coveredPositions[(_upperSize.Value.X)*x+y]);
                        }
                }
                    return ret.ToArray();
                default: return (bool[])_coveredPositions.Clone();
            }
            
        } }


    private int _rotation=0;
    public int GridRotation { get=>_rotation; set{ _rotation = (4 + (value % 4)) % 4; } }
    public Creature(String creatureName,Vector2I upperSize, bool[] coveredPositions): this( creatureName,  upperSize,  coveredPositions, Color.Color8((byte)RANDOM.Next(200),(byte)RANDOM.Next(200),(byte)RANDOM.Next(200)))
    {    
    }

    public Creature(String creatureName, Vector2I upperSize, bool[] coveredPositions, Color color)
    {
        CreatureName = creatureName;
        UpperSize = upperSize;
        _coveredPositions = coveredPositions;
        CreatureColor = color;
        
    }


 



    public override void _Ready()
    {


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
