using System;
using FlaxEngine;

namespace SLTerrain;

public class MeshGenerator : Script
{
    [Header("Mesh Settings")] [Range(2, 511)]
    public int MapSize = 511;

    public HeightMapGenerator.GenerationMode Mode = HeightMapGenerator.GenerationMode.SL;
    public float Scale = 20;
    public float ElevationScale = 10;
    public Material TerrainMaterial;

    [Header("Erosion Settings")] public int NumErosionIterations = 50000;

    [Header("Animation Settings")] public bool AnimateErosion;
    public int IterationsPerFrame = 100;
    public bool ShowNumIterations;
    public int NumAnimatedErosionIterations { get; private set; }

    private float[] _map;
    private Mesh _mesh;
    private Erosion _erosion;
    private float _lastUpdateTime;

    public override void OnStart()
    {
        _erosion = Actor.FindScript<Erosion>();
        //StartMeshGeneration ();
        Erode();
    }

    public void StartMeshGeneration()
    {
        _map = Actor.FindScript<HeightMapGenerator>().Generate(MapSize);
        GenerateMesh();
    }

    public void Erode()
    {
        _map = Actor.FindScript<HeightMapGenerator>().Generate(MapSize, Mode);
        SaveHeightMap("HeightMapRaw");
        _erosion = Actor.FindScript<Erosion>();
        _erosion.Erode(_map, MapSize, NumErosionIterations, true);
        GenerateMesh();
        SaveHeightMap("HeightMapEroded");
    }

    public override void OnUpdate()
    {
        if (AnimateErosion)
        {
            for (var i = 0; i < IterationsPerFrame; i++)
            {
                _erosion.Erode(_map, MapSize);
            }

            NumAnimatedErosionIterations += IterationsPerFrame;
            if (Time.GameTime >= _lastUpdateTime + 2f)
            {
                GenerateMesh();
                _lastUpdateTime = Time.GameTime;
            }
        }
    }

    private void SaveHeightMap(string name)
    {
        var heightmap = Content.CreateVirtualAsset<Texture>();
        var mips = new TextureBase.InitData.MipData();
        var size = MapSize * MapSize * PixelFormatExtensions.SizeInBytes(PixelFormat.R32_Float);
        mips.RowPitch = size / MapSize;
        mips.SlicePitch = size;
        mips.Data = new byte[size];
        ValidateMap();
        Buffer.BlockCopy(_map, 0, mips.Data, 0, size);
        var initData = new TextureBase.InitData
        {
            Format = PixelFormat.R32_Float,
            Height = MapSize,
            Width = MapSize,
            Mips = new[] { mips },
            ArraySize = 1,
            GenerateMips = false,
            GenerateMipsLinear = false
        };
        heightmap.Init(ref initData);
        heightmap.Save(Globals.ProjectContentFolder + "/" + name + ".flax");
    }

    void ValidateMap()
    {
        var min = float.MaxValue;
        var max = float.MinValue;
        var invalidPixels = 0;

        for (var i = 0; i < MapSize * MapSize; i++)
        {
            if (_map[i] < 0 || float.IsNaN(_map[i]) || float.IsInfinity(_map[i]))
            {
                _map[i] = 0;
                invalidPixels++;
            }
            min = Mathf.Min(min, _map[i]);
            max = Mathf.Max(max, _map[i]);
        }
        Debug.Log($"_map values are in the range {min} to {max} (found {invalidPixels} invalid pixels)");
    }

    void GenerateMesh()
    {
        var verts = new Float3[MapSize * MapSize];
        var triangles = new int[(MapSize - 1) * (MapSize - 1) * 6];
        var t = 0;

        for (var y = 0; y < MapSize; y++)
        {
            for (var x = 0; x < MapSize; x++)
            {
                var i = y * MapSize + x;

                var percent = new Float2(x / (MapSize - 1f), y / (MapSize - 1f));
                var pos = new Float3(percent.X * 2 - 1, 0, percent.Y * 2 - 1) * Scale;
                pos += Vector3.Up * _map[i] * ElevationScale;
                verts[i] = pos;

                // Construct triangles
                if (x != MapSize - 1 && y != MapSize - 1)
                {
                    triangles[t + 0] = i + MapSize;
                    triangles[t + 1] = i + MapSize + 1;
                    triangles[t + 2] = i;

                    triangles[t + 3] = i + MapSize + 1;
                    triangles[t + 4] = i + 1;
                    triangles[t + 5] = i;
                    t += 6;
                }
            }
        }

        if (_mesh == null)
        {
            var model = Content.CreateVirtualAsset<Model>();
            model.SetupLODs(new[] { 1 });
            _mesh = model.LODs[0].Meshes[0];
            var modelActor = Actor.GetOrAddChild<StaticModel>();
            modelActor.Model = model;
            modelActor.LocalScale = Vector3.One;
            modelActor.SetMaterial(0, TerrainMaterial.CreateVirtualInstance());
        }

        _mesh.UpdateMesh(verts, triangles);
        //material.SetFloat ("_MaxHeight", elevationScale);
    }
}