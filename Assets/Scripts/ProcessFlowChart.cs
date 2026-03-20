using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// InputValue를 사용하기 위해 필수
using UnityEngine.InputSystem;

public class ProcessFlowChart : MonoBehaviour
{
    private Dictionary<int, int> _negativeValueIndex = new();
    private int _steps;

    private List<KeyValuePair<int, int>> _sortedEntries = new();
    private int _lastColIndex;
    private float _result;
    private float _minRatio;
    private int _minRatioRow;
    private int _pivotCol;

    // 키보드 입력 (Simplex Method 과정 진행)
    public void OnProcess(InputValue value)
    {
        Debug.Log($"Hi {_steps}");

        switch (_steps)
        {
            case 0:
                FindNegativeInFirstCol();
                break;
            case 1:
                CheckNegativeEntry();
                break;
            case 2:
                FindDivideRowElement();
                break;
            case 3:
                ExecuteDivideRow();
                break;
            case 4:
                FindMinFromDivideRow();
                break;
            case 5:
                FindCrossPoint();
                break;
            case 6:
                FindPointsFromRow();
                break;
            case 7:
                DivideCrossedCol();
                break;
            case 8:
                FindGaussEliminate();
                break;
            case 9:
                GaussEliminate();
                break;
            default:
                Debug.Log($"{_steps} is not valid step.");
                break;
        }

        _steps++;
    }

    // Step 0
    private void FindNegativeInFirstCol()
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

