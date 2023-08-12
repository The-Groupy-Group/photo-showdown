using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoShowdownBackend.Tests;

internal class TestUtils
{
    private static readonly Random _random = new();
    private static int _testNum = new();

    // Get in memory database
    public static PhotoShowdownDbContext GetInMemoryContext()
    {
        var dbName = $"TestDb{_testNum++}";
        var options = new DbContextOptionsBuilder<PhotoShowdownDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new PhotoShowdownDbContext(options);
    }

    // Get IMapper from automapper profile
    public static IMapper GetMapper()
    {
        //var config = new MapperConfiguration(cfg =>
        //{
        //    cfg.AddProfile<MappingConfig>();
        //});

        //return config.CreateMapper();

        return new MapperConfiguration(cfg => cfg.AddMaps(typeof(MappingConfig).Assembly)).CreateMapper();
    }
}
