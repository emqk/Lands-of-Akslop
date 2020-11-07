using System.Text;

[System.Serializable]
public class BuyRequirement
{
    public ItemType itemType;
    public int amount;

    public static string GetAsString(BuyRequirement[] buyRequirements, int multiply = 1)
    {
        StringBuilder builder = new StringBuilder();
        foreach (BuyRequirement currentRequirement in buyRequirements)
        {
            builder.Append(" | ");
            builder.Append(currentRequirement.itemType);
            builder.Append(": ");
            builder.Append(currentRequirement.amount * multiply);
        }

        return builder.ToString();
    }
}