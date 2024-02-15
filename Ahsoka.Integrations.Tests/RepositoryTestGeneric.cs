using Ahsoka.Domain.SeedWork.Repository.CommandRepository;

namespace Ahsoka.Integrations.Tests;

public class RepositoryTestGeneric<TEntity, TEntityId, TRepository>(DbContextOptions<PrincipalContext> _dbOptions)
    where TEntityId : IEquatable<TEntityId>
    where TEntity : Entity<TEntityId>
    where TRepository : ICommandRepository<TEntity, TEntityId>
{

    public async Task CreateEntityAsync(Func<TEntity> GetEntity)
    {
        //Arrange
        using PrincipalContext context = new(_dbOptions);
        var _repository = (TRepository)Activator.CreateInstance(typeof(TRepository), context)!;
        var unitWork = new UnitOfWork(context);

        var item = GetEntity.Invoke();

        //Act
        await _repository.InsertAsync(item, CancellationToken.None);

        //Assert

        using (var context2 = new PrincipalContext(_dbOptions))
        {
            var database = context2.Set<TEntity>().Find(item.Id);
            database.Should().BeNull("should not affect before unit of work commit");
        }

        await unitWork.CommitAsync(CancellationToken.None);

        using (var context2 = new PrincipalContext(_dbOptions))
        {
            var database = context2.Set<TEntity>().Find(item.Id);
            database.Should().NotBeNull("should not affect before unit of work commit");
        }
    }

    public async Task GetEntityByIdAsync(Func<TEntity> GetEntityFromDatabase)
    {
        //Arrange
        using PrincipalContext context = new(_dbOptions);
        var _repository = (TRepository)Activator.CreateInstance(typeof(TRepository), context)!;
        var unitWork = new UnitOfWork(context);

        var item = GetEntityFromDatabase.Invoke();

        //Act
        var database = await _repository.GetByIdAsync(item.Id, CancellationToken.None);

        //Assert
        database.Should().NotBeNull();
    }

    public async Task DeleteEntityAsync(Func<TEntity> GetEntityFromDatabase)
    {
        //Arrange
        var item = GetEntityFromDatabase.Invoke();

        using PrincipalContext context = new(_dbOptions);
        var _repository = (TRepository)Activator.CreateInstance(typeof(TRepository), context)!;
        var unitWork = new UnitOfWork(context);

        //Act
        await _repository.DeleteAsync(item.Id, CancellationToken.None);

        //Assert
        using (var context2 = new PrincipalContext(_dbOptions))
        {
            var database = context2.Set<TEntity>().Find(item.Id);
            database.Should().NotBeNull("should not affect before unit of work commit");
        }

        await unitWork.CommitAsync(CancellationToken.None);

        using (var context2 = new PrincipalContext(_dbOptions))
        {
            var database = context2.Set<TEntity>().Find(item.Id);
            database.Should().BeNull("should not affect before unit of work commit");
        }
    }
}
