using System;
using System.IO;
using System.Text;

namespace HQMFileConverter
{
    public sealed class HQMQuestLineReader : IQuestLineReader
    {
        public QuestLine ReadQuestLine(Stream inputStream)
        {
            using (var bReader = new BinaryReader(inputStream, Encoding.ASCII, leaveOpen: true))
            {
                var reader = new BitStreamReader(bReader);
                QuestLine questLine = new QuestLine();
                questLine.Version = reader.ReadInt32(8);

                if (questLine.Version != 20)
                {
                    throw new NotSupportedException("Version must be 20.");
                }

                questLine.PassCode = reader.ReadString(7);
                questLine.Description = reader.ReadString(16);
                questLine.QuestSets = new QuestSet[reader.ReadInt32(5)];

                for (int questSetIndex = 0; questSetIndex < questLine.QuestSets.Length; questSetIndex++)
                {
                    QuestSet questSet = questLine.QuestSets[questSetIndex] = new QuestSet();
                    questSet.Id = questSetIndex;
                    questSet.Name = reader.ReadString(5);
                    questSet.Description = reader.ReadString(16);
                }

                questLine.Reputations = new Reputation[reader.ReadInt32(8)];

                for (int reputationIndex = 0; reputationIndex < questLine.Reputations.Length; reputationIndex++)
                {
                    Reputation reputation = questLine.Reputations[reputationIndex] = new Reputation();
                    reputation.Id = reader.ReadInt32(8);
                    reputation.Name = reader.ReadString(5);

                    string neutralName = reader.ReadString(5);

                    int markerCount = reader.ReadInt32(5);

                    reputation.Markers = new ReputationMarker[markerCount + 1];
                    reputation.Markers[0] = new ReputationMarker { Name = neutralName, Value = 0 };

                    for (int markerIndex = 1; markerIndex < reputation.Markers.Length; markerIndex++)
                    {
                        ReputationMarker marker = reputation.Markers[markerIndex] = new ReputationMarker();

                        marker.Name = reader.ReadString(5);
                        marker.Value = reader.ReadInt32(32);
                    }
                }

                questLine.Quests = new Quest[reader.ReadInt32(10)];

                for (int questIndex = 0; questIndex < questLine.Quests.Length; questIndex++)
                {
                    // Don't skip this quest?
                    if (!reader.ReadBoolean())
                    {
                        continue;
                    }

                    Quest quest = questLine.Quests[questIndex] = new Quest();

                    quest.Id = questIndex;

                    quest.Name = reader.ReadString(5);
                    quest.Description = reader.ReadString(16);

                    quest.XPos = reader.ReadInt32(9);
                    quest.YPos = reader.ReadInt32(8);
                    quest.IsBig = reader.ReadBoolean();
                    quest.QuestSetId = reader.ReadInt32(5);

                    // Icon?
                    if (reader.ReadBoolean())
                    {
                        ItemStack icon = quest.Icon = new ItemStack();
                        icon.ItemId = reader.ReadString(16);
                        icon.Damage = reader.ReadInt32(16);
                        icon.NBT = reader.ReadNBT();
                    }

                    // Has requirements?
                    if (reader.ReadBoolean())
                    {
                        quest.RequiredQuestIds = new int[reader.ReadInt32(10)];
                        for (int requirementIndex = 0; requirementIndex < quest.RequiredQuestIds.Length; requirementIndex++)
                        {
                            quest.RequiredQuestIds[requirementIndex] = reader.ReadInt32(10);
                        }
                    }

                    // Has option links?
                    if (reader.ReadBoolean())
                    {
                        quest.OptionLinks = new int[reader.ReadInt32(10)];
                        for (int optionLinkIndex = 0; optionLinkIndex < quest.OptionLinks.Length; optionLinkIndex++)
                        {
                            quest.OptionLinks[optionLinkIndex] = reader.ReadInt32(10);
                        }
                    }

                    quest.RepeatType = (RepeatType)reader.ReadInt32(2);
                    if (quest.RepeatType == RepeatType.Cooldown ||
                        quest.RepeatType == RepeatType.Interval)
                    {
                        quest.RepeatIntervalHours = reader.ReadInt32(32);
                    }

                    quest.TriggerType = (TriggerType)reader.ReadInt32(2);
                    if (quest.TriggerType == TriggerType.TaskCount)
                    {
                        quest.TriggerTaskCount = reader.ReadInt32(4);
                    }

                    // Modified Parent Requirement?
                    if (reader.ReadBoolean())
                    {
                        quest.ModifiedParentRequirementCount = reader.ReadInt32(10);
                    }

                    quest.Tasks = new QuestTask[reader.ReadInt32(4)];
                    for (int taskIndex = 0; taskIndex < quest.Tasks.Length; taskIndex++)
                    {
                        QuestTaskType taskType = (QuestTaskType)reader.ReadInt32(4);
                        string taskName = reader.ReadString(5);
                        string taskDescription = reader.ReadString(16);

                        QuestTask task = null;
                        switch (taskType)
                        {
                            case QuestTaskType.Consume:
                            case QuestTaskType.Craft:
                            case QuestTaskType.Detection:
                            case QuestTaskType.QDS:
                            {
                                ItemQuestTask itemTask;
                                task = itemTask = new ItemQuestTask();
                                itemTask.Requirements = new ItemRequirement[reader.ReadInt32(6)];

                                for (int itemIndex = 0; itemIndex < itemTask.Requirements.Length; itemIndex++)
                                {
                                    ItemRequirement requirement = itemTask.Requirements[itemIndex] = new ItemRequirement();
                                    requirement.ItemStack = new ItemStack();

                                    // Item (as opposed to fluid)?
                                    if (reader.ReadBoolean())
                                    {
                                        requirement.Type = ItemType.Item;
                                        requirement.ItemStack.ItemId = reader.ReadString(16);
                                        requirement.ItemStack.Damage = reader.ReadInt32(16);
                                        requirement.ItemStack.NBT = reader.ReadNBT();

                                        requirement.Amount = reader.ReadInt32(32);
                                        requirement.Precision = (Detection)reader.ReadInt32(2);
                                    }
                                    else
                                    {
                                        requirement.Type = ItemType.Fluid;
                                        requirement.ItemStack.NBT = reader.ReadNBT();
                                    }
                                }

                                break;
                            }

                            case QuestTaskType.Location:
                            {
                                LocationQuestTask locationTask;
                                task = locationTask = new LocationQuestTask();
                                locationTask.Requirements = new LocationRequirement[reader.ReadInt32(3)];
                                for (int locationIndex = 0; locationIndex < locationTask.Requirements.Length; locationIndex++)
                                {
                                    LocationRequirement requirement = locationTask.Requirements[locationIndex] = new LocationRequirement();

                                    // Icon?
                                    if (reader.ReadBoolean())
                                    {
                                        ItemStack icon = requirement.Icon = new ItemStack();
                                        icon.ItemId = reader.ReadString(16);
                                        icon.Damage = reader.ReadInt32(16);
                                        icon.NBT = reader.ReadNBT();
                                    }

                                    requirement.Name = reader.ReadString(5);
                                    requirement.XPos = reader.ReadInt32(32);
                                    requirement.YPos = reader.ReadInt32(32);
                                    requirement.ZPos = reader.ReadInt32(32);
                                    requirement.Radius = reader.ReadInt32(32);

                                    requirement.Visibility = (Visibility)reader.ReadInt32(2);
                                    requirement.DimensionId = reader.ReadInt32(32);
                                }

                                break;
                            }

                            case QuestTaskType.KillMobs:
                            {
                                MobQuestTask mobTask;
                                task = mobTask = new MobQuestTask();
                                mobTask.Requirements = new MobRequirement[reader.ReadInt32(3)];
                                for (int mobIndex = 0; mobIndex < mobTask.Requirements.Length; mobIndex++)
                                {
                                    MobRequirement requirement = mobTask.Requirements[mobIndex] = new MobRequirement();

                                    // Icon?
                                    if (reader.ReadBoolean())
                                    {
                                        ItemStack icon = requirement.Icon = new ItemStack();
                                        icon.ItemId = reader.ReadString(16);
                                        icon.Damage = reader.ReadInt32(16);
                                        icon.NBT = reader.ReadNBT();
                                    }

                                    requirement.MobName = reader.ReadString(5);
                                    requirement.MobID = reader.ReadString(10);
                                    requirement.Amount = reader.ReadInt32(16);
                                    requirement.IsExact = reader.ReadBoolean();
                                }

                                break;
                            }

                            case QuestTaskType.Reputation:
                            case QuestTaskType.KillReputation:
                            {
                                ReputationTargetQuestTask repTask;
                                task = repTask = taskType == QuestTaskType.Reputation ? new ReputationTargetQuestTask() : new ReputationKillQuestTask();
                                repTask.Requirements = new ReputationTargetRequirement[reader.ReadInt32(3)];
                                for (int reputationIndex = 0; reputationIndex < repTask.Requirements.Length; reputationIndex++)
                                {
                                    ReputationTargetRequirement requirement = repTask.Requirements[reputationIndex] = new ReputationTargetRequirement();
                                    requirement.ReputationId = reader.ReadInt32(8);

                                    // Has lower bound?
                                    if (reader.ReadBoolean())
                                    {
                                        requirement.LowerBound = reader.ReadInt32(5);
                                    }

                                    // Has upper bound?
                                    if (reader.ReadBoolean())
                                    {
                                        requirement.UpperBound = reader.ReadInt32(5);
                                    }

                                    requirement.Inverted = reader.ReadBoolean();
                                }

                                if (taskType == QuestTaskType.KillReputation)
                                {
                                    ((ReputationKillQuestTask)repTask).KillCount = reader.ReadInt32(12);
                                }

                                break;
                            }

                            case QuestTaskType.Deaths:
                            {
                                DeathQuestTask deathTask;
                                task = deathTask = new DeathQuestTask();

                                deathTask.DeathCount = reader.ReadInt32(12);

                                break;
                            }
                        }

                        task.TaskType = taskType;
                        task.Name = taskName;
                        task.Description = taskDescription;
                        quest.Tasks[taskIndex] = task;
                    }

                    // Common Rewards?
                    if (reader.ReadBoolean())
                    {
                        quest.CommonRewards = new ItemStack[reader.ReadInt32(3)];
                        for (int commonRewardIndex = 0; commonRewardIndex < quest.CommonRewards.Length; commonRewardIndex++)
                        {
                            ItemStack commonReward = quest.CommonRewards[commonRewardIndex] = new ItemStack();

                            commonReward.ItemId = reader.ReadString(16);
                            commonReward.Size = reader.ReadInt32(16);
                            commonReward.Damage = reader.ReadInt32(16);
                            commonReward.NBT = reader.ReadNBT();
                        }
                    }

                    // Pick-One Rewards?
                    if (reader.ReadBoolean())
                    {
                        quest.PickOneRewards = new ItemStack[reader.ReadInt32(3)];
                        for (int pickOneRewardIndex = 0; pickOneRewardIndex < quest.PickOneRewards.Length; pickOneRewardIndex++)
                        {
                            ItemStack pickOneReward = quest.PickOneRewards[pickOneRewardIndex] = new ItemStack();

                            pickOneReward.ItemId = reader.ReadString(16);
                            pickOneReward.Size = reader.ReadInt32(16);
                            pickOneReward.Damage = reader.ReadInt32(16);
                            pickOneReward.NBT = reader.ReadNBT();
                        }
                    }

                    quest.ReputationRewards = new ReputationReward[reader.ReadInt32(3)];
                    for (int reputationRewardIndex = 0; reputationRewardIndex < quest.ReputationRewards.Length; reputationRewardIndex++)
                    {
                        ReputationReward reputationReward = quest.ReputationRewards[reputationRewardIndex] = new ReputationReward();

                        reputationReward.Id = reader.ReadInt32(8);
                        reputationReward.Value = reader.ReadInt32(32);
                    }
                }

                questLine.Tiers = new RewardBagTier[reader.ReadInt32(7)];
                for (int tierIndex = 0; tierIndex < questLine.Tiers.Length; tierIndex++)
                {
                    RewardBagTier tier = questLine.Tiers[tierIndex] = new RewardBagTier();

                    tier.Name = reader.ReadString(5);
                    tier.Color = (Color)reader.ReadInt32(4);
                    tier.BasicWeight = reader.ReadInt32(19);
                    tier.GoodWeight = reader.ReadInt32(19);
                    tier.GreaterWeight = reader.ReadInt32(19);
                    tier.EpicWeight = reader.ReadInt32(19);
                    tier.LegendaryWeight = reader.ReadInt32(19);
                }

                questLine.Bags = new RewardBag[reader.ReadInt32(10)];
                for (int bagIndex = 0; bagIndex < questLine.Bags.Length; bagIndex++)
                {
                    RewardBag bag = questLine.Bags[bagIndex] = new RewardBag();

                    bag.Id = reader.ReadInt32(10);
                    bag.Name = reader.ReadString(5);
                    bag.TierId = reader.ReadInt32(7);

                    bag.Rewards = new ItemStack[reader.ReadInt32(6)];
                    for (int rewardIndex = 0; rewardIndex < bag.Rewards.Length; rewardIndex++)
                    {
                        ItemStack reward = bag.Rewards[rewardIndex] = new ItemStack();

                        reward.ItemId = reader.ReadString(16);
                        reward.Size = reader.ReadInt32(16);
                        reward.Damage = reader.ReadInt32(16);
                        reward.NBT = reader.ReadNBT();
                    }

                    // Use a limit?
                    if (reader.ReadBoolean())
                    {
                        bag.Limit = reader.ReadInt32(10);
                    }
                }

                return questLine;
            }
        }
    }
}
