using Godot;
using System;
using System.Collections.Generic;

public partial class Ki : Node3D
{
	double Speed = 1;
    double last_tick = 0;

    Stacker stacker;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        stacker = GetParent<Stacker>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (stacker != null && (stacker.PlayerType == PlayerTypeEnum.NONE || stacker.GameState != StateEnum.RUNNING))
            return;

        if (last_tick + delta > Speed)
        {
            KiTick();
            last_tick = 0;
        }
        else
            last_tick += delta;
	}

    internal void InitKI(PlayerTypeEnum playerType)
    {
        switch(playerType)
		{
			case PlayerTypeEnum.Ki1:
				Speed = 1;
				break;

            case PlayerTypeEnum.Ki2:
                Speed = 0.75;
                break;

            case PlayerTypeEnum.Ki3:
                Speed = 0.5;
                break;

            default:
                Speed = 1;
                break;
        }
    }

    void KiTick()
    {
        if (stacker.PlayerType == PlayerTypeEnum.Ki1)
            KI1();

        if (stacker.PlayerType == PlayerTypeEnum.Ki2)
            KI2();

        if (stacker.PlayerType == PlayerTypeEnum.Ki3)
            KI1();
    }

    void KI1()
	{
        int direction = Settings.R.Next(3);

        if (direction == 0)
            stacker.RotateBlock();

        if (direction == 1)
            stacker.CheckAndMove(new Vector3(1, 0, 0));

        if (direction == 2)
            stacker.CheckAndMove(new Vector3(-1, 0, 0));
    }

    void KI2()
    {
        Dictionary<int, bool> bricks = GetHighestRow();
        List<Dictionary<int, bool>> blocks = GetBlockRotations();


        KI1();
    }

    void KI3()
    {

    }

    Dictionary<int, bool> GetHighestRow()
    {
        Dictionary<int, bool> bricks = new Dictionary<int, bool>();

        for(int y = Stacker.maxRows; y >= 0; y++)
        {
            bool found = false;
            for (int x = Stacker.minCols; x <= Stacker.maxCols; x++)
            {
                if (stacker.Grid[new Vector3(x, y, 0)] != null)
                    found = true;

            }

            if(found || y == 0)
            {
                for (int x = Stacker.minCols; x <= Stacker.maxCols; x++)
                {
                    bool hasBrick = stacker.Grid[new Vector3(x, y, 0)] != null;
                    bricks.Add(x, hasBrick);
                }
                break;
            }
        }
        return bricks;
    }

    List<Dictionary<int, bool>> GetBlockRotations()
    {
        List<Dictionary<int, bool>> blocks = new List<Dictionary<int, bool>>();

        for(int r = 0; r < 3; r++)
        {
            var children = stacker.CurrentBlock.GetChildren();
            foreach (Node3D c in children)
            {
            }
            stacker.RotateBlock();
        }
        stacker.RotateBlock();
        return blocks;
    }
}
