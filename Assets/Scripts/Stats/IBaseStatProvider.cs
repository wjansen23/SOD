using System;

namespace RPG.Stats
{
    public interface IBaseStatProvider
    {
        event Action onLevelUp;

        float GetStatValue(Stat stat);
        int GetLevel();
    }
}
