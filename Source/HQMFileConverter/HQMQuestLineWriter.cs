using System.IO;
using System.Text;

namespace HQMFileConverter
{
    public sealed class HQMQuestLineWriter : IQuestLineWriter
    {
        public void WriteQuestLine(QuestLine questLine, Stream outputStream)
        {
            using (var bWriter = new BinaryWriter(outputStream, Encoding.ASCII, leaveOpen: true))
            {
                var writer = new BitStreamWriter(bWriter);

                writer.WriteInt32(questLine.Version, 8);
                writer.WriteString(questLine.PassCode, 7);
                writer.WriteString(questLine.Description, 16);
                writer.WriteInt32(questLine.QuestSets.Length, 5);

                for (int questSetIndex = 0; questSetIndex < questLine.QuestSets.Length; questSetIndex++)
                {
                    QuestSet questSet = questLine.QuestSets[questSetIndex];

                    writer.WriteString(questSet.Name, 5);
                    writer.WriteString(questSet.Description, 16);
                }

                writer.WriteInt32(questLine.Reputations.Length, 8);

                for (int reputationIndex = 0; reputationIndex < questLine.Reputations.Length; reputationIndex++)
                {
                    Reputation reputation = questLine.Reputations[reputationIndex];
                    writer.WriteInt32(reputation.Id, 8);
                    writer.WriteString(reputation.Name, 5);

                    writer.WriteString(reputation.Markers[0].Name, 5);

                    writer.WriteInt32(reputation.Markers.Length - 1, 5);

                    for (int markerIndex = 1; markerIndex < reputation.Markers.Length; markerIndex++)
                    {
                        ReputationMarker marker = reputation.Markers[markerIndex];

                        writer.WriteString(marker.Name, 5);
                        writer.WriteInt32(marker.Value, 32);
                    }
                }

                writer.WriteInt32(questLine.Quests.Length, 10);

                for (int questIndex = 0; questIndex < questLine.Quests.Length; questIndex++)
                {
                    Quest quest = questLine.Quests[questIndex];

                    if (quest == null)
                    {
                        writer.WriteBoolean(false);
                        continue;
                    }

                    writer.WriteBoolean(true);

                    writer.WriteString(quest.Name, 5);
                    writer.WriteString(quest.Description, 16);

                    writer.WriteInt32(quest.XPos, 9);
                    writer.WriteInt32(quest.YPos, 8);
                    writer.WriteBoolean(quest.IsBig);
                    writer.WriteInt32(quest.QuestSetId, 5);

                    if (quest.Icon == null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        ItemStack icon = quest.Icon;
                        writer.WriteString(icon.ItemId, 16);
                        writer.WriteInt32(icon.Damage, 16);
                        writer.WriteNBT(icon.NBT);
                    }

                    if (quest.RequiredQuestIds == null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteInt32(quest.RequiredQuestIds.Length, 10);
                        for (int requirementIndex = 0; requirementIndex < quest.RequiredQuestIds.Length; requirementIndex++)
                        {
                            writer.WriteInt32(quest.RequiredQuestIds[requirementIndex], 10);
                        }
                    }

                    if (quest.OptionLinks == null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteInt32(quest.OptionLinks.Length, 10);
                        for (int optionLinkIndex = 0; optionLinkIndex < quest.OptionLinks.Length; optionLinkIndex++)
                        {
                            writer.WriteInt32(quest.OptionLinks[optionLinkIndex], 10);
                        }
                    }

                    writer.WriteInt32((int)quest.RepeatType, 2);
                    if (quest.RepeatType == RepeatType.Cooldown ||
                        quest.RepeatType == RepeatType.Interval)
                    {
                        writer.WriteInt32(quest.RepeatIntervalHours, 32);
                    }

                    writer.WriteInt32((int)quest.TriggerType, 2);
                    if (quest.TriggerType == TriggerType.TaskCount)
                    {
                        writer.WriteInt32(quest.TriggerTaskCount, 4);
                    }

                    if (quest.ModifiedParentRequirementCount.HasValue)
                    {
                        writer.WriteBoolean(true);
                        writer.WriteInt32(quest.ModifiedParentRequirementCount.Value, 10);
                    }
                    else
                    {
                        writer.WriteBoolean(false);
                    }

                    writer.WriteInt32(quest.Tasks.Length, 4);
                    for (int taskIndex = 0; taskIndex < quest.Tasks.Length; taskIndex++)
                    {
                        QuestTask task = quest.Tasks[taskIndex];

                        QuestTaskType taskType = task.TaskType;
                        writer.WriteInt32((int)taskType, 4);

                        writer.WriteString(task.Name, 5);
                        writer.WriteString(task.Description, 16);

                        switch (taskType)
                        {
                            case QuestTaskType.Consume:
                            case QuestTaskType.Craft:
                            case QuestTaskType.Detection:
                            case QuestTaskType.QDS:
                            {
                                ItemQuestTask itemTask = (ItemQuestTask)task;
                                writer.WriteInt32(itemTask.Requirements.Length, 6);

                                for (int itemIndex = 0; itemIndex < itemTask.Requirements.Length; itemIndex++)
                                {
                                    ItemRequirement requirement = itemTask.Requirements[itemIndex];

                                    if (requirement.Type == ItemType.Item)
                                    {
                                        writer.WriteBoolean(true);
                                        writer.WriteString(requirement.ItemStack.ItemId, 16);
                                        writer.WriteInt32(requirement.ItemStack.Damage, 16);
                                        writer.WriteNBT(requirement.ItemStack.NBT);

                                        writer.WriteInt32(requirement.Amount, 32);
                                        writer.WriteInt32((int)requirement.Precision, 2);
                                    }
                                    else
                                    {
                                        writer.WriteBoolean(false);
                                        writer.WriteNBT(requirement.ItemStack.NBT);
                                    }
                                }

                                break;
                            }

                            case QuestTaskType.Location:
                            {
                                LocationQuestTask locationTask = (LocationQuestTask)task;
                                writer.WriteInt32(locationTask.Requirements.Length, 3);
                                for (int locationIndex = 0; locationIndex < locationTask.Requirements.Length; locationIndex++)
                                {
                                    LocationRequirement requirement = locationTask.Requirements[locationIndex];

                                    if (requirement.Icon == null)
                                    {
                                        writer.WriteBoolean(false);
                                    }
                                    else
                                    {
                                        writer.WriteBoolean(true);
                                        ItemStack icon = requirement.Icon;
                                        writer.WriteString(icon.ItemId, 16);
                                        writer.WriteInt32(icon.Damage, 16);
                                        writer.WriteNBT(icon.NBT);
                                    }

                                    writer.WriteString(requirement.Name, 5);
                                    writer.WriteInt32(requirement.XPos, 32);
                                    writer.WriteInt32(requirement.YPos, 32);
                                    writer.WriteInt32(requirement.ZPos, 32);
                                    writer.WriteInt32(requirement.Radius, 32);

                                    writer.WriteInt32((int)requirement.Visibility, 2);
                                    writer.WriteInt32(requirement.DimensionId, 32);
                                }

                                break;
                            }

                            case QuestTaskType.KillMobs:
                            {
                                MobQuestTask mobTask = (MobQuestTask)task;
                                writer.WriteInt32(mobTask.Requirements.Length, 3);
                                for (int mobIndex = 0; mobIndex < mobTask.Requirements.Length; mobIndex++)
                                {
                                    MobRequirement requirement = mobTask.Requirements[mobIndex];

                                    if (requirement.Icon == null)
                                    {
                                        writer.WriteBoolean(false);
                                    }
                                    else
                                    {
                                        writer.WriteBoolean(true);
                                        ItemStack icon = requirement.Icon;
                                        writer.WriteString(icon.ItemId, 16);
                                        writer.WriteInt32(icon.Damage, 16);
                                        writer.WriteNBT(icon.NBT);
                                    }

                                    writer.WriteString(requirement.MobName, 5);
                                    writer.WriteString(requirement.MobID, 10);
                                    writer.WriteInt32(requirement.Amount, 16);
                                    writer.WriteBoolean(requirement.IsExact);
                                }

                                break;
                            }

                            case QuestTaskType.Reputation:
                            case QuestTaskType.KillReputation:
                            {
                                ReputationTargetQuestTask repTask = (ReputationTargetQuestTask)task;
                                writer.WriteInt32(repTask.Requirements.Length, 3);
                                for (int reputationIndex = 0; reputationIndex < repTask.Requirements.Length; reputationIndex++)
                                {
                                    ReputationTargetRequirement requirement = repTask.Requirements[reputationIndex];
                                    writer.WriteInt32(requirement.ReputationId, 8);

                                    if (requirement.LowerBound.HasValue)
                                    {
                                        writer.WriteBoolean(true);
                                        writer.WriteInt32(requirement.LowerBound.Value, 5);
                                    }
                                    else
                                    {
                                        writer.WriteBoolean(false);
                                    }

                                    if (requirement.UpperBound.HasValue)
                                    {
                                        writer.WriteBoolean(true);
                                        writer.WriteInt32(requirement.UpperBound.Value, 5);
                                    }
                                    else
                                    {
                                        writer.WriteBoolean(false);
                                    }

                                    writer.WriteBoolean(requirement.Inverted);
                                }

                                if (taskType == QuestTaskType.KillReputation)
                                {
                                    writer.WriteInt32(((ReputationKillQuestTask)repTask).KillCount, 12);
                                }

                                break;
                            }

                            case QuestTaskType.Deaths:
                            {
                                DeathQuestTask deathTask = (DeathQuestTask)task;

                                writer.WriteInt32(deathTask.DeathCount, 12);

                                break;
                            }
                        }
                    }

                    if (quest.CommonRewards == null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteInt32(quest.CommonRewards.Length, 3);
                        for (int commonRewardIndex = 0; commonRewardIndex < quest.CommonRewards.Length; commonRewardIndex++)
                        {
                            ItemStack commonReward = quest.CommonRewards[commonRewardIndex];

                            writer.WriteString(commonReward.ItemId, 16);
                            writer.WriteInt32(commonReward.Size.Value, 16);
                            writer.WriteInt32(commonReward.Damage, 16);
                            writer.WriteNBT(commonReward.NBT);
                        }
                    }

                    if (quest.PickOneRewards == null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteInt32(quest.PickOneRewards.Length, 3);
                        for (int pickOneRewardIndex = 0; pickOneRewardIndex < quest.PickOneRewards.Length; pickOneRewardIndex++)
                        {
                            ItemStack pickOneReward = quest.PickOneRewards[pickOneRewardIndex];

                            writer.WriteString(pickOneReward.ItemId, 16);
                            writer.WriteInt32(pickOneReward.Size.Value, 16);
                            writer.WriteInt32(pickOneReward.Damage, 16);
                            writer.WriteNBT(pickOneReward.NBT);
                        }
                    }

                    writer.WriteInt32(quest.ReputationRewards.Length, 3);
                    for (int reputationRewardIndex = 0; reputationRewardIndex < quest.ReputationRewards.Length; reputationRewardIndex++)
                    {
                        ReputationReward reputationReward = quest.ReputationRewards[reputationRewardIndex];

                        writer.WriteInt32(reputationReward.Id, 8);
                        writer.WriteInt32(reputationReward.Value, 32);
                    }
                }

                writer.WriteInt32(questLine.Tiers.Length, 7);
                for (int tierIndex = 0; tierIndex < questLine.Tiers.Length; tierIndex++)
                {
                    RewardBagTier tier = questLine.Tiers[tierIndex];

                    writer.WriteString(tier.Name, 5);
                    writer.WriteInt32((int)tier.Color, 4);
                    writer.WriteInt32(tier.BasicWeight, 19);
                    writer.WriteInt32(tier.GoodWeight, 19);
                    writer.WriteInt32(tier.GreaterWeight, 19);
                    writer.WriteInt32(tier.EpicWeight, 19);
                    writer.WriteInt32(tier.LegendaryWeight, 19);
                }

                writer.WriteInt32(questLine.Bags.Length, 10);
                for (int bagIndex = 0; bagIndex < questLine.Bags.Length; bagIndex++)
                {
                    RewardBag bag = questLine.Bags[bagIndex];

                    writer.WriteInt32(bag.Id, 10);
                    writer.WriteString(bag.Name, 5);
                    writer.WriteInt32(bag.TierId, 7);

                    writer.WriteInt32(bag.Rewards.Length, 6);
                    for (int rewardIndex = 0; rewardIndex < bag.Rewards.Length; rewardIndex++)
                    {
                        ItemStack reward = bag.Rewards[rewardIndex];

                        writer.WriteString(reward.ItemId, 16);
                        writer.WriteInt32(reward.Size.Value, 16);
                        writer.WriteInt32(reward.Damage, 16);
                        writer.WriteNBT(reward.NBT);
                    }

                    if (bag.Limit.HasValue)
                    {
                        writer.WriteBoolean(true);
                        writer.WriteInt32(bag.Limit.Value, 10);
                    }
                    else
                    {
                        writer.WriteBoolean(false);
                    }
                }

                writer.WriteFinalBits();
            }
        }
    }
}
