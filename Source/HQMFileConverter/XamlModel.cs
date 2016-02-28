using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using fNbt;

namespace HQMFileConverter
{
    public sealed class XamlQuestLine
    {
        public XamlQuestLine() { }
        public XamlQuestLine(QuestLine questLine)
        {
            this.Version = questLine.Version;
            this.PassCode = questLine.PassCode;
            this.Description = questLine.Description;
            this.Reputations.CopyFrom(questLine.Reputations, rep => new XamlReputation(rep));
            this.QuestSets.CopyFrom(questLine.QuestSets, set => new XamlQuestSet(set));
            this.Quests.CopyFrom(questLine.Quests, quest => quest == null ? null : new XamlQuest(quest));
            this.Tiers.CopyFrom(questLine.Tiers, tier => new XamlRewardBagTier(tier));
            this.Bags.CopyFrom(questLine.Bags, bag => new XamlRewardBag(bag));
        }

        public int Version { get; set; }
        public string PassCode { get; set; }
        public string Description { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlReputation> Reputations { get; } = new Collection<XamlReputation>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlQuestSet> QuestSets { get; } = new Collection<XamlQuestSet>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlQuest> Quests { get; } = new Collection<XamlQuest>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlRewardBagTier> Tiers { get; } = new Collection<XamlRewardBagTier>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlRewardBag> Bags { get; } = new Collection<XamlRewardBag>();

        public QuestLine ToQuestLine() => new QuestLine
        {
            Version = this.Version,
            PassCode = this.PassCode,
            Description = this.Description,
            Reputations = this.Reputations.ConvertToArray(rep => rep.ToReputation()),
            QuestSets = this.QuestSets.ConvertToArray(questSet => questSet.ToQuestSet()),
            Quests = this.Quests.ConvertToArray(quest => quest == null ? null : quest.ToQuest()),
            Tiers = this.Tiers.ConvertToArray(tier => tier.ToRewardBagTier()),
            Bags = this.Bags.ConvertToArray(bag => bag.ToRewardBag())
        };
    }

    public sealed class XamlReputation
    {
        public XamlReputation() { }
        public XamlReputation(Reputation reputation)
        {
            this.Id = reputation.Id;
            this.Name = reputation.Name;
            this.Markers.CopyFrom(reputation.Markers, mark => new XamlReputationMarker(mark));
        }

        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlReputationMarker> Markers { get; } = new Collection<XamlReputationMarker>();

        public Reputation ToReputation() => new Reputation
        {
            Id = this.Id,
            Name = this.Name,
            Markers = this.Markers.ConvertToArray(marker => marker.ToReputationMarker())
        };
    }

    public sealed class XamlReputationMarker
    {
        public XamlReputationMarker() { }
        public XamlReputationMarker(ReputationMarker marker)
        {
            this.Name = marker.Name;
            this.Value = marker.Value;
        }

        public string Name { get; set; } = String.Empty;
        public int Value { get; set; }

        public ReputationMarker ToReputationMarker() => new ReputationMarker
        {
            Name = this.Name,
            Value = this.Value
        };
    }

    public sealed class XamlQuestSet
    {
        public XamlQuestSet() { }
        public XamlQuestSet(QuestSet questSet)
        {
            this.Id = questSet.Id;
            this.Name = questSet.Name;
            this.Description = questSet.Description;
            this.ReputationBars.CopyFrom(questSet.ReputationBars, bar => new XamlReputationBar(bar));
        }

        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlReputationBar> ReputationBars { get; } = new Collection<XamlReputationBar>();

        public QuestSet ToQuestSet() => new QuestSet
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            ReputationBars = this.ReputationBars.ConvertToArray(bar => bar.ToReputationBar())
        };
    }

    public sealed class XamlReputationBar
    {
        public XamlReputationBar() { }
        public XamlReputationBar(ReputationBar bar)
        {
            this.Data = bar.Data;
        }

        public int Data { get; set; }

        public ReputationBar ToReputationBar() => new ReputationBar
        {
            Data = this.Data
        };
    }

