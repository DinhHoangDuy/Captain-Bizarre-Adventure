public interface IChip
{
    ExpansionChipSlot expansionChipSlot { get; set; }
    ExpansionChipStatus expansionChipStatus { get; set; }
    bool isBuffActive { get; set; }
    void ApplyBuff()
    {
        isBuffActive = true;
    }
    void RemoveBuff()
    {
        isBuffActive = false;
    }
}