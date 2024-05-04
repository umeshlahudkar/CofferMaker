using UnityEngine;

[CreateAssetMenu(fileName = "BlockDataSO", menuName = "SO/BlockDataSO")]
public class BlockDataSO : ScriptableObject
{
    public BlockData[] blockDatas;

    public Color GetBlockColor(BlockType blockType)
    {
        Color clr = Color.white;
        for(int i = 0; i < blockDatas.Length; i++)
        {
            if(blockDatas[i].blockType == blockType)
            {
                clr = blockDatas[i].blockColor;
            }
        }

        return clr;
    }

    public Color GetRandomBlockColor()
    {
        if (blockDatas == null || blockDatas.Length == 0)
        {
            return Color.white;
        }

        int randomIndex = UnityEngine.Random.Range(0, blockDatas.Length);
        return blockDatas[randomIndex].blockColor;
    }
}
