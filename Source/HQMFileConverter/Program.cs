using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fNbt;

namespace HQMFileConverter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            FixRemovedItemsIssue(hqmInput: @"C:\Temp\quests.hqm",
                                 itemDumpInput: @"C:\Temp\item.csv",
                                 hqmOutput: @"C:\Temp\quests.hqm.fixed1");

            FixDependenciesIssue(hqmInput: @"C:\Temp\quests.hqm",
                                 hqmOutput: @"C:\Temp\quests.hqm.fixed2");

            RemoveEmptyRewardBags(hqmInput: @"C:\Temp\quests.hqm",
                                  hqmOutput: @"C:\Temp\quests.hqm.fixed3");

            RenameChris(hqmInput: @"C:\Temp\quests.hqm",
                        hqmOutput: @"C:\Temp\quests.hqm.fixed4");

            RoundTrip(hqmInput: @"C:\Temp\quests.hqm",
                      hqmOutput: @"C:\Temp\quests.hqm.rt");
        }

        private static void FixRemovedItemsIssue(string hqmInput, string itemDumpInput, string hqmOutput)
        {
            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }

            var items = new HashSet<string>(File.ReadLines(itemDumpInput)
                                                .Skip(1)
                                                .Select(line => line.Substring(0, line.IndexOf(','))),
                                            StringComparer.Ordinal);

            foreach (var quest in questLine.Quests.Where(q => q != null))
            {
                var commonRewards = quest.CommonRewards ?? Enumerable.Empty<ItemStack>();
                var pickOneRewards = quest.PickOneRewards ?? Enumerable.Empty<ItemStack>();

                bool isBad = false;

                foreach (var r in commonRewards.Concat(pickOneRewards))
                {
                    if (!items.Contains(r.ItemId))
                    {
                        isBad = true;
                        break;
                    }
                }

                if (isBad)
                {
                    quest.CommonRewards = null;
                    quest.PickOneRewards = null;
                }
            }

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }

        private static void FixDependenciesIssue(string hqmInput, string hqmOutput)
        {
            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }

            // All the super complicated ones were on this page of the book.
            foreach (var quest in questLine.Quests.Where(q => q != null && q.QuestSetId == 8))
            {
                quest.RequiredQuestIds = null;
            }

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }

        private static void RemoveEmptyRewardBags(string hqmInput, string hqmOutput)
        {
            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }

            // It's been a little while, but I think this is what I wound up doing.
            questLine.Bags = questLine.Bags
                                      .Where(bag => bag.Rewards.Length > 0)
                                      .ToArray();

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }

        private static void RenameChris(string hqmInput, string hqmOutput)
        {
            // Just a test to make sure that we can make changes here that
            // Minecraft will accept.  By default, we just re-save the original
            // serialized NBT byte array, and this library doesn't seem to
            // guarantee that the data will be bitwise identical if unchanged.

            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }
            var bag = questLine.Bags.Single(b => b.Id == 30);

            ItemStack chris = bag.Rewards[0];

            // His name was "Chris the Unwielding" before.
            chris.NBT.Changed = true;
            chris.NBT.RootTag
                     .Get<NbtCompound>("display")
                     .Get<NbtString>("Name")
                     .Value = "Chris the Wielding";

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }

        private static void RoundTrip(string hqmInput, string hqmOutput)
        {
            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }
    }
}
