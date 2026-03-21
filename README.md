# Simplex Method Implementation Guide

이 문서는 프로젝트에서 Simplex Method를 구현하고 실행하는 로직을 설명합니다.

## 1. 행렬 구현 방법

행렬은 **2차원 배열**을 이용하여 구현합니다.

**예시:** `arr2Dim[4][6]`

```text
[-3, -5, 0, 0, 0, 0]
[1, 0, 1, 0, 0, 4]
[0, 2, 0, 1, 0, 12]
[3, 2, 0, 0, 1, 18]
```

---

## 2. Simplex Method 실행 4단계

### **Step 1**

- 첫 번째 행에서 음수를 찾습니다. (`arr2Dim[0][i]` 반복문 순회)
- 찾은 음수들을 배열에 저장한 후, **절대값 기준 내림차순**으로 정렬합니다.
- 절대값이 가장 큰 값을 선정합니다. (`vector2 pivotRow`)

### **Step 2**

- 선정된 값과 같은 열에 있는 값들(`arr2Dim[j][pivotRow.x]` j에 대해 반복문 순회, `vector2 selectedValue`)을 확인합니다.
- 0이 아닌 값은 `selectedValue`와 동일한 행의 마지막 위치 값(`arr2Dim[selectedValue.y][^1]`)과 나눕니다.
- 계산된 결과 중 **가장 작은 값**을 선정합니다. (`vector2 pivotCol`)

### **Step 3**

- `pivotRow`와 `pivotCol`의 교차점(`crossPivot`)을 찾습니다.
  - `vector2 crossPivot = new vector2(pivotRow.x, pivotCol.y)`
- `crossPivot`과 같은 행의 모든 값을 `crossPivot`의 값으로 나눕니다.
  - `arr2Dim[crossPivot.y][k]`를 k에 대해 순회하며 `arr2Dim[crossPivot.y][crossPivot.x]`로 나눔
- **참고:** 마지막 위치는 Step 2에서 이미 계산되었으므로 추가로 나눌 필요가 없습니다.

### **Step 4**

- 절대값이 가장 컸던 원본 위치(`arr2Dim[pivotCol.y][pivotRow.x]`)를 0으로 만들기 위해 가우스 소거법을 실행합니다.
- 반복문 `for (var i in arr2Dim[0].Length)`를 돌리며 아래 연산을 수행합니다:
  - `arr2Dim[pivotRow.y][i] - arr2Dim[pivotCol.y][pivotRow.x] * arr2Dim[pivotRow.y][i]`

---

## 3. 반복 실행 (Next Step)

- Step 1에서 배열에 저장했던 음수들을 순서대로 순회하며, 모든 값에 대해 **Step 2 ~ Step 4를 반복 실행**합니다.
