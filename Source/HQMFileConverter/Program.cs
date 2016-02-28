using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HQMFileConverter
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("pass either an HQM or a XAML file");
                return 1;
            }

            IQuestLineReader reader;
            IQuestLineWriter writer;

            string inputPath = args[0];
            string outputPath;

            // if we're given a XAML file, write out the equivalent HQM file.
            // if we're given a HQM file, write out the equivalent XAML file.
            switch (Path.GetExtension(inputPath))
            {
                case ".xaml":
                    reader = new XamlQuestLineReader();
                    writer = new HQMQuestLineWriter();
                    outputPath = Path.ChangeExtension(inputPath, "hqm");
                    break;

                case ".hqm":
                    reader = new HQMQuestLineReader();
                    writer = new XamlQuestLineWriter();
                    outputPath = Path.ChangeExtension(inputPath, "xaml");
                    break;

                default:
                    Console.WriteLine("pass either an HQM or a XAML file");
                    return 1;
            }

            // visitor is irrelevant in this case -- the magic is in the reader / writer.
            Transform(inputPath, outputPath, new NullVisitor<QuestLine>(), reader, writer);
            return 0;
        }

        private static void Transform(string inputPath, string outputPath, IVisitor<QuestLine> visitor, IQuestLineReader reader, IQuestLineWriter writer)
        {
            QuestLine questLine;
            using (var input = File.OpenRead(inputPath))
            {
                questLine = reader.ReadQuestLine(input);
            }

            questLine.Accept(visitor);

            using (var output = File.OpenWrite(outputPath))
            {
                writer.WriteQuestLine(questLine, output);
            }
        }

        #region Old Placeholder Main

        // this one was "Main" as a placeholder until I found something to reliably do there.
        private static void Main2(string[] args)
        {
            XamlRoundTrip(@"C:\Temp\quests.hqm", @"C:\Temp\quests.xaml", @"C:\Temp\quests-rt.hqm");

            FixRemovedItemsIssue(hqmInput: @"C:\Temp\quests.hqm",
                                 itemDumpInput: @"C:\Temp\item.csv",
                                 hqmOutput: @"C:\Temp\quests.hqm.fixed1");

            FixDependenciesIssue(hqmInput: @"C:\Temp\quests.hqm",
                                 hqmOutput: @"C:\Temp\quests.hqm.fixed2");

            RemoveEmptyRewardBags(hqmInput: @"C:\Temp\quests.hqm",
                                  hqmOutput: @"C:\Temp\quests.hqm.fixed3");

            RenameChris(hqmInput: @"C:\Temp\quests.hqm",
                        hqmOutput: @"C:\Temp\quests.hqm.fixed4");

            ImportDescriptions(hqmInput: @"C:\Temp\q3.hqm",
                               descInput: @"C:\Temp\descriptions.txt",
                               hqmOutput: @"C:\Temp\q3-desc.hqm");

            GetItemsWithNameTags(hqmInput: @"C:\Temp\questsforrewards.hqm",
                                 hqmOutput: @"C:\Temp\questsFixed3.hqm");

            TestReadingTruncatedFile();

            RoundTrip(hqmInput: @"C:\Temp\quests.hqm",
                      hqmOutput: @"C:\Temp\quests.hqm.rt");

            // TODO: write this!  it would remove nulls in the quests array & update dependencies.
            // It would fail quietly if there have ever been saves for the old version.
            ////CompactQuests(hqmInput: "...",
            ////              hqmOutput: "...");
        }

        #endregion

        #region Just do a round-trip (the basis for all other transformations)

        private static void RoundTrip(string hqmInput, string hqmOutput)
        {
            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }

            // transformation goes here!

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }

            // delete everything else in clones!
            byte[] inputBytes = File.ReadAllBytes(hqmInput);
            byte[] outputBytes = File.ReadAllBytes(hqmOutput);
            if (!inputBytes.SequenceEqual(outputBytes))
            {
                throw new Exception("round-trip changed data!");
            }
        }

        #endregion

        #region Round-trip using XAML too.

        private static void XamlRoundTrip(string hqmInput, string xamlOutput, string hqmOutput)
        {
            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }

            using (var outputStream = File.Create(xamlOutput))
            {
                new XamlQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }

            using (var inputStream = File.OpenRead(xamlOutput))
            {
                questLine = new XamlQuestLineReader().ReadQuestLine(inputStream);
            }

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }

        #endregion

        #region Deal with invalid items (obsolete after CPW's fixes)

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

        #endregion

        #region Work around performance issue

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

        #endregion

        #region Remove empty reward bags

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

        #endregion

        #region Rename an Item Stack's NameTag

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

            questLine.Accept(new RenameChrisItemStackVisitor());

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }

        private sealed class RenameChrisItemStackVisitor : VisitorBase<ItemStack>
        {
            public override void Visit(ItemStack node)
            {
                if (node.NameTag == "Chris the Unwielding")
                {
                    node.NameTag = "Chris the Wielding";
                }
            }
        }

        #endregion

        #region Import writing work from external file (obsolete now that we have JSON)

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

        #endregion

        #region Get all items with nametags (requires breakpoint to be meaningful)

        private static void GetItemsWithNameTags(string hqmInput, string hqmOutput)
        {
            QuestLine questLine;
            using (var inputStream = File.OpenRead(hqmInput))
            {
                questLine = new HQMQuestLineReader().ReadQuestLine(inputStream);
            }

            BlockingCollection<ItemStack> items = new BlockingCollection<ItemStack>();
            MyItemStackVisitor visitor = new MyItemStackVisitor(items);
            Task.Run(() => questLine.Accept(visitor));

            foreach (var item in items.GetConsumingEnumerable().Where(item => item.ItemId != null && item.NameTag != null))
            {
                // breakpoint
                int xx = 0;
            }

            using (var outputStream = File.OpenWrite(hqmOutput))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, outputStream);
            }
        }

        private sealed class MyItemStackVisitor : VisitorBase<ItemStack>
        {
            private readonly BlockingCollection<ItemStack> coll;

            internal MyItemStackVisitor(BlockingCollection<ItemStack> coll)
            {
                this.coll = coll;
            }

            public override void Visit(ItemStack node) => this.coll.Add(node);
            public override void End() => this.coll.CompleteAdding();
        }

        #endregion

        #region Tests reading a file that HQM would have truncated by 1 byte

        private static void TestReadingTruncatedFile()
        {
            var questLine = new QuestLine
            {
                Version = 22,
                Description = "D",
                PassCode = "0",
                QuestSets = new[] { new QuestSet { Name = "*", Description = "*" } },
                Quests = new Quest[6],
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

        #endregion

        #region Miscellaneous, unused

        private static void Replace(Quest oldQuest, Quest newQuest)
        {
            newQuest.CommonRewards = oldQuest.CommonRewards;
            newQuest.Description = oldQuest.Description;
            newQuest.Icon = oldQuest.Icon;
            newQuest.IsBig = oldQuest.IsBig;
            newQuest.ModifiedParentRequirementCount = oldQuest.ModifiedParentRequirementCount;
            newQuest.PickOneRewards = oldQuest.PickOneRewards;
            newQuest.RepeatIntervalHours = oldQuest.RepeatIntervalHours;
            newQuest.RepeatType = oldQuest.RepeatType;
            newQuest.ReputationRewards = oldQuest.ReputationRewards;
            newQuest.Tasks = oldQuest.Tasks;
            newQuest.TriggerTaskCount = oldQuest.TriggerTaskCount;
            newQuest.TriggerType = oldQuest.TriggerType;
            newQuest.XPos = oldQuest.XPos;
            newQuest.YPos = oldQuest.YPos;
        }

        private sealed class QuestTaskDescriptionVisitor : VisitorBase<QuestTask>
        {
            private static readonly Dictionary<QuestTaskType, string> ItemTypeDefaults = new Dictionary<QuestTaskType, string>
            {
                [QuestTaskType.Consume] = "A task where the player can hand in items or fluids. One can also use the Quest Delivery System to submit items and fluids.",
                [QuestTaskType.Craft] = "A task where the player has to craft specific items.",
                [QuestTaskType.Detection] = "A task where the player needs specific items. These do not have to be handed in, having them in one's inventory is enough.",
            };

            private static readonly Dictionary<Type, string> TaskTypeDefaults = new Dictionary<Type, string>
            {
                [typeof(MobQuestTask)] = "A task where the player has to kill certain monsters.",
                [typeof(DeathQuestTask)] = "A task where the player has to die a certain amount of times.",
                [typeof(LocationQuestTask)] = "A task where the player has to reach one or more locations.",
                [typeof(ReputationTargetQuestTask)] = "A task where the player has to reach a certain reputation."
            };

            private readonly List<QuestTask> tasksToLookFor = new List<QuestTask>();
            internal ReadOnlyCollection<QuestTask> TasksToLookFor => this.tasksToLookFor.AsReadOnly();

            public override void Visit(QuestTask node)
            {
                string defaultDescription;
                ItemQuestTask itemQuestTask = node as ItemQuestTask;

                if (((itemQuestTask != null &&
                      ItemTypeDefaults.TryGetValue(itemQuestTask.TaskType, out defaultDescription)) ||
                     TaskTypeDefaults.TryGetValue(node.GetType(), out defaultDescription)) &&
                    defaultDescription == node.Description)
                {
                    return;
                }

                this.tasksToLookFor.Add(node);
            }
        }

        #endregion
    }
}
