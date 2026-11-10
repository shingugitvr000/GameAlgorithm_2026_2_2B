using System;
using System.Linq;
using System.Reflection;

namespace AlgoLesson8
{
    public static class FactoryAlgorithmFactory
    {
        public static IProductionPlanner CreatePlanner() =>
            Create<IProductionPlanner>() ?? new LinearPlanner();

        public static IOrderScheduler CreateScheduler() =>
            Create<IOrderScheduler>() ?? new FirstOrderScheduler();

        public static IBottleneckAnalyzer CreateAnalyzer() =>
            Create<IBottleneckAnalyzer>() ?? new EmptyAnalyzer();

        private static T Create<T>() where T : class
        {
            Type contract = typeof(T);
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetTypesSafely)
                .FirstOrDefault(candidate =>
                    contract.IsAssignableFrom(candidate) &&
                    !candidate.IsInterface &&
                    !candidate.IsAbstract &&
                    candidate.Namespace == "AlgoLesson8.Student");
            return type == null ? null : Activator.CreateInstance(type) as T;
        }

        private static Type[] GetTypesSafely(Assembly assembly)
        {
            try { return assembly.GetTypes(); }
            catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null).ToArray(); }
        }

        private sealed class LinearPlanner : IProductionPlanner
        {
            public ProductionPlan BuildPlan(string targetProduct, System.Collections.Generic.IReadOnlyList<RecipeDefinition> recipes)
            {
                ProductionPlan plan = new ProductionPlan();
                plan.Steps.Add("IronOre");
                plan.Steps.Add("IronPlate");
                plan.Steps.Add(targetProduct);
                return plan;
            }
        }

        private sealed class FirstOrderScheduler : IOrderScheduler
        {
            public int SelectNextOrder(System.Collections.Generic.IReadOnlyList<WorkOrder> orders, float currentTime) =>
                orders == null || orders.Count == 0 ? -1 : orders[0].Id;
        }

        private sealed class EmptyAnalyzer : IBottleneckAnalyzer
        {
            public string FindBottleneck(System.Collections.Generic.IReadOnlyList<MachineSnapshot> machines) => string.Empty;
        }
    }
}
