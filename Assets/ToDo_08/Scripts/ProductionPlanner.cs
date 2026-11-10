using System.Collections.Generic;
using AlgoLesson8;

namespace AlgoLesson8.Student
{
    /// <summary>
    /// 제작법 그래프를 탐색해 재료부터 완제품까지의 생산 순서를 만듭니다.
    /// 
    /// 레시피는 출력물 <- 입력물들 관계 이므로 의존성 그래프(방향 그래프로)로 볼 수 있다.
    /// 예) 철판 <- 철광석 , 기어 <- 철판 , 엔진 <- 기어 + 철판
    /// 
    /// DFS + 후위 순회 
    /// 재료를 전부 방문한 뒤에 자기 자신의 결과에 추가 
    /// 
    /// 시간 : O(V+E) (V = 제품 수, E = 레시피의 재료 연결 수)
    /// 공간 : O(V) (방문 집합 + 재귀 호출 스택)
    /// </summary>
    public sealed class ProductionPlanner : IProductionPlanner
    {
        public ProductionPlan BuildPlan(string targetProduct,IReadOnlyList<RecipeDefinition> recipes)
        {
            ProductionPlan result = new ProductionPlan();

            Dictionary<string, RecipeDefinition> recipeMap = new Dictionary<string, RecipeDefinition>();

            foreach (RecipeDefinition recipe in recipes)
            {
                recipeMap[recipe.Output] = recipe;
            }

            //DFS 상태 집합 준비

            HashSet<string> visited = new HashSet<string>();
            HashSet<string> visiting = new HashSet<string>();

            //목표 제품에서 시작해 필요한 재료를 거슬러 내려가며 탐색
            Visit(targetProduct, recipeMap, visited, visiting, result);

            return result;        


            // TODO 01: output을 key로 사용하는 recipeMap을 만드세요.

            // TODO 02: 방문 완료 제품을 저장할 visited를 만드세요.

            // TODO 03: 현재 탐색 중인 제품을 저장할 visiting을 만드세요.

            // TODO 04: targetProduct부터 DFS를 시작하세요.

            // TODO 05: 탐색 중인 제품을 다시 만나면 HasCycle을 true로 만드세요.

            // TODO 06: 각 입력 재료를 먼저 재귀 탐색하세요.

            // TODO 07: 모든 입력 탐색 후 현재 제품을 Steps에 추가하세요.

            // 미완성 상태에서도 직선 제작 흐름을 볼 수 있는 임시 계획입니다.
          
        }

        //후위 순회 (재료들을 먼저 모두 처리한뒤 product 하나를 DFS로 방문
        private static void Visit(string product, Dictionary<string, RecipeDefinition> recipeMap,
            HashSet<string> visited, HashSet<string> visiting, ProductionPlan result)
        {
            if (result.HasCycle || visited.Contains(product)) return;       //이미 사이클이 발견 됐으면 종료 

            if (!visiting.Add(product))
            {
                result.HasCycle = true;
                return;
            }

            if (recipeMap.TryGetValue(product, out RecipeDefinition recipe))        //모든 재료를 먼저 방문하고 레시피가 업승면 원재료로 취급
            {
                foreach (string input in recipe.Inputs)
                    Visit(input, recipeMap, visited, visiting, result);
            }

            visiting.Remove(product);
            visited.Add(product);

            result.Steps.Add(product);  //모든 재료가 이미 step 에 들어간 뒤이므로 여기서 추가 하면 재료 -> 결과물 순서가 보장 된다.
        }

    }
}