    public sealed class XamlQuest
    {
        public XamlQuest() { }
        public XamlQuest(Quest quest)
        {
            this.Id = quest.Id;
            this.Name = quest.Name;
            this.Description = quest.Description;
            this.XPos = quest.XPos;
            this.YPos = quest.YPos;
            this.IsBig = quest.IsBig;
            this.QuestSetId = quest.QuestSetId;
            this.Icon = quest.Icon == null ? null : new XamlItemStack(quest.Icon);
            this.RequiredQuestIds.CopyFrom(quest.RequiredQuestIds ?? Enumerable.Empty<int>());
            this.OptionLinks.CopyFrom(quest.OptionLinks ?? Enumerable.Empty<int>());
            this.RepeatType = quest.RepeatType;
            this.RepeatIntervalHours = quest.RepeatIntervalHours;
            this.TriggerType = quest.TriggerType;
            this.TriggerTaskCount = quest.TriggerTaskCount;
            this.ModifiedParentRequirementCount = quest.ModifiedParentRequirementCount;
            this.Tasks.CopyFrom(quest.Tasks, XamlQuestTask.CopyFrom);
            this.CommonRewards.CopyFrom(quest.CommonRewards ?? Enumerable.Empty<ItemStack>(), reward => new XamlItemStack(reward));
            this.PickOneRewards.CopyFrom(quest.PickOneRewards ?? Enumerable.Empty<ItemStack>(), reward => new XamlItemStack(reward));
            this.ReputationRewards.CopyFrom(quest.ReputationRewards, reward => new XamlReputationReward(reward));
        }

        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int XPos { get; set; }
        public int YPos { get; set; }

        [DefaultValue(false)]
        public bool IsBig { get; set; }

        public int QuestSetId { get; set; }

        [DefaultValue(null)]
        public XamlItemStack Icon { get; set; } = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<int> RequiredQuestIds { get; } = new Collection<int>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<int> OptionLinks { get; } = new Collection<int>();

        [DefaultValue(RepeatType.None)]
        public RepeatType RepeatType { get; set; }

        [DefaultValue(0)]
        public int RepeatIntervalHours { get; set; }

        public TriggerType TriggerType { get; set; }

        [DefaultValue(0)]
        public int TriggerTaskCount { get; set; }

        [DefaultValue(null)]
        public int? ModifiedParentRequirementCount { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlQuestTask> Tasks { get; } = new Collection<XamlQuestTask>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlItemStack> CommonRewards { get; } = new Collection<XamlItemStack>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlItemStack> PickOneRewards { get; } = new Collection<XamlItemStack>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlReputationReward> ReputationRewards { get; } = new Collection<XamlReputationReward>();

        public Quest ToQuest() => new Quest
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            XPos = this.XPos,
            YPos = this.YPos,
            IsBig = this.IsBig,
            QuestSetId = this.QuestSetId,
            Icon = this.Icon?.ToItemStack(),
            RequiredQuestIds = this.RequiredQuestIds.ToArray().NullIfEmpty(),
            OptionLinks = this.OptionLinks.ToArray().NullIfEmpty(),
            RepeatType = this.RepeatType,
            RepeatIntervalHours = this.RepeatIntervalHours,
            TriggerType = this.TriggerType,
            TriggerTaskCount = this.TriggerTaskCount,
            ModifiedParentRequirementCount = this.ModifiedParentRequirementCount,
            Tasks = this.Tasks.ConvertToArray(t => t.ToQuestTask()),
            CommonRewards = this.CommonRewards.ConvertToArray(r => r.ToItemStack()).NullIfEmpty(),
            PickOneRewards = this.PickOneRewards.ConvertToArray(r => r.ToItemStack()).NullIfEmpty(),
            ReputationRewards = this.ReputationRewards.ConvertToArray(r => r.ToReputationReward())
        };
    }

    public sealed class XamlItemStack
    {
        public XamlItemStack() { }
        public XamlItemStack(ItemStack stack)
        {
            this.ItemId = stack.ItemId;
            this.Size = stack.Size;
            this.Damage = stack.Damage;
            if (stack.NBT != null)
            {
                this.NBTData = stack.NBT.Changed ? new NbtFile(stack.NBT.RootTag).SaveToBuffer(NbtCompression.GZip) : stack.NBT.OriginalData;
            }
        }

