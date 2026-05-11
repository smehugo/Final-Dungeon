using UnityEngine;
using System.Collections.Generic;

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

    public void GetRooms(List<BSPNode> output)
    {
        if (hasRoom)
        {
            output.Add(this);
        }
        if (left != null)
        {
            left.GetRooms(output);
        }
        if (right != null)
        {
            right.GetRooms(output);
        }
    }
}
