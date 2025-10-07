using Godot;
using System;

public abstract partial class SubScene : Node
{
    [Signal]
    public delegate void GotoSubSceneEventHandler(int SubSceneIndex);

    public abstract void GiveData(GameData data);

   // public SceneManager 
}