        public string ItemId { get; set; }

        [DefaultValue(null)]
        public int? Size { get; set; }

        [DefaultValue(0)]
        public int Damage { get; set; }

        [DefaultValue(null)]
        public string NBTBase64
        {
            get { return this.NBTData == null ? null : Convert.ToBase64String(this.NBTData); }
            set { this.NBTData = value == null ? null : Convert.FromBase64String(value); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] NBTData { get; set; } = null;

        public ItemStack ToItemStack()
        {
            NbtWrapper nbt = null;
            if (this.NBTData != null)
            {
                var nbtFile = new NbtFile();
                nbtFile.LoadFromBuffer(this.NBTData, 0, this.NBTData.Length, NbtCompression.GZip);

                nbt = new NbtWrapper { RootTag = nbtFile.RootTag, OriginalData = this.NBTData };
            }

            return new ItemStack
            {
                ItemId = this.ItemId,
                Size = this.Size,
                Damage = this.Damage,
                NBT = nbt
            };
        }
    }

    public abstract class XamlQuestTask
    {
        protected XamlQuestTask() { }
        protected XamlQuestTask(QuestTask task)
        {
            this.TaskType = task.TaskType;
            this.Name = task.Name;
            this.Description = task.Description;
        }

        public QuestTaskType TaskType { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        protected abstract QuestTask CreateQuestTask();
        public QuestTask ToQuestTask()
        {
            QuestTask questTask = this.CreateQuestTask();

            questTask.TaskType = this.TaskType;
            questTask.Name = this.Name;
            questTask.Description = this.Description;

            return questTask;
        }

        public static XamlQuestTask CopyFrom(QuestTask task)
        {
            ItemQuestTask itemQuestTask = task as ItemQuestTask;
            if (itemQuestTask != null)
            {
                return new XamlItemQuestTask(itemQuestTask);
            }

            LocationQuestTask locationQuestTask = task as LocationQuestTask;
            if (locationQuestTask != null)
            {
                return new XamlLocationQuestTask(locationQuestTask);
            }

            MobQuestTask mobQuestTask = task as MobQuestTask;
            if (mobQuestTask != null)
            {
                return new XamlMobQuestTask(mobQuestTask);
            }

            DeathQuestTask deathQuestTask = task as DeathQuestTask;
            if (deathQuestTask != null)
            {
                return new XamlDeathQuestTask(deathQuestTask);
            }

            ReputationKillQuestTask reputationKillQuestTask = task as ReputationKillQuestTask;
            if (reputationKillQuestTask != null)
            {
                return new XamlReputationKillQuestTask(reputationKillQuestTask);
            }

            ReputationTargetQuestTask reputationTargetQuestTask = task as ReputationTargetQuestTask;
            if (reputationTargetQuestTask != null)
            {
                return new XamlReputationTargetQuestTask(reputationTargetQuestTask);
            }

            throw new NotImplementedException("Apparently I missed something.");
        }
    }

    public sealed class XamlItemQuestTask : XamlQuestTask
    {
        public XamlItemQuestTask() { }
        public XamlItemQuestTask(ItemQuestTask task) : base(task)
        {
            this.Requirements.CopyFrom(task.Requirements, req => new XamlItemRequirement(req));
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlItemRequirement> Requirements { get; } = new Collection<XamlItemRequirement>();

        protected override QuestTask CreateQuestTask() => new ItemQuestTask
        {
            Requirements = this.Requirements.ConvertToArray(x => x.ToItemRequirement())
        };
    }

    public sealed class XamlItemRequirement
    {
        public XamlItemRequirement() { }
        public XamlItemRequirement(ItemRequirement req)
        {
            this.Type = req.Type;
            this.ItemStack = new XamlItemStack(req.ItemStack);
            this.Amount = req.Amount;
            this.PrecisionId = req.PrecisionId;
        }

        public ItemType Type { get; set; }
        public XamlItemStack ItemStack { get; set; }
        public int Amount { get; set; }
        public string PrecisionId { get; set; } = String.Empty;

