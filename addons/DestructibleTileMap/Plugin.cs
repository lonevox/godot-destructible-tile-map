#if TOOLS
using Godot;
using System;

[Tool]
public partial class Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
		var tileBodyScript = GD.Load<Script>("res://addons/DestructibleTileMap/TileBody.cs");
        AddCustomType("TileBody", "RigidBody2D", tileBodyScript, null);
    }

    public override void _ExitTree()
    {
		RemoveCustomType("TileBody");
    }
}
#endif
