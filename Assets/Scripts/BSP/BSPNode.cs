using UnityEngine;

public class BSPNode
{
    public RectInt rect;
    public BSPNode left;
    public BSPNode right;
    public bool hasRoom;
    public RectInt roomRect;

    public BSPNode(RectInt rect)
    {
        this.rect = rect;
        this.left = null;
        this.right = null;

        this.hasRoom = false;
        this.roomRect = new RectInt();
    }
}
