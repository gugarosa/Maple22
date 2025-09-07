using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Maple2.Server.Core.Config;
using Maple2.Server.Game.Session;
using Maple2.Model.Enum;

namespace Maple2.Server.Game.Commands;

public class ConfigCommand : GameCommand {
    public ConfigCommand(GameSession session) : base(AdminPermissions.Debug, "config", "Show or change runtime configuration.") {
        AddCommand(new ShowCommand());
        AddCommand(new ExpCommand());
        AddCommand(new LootCommand());
        AddCommand(new MesosCommand());
        AddCommand(new DifficultyCommand());
    }

    private class ShowCommand : Command {
        public ShowCommand() : base("show", "Show current rates.") {
            this.SetHandler<InvocationContext>(Handle);
        }

        private void Handle(InvocationContext ctx) {
            var s = ConfigProvider.Settings;
            ctx.Console.Out.WriteLine($"Rates:\n  EXP: global={s.Exp.Global:0.##}, kill={s.Exp.Kill:0.##}, quest={s.Exp.Quest:0.##}, dungeon={s.Exp.Dungeon:0.##}, prestige={s.Exp.Prestige:0.##}, mastery={s.Exp.Mastery:0.##}\n  Loot: global={s.Loot.GlobalDropRate:0.##}, boss={s.Loot.BossDropRate:0.##}, rare={s.Loot.RareDropRate:0.##}\n  Mesos: drop={s.Mesos.DropRate:0.##} (perLevel {s.Mesos.PerLevelMin:0.##}-{s.Mesos.PerLevelMax:0.##})\n  Difficulty: dealt={s.Difficulty.DamageDealtRate:0.##}, taken={s.Difficulty.DamageTakenRate:0.##}, enemyHp={s.Difficulty.EnemyHpScale:0.##}, enemyLvlOffset={s.Difficulty.EnemyLevelOffset}");
        }
    }

    private class ExpCommand : Command {
        public ExpCommand() : base("exp", "Set EXP rates.") {
            var key = new Argument<string>("key", () => "global", "Rate key: global|kill|quest|dungeon|prestige|mastery");
            var value = new Argument<float>("value", description: "New multiplier (e.g., 2.0)");
            AddArgument(key);
            AddArgument(value);
            this.SetHandler<InvocationContext, string, float>(Handle, key, value);
        }

        private void Handle(InvocationContext ctx, string key, float value) {
            key = key.ToLowerInvariant();
            if (value < 0f) { ctx.Console.Error.WriteLine("Value must be >= 0"); return; }
            var exp = ConfigProvider.Settings.Exp;
            switch (key) {
                case "global": exp.Global = value; break;
                case "kill": exp.Kill = value; break;
                case "quest": exp.Quest = value; break;
                case "dungeon": exp.Dungeon = value; break;
                case "prestige": exp.Prestige = value; break;
                case "mastery": exp.Mastery = value; break;
                default: ctx.Console.Error.WriteLine("Unknown key. Use: global|kill|quest|dungeon|prestige|mastery"); return;
            }
            ctx.Console.Out.WriteLine($"EXP {key} set to {value:0.##} (runtime only)");
        }
    }

    private class LootCommand : Command {
        public LootCommand() : base("loot", "Set loot rates.") {
            var key = new Argument<string>("key", () => "global", "Rate key: global|boss|rare");
            var value = new Argument<float>("value", description: "New multiplier/value");
            AddArgument(key);
            AddArgument(value);
            this.SetHandler<InvocationContext, string, float>(Handle, key, value);
        }

        private void Handle(InvocationContext ctx, string key, float value) {
            key = key.ToLowerInvariant();
            if (value < 0f) { ctx.Console.Error.WriteLine("Value must be >= 0"); return; }
            var loot = ConfigProvider.Settings.Loot;
            switch (key) {
                case "global": loot.GlobalDropRate = value; break;
                case "boss": loot.BossDropRate = value; break;
                case "rare": loot.RareDropRate = value; break;
                default: ctx.Console.Error.WriteLine("Unknown key. Use: global|boss|rare"); return;
            }
            ctx.Console.Out.WriteLine($"Loot {key} set to {value:0.##} (runtime only)");
        }
    }

    private class MesosCommand : Command {
        public MesosCommand() : base("mesos", "Set mesos rates.") {
            var key = new Argument<string>("key", () => "drop", "Key: drop|min|max");
            var value = new Argument<float>("value", description: "New multiplier/value");
            AddArgument(key);
            AddArgument(value);
            this.SetHandler<InvocationContext, string, float>(Handle, key, value);
        }

        private void Handle(InvocationContext ctx, string key, float value) {
            key = key.ToLowerInvariant();
            if (value < 0f) { ctx.Console.Error.WriteLine("Value must be >= 0"); return; }
            var mesos = ConfigProvider.Settings.Mesos;
            switch (key) {
                case "drop": mesos.DropRate = value; break;
                case "min": mesos.PerLevelMin = value; break;
                case "max": mesos.PerLevelMax = value; break;
                default: ctx.Console.Error.WriteLine("Unknown key. Use: drop|min|max"); return;
            }
            ctx.Console.Out.WriteLine($"Mesos {key} set to {value:0.##} (runtime only)");
        }
    }

    private class DifficultyCommand : Command {
        public DifficultyCommand() : base("difficulty", "Set difficulty rates.") {
            var key = new Argument<string>("key", () => "dealt", "Key: dealt|taken|enemyhp|enemylvl");
            var value = new Argument<float>("value", description: "New value (float for dealt/taken/enemyhp, integer for enemylvl)");
            AddArgument(key);
            AddArgument(value);
            this.SetHandler<InvocationContext, string, float>(Handle, key, value);
        }

        private void Handle(InvocationContext ctx, string key, float value) {
            key = key.ToLowerInvariant();
            var diff = ConfigProvider.Settings.Difficulty;
            switch (key) {
                case "dealt": if (value < 0f) { ctx.Console.Error.WriteLine("Value must be >= 0"); return; } diff.DamageDealtRate = value; break;
                case "taken": if (value < 0f) { ctx.Console.Error.WriteLine("Value must be >= 0"); return; } diff.DamageTakenRate = value; break;
                case "enemyhp": if (value < 0f) { ctx.Console.Error.WriteLine("Value must be >= 0"); return; } diff.EnemyHpScale = value; break;
                case "enemylvl": diff.EnemyLevelOffset = (int) Math.Round(value); break;
                default: ctx.Console.Error.WriteLine("Unknown key. Use: dealt|taken|enemyhp|enemylvl"); return;
            }
            ctx.Console.Out.WriteLine($"Difficulty {key} set to {value:0.##} (runtime only)");
        }
    }
}
