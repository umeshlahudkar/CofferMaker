using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents an individual block in the game grid.
/// </summary>
public class Block : MonoBehaviour
{
    [SerializeField] private Image blockImg;                      // Image component representing the block
    [SerializeField] private BlockDirectionImg[] blockDirectionImgs; // Array of block direction images

    private int row_ID;        // Row index of the block in the grid
    private int coloum_ID;     // Column index of the block in the grid

    private BlockType blockType;   // Type of the block
    private Color blockColor;      // Color of the block

    /// <summary>
    ///  Set the properties of the block based on the provided BlockData and grid indices.
    /// </summary>
    public void SetBlock(BlockData data, int rowIndex, int columnIndex)
    {
        this.row_ID = rowIndex;
        this.coloum_ID = columnIndex;
        blockColor = data.blockColor;
        blockType = data.blockType;
        blockImg.color = data.blockColor;
    }

    /// <summary>
    /// Place a new block with the specified BlockData and activate the block game object.
    /// </summary>
    public void PlaceBlock(BlockData data)
    {
        blockType = data.blockType;
        blockImg.color = data.blockColor;
        blockColor = data.blockColor;
        gameObject.SetActive(true);
    }

    public BlockType BlockType { get { return blockType; } }

    public int Row_ID { get { return row_ID; } }

    public int Coloum_ID { get { return coloum_ID; } }

    /// <summary>
    /// Check if another block is adjacent to this block.
    /// </summary>
    public bool IsAdjacent(Block otherBlock)
    {
        int rowDiff = Mathf.Abs(this.row_ID - otherBlock.row_ID);
        int colDiff = Mathf.Abs(this.coloum_ID - otherBlock.coloum_ID);

        return (rowDiff <= 1 && colDiff <= 1);
    }

    /// <summary>
    /// Reset the block to its default state (empty block).
    /// </summary>
    public void ResetBlock()
    {
        blockImg.color = Color.white;
        blockType = BlockType.None;
        blockColor = Color.white;

        DisableDirectionImgs();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Activate the direction image corresponding to the given BlockDirection.
    /// </summary>
    public void ActivateDirectionImgByDirection(BlockDirection blockDirection)
    {
        for (int i = 0; i < blockDirectionImgs.Length; i++)
        {
            if (blockDirectionImgs[i].blockDirection == blockDirection)
            {
                blockDirectionImgs[i].blockDirImg.color = blockColor;
                blockDirectionImgs[i].blockDirImg.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    ///  Disable the direction image corresponding to the given BlockDirection.
    /// </summary>
    public void DisableDirectionImgByDirection(BlockDirection blockDirection)
    {
        for (int i = 0; i < blockDirectionImgs.Length; i++)
        {
            if (blockDirectionImgs[i].blockDirection == blockDirection)
            {
                blockDirectionImgs[i].blockDirImg.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    ///  Disable all direction images associated with the block.
    /// </summary>
    public void DisableDirectionImgs()
    {
        for (int i = 0; i < blockDirectionImgs.Length; i++)
        {
            blockDirectionImgs[i].blockDirImg.gameObject.SetActive(false);
        }
    }

    /// <summary>
    ///  Determine the direction of another block relative to this block.
    /// </summary>
    public BlockDirection GetDirection(Block otherBlock)
    {
        int rowDifference = otherBlock.row_ID - row_ID;
        int columnDifference = otherBlock.coloum_ID - coloum_ID;

        if (rowDifference == 0 && columnDifference > 0)
        {
            return BlockDirection.Right;
        }
        else if (rowDifference == 0 && columnDifference < 0)
        {
            return BlockDirection.Left;
        }
        else if (rowDifference > 0 && columnDifference == 0)
        {
            return BlockDirection.Down;
        }
        else if (rowDifference < 0 && columnDifference == 0)
        {
            return BlockDirection.Up;
        }
        else if (rowDifference > 0 && columnDifference > 0 &&
                 Mathf.Abs(rowDifference) == Mathf.Abs(columnDifference))
        {
            return BlockDirection.Diagonal_DownRight;
        }
        else if (rowDifference > 0 && columnDifference < 0 &&
                 Mathf.Abs(rowDifference) == Mathf.Abs(columnDifference))
        {
            return BlockDirection.Diagonal_DownLeft;
        }
        else if (rowDifference < 0 && columnDifference > 0 &&
                 Mathf.Abs(rowDifference) == Mathf.Abs(columnDifference))
        {
            return BlockDirection.Diagonal_UpRight;
        }
        else if (rowDifference < 0 && columnDifference < 0 &&
                 Mathf.Abs(rowDifference) == Mathf.Abs(columnDifference))
        {
            return BlockDirection.Diagonal_UpLeft;
        }
        else
        {
            return BlockDirection.Right;
        }
    }

    // Serializable class representing a block direction image.
    [System.Serializable]
    public class BlockDirectionImg
    {
        public BlockDirection blockDirection;   // Direction associated with the image
        public Image blockDirImg;               // Image component for the direction
    }
}