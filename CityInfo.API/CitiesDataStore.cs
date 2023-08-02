using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }
    // public static CitiesDataStore Current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id = 1,
                Name = "New York City",
                Description = "A big city.",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Point A",
                        Description = "A point of interest"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "Point b",
                        Description = "A point of interest"
                    }
                }
            },
            new CityDto()
            {
                Id = 2,
                Name = "Bogota",
                Description = "A huge city.",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 7,
                        Name = "Point A",
                        Description = "A point of interest"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 9,
                        Name = "Point b",
                        Description = "A point of interest"
                    }
                }
            },
            new CityDto()
            {
                Id = 3,
                Name = "Caracas",
                Description = "A latin city.",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 5,
                        Name = "Point A",
                        Description = "A point of interest"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 6,
                        Name = "Point b",
                        Description = "A point of interest"
                    }
                }
            }
        };
    }
}