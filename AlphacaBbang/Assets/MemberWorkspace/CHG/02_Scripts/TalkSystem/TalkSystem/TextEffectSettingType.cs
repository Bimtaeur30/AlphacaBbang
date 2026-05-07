using System;

namespace MemberWorkspace.CHG._02_Scripts.TextBoxSystem
{
    [Flags]
    public enum TextEffectSettingType
    {
        none = 0, 
        everything = 1 << 0,
        a = 1 << 1, 
        s = 1 << 2
    }
}