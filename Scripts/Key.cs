using Godot;
using System;

public partial class Key : Area2D
{
	[Export] public string KeyId = "default"; // Useful if you have multiple key types

	private void _on_body_entered(Node2D body)
	{
		if (body is CharacterBody2D player)
		{
			// Example: send a signal or call a method on the player
			if (player.HasMethod("CollectKey"))
				player.Call("CollectKey", KeyId);

			QueueFree(); // Remove the key
		}
	}
}
