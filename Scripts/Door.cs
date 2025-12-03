using Godot;
using System;
using System.Collections.Generic;

public partial class Door : Area2D
{
	// --- Exported Properties ---
	[ExportGroup("Door Settings")]
	[Export] public string RequiredKeyId = ""; // Every door requires a key
	[Export] public string NextScenePath = ""; // Optional manual next scene

	// --- Private Fields ---
	private AnimatedSprite2D _animatedDoor;
	private bool _isOpen = false;
	private HashSet<Node> _playersInside = new();

	public override void _Ready()
	{
		_animatedDoor = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

		if (_animatedDoor == null)
			GD.PrintErr("AnimatedSprite2D not found!");

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node body)
	{
		Node player = null;

		if (body is Player1_movement p1) player = p1;
		else if (body is Player2_movement p2) player = p2;
		else if (body is Player3_movement p3) player = p3;
		else if (body is Player4_movement p4) player = p4;

		if (player == null) return;

		_playersInside.Add(player);

		if (_isOpen) return;

		dynamic dynPlayer = player;

		// --- EVERY DOOR REQUIRES A KEY ---
		if (!dynPlayer.HasKey(RequiredKeyId))
		{
			GD.Print($"You need the {RequiredKeyId} key to open this door!");
			return;
		}

		// Player has the key, consume it
		dynPlayer.ConsumeKey(RequiredKeyId);
		GD.Print($"Door opened using key: {RequiredKeyId}");

		OpenDoor();
	}

	private void OnBodyExited(Node body)
	{
		if (body is Player1_movement ||
			body is Player2_movement ||
			body is Player3_movement ||
			body is Player4_movement)
		{
			_playersInside.Remove(body);
		}
	}

	private bool PlayerInside => _playersInside.Count > 0;

	private void OpenDoor()
	{
		_animatedDoor?.Play("open");
		_isOpen = true;
	}

	public override void _Input(InputEvent @event)
	{
		if (!_isOpen || !PlayerInside)
			return;

		if (@event.IsActionPressed("Interact"))
		{
			LoadNextScene();
		}
	}

	// --- Clear all keys for players in this door ---
	private void ClearKeysForAllPlayers()
	{
		foreach (Node player in _playersInside)
		{
			dynamic dynPlayer = player;
			try
			{
				dynPlayer.ClearAllKeys();
				GD.Print($"Cleared keys for: {player.Name}");
			}
			catch
			{
			}
		}
	}

	// --- Load the next scene ---
	private void LoadNextScene()
	{
		// Clear keys before leaving the level
		ClearKeysForAllPlayers();

		if (!string.IsNullOrEmpty(NextScenePath))
		{
			GetTree().ChangeSceneToFile(NextScenePath);
			GD.Print($"Loading next scene (manual): {NextScenePath}");
			return;
		}

		// Automatic next-level loading
		string currentScenePath = GetTree().CurrentScene.SceneFilePath;
		string currentSceneName = System.IO.Path.GetFileNameWithoutExtension(currentScenePath);

		int numberIndex = currentSceneName.LastIndexOf('_');
		if (numberIndex == -1)
		{
			GD.PrintErr("Could not determine next level.");
			return;
		}

		string prefix = currentSceneName.Substring(0, numberIndex + 1);
		string numberPart = currentSceneName.Substring(numberIndex + 1);

		if (!int.TryParse(numberPart, out int currentLevel))
		{
			GD.PrintErr("Invalid scene number.");
			return;
		}

		int nextLevel = currentLevel + 1;
		string nextScene = $"res://Scenes/{prefix}{nextLevel}.tscn";

		GD.Print($"Loading next scene (auto): {nextScene}");
		GetTree().ChangeSceneToFile(nextScene);
	}
}
