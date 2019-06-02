# Region Locator

This is simple web api that provides a reverse geocoding functionality:
- It receives a coordinate (lat/lon) and determines on which region it's contained.

The dataset used is configurable, hence this service could be used in various use-cases.

## Installation

After cloning the repo, go to top folder and type :

    dotnet restore

## Usage

    cd src
    cd RegionLocator.Api
    dotnet run

By default it will try to run on port 5001 on HTTPS. A valid URL will be:

    https://localhost:5001/api/regions/byCoordinate?longitude=-8.5&latitude=38
    
If it's working properly it should return:

    {
        "properties": {
            "ADMIN": "Portugal",
            "POP_EST": 10839514
        }
    }

If you supply a coordinate that doesn't match any polygon a 404 will be returned. Example:

    https://localhost:5001/api/Regions/byCoordinates?longitude=-30&latitude=38

Should return (besides the 404 status code):

    {
        "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        "title": "Not Found",
        "status": 404,
        "traceId": "0HLN6TQ4CH9E9:00000002"
    }

## Configuration

Included with the source code is a sample dataset that contains the polygons of all countries in the world, obtained from https://www.naturalearthdata.com/downloads/10m-cultural-vectors/

The default configuration matches this dataset, configured at `datasource.json`

    {
        "shapefilePath": "Resources/ne_10m_admin_0_countries.shp",
        "shapefileProperties": [
          "ADMIN",
          "POP_EST"
        ]
    }

Please note that the defaults should just work, as the included Shapefile has a rule for it to be automatically copied to the bin directory.

| field | description |
|-------|-------------|
| shapefilePath  |  Describes the path (either absolute or relative) to the Shapefile that will contain the various shapes to match  |
| shapefileProperties  |  Properties (from the original Shapefile) that will be returned on the matching results of the API  |

## TODO

- Supporting multiple data-sources simultaneously
- Improve performance
    - Support some sort of indexing (Quadtrees, etc)
    - Optimization of the polygons when initially loading them

- Create tests
- Organize the projects better

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request

## History

0.1. First version

## License

MIT