        // 음수인 위치를 받아서 해당 TMPro Text의 색상을 붉은색으로 변경
        foreach (var target in _negativeValueIndex.Keys)
        {
            DataSpawner.Instance.UpdateCellColor(0, target, Color.red);
        }
    }

    // Step 1
    private void CheckNegativeEntry()
    {
        // 절대값이 큰 순서대로 정렬
        _sortedEntries = _negativeValueIndex
            .OrderByDescending(x => x.Value)
            .ToList();

        // 이번 순회에 작업할 음수 확정 ( 아닌 얘들은 연하게 변경 )
        for (var i = 1; i < _sortedEntries.Count; i++)
        {
            DataSpawner.Instance.UpdateCellColor(0, _sortedEntries[i].Key, Color.black);
        }
    }

    // Step 2
    private void FindDivideRowElement()
    {
        var table = DataSpawner.Instance.RuntimeTable;
        var entry = _sortedEntries.FirstOrDefault();

        _pivotCol = entry.Key;

        _minRatio = float.MaxValue;
        _minRatioRow = -1;

        for (int i = 1; i < table.Length; i++)
        {
            float pivotValue = table[i][_pivotCol];

            if (pivotValue <= 0) continue; // 심플렉스법 Ratio Test는 양수일 때만 수행

            _lastColIndex = table[i].Length - 1;
            var lastValue = table[i][_lastColIndex];
            _result = lastValue / pivotValue;

            DataSpawner.Instance.UpdateCellColor(i, _pivotCol, Color.blue); // 나눌 값 강조
            DataSpawner.Instance.UpdateCellColor(i, _lastColIndex, Color.blue); // 나눠질 값 강조
        }
    }

    // Step 3
    private void ExecuteDivideRow()
    {
        var table = DataSpawner.Instance.RuntimeTable;

        for (int i = 1; i < table.Length; i++)
        {
            float pivotValue = table[i][_pivotCol];

            // 피벗 열의 값이 양수인 경우에만 연산을 수행 (Step 2와 동일 조건)
            if (pivotValue <= 0) continue;

            float lastValue = table[i][_lastColIndex];
            float ratio = lastValue / pivotValue;

            // 각 행의 데이터 업데이트 및 UI 반영
            DataSpawner.Instance.UpdateCellValue(i, _lastColIndex, ratio);

            // 나눠진 값 중에서 최소 비율을 가진 행을 탐색
            if (ratio < _minRatio)
            {
                _minRatio = ratio;
                _minRatioRow = i;
            }

            DataSpawner.Instance.UpdateCellColor(i, _pivotCol, Color.black); // 강조 해제
        }
    }

    // Step 4
    private void FindMinFromDivideRow()
    {
        var table = DataSpawner.Instance.RuntimeTable;

        for (int i = 1; i < table.Length; i++)
        {
            if (i == _minRatioRow) continue;

            // 계산이 수행되었던 행(pivotValue > 0)들 중 최소값이 아닌 것들은 색상을 연하게 변경
            float pivotValue = table[i][_pivotCol];
            if (pivotValue > 0)
            {
                DataSpawner.Instance.UpdateCellColor(i, _lastColIndex, Color.black);
            }
        }
    }

    // Step 5
    private void FindCrossPoint()
    {
        DataSpawner.Instance.UpdateCellColor(_minRatioRow, _pivotCol, Color.purple);
    }

    // Step 6
    private void FindPointsFromRow()
    {
        var table = DataSpawner.Instance.RuntimeTable;

        for (var i = 0; i < table[_minRatioRow].Length - 1; i++)
        {
            var current = table[_minRatioRow][i];

            if (current != 0)
            {
                DataSpawner.Instance.UpdateCellColor(_minRatioRow, i, Color.rebeccaPurple);
            }
        }
    }

    // Step 7
    private void DivideCrossedCol()
    {
        var table = DataSpawner.Instance.RuntimeTable;
        float pivotElement = table[_minRatioRow][_pivotCol];

        if (pivotElement == 0) return;

        // 보라색으로 강조된 피벗 값을 기준으로 해당 행의 모든 요소를 나눔
        // 마지막 열은 ExecuteDivideRow에서 이미 나누어졌으므로 제외하고 순회
        int colCount = table[_minRatioRow].Length;
        for (int j = 0; j < colCount - 1; j++)
        {
            float currentValue = table[_minRatioRow][j];
            float newValue = currentValue / pivotElement;

            DataSpawner.Instance.UpdateCellValue(_minRatioRow, j, newValue);
        }

        // 작업 완료 후 피벗 셀의 강조 색상 해제
        for (var i = 0; i < table[_minRatioRow].Length - 1; i++)
        {
            var current = table[_minRatioRow][i];

            if (current != 0)
            {
                DataSpawner.Instance.UpdateCellColor(_minRatioRow, i, Color.black);
            }
        }
    }

    // Step 8
    private void FindGaussEliminate()
    {
        var table = DataSpawner.Instance.RuntimeTable;
        
        for (var i = 0; i < table[0].Length; i++)
        {
            if (i == _pivotCol) continue;
            
            DataSpawner.Instance.UpdateCellColor(0, i, Color.green);
        }

        for (var i = 0; i < table[_minRatioRow].Length; i++)
        {
            DataSpawner.Instance.UpdateCellColor(_minRatioRow, i, Color.green);
        }
    }
    
    // Step 9
    private void GaussEliminate()
    {
        var table = DataSpawner.Instance.RuntimeTable;

        var originalValue = table[0][_pivotCol];
        var rowCount = table[0].Length;

        for (var j = 0; j < rowCount; j++)
        {
            var currentVal = table[0][j];
            var pivotRowVal = table[_minRatioRow][j];

            var newValue = currentVal - (originalValue * pivotRowVal);

            DataSpawner.Instance.UpdateCellValue(0, j, newValue);
        }

        for (var i = 0; i < table.Length; i++)
        {
            for (var j = 0; j < table[i].Length; j++)
            {
                DataSpawner.Instance.UpdateCellColor(i, j, Color.black);
            }
        }

        _negativeValueIndex.Remove(_negativeValueIndex.First().Key);
        
        if (_negativeValueIndex.Count == 0) return;

        Debug.Log("Another Loop Start");
        _steps = -1;
    }
}