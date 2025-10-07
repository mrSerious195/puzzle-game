using Godot;
using System;

public partial class SceneManager : Node
{

	private SubScene currentScene;
	private GameData _data;

	[Export]
	private PackedScene[] packedScenes;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GoToScene(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GoToScene(int SubSceneIndex)
	{
		if (SubSceneIndex == 0)
		{
			if (_data != null)
			{
				_data.QueueFree();
			}
			_data = new GameData();
			AddChild(_data);
		}
		if (currentScene != null)
		{
			currentScene.GotoSubScene -= GoToScene;
			currentScene.QueueFree();
		}
		currentScene = packedScenes[SubSceneIndex].Instantiate<SubScene>();
		AddChild(currentScene);
		currentScene.GotoSubScene += GoToScene;
		currentScene.GiveData(_data);
	}
}
