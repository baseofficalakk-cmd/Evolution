using System;
using System.Collections.Generic;
using UnityEngine;

namespace Evolution.Networking.Core
{
    /// <summary>
    /// Основной сетевой менеджер для клиент-сервер коммуникации
    /// Поддерживает надежные и ненадежные каналы, интерполяцию и предсказание
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }
        
        [Header("Network Settings")]
        [SerializeField] private int tickRate = 60;
        [SerializeField] private float interpolationDelay = 0.1f;
        [SerializeField] private int maxBufferedStates = 30;
        
        [Header("Connection")]
        [SerializeField] private string serverAddress = "127.0.0.1";
        [SerializeField] private int serverPort = 7777;
        
        public bool IsConnected { get; private set; }
        public bool IsServer { get; private set; }
        public bool IsClient { get; private set; }
        public float InterpolationDelay => interpolationDelay;
        public int TickRate => tickRate;
        
        private Queue<INetworkMessage> outgoingMessages = new Queue<INetworkMessage>();
        private Queue<INetworkMessage> incomingMessages = new Queue<INetworkMessage>();
        private Dictionary<Type, Action<INetworkMessage>> messageHandlers = new Dictionary<Type, Action<INetworkMessage>>();
        
        private float tickTimer;
        private float tickInterval;
        
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<float> OnLatencyUpdated;
        
        private float currentLatency;
        private List<float> latencySamples = new List<float>();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            tickInterval = 1f / tickRate;
            
            RegisterDefaultHandlers();
        }
        
        private void Update()
        {
            if (!IsConnected) return;
            
            tickTimer += Time.deltaTime;
            
            while (tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;
                ProcessTick();
            }
            
            ProcessIncomingMessages();
        }
        
        private void ProcessTick()
        {
            // Отправка накопленных сообщений
            lock (outgoingMessages)
            {
                while (outgoingMessages.Count > 0)
                {
                    var message = outgoingMessages.Dequeue();
                    SendMessageToServer(message);
                }
            }
        }
        
        private void ProcessIncomingMessages()
        {
            lock (incomingMessages)
            {
                while (incomingMessages.Count > 0)
                {
                    var message = incomingMessages.Dequeue();
                    HandleMessage(message);
                }
            }
        }
        
        public void ConnectAsClient()
        {
            IsClient = true;
            IsServer = false;
            
            Debug.Log($"Connecting to server at {serverAddress}:{serverPort}");
            
            // Здесь будет реальная логика подключения через Netcode/Photon/Mirror
            SimulateConnection();
        }
        
        public void StartAsServer()
        {
            IsServer = true;
            IsClient = false;
            
            Debug.Log($"Starting server on port {serverPort}");
            
            // Здесь будет реальная логика запуска сервера
            SimulateConnection();
        }
        
        private void SimulateConnection()
        {
            IsConnected = true;
            OnConnected?.Invoke();
            Debug.Log("Connected successfully!");
        }
        
        public void Disconnect()
        {
            IsConnected = false;
            IsClient = false;
            IsServer = false;
            
            outgoingMessages.Clear();
            incomingMessages.Clear();
            
            OnDisconnected?.Invoke();
            Debug.Log("Disconnected from server");
        }
        
        public void Send<T>(T message) where T : INetworkMessage
        {
            if (!IsConnected)
            {
                Debug.LogWarning($"Cannot send message {typeof(T).Name} - not connected");
                return;
            }
            
            lock (outgoingMessages)
            {
                outgoingMessages.Enqueue(message);
            }
        }
        
        public void Receive(INetworkMessage message)
        {
            lock (incomingMessages)
            {
                incomingMessages.Enqueue(message);
            }
        }
        
        public void RegisterHandler<T>(Action<T> handler) where T : INetworkMessage
        {
            Type messageType = typeof(T);
            
            if (!messageHandlers.ContainsKey(messageType))
            {
                messageHandlers[messageType] = (msg) => handler((T)msg);
            }
        }
        
        private void HandleMessage(INetworkMessage message)
        {
            Type messageType = message.GetType();
            
            if (messageHandlers.TryGetValue(messageType, out var handler))
            {
                handler.Invoke(message);
            }
            else
            {
                Debug.LogWarning($"No handler registered for message type: {messageType.Name}");
            }
        }
        
        private void RegisterDefaultHandlers()
        {
            // Регистрация обработчиков по умолчанию будет в других классах
        }
        
        private void SendMessageToServer(INetworkMessage message)
        {
            // Сериализация и отправка на сервер
            // В реальной реализации здесь будет код Netcode/Photon/Mirror
            byte[] data = MessageSerializer.Serialize(message);
            // SendBytes(data, message.Channel);
        }
        
        public void UpdateLatency(float latency)
        {
            latencySamples.Add(latency);
            
            if (latencySamples.Count > 10)
            {
                latencySamples.RemoveAt(0);
            }
            
            currentLatency = 0;
            foreach (var sample in latencySamples)
            {
                currentLatency += sample;
            }
            currentLatency /= latencySamples.Count;
            
            OnLatencyUpdated?.Invoke(currentLatency);
        }
        
        public float GetCurrentLatency() => currentLatency;
    }
    
    /// <summary>
    /// Сериализатор сетевых сообщений
    /// </summary>
    public static class MessageSerializer
    {
        public static byte[] Serialize(INetworkMessage message)
        {
            // Простая сериализация - в продакшене использовать Protobuf/MessagePack
            return System.Text.Encoding.UTF8.GetBytes(UnityEngine.JsonUtility.ToJson(message));
        }
        
        public static T Deserialize<T>(byte[] data) where T : INetworkMessage
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            return UnityEngine.JsonUtility.FromJson<T>(json);
        }
    }
}
