using Godot;
using Godot.Collections;
using System;

public delegate void RowFinishEventHandler(object source, EventArgs e);
public enum StateEnum { NONE, RUNNING, BREAK, END };


public partial class Stacker : Node3D
{
    public StateEnum GameState = StateEnum.NONE;

    public static int maxRows = 20;
    public static int minCols = -5;
    public static int maxCols = 4;

    public double last_tick = 0;
    public static double Speed = 0.5;

    public Block CurrentBlock = null;
    public Block NextBlock = null;

    public Ki _KI;

    public Dictionary<Vector3, Node3D> Grid = new Dictionary<Vector3, Node3D>();

    public PlayerTypeEnum _playerType = PlayerTypeEnum.NONE;
    public PlayerTypeEnum PlayerType
    {  get { return _playerType; } 
        set { 
            _playerType = value;
            if (_playerType == PlayerTypeEnum.Ki1 || _playerType == PlayerTypeEnum.Ki1 || _playerType == PlayerTypeEnum.Ki1)
            {
                _KI = (Ki)FindChild("KI");
                _KI.InitKI(PlayerType);
            }
            else
                _KI = null;
        }
    }

    int _playerNr = -1;
    public int PlayerNr
    {
        get { return _playerNr; }
        set { 
            _playerNr = value;
            KeyUp = $"P{_playerNr}_UP";
            KeyDown = $"P{_playerNr}_DOWN";
            KeyLeft = $"P{_playerNr}_LEFT";
            KeyRight = $"P{_playerNr}_RIGHT";
        }
    }

    public string KeyUp;
    public string KeyDown;
    public string KeyLeft;
    public string KeyRight;

    public event RowFinishEventHandler RowFinishEvent;

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
	{
        for (int y = 0; y < maxRows; y++)
        {
            for (int x = minCols; x <= maxCols; x++)
            {
                Grid.Add(new Vector3(x, y, 0), null);
            }
        }

        MeshInstance3D m = new MeshInstance3D();
        ImmediateMesh im = new ImmediateMesh();

        m.Mesh = im;

        m.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

        OrmMaterial3D material = new OrmMaterial3D();


        for (int i = minCols; i <= maxCols; i++)
        {
            im.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
            im.SurfaceAddVertex(new Vector3(i -0.5f, 19.5f, -0.5f));
            im.SurfaceAddVertex(new Vector3(i - 0.5f, -0.5f, -0.5f));
            im.SurfaceEnd();
        }

        for (int i = 1; i < maxRows; i++)
        {
            im.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
            im.SurfaceAddVertex(new Vector3(-5.5f, i - 0.5f, -0.5f));
            im.SurfaceAddVertex(new Vector3(4.5f, i - 0.5f, -0.5f));
            im.SurfaceEnd();


            im.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
            im.SurfaceAddVertex(new Vector3(-5.5f, i - 0.5f, -0.5f));
            im.SurfaceAddVertex(new Vector3(-5.5f, i - 0.5f, 0.5f));
            im.SurfaceEnd();

            im.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
            im.SurfaceAddVertex(new Vector3(4.5f, i - 0.5f, -0.5f));
            im.SurfaceAddVertex(new Vector3(4.5f, i - 0.5f, 0.5f));
            im.SurfaceEnd();
        }


        Color c = Colors.WhiteSmoke;
        c.A = 0.5f;

        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = c;
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;

        AddChild(m);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        if(PlayerType == PlayerTypeEnum.NONE || GameState != StateEnum.RUNNING)
                return;

        if(NextBlock == null)
        {
            PackedScene scene = (PackedScene)ResourceLoader.Load("res://games/blocks/block.tscn");
            Block n = scene.Instantiate<Block>();
            n.Position = new Vector3(0, 22, 0);
            NextBlock = n;
            AddChild(n);
        }
        else if (CurrentBlock == null)
        {
            CurrentBlock = NextBlock;
            CurrentBlock.Position = new Vector3(0, maxRows-1, 0);
            NextBlock = null;
        }
        else
        {
            last_tick += delta;
            if(last_tick >= Speed)
            {
                last_tick = 0;

                if (!CheckAndMove(new Vector3(0, -1, 0)))
                {
                    GridAddBlock();
                    CurrentBlock = null;
                }

            }
        }

        CheckfinishRows();
    }


