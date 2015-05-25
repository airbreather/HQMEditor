using System;

using HQMFileConverter;

namespace QuestEditor.ViewModels
{
    internal static class Conversions
    {
        #region Model to View Model

        internal static ItemStackViewModel ItemStackToItemStackViewModel(ItemStack itemStack)
        {
            if (itemStack == null)
            {
                return null;
            }

            ItemStackViewModel result = new ItemStackViewModel
            {
                ItemId = itemStack.ItemId,
                Damage = itemStack.Damage,
                Size = itemStack.Size
            };

            if (itemStack.NBT != null)
            {
                result.NBT = Convert.ToBase64String(itemStack.NBT.OriginalData);
            }

            return result;
        }

        internal static ReputationViewModel ReputationToReputationViewModel(Reputation reputation)
        {
            if (reputation == null)
            {
                return null;
            }

            ReputationViewModel result = new ReputationViewModel
            {
                Id = reputation.Id,
                Name = reputation.Name
            };

            foreach (var marker in reputation.Markers)
            {
                result.AddMarker(new ReputationMarkerViewModel { Name = marker.Name, Value = marker.Value });
            }

            return result;
        }

        #endregion

        #region View Model to Model

        internal static ItemStack ItemStackViewModelToItemStack(ItemStackViewModel itemStack)
        {
            if (itemStack == null)
            {
                return null;
            }

            ItemStack result = new ItemStack
            {
                ItemId = itemStack.ItemId,
                Damage = itemStack.Damage,
                Size = itemStack.Size
            };

            if (!String.IsNullOrEmpty(itemStack.NBT))
            {
                result.NBT = new NbtWrapper { OriginalData = Convert.FromBase64String(itemStack.NBT) };
            }

            return result;
        }

        internal static Reputation ReputationViewModelToReputation(ReputationViewModel reputation)
        {
            if (reputation == null)
            {
                return null;
            }

            Reputation result = new Reputation
            {
                Id = reputation.Id,
                Name = reputation.Name,
                Markers = new ReputationMarker[reputation.Markers.Count]
            };

            for (int markerIndex = 0; markerIndex < result.Markers.Length; markerIndex++)
            {
                var marker = reputation.Markers[markerIndex];

                var resultMarker = result.Markers[markerIndex] = new ReputationMarker();

                resultMarker.Name = marker.Name;
                resultMarker.Value = marker.Value;
            }

            return result;
        }

        #endregion
    }
}
