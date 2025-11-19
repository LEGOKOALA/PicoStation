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
	private HashSet<Node> _playersInside = new();

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
	// Detect which player entered
	Player1_movement p1 = body as Player1_movement;
	Player2_movement p2 = body as Player2_movement;
	Player3_movement p3 = body as Player3_movement;
	Player4_movement p4 = body as Player4_movement;

	// If none of the four match, stop
	if (p1 == null && p2 == null && p3 == null && p4 == null)
		return;

	// Pick whichever player object is not null
	Node player = p1 != null ? p1 :
				  p2 != null ? p2 :
				  p3 != null ? p3 :
							   p4;

	_playersInside.Add(player);

	// Door already open? no need to check keys
	if (_isOpen)
		return;

	// dynamic lets us call HasKey/ConsumeKey on any player type
	dynamic dynPlayer = player;

	// No key required OR player has the key
	if (string.IsNullOrEmpty(RequiredKeyId) || dynPlayer.HasKey(RequiredKeyId))
	{
		if (!string.IsNullOrEmpty(RequiredKeyId))
		{
			GD.Print($"Door opened using key: {RequiredKeyId}");
			dynPlayer.ConsumeKey(RequiredKeyId);
		}
		else if (AutoOpen)
		{
			GD.Print("Door opened automatically.");
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
