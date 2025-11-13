using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UnitTests.Base
{
    public abstract class BaseDomainTest
    {
        protected DateTimeOffset Now => DateTimeOffset.UtcNow;
        protected const long DefaultUserId = 100;
        protected const long DefaultVehicleId = 200;
        protected const long DefaultStationId = 300;

        protected EntityFactory Factory => new EntityFactory(Now);

        // có thể thêm method helper chung
        protected void ExpectException<T>(Action action, string message)
            where T : Exception
        {
            var ex = Assert.Throws<T>(action);
            ex.Message.Should().Be(message);
        }
    }
}
