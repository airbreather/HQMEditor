using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

            ////ImportDescriptions(hqmInput: @"C:\Temp\q3.hqm",
            ////                   descInput: @"C:\Temp\descriptions.txt",
            ////                   hqmOutput: @"C:\Temp\q3-desc.hqm");

            RoundTrip(hqmInput: @"C:\Temp\quests.hqm",
                      hqmOutput: @"C:\Temp\quests.hqm.rt");

            TestReadingTruncatedFile();
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

        private static void ImportDescriptions(string hqmInput, string descInput, string hqmOutput)
        {
            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }

            Dictionary<int, Quest> oldQ = questLine.Quests.Where(x => x != null).ToDictionary(x => x.Id);
            Dictionary<int, Quest> newQ = new Dictionary<int, Quest>();

            string[] descLines = File.ReadAllLines(descInput);
            int i = 0;
            while (i < descLines.Length)
            {
                Match beginMatch = Regex.Match(descLines[i++], @"BEGIN(\d+) \(Name\: (.*)\)$");
                if (!beginMatch.Success)
                {
                    throw new Exception("WTF");
                }

                int id;
                if (!Int32.TryParse(beginMatch.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                {
                    throw new Exception("WTF2");
                }

                Quest quest;
                if (!oldQ.TryGetValue(id, out quest))
                {
                    throw new Exception("WTF3");
                }

                string name = beginMatch.Groups[2].Value;
                bool doIt = quest.Name != name;

                Match descriptionMatch;
                do
                {
                    descriptionMatch = Regex.Match(descLines[i++], @"Description\: (.*)$");
                }
                while (!descriptionMatch.Success);

                string description = descriptionMatch.Groups[1].Value;
                doIt |= quest.Description != description;

                if (doIt)
                {
                    newQ[id] = new Quest { Id = id, Name = name, Description = description };
                }

                Match endMatch;
                do
                {
                    endMatch = Regex.Match(descLines[i++], @"END(\d+) \(Name\: (.*)\)$");
                }
                while (!endMatch.Success);

                if (endMatch.Groups[1].Value != beginMatch.Groups[1].Value ||
                    endMatch.Groups[2].Value != beginMatch.Groups[2].Value)
                {
                    throw new Exception("WTF3");
                }
            }

            HashSet<int> changed = new HashSet<int>();
            foreach (var kvp in newQ.OrderBy(x => x.Key))
            {
                int id = kvp.Key;
                Quest newQuest = kvp.Value;
                Quest oldQuest = oldQ[id];

                if (newQuest.Name != oldQuest.Name)
                {
                    Console.WriteLine($"#{id} Name (OLD): {oldQuest.Name}");
                    Console.WriteLine($"#{id} Name (NEW): {newQuest.Name}");

                    string answer = String.Empty;
                    bool newVersion = false;
                    while (!(newVersion = String.Equals(answer, "n", StringComparison.CurrentCultureIgnoreCase)) &&
                           !String.Equals(answer, "o", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Console.Write($"#{id}, Use the [n]ew or the [o]ld? ");
                        answer = Console.ReadLine();
                    }

                    if (newVersion)
                    {
                        oldQuest.Name = newQuest.Name;
                    }
                }
                else
                {
                    Console.WriteLine($"#{id} Name: {oldQuest.Name}", id, oldQuest.Name);
                }

                if (newQuest.Description != oldQuest.Description)
                {
                    Console.WriteLine($"#{id} Description (OLD): {oldQuest.Description}");
                    Console.WriteLine($"#{id} Description (NEW): {newQuest.Description}");

                    string answer = String.Empty;
                    bool newVersion = false;
                    while (!(newVersion = String.Equals(answer, "n", StringComparison.CurrentCultureIgnoreCase)) &&
                           !String.Equals(answer, "o", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Console.Write($"#{id}, Use the [n]ew or the [o]ld? ");
                        answer = Console.ReadLine();
                    }

                    if (newVersion)
                    {
                        oldQuest.Description = newQuest.Description;
                    }
                }
            }
            
            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }

        private static void TestReadingTruncatedFile()
        {
            var questLine = new QuestLine
            {
                Version = 20,
                Description = "D",
                PassCode = "0",
                QuestSets = new[] { new QuestSet { Name = "*", Description = "*" } },
                Quests = new Quest[6],
                Reputations = new Reputation[0],
                Tiers = new RewardBagTier[0],
                Bags = new[] { new RewardBag { Name = "*", Rewards = new[] { new ItemStack { ItemId = "minecraft:stone", Size = 1 } } } }
            };

            using (var stream = new MemoryStream())
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, stream);
                stream.Seek(0, SeekOrigin.Begin);

                // scratch the last byte written, as HQM wouldn't write it in this case... see the
                // debug output at this point in time to confirm that we had idx == 0 here.
                stream.SetLength(stream.Length - 1);
                questLine = new HQMQuestLineReader().ReadQuestLine(stream);

                // see the debug output at this point in time to confirm that we had to recover from
                // a single EndOfStreamException (a first-chance exception might have been raised).
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
