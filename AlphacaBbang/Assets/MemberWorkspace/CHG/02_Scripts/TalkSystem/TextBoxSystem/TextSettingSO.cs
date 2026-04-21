using System.Collections.Generic;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.TextBoxSystem
{
    [CreateAssetMenu(fileName = "TextSettingSO", menuName = "CHG/TextSetting", order = 0)]
    public class TextSettingSO : ScriptableObject
    {
        [TextArea(10,20)]
        public List<string> Text;
    }
}