using Godot;
using System.Collections.Generic;

public enum PlayerTypeEnum { NONE, HUMAN, Ki1, Ki2, Ki3};


public static class MainSettings
{
    public static Godot.Collections.Dictionary<string, PlayerTypeEnum> Players { get; set; } = new Godot.Collections.Dictionary<string, PlayerTypeEnum>()
    {
        { "Hans", PlayerTypeEnum.HUMAN },
        { "Franz", PlayerTypeEnum.Ki1 },
        { "Marry", PlayerTypeEnum.Ki2 },
        { "Lee", PlayerTypeEnum.Ki3 }
    };
}

public static class Helper
{
    static public ImageTexture CreateTexture(string path)
    {
        Image img = Image.LoadFromFile(path);
        ImageTexture tex = ImageTexture.CreateFromImage(img);
        return tex;
    }

}


public partial class Main : Control
{
    ItemList games;
    ItemList player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        games = GetNode<ItemList>("ilGames");

		string[] dirs = Godot.DirAccess.GetDirectoriesAt("res://games/");

		foreach (string dir in dirs)
		{
            games.AddItem(dir, Helper.CreateTexture("res://games/" + dir + "/assets/icon.png"));
        }

        player = GetNode<ItemList>("ilPlayer");
        foreach(KeyValuePair<string, PlayerTypeEnum> pair in MainSettings.Players)
        {
            if (pair.Value == PlayerTypeEnum.NONE)
                continue;

            string avatar = "player";
            if (pair.Value == PlayerTypeEnum.Ki1)
                avatar = "ki1";
            else if (pair.Value == PlayerTypeEnum.Ki2)
                avatar = "ki2";
            else if (pair.Value == PlayerTypeEnum.Ki3)
                avatar = "ki3";

            player.AddItem(pair.Key, Helper.CreateTexture("res://assets/avatare/" + avatar + ".png"));
        }
    }




    public void OnButtonPressed()
    {
		int[] indexes = games.GetSelectedItems();
		string game = games.GetItemText(indexes[0]);

		string path = "res://games/" + game + "/main.tscn";

        GD.Print("Starte " + path);

		Node simultaneousScene = ResourceLoader.Load<PackedScene>(path).Instantiate();
        GetTree().Root.AddChild(simultaneousScene);

		GetTree().Root.RemoveChild(this);
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
