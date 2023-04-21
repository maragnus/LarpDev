using Larp.Data.MwFifth;

namespace Larp.Landing.Client.Pages;

public record SkillItem(SkillDefinition Definition, CharacterSkill Purchase)
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";

    public bool IsPurchased => Purchase.Purchases > 0;
    public bool IsLastRank => Purchase.Purchases == 1;

    public void Update()
    {
        var ranksPerPurchase = Definition.RanksPerPurchase ?? 1;
        Purchase.Rank = (Purchase.Purchases ?? 0) * ranksPerPurchase;

        if (Purchase.Purchases is null or 0)
            Title = ranksPerPurchase > 1 ? $"{Definition.Name} {Definition.RanksPerPurchase}" : Definition.Title;
        else if (Definition.Purchasable == SkillPurchasable.Once)
            Title = ranksPerPurchase == 1 ? Definition.Title : $"{Definition.Name} {Purchase.Rank}";
        else
            Title = $"{Definition.Name} {Purchase.Rank}";

        if (Definition.Purchasable == SkillPurchasable.Unavailable)
            Subtitle = "Unavailable for purchase";
        else if (Definition.Purchasable == SkillPurchasable.Once)
            Subtitle = ranksPerPurchase == 1
                ? $"{Definition.CostPerPurchase} MS"
                : $"{Definition.CostPerPurchase} MS for {Definition.RanksPerPurchase} ranks";
        else
            Subtitle = ranksPerPurchase == 1
                ? $"{Definition.CostPerPurchase} MS per rank"
                : $"{Definition.CostPerPurchase} MS for {ranksPerPurchase} ranks";
    }

    public void Add()
    {
        switch (Definition.Purchasable)
        {
            case SkillPurchasable.Unavailable:
                Purchase.Purchases = 1;
                return;
            case SkillPurchasable.Once:
                Purchase.Purchases = 1;
                return;
            case SkillPurchasable.Multiple:
            default:
                Purchase.Purchases += 1;
                break;
        }

        Update();
    }

    public void Remove()
    {
        switch (Definition.Purchasable)
        {
            case SkillPurchasable.Unavailable:
                Purchase.Purchases = 0;
                return;
            case SkillPurchasable.Once:
                Purchase.Purchases = 0;
                return;
            case SkillPurchasable.Multiple:
            default:
                Purchase.Purchases = Math.Max(0, (Purchase.Purchases ?? 0) - 1);
                break;
        }

        Update();
    }
}