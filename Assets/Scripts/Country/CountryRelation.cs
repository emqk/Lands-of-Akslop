[System.Serializable]
public class CountryRelation
{
    Country country1;
    Country country2;
    [UnityEngine.SerializeField] float amount;

    public const float minAmount = -100;
    public const float maxAmount = 100;


    public CountryRelation(Country c1, Country c2, float relationAmount)
    {
        if(c1 == c2)
            UnityEngine.Debug.LogError("Both countries are the same!");

        country1 = c1;
        country2 = c2;
        amount = relationAmount;
    }

    public float GetAmount()
    {
        return amount;
    }

    public void SetAmount(float newAmount)
    {
        amount = newAmount;
    }

    public bool IsEnemy()
    {
        return amount < 0;
    }

    public void MakeWar()
    {
     /*   UnityEngine.Debug.Log("Made war");
        if(ContainsPlayerCountry())
            ActionManager.instance.CreateAction(ActionManager.ActionInformationContent.WarDeclared);

        SetAmount(-5);*/
    }

    public void MakePeace()
    {
       /* UnityEngine.Debug.Log("Made peace");
        if(ContainsPlayerCountry())
            ActionManager.instance.CreateAction(ActionManager.ActionInformationContent.PeaceDeclared);
        
        SetAmount(0);*/
    }

    public void ChangeAmount(float amountToAdd)
    {
      /*  amount = UnityEngine.Mathf.Clamp(amount + amountToAdd, minAmount, maxAmount);
        UnityEngine.Debug.Log("Country relations between " + country1.GetCountryName() + " and " + country2.GetCountryName() + " changed to: " + amount);*/
    }

    public Country GetCountryOtherThan(Country c1)
    {
        if (country1 == c1)
            return country2;
        if (country2 == c1)
            return country1;

        UnityEngine.Debug.LogError("Both countries are the same!");

        return null;
    }

    public bool Contains(Country c1, Country c2)
    {
        if (c1 == c2)
            UnityEngine.Debug.LogError("Both countries are the same! I can't find realation between them!");

        return (Contains(c1) && Contains(c2));
    }
    public bool Contains(Country c1)
    {
        return (country1 == c1 || country2 == c1);
    }

    bool ContainsPlayerCountry()
    {
        return (country1.isPlayerCountry || country2.isPlayerCountry);
    }

}