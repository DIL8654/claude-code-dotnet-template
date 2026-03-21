using Application.Abstractions;
using Application.Common.Exceptions;
using Application.WorkItems;
using Domain.WorkItems;
using FluentAssertions;
using NSubstitute;

namespace UnitTests.Application;

public sealed class WorkItemServiceTests
{
    private static readonly DateTimeOffset FixedNow = new(2026, 3, 21, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateAsync_ShouldPersistWorkItem_WhenRequestIsValid()
    {
        IWorkItemRepository repository = Substitute.For<IWorkItemRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(FixedNow);

        WorkItemService sut = new(repository, unitOfWork, clock);

        WorkItemDto result = await sut.CreateAsync(new CreateWorkItemRequest("Template repo", "Seed use case"), CancellationToken.None);

        result.Title.Should().Be("Template repo");
        result.Status.Should().Be(WorkItemStatus.Active);
        result.CreatedUtc.Should().Be(FixedNow);
        await repository.Received(1).AddAsync(Arg.Any<WorkItem>(), CancellationToken.None);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTitleIsMissing()
    {
        IWorkItemRepository repository = Substitute.For<IWorkItemRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(FixedNow);

        WorkItemService sut = new(repository, unitOfWork, clock);

        Func<Task> action = async () => await sut.CreateAsync(new CreateWorkItemRequest(" ", null), CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>();
        await repository.DidNotReceive().AddAsync(Arg.Any<WorkItem>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAsync_ShouldThrowNotFound_WhenWorkItemDoesNotExist()
    {
        IWorkItemRepository repository = Substitute.For<IWorkItemRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IClock clock = Substitute.For<IClock>();

        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((WorkItem?)null);

        WorkItemService sut = new(repository, unitOfWork, clock);

        Func<Task> action = async () => await sut.GetAsync(Guid.NewGuid(), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }
}
