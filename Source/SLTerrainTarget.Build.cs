﻿using Flax.Build;

public class SLTerrainTarget : GameProjectTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for game
        Modules.Add("SLTerrain");
    }
}
