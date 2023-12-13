using AutoFixture;
using FluentAssertions;
using MassTransit;
using NSubstitute;
using Warehouse.Application.Commands;
using Warehouse.Domain.Abstractions;
using Warehouse.Domain.Exceptions;
using Xunit;

namespace Warehouse.Orders.UnitTests.Application.Commands
{
    public class TerminateMakeOrderCommandHandlerTests
    {
        public Fixture Fixture { get; set; } = new Fixture();

        [Fact]
        public async Task Should_Throw_When_Product_NotFound()
        {
            // arrange
            var order = Fixture.Create<MakeOrderCommand>();

            order.ProductId = 1;

            var handler = new MakeOrderCommandHandler(Substitute.For<IBus>(), Substitute.For<IProductRepository>(), Substitute.For<IOrderRepository>());

            // act
            var act = () => handler.Handle(order, CancellationToken.None);

            // assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
