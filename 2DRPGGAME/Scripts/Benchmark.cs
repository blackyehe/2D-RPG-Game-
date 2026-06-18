using System;
using System.Reflection;
using System.Diagnostics;
using Godot;
using MethodDecorator.Fody.Interfaces;

[AttributeUsage(AttributeTargets.Method)]
public sealed class BenchmarkAttribute : Attribute, IMethodDecorator
{
    private long _timeStamp;
    private string _methodName;

    public void Init(object instance, MethodBase method, object[] args)
    {
        _methodName = method.Name;
    }

    public void OnEntry()
    {
        _timeStamp = Stopwatch.GetTimestamp();
    }

    public void OnException(Exception exception)
    {
        return;
    }

    public void OnExit()
    {
        GD.Print($"{_methodName}: {Stopwatch.GetElapsedTime(_timeStamp).TotalMilliseconds} ms");
    }
}