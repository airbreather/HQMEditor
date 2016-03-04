using System;
using System.Linq;

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

    public interface IVisitor<T>
    {
        void Begin();
        void Visit(T node);
        void End();
    }

    public abstract class VisitorBase<T> : IVisitor<T>
    {
        public virtual void Begin()
        {
        }

        public virtual void End()
        {
        }

        public abstract void Visit(T node);
    }

    public sealed class NullVisitor<T> : VisitorBase<T>
    {
        public override void Visit(T node)
        {
        }
    }

    public sealed class QuestLine
    {
        public int Version { get; set; } = 22;
        public string PassCode { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public Reputation[] Reputations { get; set; } = Array.Empty<Reputation>();
        public QuestSet[] QuestSets { get; set; } = Array.Empty<QuestSet>();
        public Quest[] Quests { get; set; } = Array.Empty<Quest>();
        public RewardBagTier[] Tiers { get; set; } = Array.Empty<RewardBagTier>();
        public RewardBag[] Bags { get; set; } = Array.Empty<RewardBag>();

        public void Accept(IVisitor<QuestLine> visitor)
        {
            visitor.Begin();
            visitor.Visit(this);
            visitor.End();
        }

        public void Accept(IVisitor<Reputation> visitor)
        {
            visitor.Begin();

            foreach (var reputation in this.Reputations ?? Enumerable.Empty<Reputation>())
            {
                visitor.Visit(reputation);
            }

            visitor.End();
        }

        public void Accept(IVisitor<QuestSet> visitor)
        {
            visitor.Begin();
            foreach (var questSet in this.QuestSets ?? Enumerable.Empty<QuestSet>())
            {
                visitor.Visit(questSet);
            }

            visitor.End();
        }

        public void Accept(IVisitor<Quest> visitor)
        {
            visitor.Begin();
            foreach (var quest in (this.Quests ?? Enumerable.Empty<Quest>()).Where(quest => quest != null))
            {
                visitor.Visit(quest);
            }

            visitor.End();
        }

        public void Accept(IVisitor<RewardBagTier> visitor)
        {
            visitor.Begin();
            foreach (var tier in this.Tiers ?? Enumerable.Empty<RewardBagTier>())
            {
                visitor.Visit(tier);
            }

            visitor.End();
        }

        public void Accept(IVisitor<RewardBag> visitor)
        {
            visitor.Begin();
            foreach (var bag in this.Bags ?? Enumerable.Empty<RewardBag>())
            {
                visitor.Visit(bag);
            }

            visitor.End();
        }

        public void Accept(IVisitor<QuestTask> visitor)
        {
            var tasks = from quest in this.Quests
                        where quest != null
                        from task in quest.Tasks
                        select task;

            visitor.Begin();
            foreach (var task in tasks)
            {
                visitor.Visit(task);
            }

            visitor.End();
        }

        public void Accept(IVisitor<ItemStack> visitor)
        {
            visitor.Begin();

            var bagItems = from bag in this.Bags
                           from item in bag.Rewards
                           select item;

            visitor.Visit(bagItems, visitNulls: false);

            var questIcons = from quest in this.Quests
                             select quest?.Icon;

            visitor.Visit(questIcons, visitNulls: false);

            var questRewardsCommon = from quest in this.Quests
                                     from reward in quest?.CommonRewards ?? Enumerable.Empty<ItemStack>()
                                     select reward;

            visitor.Visit(questRewardsCommon, visitNulls: false);

            var questRewardsPickOne = from quest in this.Quests
                                      from reward in quest?.PickOneRewards ?? Enumerable.Empty<ItemStack>()
                                      select reward;

            visitor.Visit(questRewardsPickOne, visitNulls: false);

            var requirements = from quest in this.Quests
                               from task in quest?.Tasks?.OfType<ItemQuestTask>() ?? Enumerable.Empty<ItemQuestTask>()
                               from requirement in task.Requirements
                               select requirement.ItemStack;

            visitor.Visit(requirements, visitNulls: false);

            var requirementIcons1 = from quest in this.Quests
                                    from task in quest?.Tasks?.OfType<LocationQuestTask>() ?? Enumerable.Empty<LocationQuestTask>()
                                    from requirement in task.Requirements
                                    select requirement.Icon;

            visitor.Visit(requirementIcons1, visitNulls: false);

            var requirementIcons2 = from quest in this.Quests
                                    from task in quest?.Tasks?.OfType<MobQuestTask>() ?? Enumerable.Empty<MobQuestTask>()
                                    from requirement in task.Requirements
                                    select requirement.Icon;

            visitor.Visit(requirementIcons2, visitNulls: false);

            visitor.End();
        }
    }

    public sealed class Reputation
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public ReputationMarker[] Markers { get; set; } = Array.Empty<ReputationMarker>();
    }

    public sealed class ReputationMarker
    {
        public string Name { get; set; } = String.Empty;
        public int Value { get; set; }
    }

    public sealed class QuestSet
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public ReputationBar[] ReputationBars { get; set; } = Array.Empty<ReputationBar>();
    }

    public sealed class ReputationBar
    {
        public int Data { get; set; }
    }

    public sealed class Quest
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int XPos { get; set; }
        public int YPos { get; set; }
        public bool IsBig { get; set; }
        public int QuestSetId { get; set; }
        public ItemStack Icon { get; set; } = null;
        public int[] RequiredQuestIds { get; set; } = null;
        public int[] OptionLinks { get; set; } = null;
        public RepeatType RepeatType { get; set; }
        public int RepeatIntervalHours { get; set; }
        public TriggerType TriggerType { get; set; }
        public int TriggerTaskCount { get; set; }
        public int? ModifiedParentRequirementCount { get; set; }
        public QuestTask[] Tasks { get; set; } = Array.Empty<QuestTask>();
        public ItemStack[] CommonRewards { get; set; } = null;
        public ItemStack[] PickOneRewards { get; set; } = null;
        public ReputationReward[] ReputationRewards { get; set; } = Array.Empty<ReputationReward>();
    }

    public sealed class ItemStack
    {
        public string ItemId { get; set; }
        public int? Size { get; set; }
        public int Damage { get; set; }
        public NbtWrapper NBT { get; set; } = null;
        public string NameTag
        {
            get
            {
                return this.NBT?.RootTag?.Get<NbtCompound>("display")?.Get<NbtString>("Name")?.Value;
            }

            set
            {
                var nbt = this.NBT;
                if (nbt == null)
                {
                    nbt = this.NBT = new NbtWrapper();
                }

                var rootTag = nbt.RootTag;
                if (rootTag == null)
                {
                    rootTag = nbt.RootTag = new NbtCompound();
                }

                var displayTag = rootTag.Get<NbtCompound>("display");
                if (displayTag == null)
                {
                    displayTag = new NbtCompound("display");
                    rootTag.Add(displayTag);
                }

                var nameTag = displayTag.Get<NbtString>("Name");
                if (nameTag == null)
                {
                    nameTag = new NbtString("Name", String.Empty);
                    displayTag.Add(nameTag);
                }

                nbt.Changed = true;
                nameTag.Value = value;
            }
        }
    }

    public abstract class QuestTask
    {
        public QuestTaskType TaskType { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
    }

    public sealed class ItemQuestTask : QuestTask
    {
        public ItemRequirement[] Requirements { get; set; } = Array.Empty<ItemRequirement>();
    }

    public sealed class ItemRequirement
    {
        public ItemType Type { get; set; }
        public ItemStack ItemStack { get; set; }
        public int Amount { get; set; }
        public string PrecisionId { get; set; } = String.Empty;
    }

    public sealed class LocationQuestTask : QuestTask
    {
        public LocationRequirement[] Requirements { get; set; } = Array.Empty<LocationRequirement>();
    }

    public sealed class LocationRequirement
    {
        public ItemStack Icon { get; set; } = null;
        public string Name { get; set; } = String.Empty;
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
        public int Radius { get; set; }
        public Visibility Visibility { get; set; }
        public int DimensionId { get; set; }
    }

    public sealed class MobQuestTask : QuestTask
    {
        public MobRequirement[] Requirements { get; set; } = Array.Empty<MobRequirement>();
    }

    public sealed class MobRequirement
    {
        public ItemStack Icon { get; set; } = null;
        public string MobName { get; set; } = String.Empty;
        public string MobID { get; set; } = String.Empty;
        public int Amount { get; set; }
        public bool IsExact { get; set; }
    }

    public sealed class DeathQuestTask : QuestTask
    {
        public int DeathCount { get; set; }
    }

    public class ReputationTargetQuestTask : QuestTask
    {
        public ReputationTargetRequirement[] Requirements { get; set; } = Array.Empty<ReputationTargetRequirement>();
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
        public string Name { get; set; } = String.Empty;
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
        public string Name { get; set; } = String.Empty;
        public int TierId { get; set; }
        public ItemStack[] Rewards { get; set; } = Array.Empty<ItemStack>();
        public int? Limit { get; set; }
    }

    public sealed class NbtWrapper
    {
        public NbtCompound RootTag { get; set; }
        public byte[] OriginalData { get; set; } = Array.Empty<byte>();
        public bool Changed { get; set; }
    }
}