        public ItemRequirement ToItemRequirement() => new ItemRequirement
        {
            Type = this.Type,
            ItemStack = this.ItemStack?.ToItemStack(),
            Amount = this.Amount,
            PrecisionId = this.PrecisionId
        };
    }

    public sealed class XamlLocationQuestTask : XamlQuestTask
    {
        public XamlLocationQuestTask() { }
        public XamlLocationQuestTask(LocationQuestTask task) : base(task)
        {
            this.Requirements.CopyFrom(task.Requirements, req => new XamlLocationRequirement(req));
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlLocationRequirement> Requirements { get; } = new Collection<XamlLocationRequirement>();

        protected override QuestTask CreateQuestTask() => new LocationQuestTask
        {
            Requirements = this.Requirements.ConvertToArray(req => req.ToLocationRequirement())
        };
    }

    public sealed class XamlLocationRequirement
    {
        public XamlLocationRequirement() { }
        public XamlLocationRequirement(LocationRequirement req)
        {
            this.Icon = req.Icon == null ? null : new XamlItemStack(req.Icon);
            this.Name = req.Name;
            this.XPos = req.XPos;
            this.YPos = req.YPos;
            this.ZPos = req.ZPos;
            this.Radius = req.Radius;
            this.Visibility = req.Visibility;
            this.DimensionId = req.DimensionId;
        }

        [DefaultValue(null)]
        public XamlItemStack Icon { get; set; } = null;
        public string Name { get; set; } = String.Empty;
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int ZPos { get; set; }
        public int Radius { get; set; }
        public Visibility Visibility { get; set; }
        public int DimensionId { get; set; }

        public LocationRequirement ToLocationRequirement() => new LocationRequirement
        {
            Icon = this.Icon?.ToItemStack(),
            Name = this.Name,
            XPos = this.XPos,
            YPos = this.YPos,
            ZPos = this.ZPos,
            Radius = this.Radius,
            Visibility = this.Visibility,
            DimensionId = this.DimensionId
        };
    }

    public sealed class XamlMobQuestTask : XamlQuestTask
    {
        public XamlMobQuestTask() { }
        public XamlMobQuestTask(MobQuestTask task) : base(task)
        {
            this.Requirements.CopyFrom(task.Requirements, req => new XamlMobRequirement(req));
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlMobRequirement> Requirements { get; } = new Collection<XamlMobRequirement>();

        protected override QuestTask CreateQuestTask() => new MobQuestTask
        {
            Requirements = this.Requirements.ConvertToArray(r => r.ToMobRequirement())
        };
    }

    public sealed class XamlMobRequirement
    {
        public XamlMobRequirement() { }
        public XamlMobRequirement(MobRequirement req)
        {
            this.Icon = req.Icon == null ? null : new XamlItemStack(req.Icon);
            this.MobName = req.MobName;
            this.MobID = req.MobID;
            this.Amount = req.Amount;
            this.IsExact = req.IsExact;
        }

        [DefaultValue(null)]
        public XamlItemStack Icon { get; set; } = null;
        public string MobName { get; set; } = String.Empty;
        public string MobID { get; set; } = String.Empty;
        public int Amount { get; set; }
        public bool IsExact { get; set; }

        public MobRequirement ToMobRequirement() => new MobRequirement
        {
            Icon = this.Icon?.ToItemStack(),
            MobName = this.MobName,
            MobID = this.MobID,
            Amount = this.Amount,
            IsExact = this.IsExact
        };
    }

    public sealed class XamlDeathQuestTask : XamlQuestTask
    {
        public XamlDeathQuestTask() { }
        public XamlDeathQuestTask(DeathQuestTask task) : base(task)
        {
            this.DeathCount = task.DeathCount;
        }

        public int DeathCount { get; set; }

        protected override QuestTask CreateQuestTask() => new DeathQuestTask
        {
            DeathCount = this.DeathCount
        };
    }

    public class XamlReputationTargetQuestTask : XamlQuestTask
    {
        public XamlReputationTargetQuestTask() { }
        public XamlReputationTargetQuestTask(ReputationTargetQuestTask task) : base(task)
        {
            this.Requirements.CopyFrom(task.Requirements, req => new XamlReputationTargetRequirement(req));
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlReputationTargetRequirement> Requirements { get; } = new Collection<XamlReputationTargetRequirement>();

        protected override QuestTask CreateQuestTask() => new ReputationTargetQuestTask
        {
            Requirements = this.Requirements.ConvertToArray(req => req.ToReputationTargetRequirement())
        };
    }

