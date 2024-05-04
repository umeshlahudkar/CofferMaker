using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the core logic of the game
/// </summary>
public class GamePlayController : MonoBehaviour
{
    [SerializeField] private BlockDataSO blockDataSO;  // Reference to the ScriptableObject holding block data
    [SerializeField] private BlockType[] blockTypes;   // Array of available block types
    [SerializeField] private LayerMask blockLayer;     // Layer mask for raycasting to detect blocks

    [SerializeField] private BoardGenerator boardGenerator;  // Reference to the board generator script

    private Block[,] grid;         // 2D array to hold the grid of blocks
    private int row;               // Number of rows in the grid
    private int col;               // Number of columns in the grid

    private List<Block> selectedBlock = new List<Block>();  // List to hold selected blocks during gameplay

    private void Start()
    {
        boardGenerator.GenerateBoard(this);
    }

    /// <summary>
    /// Initialize the grid with given row and column dimensions
    /// </summary>
    public void InitGrid(int _row, int _col)
    {
        row = _row;
        col = _col;
        grid = new Block[row, col];
    }

    /// <summary>
    /// Update the grid with a specific block at given row and column indices
    /// </summary>
    public void UpdateGridBlock(int row, int col, Block block)
    {
        grid[row, col] = block;
    }

    /// <summary>
    /// Generate a random block data with a random block type and corresponding color
    /// </summary>
    public BlockData GetRandomBlockData()
    {
        BlockData blockData = new BlockData();
        blockData.blockType = blockTypes[Random.Range(0, blockTypes.Length)];
        blockData.blockColor = blockDataSO.GetBlockColor(blockData.blockType);

        return blockData;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnPointerDown();
        }
        else if (Input.GetMouseButton(0))
        {
            OnPointerMoved();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnPointerUp();
        }
    }

    /// <summary>
    /// Handles the mouse pointer down event. Adds the clicked block to the selected block list if a block is clicked.
    /// </summary>
    private void OnPointerDown()
    {
        Block block = GetClickedBlock();
        if (block != null)
        {
            selectedBlock.Add(block);
        }
    }

    /// <summary>
    /// Handles the mouse pointer moved event. Manages the logic for selecting blocks and updating direction indicators.
    /// </summary>
    private void OnPointerMoved()
    {
        // If no block is currently selected, attempt to select a block when the pointer moves
        if (selectedBlock.Count == 0)
        {
            OnPointerDown();
        }
        else
        {
            Block block = GetClickedBlock();

            // Check if the clicked block matches the type of the initially selected block and is adjacent to the last selected block
            if (block != null && block.BlockType == selectedBlock[0].BlockType && selectedBlock[selectedBlock.Count - 1] != block)
            {
                // Handle selecting and deselecting blocks based on adjacency and matching types
                if (selectedBlock.Contains(block))
                {
                    for (int i = selectedBlock.Count - 1; i >= 0; i--)
                    {
                        if (selectedBlock[i] != block)
                        {
                            selectedBlock[i].DisableDirectionImgs();
                            selectedBlock.RemoveAt(i);
                        }
                        else
                        {
                            // Deselect previous blocks and activate direction indicators for the selected chain of blocks
                            selectedBlock[i].DisableDirectionImgs();
                            if (selectedBlock.Count > 1)
                            {
                                BlockDirection direction = selectedBlock[i - 1].GetDirection(selectedBlock[i]);
                                selectedBlock[i].ActivateDirectionImgByDirection(GetReverseDirection(direction));
                            }
                            break;
                        }
                    }
                }
                else if (block.IsAdjacent(selectedBlock[selectedBlock.Count - 1]))
                {
                    // Activate direction indicators between the last selected block and the newly selected block
                    BlockDirection direction = selectedBlock[selectedBlock.Count - 1].GetDirection(block);
                    selectedBlock[selectedBlock.Count - 1].ActivateDirectionImgByDirection(direction);
                    block.ActivateDirectionImgByDirection(GetReverseDirection(direction));
                    selectedBlock.Add(block);
                }
            }
        }
    }

    /// <summary>
    /// Handles the mouse pointer up event. Initiates block clearing and replacement logic if a sufficient number of blocks are selected.
    /// </summary>
    private void OnPointerUp()
    {
        // If enough blocks are selected, reset and clear them, then trigger block replacement logic
        if (selectedBlock.Count >= 3)
        {
            for (int i = 0; i < selectedBlock.Count; i++)
            {
                selectedBlock[i].ResetBlock();
            }

            PlaceNewBlock();
        }

        // Disable all direction indicators after processing selected blocks
        for (int i = 0; i < selectedBlock.Count; i++)
        {
            selectedBlock[i].DisableDirectionImgs();
        }

        // Clear the list of selected blocks
        selectedBlock.Clear();
    }

    /// <summary>
    /// Returns the reverse direction of the given block direction
    /// </summary>
    private BlockDirection GetReverseDirection(BlockDirection direction)
    {
        switch (direction)
        {
            case BlockDirection.Right:
                return BlockDirection.Left;

            case BlockDirection.Left:
                return BlockDirection.Right;

            case BlockDirection.Up:
                return BlockDirection.Down;

            case BlockDirection.Down:
                return BlockDirection.Up;

            case BlockDirection.Diagonal_UpRight:
                return BlockDirection.Diagonal_DownLeft;

            case BlockDirection.Diagonal_UpLeft:
                return BlockDirection.Diagonal_DownRight;

            case BlockDirection.Diagonal_DownRight:
                return BlockDirection.Diagonal_UpLeft;

            case BlockDirection.Diagonal_DownLeft:
                return BlockDirection.Diagonal_UpRight;
        }

        return BlockDirection.Right;
    }

    /// <summary>
    /// Raycasts from the mouse position to detect and return a clicked block
    /// </summary>
    private Block GetClickedBlock()
    {
        Block block = null;
        RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.down, Mathf.Infinity, blockLayer);
        if (hit.collider != null)
        {
            block = hit.collider.gameObject.GetComponent<Block>();
        }
        return block;
    }

    /// <summary>
    /// Places new blocks based on selected blocks and clears rows that have been fully selected
    /// </summary>
    private void PlaceNewBlock()
    {
        List<Vector2> clearedBlock = new List<Vector2>();

        // Identify cols that need to be cleared based on selected blocks
        for (int i = 0; i < selectedBlock.Count; i++)
        {
            if (!IsColumnPresent(clearedBlock, selectedBlock[i].Coloum_ID))
            {
                clearedBlock.Add(new Vector2(selectedBlock[i].Row_ID, selectedBlock[i].Coloum_ID));
            }
        }

        // Process each cleared row to move blocks down and replace empty spaces
        for (int i = 0; i < clearedBlock.Count; i++)
        {
            List<Block> emptyBlocks = new List<Block>();

            // Find empty blocks in the current column for the cleared row
            for (int j = row - 1; j >= 0; j--)
            {
                if (grid[j, (int)clearedBlock[i].y].BlockType == BlockType.None)
                {
                    emptyBlocks.Add(grid[j, (int)clearedBlock[i].y]);
                }
            }

            // Start coroutine to move blocks down and fill empty spaces
            StartCoroutine(MoveColoums((int)clearedBlock[i].y, emptyBlocks));
        }
    }

    /// <summary>
    /// Coroutine to animate the movement of blocks downwards to fill empty spaces
    /// </summary>
    private IEnumerator MoveColoums(int coloum, List<Block> emptyBlocks)
    {
        int iteration = emptyBlocks.Count;

        for (int i = 0; i < iteration; i++)
        {
            yield return new WaitForSeconds(0.15f);

            // Move each empty block down by one row
            for (int j = emptyBlocks[emptyBlocks.Count - 1].Row_ID; j >= 0; j--)
            {
                BlockData blockData = new BlockData();

                // If moving to the topmost row, generate a new random block
                if (j - 1 < 0)
                {
                    blockData = GetRandomBlockData();
                }
                else
                {
                    // Move the block data from the row above to the current row
                    blockData.blockType = grid[j - 1, coloum].BlockType;
                    blockData.blockColor = blockDataSO.GetBlockColor(blockData.blockType);
                    grid[j - 1, coloum].ResetBlock();
                }

                // Place the block data into the current row and column
                grid[j, coloum].PlaceBlock(blockData);
            }

            // Remove the processed empty block from the list
            emptyBlocks.RemoveAt(emptyBlocks.Count - 1);
        }
    }

    /// <summary>
    /// Checks if a specific column is already present in the list of cleared blocks
    /// </summary>
    private bool IsColumnPresent(List<Vector2> clearedBlock, int coloum)
    {
        for (int i = 0; i < clearedBlock.Count; i++)
        {
            if (clearedBlock[i].y == coloum)
            {
                return true;
            }
        }
        return false;
    }
}

