using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace AlgoLesson8
{
    public sealed class FactoryGame : MonoBehaviour
    {
        private readonly List<FactoryMachine> machines = new List<FactoryMachine>();
        private readonly List<RecipeDefinition> recipes = new List<RecipeDefinition>();
        private readonly List<WorkOrder> orders = new List<WorkOrder>();
        private readonly Queue<WorkOrder> waitingOrders = new Queue<WorkOrder>();
        private IProductionPlanner planner;
        private IOrderScheduler scheduler;
        private IBottleneckAnalyzer analyzer;
        private bool running;
        private int score;
        private int completed;
        private string status;
        private string bottleneck;
        private ProductionPlan currentPlan;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().name != "Lesson08_AutomationFactory") return;
            planner = FactoryAlgorithmFactory.CreatePlanner();
            scheduler = FactoryAlgorithmFactory.CreateScheduler();
            analyzer = FactoryAlgorithmFactory.CreateAnalyzer();
            machines.AddRange(FindObjectsByType<FactoryMachine>(FindObjectsSortMode.None));
            BuildData();
            ResetGame();
        }

        private void BuildData()
        {
            recipes.Add(new RecipeDefinition("IronPlate", "IronOre"));
            recipes.Add(new RecipeDefinition("CopperWire", "CopperOre"));
            recipes.Add(new RecipeDefinition("Gear", "IronPlate"));
            recipes.Add(new RecipeDefinition("Circuit", "IronPlate", "CopperWire"));
            recipes.Add(new RecipeDefinition("Robot", "Gear", "Circuit"));

            orders.Add(new WorkOrder(101, "Gear", 1, 45f, 100));
            orders.Add(new WorkOrder(102, "Circuit", 3, 30f, 180));
            orders.Add(new WorkOrder(103, "Robot", 5, 22f, 350));
            orders.Add(new WorkOrder(104, "Gear", 2, 18f, 120));
        }

        private void ResetGame()
        {
            StopAllCoroutines();
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Respawn")) Destroy(item);
            waitingOrders.Clear();
            foreach (WorkOrder order in orders) waitingOrders.Enqueue(order);
            score = 0;
            completed = 0;
            running = false;
            status = "SPACE: 공장 시작   R: 초기화";
            bottleneck = analyzer.FindBottleneck(machines.Select(m => m.CreateSnapshot()).ToList());
        }

        private void Update()
        {
            if (Keyboard.current == null) return;
            if (Keyboard.current.rKey.wasPressedThisFrame) ResetGame();
            if (Keyboard.current.spaceKey.wasPressedThisFrame && !running)
                StartCoroutine(RunFactory());
        }

        private IEnumerator RunFactory()
        {
            running = true;
            status = "주문 우선순위를 계산하는 중";

            while (waitingOrders.Count > 0)
            {
                List<WorkOrder> candidates = waitingOrders.ToList();
                int selectedId = scheduler.SelectNextOrder(candidates, Time.time);
                WorkOrder selected = candidates.FirstOrDefault(o => o.Id == selectedId) ?? candidates[0];
                RebuildQueueWithout(selected.Id);

                currentPlan = planner.BuildPlan(selected.Product, recipes);
                if (currentPlan == null || currentPlan.HasCycle || currentPlan.Steps.Count == 0)
                {
                    status = $"주문 {selected.Id}: 제작 순서 오류 또는 순환 발견";
                    yield return new WaitForSeconds(1.2f);
                    continue;
                }

                status = $"주문 {selected.Id}  {selected.Product} 생산 중";
                yield return ProduceOrder(selected);
                completed++;
                score += selected.Reward;
                status = $"주문 {selected.Id} 완료  +{selected.Reward}";
                yield return new WaitForSeconds(0.7f);
            }

            running = false;
            status = "모든 주문 처리 완료   R: 다시 시작";
        }

        private void RebuildQueueWithout(int selectedId)
        {
            List<WorkOrder> remain = waitingOrders.Where(o => o.Id != selectedId).ToList();
            waitingOrders.Clear();
            foreach (WorkOrder order in remain) waitingOrders.Enqueue(order);
        }

        private IEnumerator ProduceOrder(WorkOrder order)
        {
            List<FactoryMachine> route = GetRouteFromPlan();
            GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.name = $"Product_{order.Id}_{order.Product}";
            item.tag = "Respawn";
            item.transform.localScale = Vector3.one * 0.55f;
            item.GetComponent<Renderer>().material.color = OrderColor(order.Product);

            if (route.Count > 0)
                item.transform.position = route[0].transform.position + Vector3.up * 0.7f;

            foreach (FactoryMachine machine in route)
            {
                Vector3 target = machine.transform.position + Vector3.up * 0.7f;
                while (Vector3.Distance(item.transform.position, target) > 0.03f)
                {
                    item.transform.position = Vector3.MoveTowards(item.transform.position, target, 4.5f * Time.deltaTime);
                    item.transform.Rotate(70f * Time.deltaTime, 90f * Time.deltaTime, 0f);
                    yield return null;
                }

                machine.SetWorking(true);
                yield return new WaitForSeconds(Mathf.Clamp(machine.SecondsPerItem * 0.28f, 0.25f, 0.8f));
                machine.SetWorking(false);
            }

            Destroy(item);
        }

        private List<FactoryMachine> GetRouteFromPlan()
        {
            List<string> ids = new List<string>();

            foreach (string step in currentPlan.Steps)
            {
                string id = step switch
                {
                    "IronOre" => "IronMine",
                    "CopperOre" => "CopperMine",
                    "IronPlate" => "Smelter",
                    "CopperWire" => "WireMill",
                    "Gear" => "GearPress",
                    "Circuit" => "Assembler",
                    "Robot" => "Assembler",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(id) && !ids.Contains(id))
                    ids.Add(id);
            }

            ids.Add("Shipping");
            return ids.Select(id => machines.FirstOrDefault(m => m.MachineId == id))
                .Where(m => m != null).ToList();
        }

        private static Color OrderColor(string product) => product == "Gear"
            ? new Color(1f, 0.65f, 0.15f)
            : product == "Circuit" ? new Color(0.2f, 0.9f, 0.55f) : new Color(0.55f, 0.4f, 1f);

        private void OnGUI()
        {
            GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            GUIStyle body = new GUIStyle(GUI.skin.label) { fontSize = 16, normal = { textColor = Color.white } };
            GUI.Box(new Rect(16, 16, 560, 170), string.Empty);
            GUI.Label(new Rect(30, 26, 520, 34), "자동화 공장 알고리즘", title);
            GUI.Label(new Rect(30, 68, 520, 26), status, body);
            GUI.Label(new Rect(30, 98, 520, 26), $"완료 주문: {completed}/4   점수: {score}", body);
            GUI.Label(new Rect(30, 128, 520, 26), string.IsNullOrEmpty(bottleneck) ? "병목: TODO를 완성하세요" : $"병목: {bottleneck}", body);
            string plan = currentPlan == null ? "-" : string.Join(" > ", currentPlan.Steps);
            GUI.Label(new Rect(30, 154, 520, 24), $"제작 순서: {plan}", body);
        }
    }
}
