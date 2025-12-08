using Godot;

public partial class KeyUI : Control
{
	[Export] public TextureRect Player1KeyIcon;
	[Export] public TextureRect Player2KeyIcon;
	[Export] public TextureRect Player3KeyIcon;
	[Export] public TextureRect Player4KeyIcon;

	public void SetKeyVisible(int playerId, bool visible)
	{
		switch (playerId)
		{
			case 1: Player1KeyIcon.Visible = visible; break;
			case 2: Player2KeyIcon.Visible = visible; break;
			case 3: Player3KeyIcon.Visible = visible; break;
			case 4: Player4KeyIcon.Visible = visible; break;
		}
	}
}
 
