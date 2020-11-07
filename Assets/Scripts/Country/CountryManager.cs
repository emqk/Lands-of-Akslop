using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CountryManager : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] Color playerCountryColor;
    [SerializeField] Color noCountryColor;
    public Color NoCountryColor { get => noCountryColor; }

    List<Country> countries = new List<Country>();
    List<City> allCities = new List<City>();
    List<UtilityBuilding> allUtilityBuildings = new List<UtilityBuilding>();

    Country playerCountry;
    public Country PlayerCountry { get => playerCountry; }

    Country defaultCountry;
    public Country DefaultCountry { get => defaultCountry; }

    [SerializeField] List<CountryRelation> countryRelations = new List<CountryRelation>();


    public static CountryManager instance;


    CountryManager()
    {
        instance = this;
    }

    private void Awake()
    {
        //Get all cities and utility buildings on the map
        allCities = GameObject.FindObjectsOfType<City>().ToList();
        allUtilityBuildings = GameObject.FindObjectsOfType<UtilityBuilding>().ToList();

        defaultCountry = CreateCountry("Default Country", false, true, noCountryColor);
        playerCountry = CreateCountry("Player Country", true, false, playerCountryColor);
        CreateCountry("Blue Player", false, false, colors[0]);
        CreateCountry("Yellow Player", false, false, colors[1]);
        CreateCountry("Purple Player", false, false, colors[2]);
        CreateCountry("Orange Player", false, false, colors[3]);

        CreateCountryRelations();
    }

    private void Update()
    {
        for (int i = countries.Count-1; i >= 0; i--)
        {
            if (countries[i].ShoulBeDestroyed() && !countries[i].isDefaultCountry && !countries[i].isPlayerCountry)
                RemoveCountry(countries[i]);
        }

        //Update each Country, because they are not MonoBehaviours
        foreach (Country country in countries)
        {
            country.Update();
        }
    }

    public bool IsEveryCityDefaultOrSameCountry()
    {
        Country differentCountry = null;

        List<City> notDefaultCountries = GetCitiesFromCountryOtherThan(DefaultCountry);

        if (notDefaultCountries.Count <= 0)
            return true;

        foreach (City city in notDefaultCountries)
        {
            if (differentCountry == null)
            {
                differentCountry = city.MyCountry;
            }
            else if (differentCountry != city.MyCountry)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsItDefaultCountry(Country country)
    {
        return country == defaultCountry;
    }

    Country CreateCountry(string _countryName, bool _isPlayerCountry, bool _isDefaultCountry, Color _color)
    {
        Country country = new Country(_countryName, _isPlayerCountry, _isDefaultCountry, _color);
        countries.Add(country);

        return country;
    }

    public void RemoveCountry(Country country)
    {
        countries.Remove(country);

        for (int i = countryRelations.Count - 1; i >= 0; i--)
        {
            if (countryRelations[i].Contains(country))
                countryRelations.RemoveAt(i);           
        }
    }

    void CreateCountryRelations()
    {
        for (int from = 0; from < countries.Count - 1; from++)
        {
            for (int to = from + 1; to < countries.Count; to++)
            {
                countryRelations.Add(new CountryRelation(countries[from], countries[to], -100));
            }
        }
    }

    public List<Country> GetEnemyCountriesForCountry(Country country)
    {
        List<Country> result = new List<Country>();

        foreach (CountryRelation countryRelation in countryRelations)
        {
            if (countryRelation.Contains(country))
            {
                if (countryRelation.IsEnemy())
                {
                    result.Add(countryRelation.GetCountryOtherThan(country));
                }
            }
        }

        return result;
    }
    List<Country> GetFriendlyCountriesForCountry(Country country)
    {
        List<Country> result = new List<Country>();

        foreach (CountryRelation countryRelation in countryRelations)
        {
            if (countryRelation.Contains(country))
            {
                if (!countryRelation.IsEnemy())
                {
                    result.Add(countryRelation.GetCountryOtherThan(country));
                }
            }
        }

        return result;
    }
    public List<Country> GetFriendlyCountriesWithWorstRelationsWithCountryExceptDefaultCountry(Country country)
    {
        List<Country> friendlyCountries = GetFriendlyCountriesForCountry(country);
        friendlyCountries.Remove(DefaultCountry);
        float worstRelation = float.MaxValue;
        List<Country> result = new List<Country>();

        //Find worst relation
        foreach (Country item in friendlyCountries)
        {
            CountryRelation countryRelation = GetRelationBetweenCountries(country, item);
            if (countryRelation.GetAmount() < worstRelation)
            {
                worstRelation = countryRelation.GetAmount();
            }
        }

        //Add Countries with the worst relations (the might be more than one with same relation) to result
        foreach (Country item in friendlyCountries)
        {
            CountryRelation countryRelation = GetRelationBetweenCountries(country, item);
            if (countryRelation.GetAmount() == worstRelation)
            {
                result.Add(item);
            }
        }

        return result;
    }


    public CountryRelation GetRelationBetweenCountries(Country c1, Country c2)
    {
        for (int i = 0; i < countryRelations.Count; i++)
        {
            if (countryRelations[i].Contains(c1, c2))
            {
                return countryRelations[i];
            }
        }

        Debug.LogError("Can not find relation between given countries!");
        return null;
    }

    public List<UtilityBuilding> GetEnemyUtilityBuildingsForCountry(Country country)
    {
        return GetEnemyBuildingsForCountry<UtilityBuilding>(country, allUtilityBuildings);
    }

    public List<City> GetEnemyCitiesForCountry(Country country)
    {
        return GetEnemyBuildingsForCountry<City>(country, allCities);
    }

    public List<T> GetEnemyBuildingsForCountry<T>(Country country, List<T> sourceList) where T : Building
    {
        List<T> result = new List<T>();

        foreach (T currBuilding in sourceList)
        {
            if (country == currBuilding.MyCountry)
                continue;

            CountryRelation currCountryRelation = GetRelationBetweenCountries(country, currBuilding.MyCountry);
            if (currCountryRelation.IsEnemy())
            {
                result.Add(currBuilding);
            }
        }

        return result;
    }

    public City GetCityWithTheWorstRelationsWithCountry(Country country)
    {
        float smallestRelation = CountryRelation.maxAmount + 99;
        City resultCity = null;
        List<City> citiesFromCountryOtherThan = GetCitiesFromCountryOtherThan(country);
        foreach (City city in citiesFromCountryOtherThan)
        {
            CountryRelation currCountryRelation = GetRelationBetweenCountries(country, city.MyCountry);
            float relationAmount = currCountryRelation.GetAmount();
            if (relationAmount < smallestRelation)
            {
                smallestRelation = relationAmount;
                resultCity = city;
            }
        }

        return resultCity;
    }
    List<City> GetCitiesFromCountryOtherThan(Country country)
    {
        List<City> result = new List<City>();
        foreach (City city in allCities)
        {
            if (city.MyCountry != country)
            {
                result.Add(city);
            }
        }

        return result;
    }

    public City GetRandomNotCapturedCity()
    {
        List<City> notCapturedCities = new List<City>();
        foreach (City currCity in allCities)
        {
            if (currCity.MyCountry == null)
            {
                notCapturedCities.Add(currCity);
            }
        }

        if (notCapturedCities.Count > 0)
        {
            int index = Random.Range(0, notCapturedCities.Count);
            return notCapturedCities[index];
        }

       /* foreach (City currCity in allCities)
        {
            if (currCity.MyCountry == null)
            {
                return currCity;
            }
        }*/

        return null;
    }

    public List<City> GetAllCitiesWithDefaultCountry()
    {
        return GetAllBuildingsWithoutCountry<City>(allCities);
    }

    public List<UtilityBuilding> GetAllUtilityBuildings()
    {
        return allUtilityBuildings;
    }
    
    public List<UtilityBuilding> GetAllUtilityBuildingsWithDefaultCountry()
    {
        return GetAllBuildingsWithoutCountry<UtilityBuilding>(allUtilityBuildings);
    }

    List<T> GetAllBuildingsWithoutCountry<T>(List<T> sourceList) where T : TargetableObject 
    {
        List<T> result = new List<T>();

        foreach (T currBuilding in sourceList)
        {
            if (IsItDefaultCountry(currBuilding.MyCountry))
            {
                result.Add(currBuilding);
            }
        }

        return result;
    } 
}