    public sealed class XamlReputationTargetRequirement
    {
        public XamlReputationTargetRequirement() { }
        public XamlReputationTargetRequirement(ReputationTargetRequirement requirement)
        {
            this.ReputationId = requirement.ReputationId;
            this.LowerBound = requirement.LowerBound;
            this.UpperBound = requirement.UpperBound;
            this.Inverted = requirement.Inverted;
        }

        public int ReputationId { get; set; }
        public int? LowerBound { get; set; }
        public int? UpperBound { get; set; }
        public bool Inverted { get; set; }

        public ReputationTargetRequirement ToReputationTargetRequirement() => new ReputationTargetRequirement
        {
            ReputationId = this.ReputationId,
            LowerBound = this.LowerBound,
            UpperBound = this.UpperBound,
            Inverted = this.Inverted
        };
    }

    public sealed class XamlReputationKillQuestTask : XamlReputationTargetQuestTask
    {
        public XamlReputationKillQuestTask() { }
        public XamlReputationKillQuestTask(ReputationKillQuestTask task) : base(task)
        {
            this.KillCount = task.KillCount;
        }

        public int KillCount { get; set; }

        protected override QuestTask CreateQuestTask() => new ReputationKillQuestTask
        {
            Requirements = this.Requirements.ConvertToArray(req => req.ToReputationTargetRequirement()),
            KillCount = this.KillCount
        };
    }

    public sealed class XamlReputationReward
    {
        public XamlReputationReward() { }
        public XamlReputationReward(ReputationReward reward)
        {
            this.Id = reward.Id;
            this.Value = reward.Value;
        }

        public int Id { get; set; }
        public int Value { get; set; }

        public ReputationReward ToReputationReward() => new ReputationReward
        {
            Id = this.Id,
            Value = this.Value
        };
    }

    public sealed class XamlRewardBagTier
    {
        public XamlRewardBagTier() { }
        public XamlRewardBagTier(RewardBagTier tier)
        {
            this.Id = tier.Id;
            this.Name = tier.Name;
            this.Color = tier.Color;
            this.BasicWeight = tier.BasicWeight;
            this.GoodWeight = tier.GoodWeight;
            this.GreaterWeight = tier.GreaterWeight;
            this.EpicWeight = tier.EpicWeight;
            this.LegendaryWeight = tier.LegendaryWeight;
        }

        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public Color Color { get; set; }
        public int BasicWeight { get; set; }
        public int GoodWeight { get; set; }
        public int GreaterWeight { get; set; }
        public int EpicWeight { get; set; }
        public int LegendaryWeight { get; set; }

        public RewardBagTier ToRewardBagTier() => new RewardBagTier
        {
            Id = this.Id,
            Name = this.Name,
            Color = this.Color,
            BasicWeight = this.BasicWeight,
            GoodWeight = this.GoodWeight,
            GreaterWeight = this.GreaterWeight,
            EpicWeight = this.EpicWeight,
            LegendaryWeight = this.LegendaryWeight
        };
    }

    public sealed class XamlRewardBag
    {
        public XamlRewardBag() { }
        public XamlRewardBag(RewardBag bag)
        {
            this.Id = bag.Id;
            this.Name = bag.Name;
            this.TierId = bag.TierId;
            this.Rewards.CopyFrom(bag.Rewards, reward => new XamlItemStack(reward));
            this.Limit = bag.Limit;
        }

        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public int TierId { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<XamlItemStack> Rewards { get; } = new Collection<XamlItemStack>();

        [DefaultValue(null)]
        public int? Limit { get; set; }

        public RewardBag ToRewardBag() => new RewardBag
        {
            Id = this.Id,
            Name = this.Name,
            TierId = this.TierId,
            Rewards = this.Rewards.ConvertToArray(reward => reward.ToItemStack()),
            Limit = this.Limit
        };
    }
}
