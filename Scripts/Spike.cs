using Godot;

public partial class Spike : Area2D
{
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public void OnBodyEntered(Node body)
	{

		// If the object is any of your player types, reload
		if (body is Player1_movement ||
			body is Player2_movement ||
			body is Player3_movement ||
			body is Player4_movement)
		{
			GetTree().ReloadCurrentScene();
		}
	}
}