    public override void _Input(InputEvent @event)
    {
        if (GameState != StateEnum.RUNNING || CurrentBlock == null)
            return;

        if(Input.IsActionPressed(KeyUp))
            RotateBlock();

        if (Input.IsActionPressed(KeyRight))
            CheckAndMove(new Vector3(1, 0, 0));

        if (Input.IsActionPressed(KeyLeft))
            CheckAndMove(new Vector3(-1, 0, 0));

        if (Input.IsActionPressed(KeyDown))
            CheckAndMove(new Vector3(0, -1, 0));
    }



    private void GridAddBlock()
    {
        Vector3 p = CurrentBlock.Position;

        var children = CurrentBlock.GetChildren();

        foreach (Node3D child in children)
        {
            Vector3 pos = p + child.Position;

            if (pos.Y < maxRows)
            {
                Grid[pos] = child;
            }
            else
            {
                //TODO: EVENT GAME OVER
                //OS.Alert("GAME OVER");
                GameState = StateEnum.END;
            }
        }

    }

    public bool CheckAndMove(Vector3 v)
    {
        Vector3 p = CurrentBlock.Position;
        var children = CurrentBlock.GetChildren();

        foreach (Node3D child in children)
        {
            Vector3 pos = p + child.Position + v;
            if (!CheckPosition(pos))
                return false;
        }

        CurrentBlock.Position += v;

        return true;
    }

    public bool CheckPosition(Vector3 v)
    {
        int y = ((int)v.Y);
        var x = ((int)v.X);

        return y >= 0 && y < maxRows
            && x >= minCols && x <= maxCols
            && Grid[v] == null;
    }

    public void CheckfinishRows()
    {
        for (int y = 0; y < maxRows; y++)
        {
            bool complete = true;
            for(int x = minCols; x <= maxCols; x++)
            {
                if (Grid[new Vector3(x, y, 0)] == null)
                {
                    complete = false;
                    continue;
                }
            }

            if(complete)
            {
                //TODO: EVENT FINISH
                Stacker.Speed *= 0.9;

                //Nodes Löschen
                for (int x = minCols; x <= maxCols; x++)
                {
                    Vector3 vDel = new Vector3(x, y, 0);

                    Node3D nDel = Grid[vDel];
                    Grid[vDel] = null;
                    nDel.QueueFree();
                }


                for (int yMove = y; yMove < maxRows -1 ; yMove++)
                {
                    for (int x = minCols; x <= maxCols; x++)
                    {
                        Vector3 target = new Vector3(x, yMove, 0);
                        Vector3 source = new Vector3(x, yMove + 1, 0);

                        Node3D b = Grid[source];
                        if (b != null)
                        {
                            b.Position += new Vector3(0, -1, 0);
                            Grid[target] = Grid[source];
                        }
                        else
                            Grid[target] = null;
                    }
                }

                if(RowFinishEvent != null)
                    RowFinishEvent(this, EventArgs.Empty);
            }
        }
    }


    public void AddRow()
    {
        for (int y = maxRows-2; y > 0 - 1; y--)
        {
            for (int x = minCols; x <= maxCols; x++)
            {
                Vector3 target = new Vector3(x, y + 1, 0);
                Vector3 source = new Vector3(x, y, 0);

                Node3D b = Grid[source];
                if (b != null)
                {
                    b.Position += new Vector3(0, + 1, 0);
                    Grid[target] = Grid[source];
                }
                else
                    Grid[target] = null;
            }
        }


        int spaceCol = Settings.R.Next(minCols, maxCols);

        for (int x = minCols; x <= maxCols; x++)
        {
            Vector3 pos = new Vector3(x, 0, 0);

            if (x == spaceCol)
            {
                Grid[pos] = null;
                continue;
            }

            Node3D b = Block.GetRandomBrick();
            b.Position = pos;
            AddChild(b);
            Grid[pos] = b;
        }
    }


    public void RotateBlock()
    {
        if (CurrentBlock.Shape == BlockShape.O)
            return;

        var children = CurrentBlock.GetChildren();
        foreach (Node3D c in children)
        {
            int x = (int)c.Position.X;
            int y = (int)c.Position.Y;

            int xNew = x;
            int yNew = y;

            if (x == 0 && y == 0)
                continue;

            xNew = y;

            if (x == 0)
                yNew = 0;
            else
                yNew = x * -1;

            c.Position = new Vector3(xNew, yNew, 0);
        }
    }
}