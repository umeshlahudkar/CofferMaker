
using UnityEngine;

public enum BlockType
{
    None = 0,
    Blue,
    Brown
}

[System.Serializable]
public class BlockData
{
    public BlockType blockType;
    public Color blockColor;
}
