using System;
using System.Collections.Generic;
using UnityEngine;

namespace Evolution.AI.BehaviorTrees
{
    /// <summary>
    /// Базовый класс для узлов дерева поведения
    /// </summary>
    public abstract class BehaviorNode
    {
        public string NodeName { get; set; }
        public BehaviorNodeStatus Status { get; protected set; } = BehaviorNodeStatus.Ready;
        
        public virtual void Initialize() { }
        public virtual void Terminate() { }
        public abstract BehaviorNodeStatus Execute(float deltaTime);
        public virtual void Reset() => Status = BehaviorNodeStatus.Ready;
    }
    
    public enum BehaviorNodeStatus
    {
        Ready,
        Running,
        Success,
        Failure
    }
    
    /// <summary>
    /// Узел последовательности - выполняет дочерние узлы по порядку
    /// </summary>
    public class SequenceNode : BehaviorNode
    {
        private List<BehaviorNode> children = new List<BehaviorNode>();
        private int currentChildIndex = 0;
        
        public void AddChild(BehaviorNode child)
        {
            children.Add(child);
            child.Initialize();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            foreach (var child in children)
            {
                child.Initialize();
            }
        }
        
        public override void Terminate()
        {
            if (currentChildIndex < children.Count)
            {
                children[currentChildIndex].Terminate();
            }
            base.Terminate();
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            while (currentChildIndex < children.Count)
            {
                var child = children[currentChildIndex];
                var status = child.Execute(deltaTime);
                
                switch (status)
                {
                    case BehaviorNodeStatus.Ready:
                        continue;
                        
                    case BehaviorNodeStatus.Running:
                        Status = BehaviorNodeStatus.Running;
                        return BehaviorNodeStatus.Running;
                        
                    case BehaviorNodeStatus.Success:
                        currentChildIndex++;
                        continue;
                        
                    case BehaviorNodeStatus.Failure:
                        Status = BehaviorNodeStatus.Failure;
                        return BehaviorNodeStatus.Failure;
                }
            }
            
            currentChildIndex = 0;
            Status = BehaviorNodeStatus.Success;
            return BehaviorNodeStatus.Success;
        }
        
        public override void Reset()
        {
            base.Reset();
            currentChildIndex = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }
    
    /// <summary>
    /// Узел селектора - выполняет дочерние узлы пока один не вернет успех
    /// </summary>
    public class SelectorNode : BehaviorNode
    {
        private List<BehaviorNode> children = new List<BehaviorNode>();
        private int currentChildIndex = 0;
        
        public void AddChild(BehaviorNode child)
        {
            children.Add(child);
            child.Initialize();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            foreach (var child in children)
            {
                child.Initialize();
            }
        }
        
        public override void Terminate()
        {
            if (currentChildIndex < children.Count)
            {
                children[currentChildIndex].Terminate();
            }
            base.Terminate();
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            while (currentChildIndex < children.Count)
            {
                var child = children[currentChildIndex];
                var status = child.Execute(deltaTime);
                
                switch (status)
                {
                    case BehaviorNodeStatus.Ready:
                        continue;
                        
                    case BehaviorNodeStatus.Running:
                        Status = BehaviorNodeStatus.Running;
                        return BehaviorNodeStatus.Running;
                        
                    case BehaviorNodeStatus.Success:
                        currentChildIndex = 0;
                        Status = BehaviorNodeStatus.Success;
                        return BehaviorNodeStatus.Success;
                        
                    case BehaviorNodeStatus.Failure:
                        currentChildIndex++;
                        continue;
                }
            }
            
            currentChildIndex = 0;
            Status = BehaviorNodeStatus.Failure;
            return BehaviorNodeStatus.Failure;
        }
        
        public override void Reset()
        {
            base.Reset();
            currentChildIndex = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }
    
    /// <summary>
    /// Узел повторения - повторяет выполнение дочернего узла
    /// </summary>
    public class RepeatNode : BehaviorNode
    {
        private BehaviorNode child;
        private int repeatCount = -1; // -1 = бесконечно
        private int currentRepeats = 0;
        
        public RepeatNode(int repeatCount = -1)
        {
            this.repeatCount = repeatCount;
        }
        
        public void SetChild(BehaviorNode child)
        {
            this.child = child;
            child.Initialize();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            child?.Initialize();
        }
        
        public override void Terminate()
        {
            child?.Terminate();
            base.Terminate();
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (child == null)
            {
                Status = BehaviorNodeStatus.Failure;
                return BehaviorNodeStatus.Failure;
            }
            
            var status = child.Execute(deltaTime);
            
            if (status == BehaviorNodeStatus.Success)
            {
                currentRepeats++;
                
                if (repeatCount != -1 && currentRepeats >= repeatCount)
                {
                    currentRepeats = 0;
                    Status = BehaviorNodeStatus.Success;
                    return BehaviorNodeStatus.Success;
                }
                
                child.Reset();
                Status = BehaviorNodeStatus.Running;
                return BehaviorNodeStatus.Running;
            }
            
            if (status == BehaviorNodeStatus.Failure)
            {
                currentRepeats = 0;
                Status = BehaviorNodeStatus.Failure;
                return BehaviorNodeStatus.Failure;
            }
            
            Status = BehaviorNodeStatus.Running;
            return BehaviorNodeStatus.Running;
        }
        
        public override void Reset()
        {
            base.Reset();
            currentRepeats = 0;
            child?.Reset();
        }
    }
    
    /// <summary>
    /// Узел инвертора - инвертирует результат дочернего узла
    /// </summary>
    public class InverterNode : BehaviorNode
    {
        private BehaviorNode child;
        
        public void SetChild(BehaviorNode child)
        {
            this.child = child;
            child.Initialize();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            child?.Initialize();
        }
        
        public override void Terminate()
        {
            child?.Terminate();
            base.Terminate();
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (child == null)
            {
                Status = BehaviorNodeStatus.Failure;
                return BehaviorNodeStatus.Failure;
            }
            
            var status = child.Execute(deltaTime);
            
            if (status == BehaviorNodeStatus.Success)
            {
                Status = BehaviorNodeStatus.Failure;
                return BehaviorNodeStatus.Failure;
            }
            
            if (status == BehaviorNodeStatus.Failure)
            {
                Status = BehaviorNodeStatus.Success;
                return BehaviorNodeStatus.Success;
            }
            
            Status = BehaviorNodeStatus.Running;
            return BehaviorNodeStatus.Running;
        }
        
        public override void Reset()
        {
            base.Reset();
            child?.Reset();
        }
    }
}
