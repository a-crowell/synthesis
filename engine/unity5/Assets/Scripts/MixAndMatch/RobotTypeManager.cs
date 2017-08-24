﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RobotTypeManager{
    public static bool IsMixAndMatch;
    public static bool HasManipulator;
    public static bool IsMecanum;

    public static string RobotPath;
    public static string ManipulatorPath;
    public static string WheelPath;

    public static float WheelMass;
    public static float WheelRadius;
    public static float WheelFriction;

    public static void SetProperties (bool isMixAndMatch, bool hasManipulator, bool isMecanum  )
    {
        IsMixAndMatch = isMixAndMatch;
        HasManipulator = hasManipulator;
        IsMecanum = isMecanum; 
    }

    public static void SetProperties(bool isMixAndMatch)
    {
        IsMixAndMatch = isMixAndMatch;
        HasManipulator = false;
        IsMecanum = false;
    }

    public static void SetWheelProperties(float mass, float radius, float friction)
    {
        WheelMass = mass;
        WheelRadius = radius;
        WheelFriction = friction;
    }
}