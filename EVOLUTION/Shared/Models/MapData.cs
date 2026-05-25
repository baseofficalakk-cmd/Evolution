using System;

namespace EVOLUTION.Shared.Models
{
    /// <summary>
    /// Map data model with procedural generation config
    /// </summary>
    [Serializable]
    public class MapData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public GameMode[] SupportedModes { get; set; }
        
        // Dimensions
        public float Width { get; set; }
        public float Height { get; set; }
        
        // Procedural generation config
        public int WallBlockCount { get; set; }
        public int BushZoneCount { get; set; }
        public int DecorationCount { get; set; }
        public int SpawnPointCount { get; set; }
        
        // Generated elements
        public WallBlock[] Walls { get; set; }
        public BushZone[] Bushes { get; set; }
        public SpawnPoint[] SpawnPoints { get; set; }
        public Decoration[] Decorations { get; set; }
        
        // Navigation
        public NavMeshData NavMesh { get; set; }
        
        // Visual theme
        public string Theme { get; set; }
        public string SkyboxPath { get; set; }
        public string AmbientMusicPath { get; set; }
        
        public MapData()
        {
            Id = Guid.NewGuid().ToString();
            Width = GameConstants.MAP_WIDTH;
            Height = GameConstants.MAP_HEIGHT;
            WallBlockCount = 20;
            BushZoneCount = 15;
            SpawnPointCount = 12;
            SupportedModes = Array.Empty<GameMode>();
            Walls = Array.Empty<WallBlock>();
            Bushes = Array.Empty<BushZone>();
            SpawnPoints = Array.Empty<SpawnPoint>();
            Decorations = Array.Empty<Decoration>();
        }
    }

    /// <summary>
    /// Wall block obstacle
    /// </summary>
    [Serializable]
    public class WallBlock
    {
        public Vector2Float Position { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Rotation { get; set; }
        public bool IsDestructible { get; set; }
        public int Health { get; set; }
        public string ModelPath { get; set; }
    }

    /// <summary>
    /// Bush zone for stealth mechanics
    /// </summary>
    [Serializable]
    public class BushZone
    {
        public Vector2Float Center { get; set; }
        public float Radius { get; set; }
        public string BushType { get; set; }
        public bool IsVisible { get; set; } // False = hidden until player enters
    }

    /// <summary>
    /// Spawn point for players
    /// </summary>
    [Serializable]
    public class SpawnPoint
    {
        public Vector2Float Position { get; set; }
        public TeamId Team { get; set; } // None = neutral (Battle Royale)
        public bool IsProtected { get; set; }
        public float ProtectionDuration { get; set; }
    }

    /// <summary>
    /// Environmental decoration
    /// </summary>
    [Serializable]
    public class Decoration
    {
        public Vector2Float Position { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public string ModelPath { get; set; }
        public DecorationType Type { get; set; }
        public bool IsCollidable { get; set; }
    }

    /// <summary>
    /// Decoration types
    /// </summary>
    public enum DecorationType
    {
        Tree = 0,
        Rock = 1,
        Building = 2,
        Prop = 3,
        Light = 4,
        Water = 5
    }

    /// <summary>
    /// Navigation mesh data
    /// </summary>
    [Serializable]
    public class NavMeshData
    {
        public Vector2Float[] Vertices { get; set; }
        public int[] Triangles { get; set; }
        public float CellSize { get; set; }
        public float AgentRadius { get; set; }
        public float AgentHeight { get; set; }
        public float MaxSlope { get; set; }
    }

    /// <summary>
    /// Map generation seed and parameters
    /// </summary>
    [Serializable]
    public class MapGenerationParams
    {
        public int Seed { get; set; }
        public float WallDensity { get; set; }
        public float BushDensity { get; set; }
        public float OpenSpaceRatio { get; set; }
        public bool SymmetricLayout { get; set; }
        public int LaneCount { get; set; }
        public bool HasRiver { get; set; }
        public bool HasJungle { get; set; }
        
        public MapGenerationParams()
        {
            Seed = UnityEngine.Random.Range(0, int.MaxValue);
            WallDensity = 0.3f;
            BushDensity = 0.2f;
            OpenSpaceRatio = 0.4f;
            SymmetricLayout = true;
            LaneCount = 3;
        }
    }
}

// Unity fallback for non-Unity environments
#if !UNITY_5_3_OR_NEWER
namespace UnityEngine
{
    public static class Random
    {
        private static System.Random _random = new System.Random();
        
        public static int Range(int min, int max)
        {
            return _random.Next(min, max);
        }
        
        public static float Range(float min, float max)
        {
            return (float)(_random.NextDouble() * (max - min) + min);
        }
    }
}
#endif
