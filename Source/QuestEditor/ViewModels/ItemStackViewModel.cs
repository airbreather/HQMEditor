using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class ItemStackViewModel : ViewModelBase
    {
        public void CopyFrom(ItemStackViewModel other)
        {
            this.ItemId = other.itemId;
            this.Size = other.size;
            this.Damage = other.damage;
            this.NBT = other.nbt;
        }

        private string itemId;
        public string ItemId
        {
            get { return this.itemId; }
            set { this.Set(ref this.itemId, value); }
        }

        private int? size;
        public int? Size
        {
            get { return this.size; }
            set { this.Set(ref this.size, value); }
        }

        private int damage;
        public int Damage
        {
            get { return this.damage; }
            set { this.Set(ref this.damage, value); }
        }

        private string nbt;
        public string NBT
        {
            get { return this.nbt; }
            set { this.Set(ref this.nbt, value); }
        }
    }
}
