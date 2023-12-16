using Flax.Build;

public class SLTerrainEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add("SLTerrain");
        Modules.Add("SLTerrainEditor");
    }
}
