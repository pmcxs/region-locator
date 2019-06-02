using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using RegionLocator.Core;

namespace RegionLocator.API
{
    public class ShapefileRegionsLookupHandler : IRegionsLookupHandler
    {
        private readonly FeatureCollection _features;

        public ShapefileRegionsLookupHandler(IConfiguration configuration)
        {
            string[] propertiesToInclude = configuration
                .GetSection("shapefileProperties")
                .GetChildren()
                .ToList()
                .Select(c => c.Value.ToUpper())
                .ToArray();

            _features = LoadFeatures(configuration["shapefilePath"], propertiesToInclude);

        }

        public Region GetRegion(double lon, double lat)
        {
            return QueryRegions(new Coordinate(lon, lat));
        }

        private FeatureCollection LoadFeatures(string shpFilename, string[] propertiesToInclude)
        {
            FeatureCollection features = new FeatureCollection();

            GeometryFactory factory = new GeometryFactory();

            using (var shapeFileDataReader = new ShapefileDataReader(shpFilename, factory))
            {
                DbaseFileHeader header = shapeFileDataReader.DbaseHeader;

                while (shapeFileDataReader.Read())
                {
                    Feature feature = new Feature();
                    AttributesTable attributesTable = new AttributesTable();

                    string[] keys = new string[header.NumFields];

                    IGeometry geometry = (Geometry)shapeFileDataReader.Geometry;

                    for (int i = 0; i < header.NumFields; i++)
                    {
                        DbaseFieldDescriptor fldDescriptor = header.Fields[i];

                        if(propertiesToInclude.Contains(fldDescriptor.Name.ToUpper()))
                        {
                            keys[i] = fldDescriptor.Name;
                            attributesTable.Add(fldDescriptor.Name, shapeFileDataReader.GetValue(i + 1));
                        }
                    }

                    feature.Geometry = geometry;
                    IGeometry envelope = geometry.Envelope;
                    feature.BoundingBox = new Envelope(envelope.Coordinates[0], envelope.Coordinates[2]);
                    feature.Attributes = attributesTable;
                    features.Add(feature);
                }
            }

            return features;
        }

        private Region QueryRegions(Coordinate coordinate)
        {
            IFeature matchFeature = null;

            for (var i = 0; i < _features.Count; i++)
            {
                if (_features[i].BoundingBox.Contains(coordinate))
                {
                    if (_features[i].Geometry.Contains(new Point(coordinate)))
                    {
                        matchFeature = _features[i];
                        break;
                    }
                }
            }

            if (matchFeature == null)
                return null;

            var region = new Region
            {
                Properties = new Dictionary<string, object>()
            };


            foreach (var attributeName in matchFeature.Attributes.GetNames())
            {
                region.Properties.Add(attributeName, matchFeature.Attributes[attributeName]);

            }

            return region;

        }
    }
}