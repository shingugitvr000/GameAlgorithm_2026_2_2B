using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlgoLesson8
{
    [Serializable]
    public sealed class RecipeDefinition
    {
        public RecipeDefinition(string output, params string[] inputs)
        {
            Output = output;
            Inputs = new List<string>(inputs);
        }

        public string Output { get; }
        public IReadOnlyList<string> Inputs { get; }
    }

    [Serializable]
    public sealed class WorkOrder
    {
        public WorkOrder(int id, string product, int priority, float deadline, int reward)
        {
            Id = id;
            Product = product;
            Priority = priority;
            Deadline = deadline;
            Reward = reward;
        }

        public int Id { get; }
        public string Product { get; }
        public int Priority { get; }
        public float Deadline { get; }
        public int Reward { get; }
    }

    [Serializable]
    public sealed class MachineSnapshot
    {
        public MachineSnapshot(string id, string displayName, float secondsPerItem, bool enabled)
        {
            Id = id;
            DisplayName = displayName;
            SecondsPerItem = secondsPerItem;
            Enabled = enabled;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public float SecondsPerItem { get; }
        public bool Enabled { get; }
        public float ItemsPerMinute => Enabled && SecondsPerItem > 0f ? 60f / SecondsPerItem : 0f;
    }

    public sealed class ProductionPlan
    {
        public List<string> Steps { get; } = new List<string>();
        public bool HasCycle { get; set; }
    }
}
