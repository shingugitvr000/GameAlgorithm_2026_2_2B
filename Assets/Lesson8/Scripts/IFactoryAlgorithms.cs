using System.Collections.Generic;

namespace AlgoLesson8
{
    public interface IProductionPlanner
    {
        ProductionPlan BuildPlan(string targetProduct, IReadOnlyList<RecipeDefinition> recipes);
    }

    public interface IOrderScheduler
    {
        int SelectNextOrder(IReadOnlyList<WorkOrder> orders, float currentTime);
    }

    public interface IBottleneckAnalyzer
    {
        string FindBottleneck(IReadOnlyList<MachineSnapshot> machines);
    }
}
