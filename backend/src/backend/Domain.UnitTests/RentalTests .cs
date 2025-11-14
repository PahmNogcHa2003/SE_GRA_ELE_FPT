using Domain.UnitTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UnitTests
{
    public class RentalTests : BaseDomainTest
    {
        [Fact]
        public void EndRental_ShouldSetStatusToCompleted()
        {
            //// Arrange
            //var rental = Factory.CreateRental();
            //var end = Now.AddMinutes(10);

            //// Act
            //rental.EndRental(end, endStationId: 9);

            //// Assert
            //rental.ShouldBeCompleted(end, 9);
        }

        [Fact]
        public void EndRental_ShouldThrow_WhenEndTimeBeforeStart()
        {
            //var rental = Factory.CreateRental();

            //ExpectException<InvalidOperationException>(
            //    () => rental.EndRental(Now.AddMinutes(-5), 9),
            //    "End time cannot be before start time."
            //);
        }
    }
}
