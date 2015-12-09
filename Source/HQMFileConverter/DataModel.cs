using fNbt;

namespace HQMFileConverter
{
    public enum ItemType : byte
    {
        Item,
        Fluid
    }

    public enum RepeatType : byte
    {
        None = 0,
        Instant = 1,
        Interval = 2,
        Cooldown = 3
    }

    public enum TriggerType : byte
    {
        Normal = 0,
        Trigger = 1,
        TaskCount = 2,
        ReverseTrigger = 3
    }

    public enum QuestTaskType : byte
    {
        Consume = 0,
        Craft = 1,
        Location = 2,
        QDS = 3,
        Detection = 4,
        KillMobs = 5,
        Deaths = 6,
        Reputation = 7,
        KillReputation = 8
    }

    public enum Visibility : byte
    {
        FullyVisible = 0,
        ShowLocation = 1,
        None = 2
    }

    public enum Color : byte
    {
        Black = 0,
        Blue = 1,
        Green = 2,
        Cyan = 3,
        Red = 4,
        Purple = 5,
        Orange = 6,
        LightGray = 7,
        Gray = 8,
        LightBlue = 9,
        Lime = 10,
        Turqouise = 11,
        Pink = 12,
        Magenta = 13,
        Yellow = 14,
        White = 15
    }

    public enum Tier : byte
    {
        Basic = 0,
        Good = 1,
        Greater = 2,
        Epic = 3,
        Legendary = 4
    }

    public sealed class QuestLine
    {
        public int Version { get; set; }
        public string PassCode { get; set; }
        public string Description { get; set; }
        public Reputation[] Reputations { get; set; }
        public QuestSet[] QuestSets { get; set; }
        public Quest[] Quests { get; set; }
        public RewardBagTier[] Tiers { get; set; }
        public RewardBag[] Bags { get; set; }
    }

    public sealed class Reputation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ReputationMarker[] Markers { get; set; }
    }

    public sealed class ReputationMarker
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public sealed class QuestSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ReputationBar[] ReputationBars { get; set; }
    }

    public sealed class ReputationBar
    {
        public int Data { get; set; }
    }

    public sealed class Quest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public bool IsBig { get; set; }
        public int QuestSetId { get; set; }
        public ItemStack Icon { get; set; }
        public int[] RequiredQuestIds { get; set; }
        public int[] OptionLinks { get; set; }
        public RepeatType RepeatType { get; set; }
        public int RepeatIntervalHours { get; set; }
        public TriggerType TriggerType { get; set; }
        public int TriggerTaskCount { get; set; }
        public int? ModifiedParentRequirementCount { get; set; }
        public QuestTask[] Tasks { get; set; }
        public ItemStack[] CommonRewards { get; set; }
        public ItemStack[] PickOneRewards { get; set; }
        public ReputationReward[] ReputationRewards { get; set; }
    }

    public sealed class ItemStack
    {
        public string ItemId { get; set; }
        public int? Size { get; set; }
        public int Damage { get; set; }
        public NbtWrapper NBT { get; set; }
    }

    public abstract class QuestTask
    {
        public QuestTaskType TaskType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public sealed class ItemQuestTask : QuestTask
    {
        public ItemRequirement[] Requirements { get; set; }
    }

    public sealed class ItemRequirement
    {
        public ItemType Type { get; set; }
        public ItemStack ItemStack { get; set; }
        public int Amount { get; set; }
        public string PrecisionId { get; set; }
    }

    public sealed class LocationQuestTask : QuestTask
    {
        public LocationRequirement[] Requirements { get; set; }
    }

    public sealed class LocationRequirement
    {
        public ItemStack Icon { get; set; }
        public string Name { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
        public int Radius { get; set; }
        public Visibility Visibility { get; set; }
        public int DimensionId { get; set; }
    }

    public sealed class MobQuestTask : QuestTask
    {
        public MobRequirement[] Requirements { get; set; }
    }

    public sealed class MobRequirement
    {
        public ItemStack Icon { get; set; }
        public string MobName { get; set; }
        public string MobID { get; set; }
        public int Amount { get; set; }
        public bool IsExact { get; set; }
    }

    public sealed class DeathQuestTask : QuestTask
    {
        public int DeathCount { get; set; }
    }

    public class ReputationTargetQuestTask : QuestTask
    {
        public ReputationTargetRequirement[] Requirements { get; set; }
    }

    public sealed class ReputationTargetRequirement
    {
        public int ReputationId { get; set; }
        public int? LowerBound { get; set; }
        public int? UpperBound { get; set; }
        public bool Inverted { get; set; }
    }

    public sealed class ReputationKillQuestTask : ReputationTargetQuestTask
    {
        public int KillCount { get; set; }
    }

    public sealed class ReputationReward
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public sealed class RewardBagTier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int BasicWeight { get; set; }
        public int GoodWeight { get; set; }
        public int GreaterWeight { get; set; }
        public int EpicWeight { get; set; }
        public int LegendaryWeight { get; set; }
    }

    public sealed class RewardBag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TierId { get; set; }
        public ItemStack[] Rewards { get; set; }
        public int? Limit { get; set; }
    }

    public sealed class NbtWrapper
    {
        public NbtCompound RootTag { get; set; }
        public byte[] OriginalData { get; set; }
        public bool Changed { get; set; }
    }
}
