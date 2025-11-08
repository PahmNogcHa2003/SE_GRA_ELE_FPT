using Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UnitTests.Base
{
    public static class CustomAssertions
    {
        public static void ShouldBeCompleted(this Rental rental, DateTimeOffset expectedEndTime, long expectedEndStation)
        {
            rental.Status.Should().Be("Completed");
            rental.EndTime.Should().Be(expectedEndTime);
            rental.EndStationId.Should().Be(expectedEndStation);
        }

        public static void ShouldBeOngoing(this Rental rental)
        {
            rental.Status.Should().Be("Ongoing");
            rental.EndTime.Should().BeNull();
        }
    }
}
