using System.Collections.Generic;
using AlgoLesson8;

namespace AlgoLesson8.Student
{
    /// <summary>
    /// 활성 기계 중 분당 처리량이 가장 낮은 병목 기계를 찾습니다.
    /// 
    /// 생산 라인에서 병목 기계를 찾는 분석기
    /// 
    /// 생산 라인 전체의 처리 속도는 가장 느린 기계에 의해 결정
    /// 목록을 한 번 훑으면서 현재까지의 초소값을 갱신하는 선형 탐생(Linear Scan)을 한다. 
    /// 
    /// [복잡도]
    /// - 시간 : O(n) 
    /// - 공간 : O(1)
    /// 
    /// </summary>

    public sealed class BottleneckAnalyzer : IBottleneckAnalyzer
    {
        public string FindBottleneck(IReadOnlyList<MachineSnapshot> machines)
        {
           
            if (machines == null || machines.Count == 0)  //입력이 없으면 분석할 대상이 없으므로 빈 문자열 반환
                return string.Empty;

            MachineSnapshot best = null;

            foreach (MachineSnapshot machine in machines)
            {

                if (!machine.Enabled) continue;                         //꺼져 있는 기계는 라인 처리량에 영향을 주지 않으므로 후보에서 제외

                // 1. 아직 후보가 없다. (첫 번째 가동 기계)
                // 2. 현재 기계가 후보보다 더 느리다. (처리량이 더 작다)
                // 3. 처리량이 같다면 ID 가 사전순으로 더 앞선다. 
                if(best == null || 
                    machine.ItemsPerMinute < best.ItemsPerMinute ||
                    (machine.ItemsPerMinute == best.ItemsPerMinute && string.CompareOrdinal(machine.Id, best.Id) < 0))
                {
                    best = machine;
                }
            }

            return best == null ? string.Empty : best.DisplayName;
        }
    }
}
