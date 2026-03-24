using Application.Abstractions;
using Application.Tenants;
using Domain.Tenants;
using FluentAssertions;
using NSubstitute;

namespace UnitTests.Application;

public sealed class TenantServiceTests
{
    private static readonly DateTimeOffset FixedNow = new(2026, 3, 21, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateAsync_ShouldPersistTenant_WhenRequestIsValid()
    {
        ITenantRepository repository = Substitute.For<ITenantRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(FixedNow);

        TenantService sut = new(repository, unitOfWork, clock);

        TenantDto result = await sut.CreateAsync(
            new CreateTenantRequest("Acme Corp", "Test tenant", "acme-corp", null),
            CancellationToken.None);

        result.TenantName.Should().Be("Acme Corp");
        result.Slug.Should().Be("acme-corp");
        result.CreatedUtc.Should().Be(FixedNow);
        await repository.Received(1).AddAsync(Arg.Any<Tenant>(), CancellationToken.None);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTenantNameIsMissing()
    {
        ITenantRepository repository = Substitute.For<ITenantRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(FixedNow);

        TenantService sut = new(repository, unitOfWork, clock);

        Func<Task> action = async () => await sut.CreateAsync(
            new CreateTenantRequest(" ", null, "valid-slug", null),
            CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>();
        await repository.DidNotReceive().AddAsync(Arg.Any<Tenant>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenSlugIsMissing()
    {
        ITenantRepository repository = Substitute.For<ITenantRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(FixedNow);

        TenantService sut = new(repository, unitOfWork, clock);

        Func<Task> action = async () => await sut.CreateAsync(
            new CreateTenantRequest("Acme Corp", null, " ", null),
            CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>();
        await repository.DidNotReceive().AddAsync(Arg.Any<Tenant>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistTenant_WhenMetadataIsProvided()
    {
        ITenantRepository repository = Substitute.For<ITenantRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(FixedNow);

        TenantService sut = new(repository, unitOfWork, clock);
        const string metadata = """{"region":"eu","tier":"enterprise"}""";

        TenantDto result = await sut.CreateAsync(
            new CreateTenantRequest("Beta Corp", null, "beta-corp", metadata),
            CancellationToken.None);

        result.Metadata.Should().Be(metadata);
        await repository.Received(1).AddAsync(Arg.Any<Tenant>(), CancellationToken.None);
    }
}
