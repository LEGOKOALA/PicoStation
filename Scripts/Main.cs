using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	private readonly string[] playerScenes = {
		"res://Scenes/Player1.tscn",
		"res://Scenes/Player2.tscn",
		"res://Scenes/Player3.tscn",
		"res://Scenes/Player4.tscn",
	};

	private const int MAX_PLAYERS = 4;

	public override void _Ready()
	{
		// Detect all connected controllers
		List<int> connectedJoypads = GetConnectedJoypads();

		// --- Always spawn Player 1 (keyboard + controller 0) ---
		PackedScene player1Scene = GD.Load<PackedScene>(playerScenes[0]);
		Node2D player1 = (Node2D)player1Scene.Instantiate();
		player1.Position = new Vector2(-240, 0); // fixed starting point
		AddChild(player1);

		// --- Determine how many other players should spawn ---
		int totalPlayers = Math.Min(connectedJoypads.Count, MAX_PLAYERS);
		int remainingPlayers = totalPlayers - 1;

		// --- Calculate spacing so last player doesn't go past X = -8 ---
		float startX = -240f;
		float endX = -8f;
		float spacing = remainingPlayers > 0 ? (endX - startX) / remainingPlayers : 0;

		// --- Spawn players 2–4 dynamically ---
		int sceneIndex = 1;
		foreach (int joyIndex in connectedJoypads)
		{
			if (joyIndex == 0) continue; // skip controller 0 (used by Player 1)
			if (sceneIndex >= MAX_PLAYERS) break;

			PackedScene scene = GD.Load<PackedScene>(playerScenes[sceneIndex]);
			Node2D player = (Node2D)scene.Instantiate();

			// evenly space out between -240 and -8
			player.Position = new Vector2(startX + spacing * sceneIndex, 0);

			AddChild(player);
			sceneIndex++;
		}
	}

	// Helper: find all connected controllers
	private List<int> GetConnectedJoypads()
	{
		List<int> joypads = new List<int>();
		for (int i = 0; i < 8; i++) // check up to 8 controllers
		{
			if (Input.IsJoyKnown(i))
				joypads.Add(i);
		}
		return joypads;
	}
}
