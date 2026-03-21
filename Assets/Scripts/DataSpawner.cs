using UnityEngine;
using TMPro;

public class DataSpawner : MonoBehaviour
{
    public static DataSpawner Instance { get; private set; }

    [SerializeField] private Data2DObject dataObj;
    [SerializeField] private float cellWidth = 100f;
    [SerializeField] private float cellHeight = 50f;

    public float[][] RuntimeTable { get; private set; }
    private TextMeshProUGUI[][] _cellTexts;
    
    private Canvas _parentCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeRuntimeTable();
    }

    private void InitializeRuntimeTable()
    {
        if (dataObj == null || dataObj.Columns == null || dataObj.Columns.Length == 0) return;

        var rowCount = dataObj.Columns.Length;
        RuntimeTable = new float[rowCount][];
        _cellTexts = new TextMeshProUGUI[rowCount][];

        for (var i = 0; i < rowCount; i++)
        {
            var colCount = dataObj.Columns[i].rowValues.Length;
            RuntimeTable[i] = new float[colCount];
            _cellTexts[i] = new TextMeshProUGUI[colCount];
            for (var j = 0; j < colCount; j++)
            {
                RuntimeTable[i][j] = (float)dataObj.Columns[i].rowValues[j];
            }
        }
    }

    private void Start()
    {
        if (RuntimeTable == null || RuntimeTable.Length == 0) return;

        _parentCanvas = GetComponentInParent<Canvas>();
        if (_parentCanvas == null)
        {
            _parentCanvas = FindObjectOfType<Canvas>();
        }
        
        var verticalCount = RuntimeTable.Length;
        var horizontalCount = RuntimeTable[0].Length;

        var startX = -(horizontalCount - 1) * cellWidth / 2f;
        var startY = (verticalCount - 1) * cellHeight / 2f;

        for (var i = 0; i < verticalCount; i++)
        {
            for (var j = 0; j < RuntimeTable[i].Length; j++)
            {
                var cellValue = RuntimeTable[i][j];

                var dataObjComp = new GameObject($"Data_{i}_{j}");
                dataObjComp.transform.SetParent(transform, false);

                var dataText = dataObjComp.AddComponent<TextMeshProUGUI>();
                dataText.text = cellValue.ToString("F2"); // 소수점 둘째자리까지 표시
                dataText.enableAutoSizing = true;
                dataText.fontSizeMin = 8f;   // 최소 폰트 크기
                dataText.fontSizeMax = 48f;
                dataText.color = Color.black;
                dataText.alignment = TextAlignmentOptions.Center;

                _cellTexts[i][j] = dataText;

                var rectTransform = dataText.rectTransform;
                rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);

                var posX = startX + (j * cellWidth);
                var posY = startY - (i * cellHeight);

                rectTransform.anchoredPosition = new Vector2(posX, posY);
            }
        }
    }

    public void UpdateCellValue(int row, int col, float value)
    {
        if (row < 0 || row >= RuntimeTable.Length || col < 0 || col >= RuntimeTable[row].Length) return;

        RuntimeTable[row][col] = value;

        if (_cellTexts[row][col] != null)
        {
            _cellTexts[row][col].text = value.ToString("F2"); // 업데이트 시에도 소수점 표시
        }
    }

    public void UpdateCellColor(int row, int col, Color color)
    {
        if (row < 0 || row >= _cellTexts.Length || col < 0 || col >= _cellTexts[row].Length) return;

        if (_cellTexts[row][col] != null)
        {
            _cellTexts[row][col].color = color;
        }
    }
}