using Godot;
using System;
using System.Collections.Generic;

public partial class Main_blocks : Node3D
{
    List<Stacker> Players = new List<Stacker>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Players.Add((Stacker)FindChild("Stacker1"));
        Players.Add((Stacker)FindChild("Stacker2"));
        Players.Add((Stacker)FindChild("Stacker3"));
        Players.Add((Stacker)FindChild("Stacker4"));

        int pNr = 1;
        foreach (var p in Players)
        {
            p.RowFinishEvent += P_RowFinishEvent;

            p.PlayerNr = pNr;

            if (p.PlayerNr == 1)
                p.PlayerType = PlayerTypeEnum.HUMAN;

            if (p.PlayerNr == 2)
                p.PlayerType = PlayerTypeEnum.Ki1;

            if (p.PlayerNr == 3)
                p.PlayerType = PlayerTypeEnum.Ki2;

            if (p.PlayerNr == 4)
                p.PlayerType = PlayerTypeEnum.Ki3;

            p.AddRow();
            p.GameState = StateEnum.RUNNING;

            pNr++;
        }
    }

    private void P_RowFinishEvent(object source, EventArgs e)
    {
        foreach (var p in Players)
        {
            if(p != source && p.GameState == StateEnum.RUNNING)
            {
                p.AddRow();
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {

    }


}
