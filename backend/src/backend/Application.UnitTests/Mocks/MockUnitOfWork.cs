using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System.Threading;

namespace Application.UnitTests.Mocks
{
    public static class MockUnitOfWork
    {
        public static Mock<IUnitOfWork> GetMockUnitOfWork()
        {
            var mockUow = new Mock<IUnitOfWork>();

            // 1. Mock Transaction
            // Cần mock IAsyncDisposable và IDisposable
            var mockTransaction = new Mock<IDbContextTransaction>();

            // 2. "Dạy" BeginTransactionAsync trả về mock transaction
            mockUow.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(mockTransaction.Object);

            // 3. "Dạy" SaveChangesAsync trả về 1 (thành công)
            mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

            // 4. "Dạy" các hàm Commit/Rollback (chúng ta sẽ Verify sau)
            mockUow.Setup(u => u.CommitTransactionAsync(It.IsAny<IDbContextTransaction>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

            mockUow.Setup(u => u.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

            return mockUow;
        }
    }
}