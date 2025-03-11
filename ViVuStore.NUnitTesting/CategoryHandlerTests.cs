using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using ViVuStore.Business.Handlers;
using ViVuStore.Business.ViewModels;
using ViVuStore.Core.Exceptions;
using ViVuStore.Data;
using ViVuStore.Data.Repositories;
using ViVuStore.Data.UnitOfWorks;
using ViVuStore.Models.Common;
using ViVuStore.Models.Security;

namespace ViVuStore.NUnitTesting;

[TestFixture]
public class CategoryHandlerTests
{
    private ViVuStoreDbContext _dbContext;
    private IMapper _mapper;
    private IUnitOfWork _unitOfWork;
    private List<Category> _categories;
    private User _testUser;
    private string _databaseName;
    private Mock<IUserIdentity> _userIdentityMock;

    [SetUp]
    public void Setup()
    {
        // Create a unique database name for each test to ensure isolation
        _databaseName = $"ViVuStoreTest_{Guid.NewGuid()}";
        
        // Setup the in-memory database
        var options = new DbContextOptionsBuilder<ViVuStoreDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;
        
        _dbContext = new ViVuStoreDbContext(options);
        
        // Setup AutoMapper
        var mapperConfig = new MapperConfiguration(cfg => 
        {
            // Add your mapping profiles here
            cfg.CreateMap<Category, CategoryViewModel>();
        });
        _mapper = mapperConfig.CreateMapper();
        
        // Setup test data
        _testUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User"
        };
        
        _dbContext.Users.Add(_testUser);
        _dbContext.SaveChanges();
        
        // Setup mock user identity
        _userIdentityMock = new Mock<IUserIdentity>();
        _userIdentityMock.Setup(ui => ui.UserId).Returns(_testUser.Id);
        _userIdentityMock.Setup(ui => ui.UserName).Returns(_testUser.UserName);
        
        _categories = new List<Category>
        {
            new Category 
            { 
                Id = Guid.NewGuid(), 
                Name = "Category 1", 
                Description = "Description 1",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedById = _testUser.Id,
                CreatedBy = _testUser
            },
            new Category 
            { 
                Id = Guid.NewGuid(), 
                Name = "Category 2", 
                Description = "Description 2",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedById = _testUser.Id,
                CreatedBy = _testUser
            }
        };
        
        _dbContext.Categories.AddRange(_categories);
        _dbContext.SaveChanges();
        
        // Setup UnitOfWork with the real DbContext and mock user identity
        _unitOfWork = new UnitOfWork(_dbContext, _userIdentityMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        // Fix the order of operations:
        // Delete the database first, then dispose objects
        _dbContext.Database.EnsureDeleted();
        _unitOfWork.Dispose();
        _dbContext.Dispose();
    }

    [Test]
    public async Task CategoryGetAllQuery_ReturnsAllCategories()
    {
        // Arrange
        var handler = new CategoryGetAllQueryHandler(_unitOfWork, _mapper);
        var query = new CategoryGetAllQuery();
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<CategoryViewModel>>());
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(c => c.Name == "Category 1"), Is.True);
        Assert.That(result.Any(c => c.Name == "Category 2"), Is.True);
    }
    
    [Test]
    public async Task CategoryGetByIdQuery_WithValidId_ReturnsCategory()
    {
        // Arrange
        var targetCategory = _categories[0];
        var handler = new CategoryGetByIdQueryHandler(_unitOfWork, _mapper);
        var query = new CategoryGetByIdQuery { Id = targetCategory.Id };
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(targetCategory.Id));
        Assert.That(result.Name, Is.EqualTo(targetCategory.Name));
        Assert.That(result.Description, Is.EqualTo(targetCategory.Description));
    }
    
    [Test]
    public void CategoryGetByIdQuery_WithInvalidId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var handler = new CategoryGetByIdQueryHandler(_unitOfWork, _mapper);
        var query = new CategoryGetByIdQuery { Id = invalidId };
        
        // Act & Assert
        Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
            await handler.Handle(query, CancellationToken.None));
    }
}
