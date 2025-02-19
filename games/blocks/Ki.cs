using Godot;
using System;
using System.Collections.Generic;

public partial class Ki : Node3D
{
	double Speed = 1;
    double last_tick = 0;

    Stacker stacker;

    public bool _isCalced = false;
    int _r = 0;
    int _x = 0;

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

        if (stacker.CurrentBlock == null)
            _isCalced = false;

        if (stacker.CurrentBlock != null && last_tick + delta > Speed)
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
        if (!_isCalced)
        {
            Dictionary<int, int> bricks = GetHighestRow();
            List<Dictionary<int, int>> blocks = GetBlockRotations();

            int best = -1000;
            int bestX = 0;
            int bestR = 0;

            int r = 0;

            string bestTmp = "";
            //Rotation
            foreach (Dictionary<int, int> pair in blocks)
            {
                //x Position des Blockes
                for (int x = Stacker.minCols; x <= Stacker.maxCols; x++)
                {
                    int value = 0;
                    string tmp = "";
                    bool blockFits = stacker.Check(new Vector3(x, 0, 0));

                    //Zeile auslesen mit col als x pos
                    for (int col = Stacker.minCols; col <= Stacker.maxCols; col++)
                    {
                        int v = bricks[col];

                        if (blockFits)
                        {
                            foreach (KeyValuePair<int, int> p in pair)
                            {
                                if (p.Key + x == col && p.Value > 0)
                                {
                                    v = v > 0 ? 0 : 9;
                                }
                            }
                        }

                        tmp += v.ToString();

                        value += v;
                    }

                    if (!blockFits)
                        value = -100;
                    //GD.Print("R: " + r + " X: " + x + " ==> " + value);

                    if (value >= best)
                    {
                        best = value;
                        bestX = x;
                        bestR = r;
                        bestTmp = tmp;
                    }
                }

                r++;
            }

            GD.Print(bestTmp);
            GD.Print("R: " + bestR + " X: " + bestX + " ==> " + best);

            _r = bestR;
            _x = bestX;

            _isCalced = true;
        }
        
        if (_r > 0)
        {
            stacker.RotateBlock();
            _r--;
        }
        else if (_x != 0)
        {
            int xMove = _x > 0 ? 1 : 0;

            Vector3 vMove = new Vector3(xMove, 0, 0);
            stacker.CheckAndMove(vMove);
            _x += (xMove * -1);
        }
    }

    void KI3()
    {

    }

    

    Dictionary<int, int> GetHighestRow()
    {
        Dictionary<int, int> bricks = new Dictionary<int, int>();

        for(int y = Stacker.maxRows-1; y >= 0; y--)
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
                    int hasBrick = stacker.Grid[new Vector3(x, y, 0)] != null ? 2 : 0;
                    bricks.Add(x, hasBrick);
                }
                break;
            }
        }
        return bricks;
    }

    List<Dictionary<int, int>> GetBlockRotations()
    {
        List<Dictionary<int, int>> blocks = new List<Dictionary<int, int>>();

        for(int r = 0; r < 4; r++)
        {
            var children = stacker.CurrentBlock.GetChildren();

            for(int y = -1; y <= 1; y++)
            {
                Dictionary<int, int> block = new Dictionary<int, int>();

                bool found = false;

                for (int x = -1; x <= 1; x++)
                {
                    Vector3 v = new Vector3(x, y, 0);
                    block.Add(x, 0);

                    foreach (Node3D c in children)
                    {
                        //GD.Print(v + " => " + c.Position);

                        if (c.Position == v)
                        {
                            block[x] = 1;
                            found = true;
                        }
                    }                    
                }

                if(found)
                {
                    blocks.Add(block);
                    break;
                }
            }
            stacker.RotateBlock();
        }
        return blocks;
    }
}
