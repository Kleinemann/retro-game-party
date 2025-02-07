using Godot;
using System;

public enum BlockShape { O, L, L2, Z, Z2, I, T };

public partial class Block : Node3D
{
    public BlockShape Shape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //Init Shape
        string[] names = Enum.GetNames(typeof(BlockShape));
        Shape = (BlockShape)Enum.Parse(typeof(BlockShape), names[Settings.R.Next(names.Length)]);

        //Init Material
        StandardMaterial3D mat = GetRandomMaterial();

        switch (Shape)
        {
            case BlockShape.O:
                AddBrick(mat, new Vector3I(0, 0, 0));
                AddBrick(mat, new Vector3I(1, 0, 0));
                AddBrick(mat, new Vector3I(0, 1, 0));
                AddBrick(mat, new Vector3I(1, 1, 0));
                break;
            case BlockShape.L:
                AddBrick(mat, new Vector3I(0, 0, 0));
                AddBrick(mat, new Vector3I(-1, 0, 0));
                AddBrick(mat, new Vector3I(1, 1, 0));
                AddBrick(mat, new Vector3I(1, 0, 0));
                break;
            case BlockShape.L2:
                AddBrick(mat, new Vector3I(0, 0, 0));
                AddBrick(mat, new Vector3I(1, 0, 0));
                AddBrick(mat, new Vector3I(-1, 0, 0));
                AddBrick(mat, new Vector3I(-1, 1, 0));
                break;
            case BlockShape.Z:
                AddBrick(mat, new Vector3I(0, 0, 0));
                AddBrick(mat, new Vector3I(-1, 0, 0));
                AddBrick(mat, new Vector3I(0, 1, 0));
                AddBrick(mat, new Vector3I(1, 1, 0));
                break;
            case BlockShape.Z2:
                AddBrick(mat, new Vector3I(0, 0, 0));
                AddBrick(mat, new Vector3I(1, 0, 0));
                AddBrick(mat, new Vector3I(0, 1, 0));
                AddBrick(mat, new Vector3I(-1, 1, 0));
                break;
            case BlockShape.I:
                AddBrick(mat, new Vector3I(0, 0, 0));
                AddBrick(mat, new Vector3I(-1, 0, 0));
                AddBrick(mat, new Vector3I(1, 0, 0));
                break;
            case BlockShape.T:
                AddBrick(mat, new Vector3I(0, 0, 0));
                AddBrick(mat, new Vector3I(-1, 0, 0));
                AddBrick(mat, new Vector3I(1, 0, 0));
                AddBrick(mat, new Vector3I(0, 1, 0));
                break;
        }
    }


    void AddBrick(StandardMaterial3D mat, Vector3I v)
    {
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://games/blocks/brick.tscn");
        Node3D b = scene.Instantiate<Node3D>();
        b.Position = v;
        b.GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = mat;
        AddChild(b);
    }


    public static StandardMaterial3D GetRandomMaterial()
	{
        int nr = Settings.R.Next(1, 8);

        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoTexture = GD.Load<Texture2D>($"res://games/blocks/assets/b{nr}.png");
        material.Uv1Scale = new Vector3(3, 2, 1);
		return material;
    }


    public static Node3D GetRandomBrick()
    {
        StandardMaterial3D mat = GetRandomMaterial();
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://games/blocks/brick.tscn");
        Node3D b = scene.Instantiate<Node3D>();
        b.GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = mat;

        return b;
    }
}
