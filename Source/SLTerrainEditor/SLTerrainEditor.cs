using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Editors;
using FlaxEngine;
using SLTerrain;

namespace SLTerrainEditor;

/// <summary>
/// SLTerrainEditor Script.
/// </summary>
[CustomEditor(typeof(MeshGenerator))]
public class SLTerrainEditor : GenericEditor
{
    public override void Initialize(LayoutElementsContainer layout)
    {
        var meshGenerator = Values[0] as MeshGenerator;
        base.Initialize(layout);
        var button = layout.Button("Create Terrain");
        button.Button.Clicked += () => { UpdateTerrain(meshGenerator);};
    }

    public void UpdateTerrain(MeshGenerator meshGenerator)
    {
        meshGenerator?.Erode();
    }
}
