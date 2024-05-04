using UnityEngine;

/// <summary>
/// Generates the game board by instantiating block prefabs in a grid layout.
/// </summary>
public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private Block blockPrefab;        // Prefab for the individual block
    [SerializeField] private RectTransform thisTransform;  // RectTransform of the board generator
    [SerializeField] private RectTransform blockHolder;     // RectTransform to hold the instantiated blocks

    [SerializeField] private int rows;                  // Number of rows in the game board
    [SerializeField] private int columns;               // Number of columns in the game board

    /// <summary>
    /// Generates the game board with the specified number of rows and columns.
    /// </summary>
    public void GenerateBoard(GamePlayController playController)
    {
        playController.InitGrid(rows, columns);

        // Calculate the size and spacing of each block based on the board dimensions
        float totalScreenWidth = thisTransform.rect.width;
        float useableWidth = totalScreenWidth * 0.9f;
        float totalBlockSpace = useableWidth * 0.2f;
        int maxBlockInRowCol = Mathf.Max(rows, columns);
        float blockSize = (useableWidth - totalBlockSpace) / maxBlockInRowCol;
        float blockSpace = totalBlockSpace / (columns + 1);

        float startPointX = GetStartPointX(blockSize, rows, blockSpace);
        float startPointY = GetStartPointY(blockSize, columns, blockSpace);

        float currentPositionX = startPointX;
        float currentPositionY = startPointY;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Block block = Instantiate(blockPrefab, blockHolder);
                block.gameObject.name = "Block_" + i + " " + j;
                block.transform.localPosition = new Vector3(currentPositionX, currentPositionY, 0);
                block.GetComponent<RectTransform>().sizeDelta = Vector3.one * blockSize;
                block.GetComponent<BoxCollider2D>().size = Vector3.one * blockSize;

                block.SetBlock(playController.GetRandomBlockData(), i, j);
                playController.UpdateGridBlock(i, j, block);

                currentPositionX += (blockSize + blockSpace);
                block.gameObject.SetActive(true);
            }
            currentPositionX = startPointX;
            currentPositionY -= (blockSize + blockSpace);
        }

        // Set the size of the block holder to fit all the blocks
        blockHolder.sizeDelta = new Vector2(useableWidth, useableWidth);
    }

    /// <summary>
    /// Calculate the starting X position for laying out blocks in a row.
    /// </summary>
    private float GetStartPointX(float blockSize, int rowSize, float blockSpace)
    {
        float totalWidth = (blockSize * rowSize) + ((rowSize - 1) * blockSpace);
        return -((totalWidth / 2) - (blockSize / 2));
    }

    /// <summary>
    /// Calculate the starting Y position for laying out blocks in a column.
    /// </summary>
    private float GetStartPointY(float blockSize, int columnSize, float blockSpace)
    {
        float totalHeight = (blockSize * columnSize) + ((columnSize - 1) * blockSpace);
        return (totalHeight / 2) - (blockSize / 2);
    }
}