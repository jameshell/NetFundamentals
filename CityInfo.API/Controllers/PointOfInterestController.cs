﻿using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController, Route("api/cities/{cityId}/pointsofinterest")]
public class PointOfInterestController : ControllerBase
{
    private readonly ILogger<PointOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly CitiesDataStore _citiesDataStore;

    public PointOfInterestController(
        ILogger<PointOfInterestController> logger,
        IMailService mailService,
        CitiesDataStore citiesDataStore)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            var city = _citiesDataStore
                .Cities
                .FirstOrDefault(city => city.Id == cityId);
            
            if (city == null)
            {
                _logger.LogInformation($"Information: City {cityId} was not found.");
                return NotFound();
            }

            return Ok(city);
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"Exception while getting points of interest for city with id {cityId}", e);
            return StatusCode(500, "A problem while handling your request.");
        }
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null) return NotFound($"No cities were found");

        var pointOfInterest = city.PointsOfInterest
            .FirstOrDefault(point => point.Id == pointOfInterestId);
        if (pointOfInterest == null) return NotFound($"No points of interest were found in the city.");


        return Ok(pointOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreationPointOfInterest(
        int cityId,
        [FromBody] PointOfInterestForCreationDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null) return NotFound($"No cities were found");

        // Demo purposes... to be improved.
        var maxPointOfInterestId = _citiesDataStore.Cities
            .SelectMany(
                cities => cities.PointsOfInterest
            ).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointsOfInterest.Add(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest",
            new
            {
                cityId,
                pointOfInterestId = finalPointOfInterest.Id
            },
            finalPointOfInterest);
    }

    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointOfInterest(int cityId,
        int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null) return NotFound($"No cities were found");

        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(point => point.Id == pointOfInterestId);
        if (pointOfInterestFromStore == null) return NotFound($"No points of interest were found in the city.");

        pointOfInterestFromStore.Name = pointOfInterest.Name;
        pointOfInterestFromStore.Description = pointOfInterest.Description;

        return Ok(pointOfInterestFromStore);
    }

    [HttpPatch("{pointOfInterestId}")]
    public ActionResult PartialUpdatePointOfInterest(
        int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null) return NotFound($"No cities were found");

        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(point => point.Id == pointOfInterestId);
        if (pointOfInterestFromStore == null) return NotFound($"No points of interest were found in the city.");

        var pointOfInterestToPatch =
            new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
        if (city == null) return NotFound($"No cities were found");

        var pointOfInterestFromStore = city.PointsOfInterest
            .FirstOrDefault(point => point.Id == pointOfInterestId);
        if (pointOfInterestFromStore == null) return NotFound($"No points of interest were found in the city.");

        city.PointsOfInterest.Remove(pointOfInterestFromStore);
        _mailService.Send($"POI {pointOfInterestFromStore.Name} was deleted" ,
            $"Point of interest (ID= {pointOfInterestFromStore.Id}) was deleted.");
        return NoContent();
    }
}