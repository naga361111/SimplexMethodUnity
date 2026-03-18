using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// InputValue를 사용하기 위해 필수
using UnityEngine.InputSystem;

public class ProcessFlowChart : MonoBehaviour
{
    private Dictionary<int, int> _negativeValueIndex = new();

    public void OnJump(InputValue value)
    {
        var isPressed = value.isPressed;

        if (isPressed)
        {
            CheckNegativeEntry();
        }
    }

    private void CheckNegativeEntry()
    {
        if (DataSpawner.Instance == null || DataSpawner.Instance.RuntimeTable == null)
        {
            Debug.LogWarning("DataSpawner가 없거나 RuntimeTable이 초기화되지 않았습니다.");
            return;
        }

        var table = DataSpawner.Instance.RuntimeTable;
        if (table.Length == 0) return;

        _negativeValueIndex.Clear();
        var firstRow = table[0];
        var hasNegative = false;

        // 첫 번째 행에서 음수를 찾아 열 인덱스와 절대값을 저장합니다.
        for (var i = 0; i < firstRow.Length; i++)
        {
            if (firstRow[i] < 0)
            {
                _negativeValueIndex.Add(i, (int)Mathf.Abs(firstRow[i]));
                hasNegative = true;
            }
        }

        if (!hasNegative)
        {
            Debug.Log("[Runtime] 첫 번째 행에 음수 값이 없습니다.");
            return;
        }

        DivideLastRowValue();
    }

    private void DivideLastRowValue()
    {
        // 절대값이 큰 순서대로 정렬
        var sortedEntries = _negativeValueIndex
            .OrderByDescending(x => x.Value)
            .ToList();

        var table = DataSpawner.Instance.RuntimeTable;

        foreach (var entry in sortedEntries)
        {
            int pivotCol = entry.Key;
            Debug.Log($"[Simplex] 피벗 열 {pivotCol} 처리 (첫 행 절대값: {entry.Value})");

            float minRatio = float.MaxValue;
            int minRatioRow = -1;

            for (int i = 1; i < table.Length; i++)
            {
                float pivotValue = table[i][pivotCol];

                if (pivotValue <= 0) continue; // 심플렉스법 Ratio Test는 양수일 때만 수행

                int lastColIndex = table[i].Length - 1;
                float lastValue = table[i][lastColIndex];
                float result = lastValue / pivotValue;

                // 데이터 업데이트 및 UI 반영
                DataSpawner.Instance.UpdateCellValue(i, lastColIndex, result);

                if (result < minRatio)
                {
                    minRatio = result;
                    minRatioRow = i;
                }
            }

            NormalizePivotRow(minRatioRow, pivotCol);
            EliminateObjectiveRow(minRatioRow, pivotCol); // 가우스 소거법 추가

            Debug.Log("--------------------------------------------------");
        }
    }

    // 나눠진 끝에 위치한 row들 중에서 가장 작은 값을 찾아서 그 값으로 해당 row 전체를 나눔 (마지막은 이미 나누어졌으므로 불필요)
    private void NormalizePivotRow(int targetRow, int pivotCol)
    {
        var table = DataSpawner.Instance.RuntimeTable;
        float pivotElement = table[targetRow][pivotCol];

        if (pivotElement == 0) return;

        Debug.Log($"[Simplex] 행 {targetRow} 정규화 시작 (교차 지점 값: {pivotElement})");

        int colCount = table[targetRow].Length;
        for (int j = 0; j < colCount - 1; j++)
        {
            float currentValue = table[targetRow][j];
            float newValue = currentValue / pivotElement;

            DataSpawner.Instance.UpdateCellValue(targetRow, j, newValue);
        }

        Debug.Log($"[Simplex] 행 {targetRow} 정규화 완료");
    }

    private void EliminateObjectiveRow(int pivotRow, int pivotCol)
    {
        var table = DataSpawner.Instance.RuntimeTable;

        // 1. 첫 번째 행의 피벗 열 원본 값(음수)을 저장
        float originalObjectiveValue = table[0][pivotCol];

        Debug.Log($"[Simplex] 가우스 소거법 시작 (목적함수 행, 피벗 열 값: {originalObjectiveValue})");

        int colCount = table[0].Length;

        // 2. 첫 번째 행의 모든 요소에 대해: NewRow0 = OldRow0 - (OriginalValue * PivotRow)
        for (int j = 0; j < colCount; j++)
        {
            float currentVal = table[0][j];
            float pivotRowVal = table[pivotRow][j];

            // 계산: 원본 값 * 정규화된 행의 요소 를 기존 값에서 뺌
            float newValue = currentVal - (originalObjectiveValue * pivotRowVal);

            DataSpawner.Instance.UpdateCellValue(0, j, newValue);
        }

        Debug.Log($"[Simplex] 가우스 소거법 완료 (첫 번째 행의 {pivotCol}번 열이 0이 됨)");
    }
}