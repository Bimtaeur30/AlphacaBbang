using UnityEngine;

namespace MemberWorkspace.JJG._02_Scripts.Item.Data
{
    public interface IUsableItem
    {
        bool Use(GameObject user);
    }
}