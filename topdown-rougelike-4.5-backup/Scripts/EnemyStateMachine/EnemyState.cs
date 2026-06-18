using Godot;
using System;

public abstract class EnemyState
{
    public Enemy Enemy; //thumbsup

    public abstract void PhysicsProcess(double delta);

    public abstract void EnterState();

    public abstract void ExitState();
}

