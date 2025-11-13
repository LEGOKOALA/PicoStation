using Godot;
using System;
using System.Collections.Generic;

public partial class Door : Area2D
{
	// --- Exported Properties ---
	[ExportGroup("Door Settings")]
	[Export] public string RequiredKeyId = ""; // leave empty if no key required
	[Export] public string NextScenePath = ""; // set this per-door in the editor
	[Export] public bool AutoOpen = false; // optional — if true, door opens automatically

	// --- Private Fields ---
	private AnimatedSprite2D _animatedDoor;
	private CollisionShape2D _solidCollider;
	private bool _isOpen = false;
	private HashSet<Player1_movement> _playersInside = new();

	public override void _Ready()
	{
		// Get child nodes safely
		_animatedDoor = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		_solidCollider = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

		if (_animatedDoor == null)
			GD.PrintErr("AnimatedSprite2D not found! Make sure it exists as a child node.");
		if (_solidCollider == null)
			GD.PrintErr("CollisionShape2D not found! Make sure it exists as a child node.");

		// Connect signals
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is not Player1_movement player)
			return;

		_playersInside.Add(player);

		if (_isOpen)
			return;

		// Check key or open automatically
		if (string.IsNullOrEmpty(RequiredKeyId) || player.HasKey(RequiredKeyId))
		{
			if (!string.IsNullOrEmpty(RequiredKeyId))
			{
				GD.Print($"Door opened using key: {RequiredKeyId}");
				player.ConsumeKey(RequiredKeyId);
			}
			else if (AutoOpen)
			{
				GD.Print("Door opened automatically (no key required).");
			}

			OpenDoor();
		}
		else
		{
			GD.Print($"You need the {RequiredKeyId} key to open this door!");
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body is Player1_movement player)
			_playersInside.Remove(player);
	}

	private bool PlayerInside => _playersInside.Count > 0;

	private void OpenDoor()
	{
		_animatedDoor?.Play("open");
		if (_solidCollider != null)
			_solidCollider.Disabled = true;
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

	private void LoadNextScene()
{
	// If NextScenePath is set manually, use that
	if (!string.IsNullOrEmpty(NextScenePath))
	{
		var result = GetTree().ChangeSceneToFile(NextScenePath);
		if (result != Error.Ok)
			GD.PrintErr($"Failed to load scene: {NextScenePath}, error: {result}");
		else
			GD.Print($"Loading next scene: {NextScenePath}");
		return;
	}

	// Otherwise, try to auto-load the next numbered scene
	string currentScenePath = GetTree().CurrentScene.SceneFilePath;
	string currentSceneName = System.IO.Path.GetFileNameWithoutExtension(currentScenePath);

	// Try to find a number at the end (e.g., "Level_1" → 1)
	int numberIndex = currentSceneName.LastIndexOf('_');
	if (numberIndex == -1 || numberIndex == currentSceneName.Length - 1)
	{
		GD.PrintErr("Could not determine next level (no number in scene name).");
		return;
	}

	string prefix = currentSceneName.Substring(0, numberIndex + 1);
	string numberPart = currentSceneName.Substring(numberIndex + 1);

	if (!int.TryParse(numberPart, out int currentLevel))
	{
		GD.PrintErr("Could not parse level number from scene name.");
		return;
	}

	int nextLevel = currentLevel + 1;
	string nextSceneName = $"{prefix}{nextLevel}.tscn";
	string nextSceneFullPath = $"res://Scenes/{nextSceneName}";

	GD.Print($"Attempting to load next scene automatically: {nextSceneFullPath}");

	var error = GetTree().ChangeSceneToFile(nextSceneFullPath);
	if (error != Error.Ok)
	{
		GD.PrintErr($"Failed to load next scene: {nextSceneFullPath} (Error: {error})");
	}
}}
