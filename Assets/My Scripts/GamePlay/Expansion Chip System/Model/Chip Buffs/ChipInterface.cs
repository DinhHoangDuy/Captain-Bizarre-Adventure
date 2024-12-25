public interface IChip
{
    ExpansionChipSlot expansionChipSlot { get; set; }
    ExpansionChipStatus expansionChipStatus { get; set; }
    bool isBuffActive { get; set; }
    public abstract void ApplyBuff();
    public abstract void RemoveBuff();
}