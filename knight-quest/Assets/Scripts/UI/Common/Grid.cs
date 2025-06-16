using UnityEngine;
using UnityEngine.UI;

public class Grid : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public int Rows;
    public int Columns;
    public Vector2 CellSize;
    public Vector2 Spacing;
    public FitType fitType = FitType.Uniform;
    public bool FitX;
    public bool FitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();


        if (fitType != FitType.FixedColumns && fitType != FitType.FixedRows)
        {
            FitX = true;
            FitY = true;

            var sqrt = Mathf.Sqrt(transform.childCount);
            Rows = Mathf.CeilToInt(sqrt);
            Columns = Mathf.CeilToInt(sqrt);
        }

        switch (fitType)
        {
            case FitType.Width:
            case FitType.FixedColumns:
                Rows = Mathf.CeilToInt(transform.childCount / (float)Columns);
                break;
            case FitType.Height:
            case FitType.FixedRows:
                Columns = Mathf.CeilToInt(transform.childCount / (float)Rows);
                break;
        }

        var parentWidth = rectTransform.rect.width;
        var parentHeight = rectTransform.rect.height;

        var cellX = parentWidth / Columns - Spacing.x / Columns * (Columns - 1) - padding.left / (float)Columns -
                    padding.right / (float)Columns;
        var cellY = parentHeight / Rows - Spacing.y / Rows * (Rows - 1) - padding.top / (float)Rows -
                    padding.bottom / (float)Rows;


        var cellSize = new Vector2(
            FitX ? cellX : CellSize.x,
            FitY ? cellY : CellSize.y
        );

        CellSize = cellSize;

        for (var i = 0; i < rectChildren.Count; i++)
        {
            var rowCount = i / Columns;
            var columnCount = i % Columns;

            var child = rectChildren[i];

            var xPos = columnCount * cellSize.x + columnCount + Spacing.x * columnCount + padding.left;
            var yPos = rowCount * cellSize.y + rowCount + Spacing.y * rowCount + padding.top;

            SetChildAlongAxis(child, 0, xPos, cellSize.x);
            SetChildAlongAxis(child, 1, yPos, cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical() { }
    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }
}