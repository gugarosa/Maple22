using Maple2.Model.Enum;

namespace Maple2.Server.Core.Config;

public sealed class ServerSettings {
    public RatesSection Rates { get; init; } = new();
    public LootSection Loot { get; init; } = new();
    public DifficultySection Difficulty { get; init; } = new();

    public sealed class RatesSection {
        public ExpRates Exp { get; set; } = new();

        public sealed class ExpRates {
            public float Global { get; set; } = 1.0f;
            public float Kill { get; set; } = 1.0f;
            public float Quest { get; set; } = 1.0f;
            public float Dungeon { get; set; } = 1.0f;
            public float Prestige { get; set; } = 1.0f;
            public float Mastery { get; set; } = 1.0f;
        }
    }


    public sealed class LootSection {
        public float GlobalDropRate { get; set; } = 1.0f;
        public float BossDropRate { get; set; } = 1.0f;
        public float RareDropRate { get; set; } = 1.0f;
        public float MesosDropRate { get; set; } = 1.0f;
        public float MesosPerLevelMin { get; set; } = 1.0f;
        public float MesosPerLevelMax { get; set; } = 3.0f;
    }

    public sealed class DifficultySection {
        public float DamageDealtRate { get; set; } = 1.0f;
        public float DamageTakenRate { get; set; } = 1.0f;
        public float EnemyHpScale { get; set; } = 1.0f;
        public int EnemyLevelOffset { get; set; } = 0;
    }
}

public static class ServerSettingsExtensions {
    public static float ExpMultiplier(this ServerSettings settings, ExpType type) {
        float g = settings.Rates.Exp.Global;
        return type switch {
            ExpType.monster or ExpType.monsterBoss or ExpType.monsterElite or ExpType.assist or ExpType.assistBonus => g * settings.Rates.Exp.Kill,
            ExpType.quest or ExpType.epicQuest or ExpType.mission or ExpType.questEtc => g * settings.Rates.Exp.Quest,
            ExpType.dungeonClear or ExpType.dungeonBoss or ExpType.dungeonRelative => g * settings.Rates.Exp.Dungeon,
            ExpType.fishing or ExpType.gathering or ExpType.manufacturing or ExpType.arcade or ExpType.miniGame or ExpType.userMiniGame or ExpType.musicMastery1 or ExpType.musicMastery2 or ExpType.musicMastery3 or ExpType.musicMastery4 => g * settings.Rates.Exp.Mastery,
            _ => g,
        };
    }
}
