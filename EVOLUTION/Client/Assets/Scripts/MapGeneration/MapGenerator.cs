using System.Collections.Generic;
using UnityEngine;

namespace Evolution.MapGeneration
{
    /// <summary>
    /// Процедурная генерация карт для EVOLUTION
    /// Карта: 2400x1600 единиц
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {
        [Header("Map Settings")]
        [SerializeField] private int mapWidth = 2400;
        [SerializeField] private int mapHeight = 1600;
        [SerializeField] private int wallCount = 20;
        [SerializeField] private int bushCount = 15;
        [SerializeField] private int spawnPointCount = 12;
        
        [Header("Wall Settings")]
        [SerializeField] private float minWallSize = 3f;
        [SerializeField] private float maxWallSize = 8f;
        [SerializeField] private float wallHeight = 3f;
        
        [Header("Bush Settings")]
        [SerializeField] private float minBushRadius = 2f;
        [SerializeField] private float maxBushRadius = 5f;
        
        [Header("Decoration Settings")]
        [SerializeField] private int decorationCount = 30;
        [SerializeField] private GameObject[] decorationPrefabs;
        
        [Header("References")]
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject bushPrefab;
        [SerializeField] private GameObject spawnPointPrefab;
        
        // Сгенерированные данные
        private List<Vector3> generatedWalls = new List<Vector3>();
        private List<Vector3> generatedBushes = new List<Vector3>();
        private List<Vector3> generatedSpawnPoints = new List<Vector3>();
        private List<Vector3> generatedDecorations = new List<Vector3>();
        
        public List<Vector3> GeneratedWalls => generatedWalls;
        public List<Vector3> GeneratedBushes => generatedBushes;
        public List<Vector3> GeneratedSpawnPoints => generatedSpawnPoints;
        
        /// <summary>
        /// Генерация всей карты
        /// </summary>
        public void GenerateMap()
        {
            ClearExistingMap();
            
            GenerateBoundaries();
            GenerateWalls();
            GenerateBushes();
            GenerateSpawnPoints();
            GenerateDecorations();
            
            Debug.Log($"Map generated: {mapWidth}x{mapHeight}, Walls: {generatedWalls.Count}, Bushes: {generatedBushes.Count}, Spawns: {generatedSpawnPoints.Count}");
        }
        
        /// <summary>
        /// Очистка существующих объектов карты
        /// </summary>
        private void ClearExistingMap()
        {
            generatedWalls.Clear();
            generatedBushes.Clear();
            generatedSpawnPoints.Clear();
            generatedDecorations.Clear();
            
            // Удаление старых объектов сцены
            var existingWalls = GameObject.FindGameObjectsWithTag("Wall");
            foreach (var wall in existingWalls)
            {
                if (Application.isPlaying)
                    Destroy(wall);
                else
                    DestroyImmediate(wall);
            }
            
            var existingBushes = GameObject.FindGameObjectsWithTag("Bush");
            foreach (var bush in existingBushes)
            {
                if (Application.isPlaying)
                    Destroy(bush);
                else
                    DestroyImmediate(bush);
            }
            
            var existingSpawns = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (var spawn in existingSpawns)
            {
                if (Application.isPlaying)
                    Destroy(spawn);
                else
                    DestroyImmediate(spawn);
            }
        }
        
        /// <summary>
        /// Генерация границ карты
        /// </summary>
        private void GenerateBoundaries()
        {
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;
            
            // Создаем 4 граничные стены
            CreateWall(new Vector3(0, 0, -halfHeight), mapWidth, 1f); // Верх
            CreateWall(new Vector3(0, 0, halfHeight), mapWidth, 1f);  // Низ
            CreateWall(new Vector3(-halfWidth, 0, 0), 1f, mapHeight); // Лево
            CreateWall(new Vector3(halfWidth, 0, 0), 1f, mapHeight);  // Право
        }
        
        /// <summary>
        /// Генерация случайных стен
        /// </summary>
        private void GenerateWalls()
        {
            for (int i = 0; i < wallCount; i++)
            {
                Vector3 randomPos = GetRandomPositionInMap();
                
                // Проверяем, чтобы стена не перекрывала спавн точки
                if (!IsPositionNearSpawns(randomPos, 10f))
                {
                    float width = Random.Range(minWallSize, maxWallSize);
                    float height = Random.Range(minWallSize, maxWallSize);
                    
                    CreateWall(randomPos, width, height);
                }
            }
        }
        
        /// <summary>
        /// Создание стены
        /// </summary>
        private void CreateWall(Vector3 position, float width, float height)
        {
            if (wallPrefab != null)
            {
                GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                wall.tag = "Wall";
                wall.name = $"Wall_{generatedWalls.Count}";
                
                // Настраиваем масштаб
                wall.transform.localScale = new Vector3(width, wallHeight, height);
            }
            
            generatedWalls.Add(position);
        }
        
        /// <summary>
        /// Генерация кустов
        /// </summary>
        private void GenerateBushes()
        {
            for (int i = 0; i < bushCount; i++)
            {
                Vector3 randomPos = GetRandomPositionInMap();
                
                // Проверяем, чтобы куст не перекрывал спавн точки
                if (!IsPositionNearSpawns(randomPos, 8f))
                {
                    float radius = Random.Range(minBushRadius, maxBushRadius);
                    CreateBush(randomPos, radius);
                }
            }
        }
        
        /// <summary>
        /// Создание куста
        /// </summary>
        private void CreateBush(Vector3 position, float radius)
        {
            if (bushPrefab != null)
            {
                GameObject bush = Instantiate(bushPrefab, position, Quaternion.identity);
                bush.tag = "Bush";
                bush.name = $"Bush_{generatedBushes.Count}";
                
                // Настраиваем масштаб (круглый куст)
                bush.transform.localScale = new Vector3(radius * 2, 1f, radius * 2);
            }
            
            generatedBushes.Add(position);
        }
        
        /// <summary>
        /// Генерация точек спавна
        /// </summary>
        private void GenerateSpawnPoints()
        {
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;
            
            // Генерируем спавны по краям карты для команд
            int teamASpawns = spawnPointCount / 2;
            int teamBSpawns = spawnPointCount - teamASpawns;
            
            // Команда A (левая сторона)
            for (int i = 0; i < teamASpawns; i++)
            {
                Vector3 spawnPos = new Vector3(
                    -halfWidth + Random.Range(100f, 300f),
                    0,
                    Random.Range(-halfHeight + 100f, halfHeight - 100f)
                );
                
                CreateSpawnPoint(spawnPos, 0); // Team 0
            }
            
            // Команда B (правая сторона)
            for (int i = 0; i < teamBSpawns; i++)
            {
                Vector3 spawnPos = new Vector3(
                    halfWidth - Random.Range(100f, 300f),
                    0,
                    Random.Range(-halfHeight + 100f, halfHeight - 100f)
                );
                
                CreateSpawnPoint(spawnPos, 1); // Team 1
            }
        }
        
        /// <summary>
        /// Создание точки спавна
        /// </summary>
        private void CreateSpawnPoint(Vector3 position, int teamId)
        {
            if (spawnPointPrefab != null)
            {
                GameObject spawn = Instantiate(spawnPointPrefab, position, Quaternion.identity);
                spawn.tag = "SpawnPoint";
                spawn.name = $"Spawn_Team{teamId}_{generatedSpawnPoints.Count}";
                
                // Сохраняем teamId в компоненте или через PlayerPrefs для редактора
            }
            
            generatedSpawnPoints.Add(position);
        }
        
        /// <summary>
        /// Генерация декораций
        /// </summary>
        private void GenerateDecorations()
        {
            if (decorationPrefabs == null || decorationPrefabs.Length == 0)
                return;
            
            for (int i = 0; i < decorationCount; i++)
            {
                Vector3 randomPos = GetRandomPositionInMap();
                
                // Выбираем случайную декорацию
                GameObject prefab = decorationPrefabs[Random.Range(0, decorationPrefabs.Length)];
                
                GameObject decoration = Instantiate(prefab, randomPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                decoration.name = $"Decoration_{i}";
                
                generatedDecorations.Add(randomPos);
            }
        }
        
        /// <summary>
        /// Получение случайной позиции на карте
        /// </summary>
        private Vector3 GetRandomPositionInMap()
        {
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;
            
            return new Vector3(
                Random.Range(-halfWidth + 50f, halfWidth - 50f),
                0,
                Random.Range(-halfHeight + 50f, halfHeight - 50f)
            );
        }
        
        /// <summary>
        /// Проверка, находится ли позиция рядом со спавн точками
        /// </summary>
        private bool IsPositionNearSpawns(Vector3 position, float minDistance)
        {
            foreach (var spawn in generatedSpawnPoints)
            {
                if (Vector3.Distance(position, spawn) < minDistance)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Проверка линии обзора (для стрельбы)
        /// </summary>
        public bool HasLineOfSight(Vector3 from, Vector3 to)
        {
            Vector3 direction = (to - from).normalized;
            float distance = Vector3.Distance(from, to);
            
            RaycastHit hit;
            if (Physics.Raycast(from, direction, out hit, distance))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Поиск ближайшего куста
        /// </summary>
        public Vector3? FindNearestBush(Vector3 position, float maxDistance = 20f)
        {
            Vector3? nearest = null;
            float minDist = float.MaxValue;
            
            foreach (var bush in generatedBushes)
            {
                float dist = Vector3.Distance(position, bush);
                if (dist < minDist && dist <= maxDistance)
                {
                    minDist = dist;
                    nearest = bush;
                }
            }
            
            return nearest;
        }
        
        /// <summary>
        /// Экспорт данных карты в JSON
        /// </summary>
        public string ExportMapData()
        {
            MapData data = new MapData
            {
                width = mapWidth,
                height = mapHeight,
                walls = generatedWalls.ToArray(),
                bushes = generatedBushes.ToArray(),
                spawnPoints = generatedSpawnPoints.ToArray()
            };
            
            return JsonUtility.ToJson(data, true);
        }
        
        /// <summary>
        /// Импорт данных карты из JSON
        /// </summary>
        public void ImportMapData(string jsonData)
        {
            MapData data = JsonUtility.FromJson<MapData>(jsonData);
            
            mapWidth = data.width;
            mapHeight = data.height;
            
            generatedWalls = new List<Vector3>(data.walls);
            generatedBushes = new List<Vector3>(data.bushes);
            generatedSpawnPoints = new List<Vector3>(data.spawnPoints);
            
            // Воссоздаем объекты на сцене
            RecreateMapFromData();
        }
        
        private void RecreateMapFromData()
        {
            foreach (var wallPos in generatedWalls)
            {
                CreateWall(wallPos, Random.Range(minWallSize, maxWallSize), Random.Range(minWallSize, maxWallSize));
            }
            
            foreach (var bushPos in generatedBushes)
            {
                CreateBush(bushPos, Random.Range(minBushRadius, maxBushRadius));
            }
            
            // Для спавнов нужно сохранить teamId - упрощенная версия
            for (int i = 0; i < generatedSpawnPoints.Count; i++)
            {
                CreateSpawnPoint(generatedSpawnPoints[i], i % 2);
            }
        }
        
        private void OnDrawGizmos()
        {
            // Рисуем границы карты
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector3(-mapWidth/2, 0, -mapHeight/2), new Vector3(mapWidth/2, 0, -mapHeight/2));
            Gizmos.DrawLine(new Vector3(mapWidth/2, 0, -mapHeight/2), new Vector3(mapWidth/2, 0, mapHeight/2));
            Gizmos.DrawLine(new Vector3(mapWidth/2, 0, mapHeight/2), new Vector3(-mapWidth/2, 0, mapHeight/2));
            Gizmos.DrawLine(new Vector3(-mapWidth/2, 0, mapHeight/2), new Vector3(-mapWidth/2, 0, -mapHeight/2));
            
            // Рисуем стены
            Gizmos.color = Color.gray;
            foreach (var wall in generatedWalls)
            {
                Gizmos.DrawCube(wall, new Vector3(5, wallHeight, 5));
            }
            
            // Рисуем кусты
            Gizmos.color = Color.green;
            foreach (var bush in generatedBushes)
            {
                Gizmos.DrawSphere(bush, 3f);
            }
            
            // Рисуем спавн точки
            Gizmos.color = Color.blue;
            foreach (var spawn in generatedSpawnPoints)
            {
                Gizmos.DrawSphere(spawn, 2f);
            }
        }
    }
    
    /// <summary>
    /// Данные карты для сериализации
    /// </summary>
    [System.Serializable]
    public class MapData
    {
        public int width;
        public int height;
        public Vector3[] walls;
        public Vector3[] bushes;
        public Vector3[] spawnPoints;
    }
}
