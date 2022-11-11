using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class TileBody : RigidBody2D
{
    private TileMap tileMap;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ChildEnteredTree += (node) => OnChildEnteredTree(node);
        ChildExitingTree += (node) => OnChildExitingTree(node);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        // Add all RigidBody2D warnings besides the collision shape one.
        var rigidBody2DWarnings = base._GetConfigurationWarnings();
        if (rigidBody2DWarnings != null) warnings.AddRange(rigidBody2DWarnings);
        // BUG For some reason the below Remove doesnt work. It seems like the warning is added after this method is called.
        warnings.Remove("This node has no shape, so it can't collide or interact with other objects.\nConsider adding a CollisionShape2D or CollisionPolygon2D as a child to define its shape.");

        var tileMapChild = GetChildren()
            .Where(child => child is TileMap)
            .Select(tileMap => tileMap as TileMap)
            .FirstOrDefault();
        // Add warning for not having a TileMap child.
        if (tileMapChild == null)
        {
            warnings.Add("This node has no TileMap, so it can't be properly physically simulated. Without a TileMap, a TileBody acts the same as a RigidBody2D. Consider adding a TileMap as a child.");
        }
        else
        {
            // Add warning if the TileMap doesn't have the required layers in it's TileSet.
            var tileSet = tileMapChild.TileSet;
            if (tileSet?.GetCustomDataLayerByName("Mass") == -1)
            {
                warnings.Add("Child TileMap '" + tileMapChild.Name + "' needs a custom layer called 'Mass' in it's TileSet to function correctly.");
            }
            if (tileSet?.GetCustomDataLayerByName("Strength") == -1)
            {
                warnings.Add("Child TileMap '" + tileMapChild.Name + "' needs a custom layer called 'Strength' in it's TileSet to function correctly.");
            }
        }

        return warnings.ToArray();
    }


    private void OnChildEnteredTree(Node node)
    {
        // Record TileMap.
        if (tileMap == null && node is TileMap)
        {
            tileMap = node as TileMap;
        }
    }

    private void OnChildExitingTree(Node node)
    {
        // Remove tileMap if this child was the TileMap.
        if (node != null && node == tileMap)
        {
            tileMap = null;
        }
    }
}
