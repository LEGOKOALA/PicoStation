using Godot;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		PackedScene player1Scene = GD.Load<PackedScene>("res://Player1.tscn");
		PackedScene player2Scene = GD.Load<PackedScene>("res://Player2.tscn");
		PackedScene player3Scene = GD.Load<PackedScene>("res://Player3.tscn");
		PackedScene player4Scene = GD.Load<PackedScene>("res://Player4.tscn");

		Player1_movement player1 = (Player1_movement)player1Scene.Instantiate();
		Player2_movement player2 = (Player2_movement)player2Scene.Instantiate();
		Player3_movement player3 = (Player3_movement)player3Scene.Instantiate();
		Player4_movement player4 = (Player4_movement)player4Scene.Instantiate();

		player1.Position = new Vector2(100, 200);
		player2.Position = new Vector2(250, 200);
		player3.Position = new Vector2(400, 200);
		player4.Position = new Vector2(550, 200);

		AddChild(player1);
		AddChild(player2);
		AddChild(player3);
		AddChild(player4);
	}
}
