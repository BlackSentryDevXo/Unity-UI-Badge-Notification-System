using System.Collections.Generic;

namespace SentryToolkit
{
    [System.Serializable]
    public class BadgeState
    {
        public BadgeType Type;
    }

    [System.Serializable]
    public class BadgeChildMapping
    {
        public BadgeID Parent;
        public List<BadgeID> Children;
    }
}