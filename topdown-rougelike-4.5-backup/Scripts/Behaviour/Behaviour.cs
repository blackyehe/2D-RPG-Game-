using Godot;
using System;

public abstract partial class Behaviour : Resource
{
    public Enemy ownerEntity;
    public abstract void DecideAction();
}
