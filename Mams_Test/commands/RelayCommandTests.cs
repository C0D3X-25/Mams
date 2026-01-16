using Mams_App.src.commands;

namespace Mams_Test.commands;

public class RelayCommandTests
{
    [Fact]
    public void Execute_WithNullParameter_ExecutesAction()
    {
        // Arrange
        var executed = false;
        var command = new RelayCommand(_ => executed = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void Execute_WithParameter_PassesParameterToAction()
    {
        // Arrange
        object? receivedParameter = null;
        var command = new RelayCommand(param => receivedParameter = param);
        var expectedParameter = "test parameter";

        // Act
        command.Execute(expectedParameter);

        // Assert
        Assert.Equal(expectedParameter, receivedParameter);
    }

    [Fact]
    public void CanExecute_NoCanExecuteProvided_ReturnsTrue()
    {
        // Arrange
        var command = new RelayCommand(_ => { });

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanExecute_CanExecuteReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var command = new RelayCommand(_ => { }, _ => true);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanExecute_CanExecuteReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var command = new RelayCommand(_ => { }, _ => false);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanExecute_WithParameter_PassesParameterToCanExecute()
    {
        // Arrange
        object? receivedParameter = null;
        var command = new RelayCommand(_ => { }, param =>
        {
            receivedParameter = param;
            return true;
        });
        var expectedParameter = "test parameter";

        // Act
        command.CanExecute(expectedParameter);

        // Assert
        Assert.Equal(expectedParameter, receivedParameter);
    }

    [Fact]
    public void CanExecute_BasedOnParameterValue_ReturnsCorrectResult()
    {
        // Arrange
        var command = new RelayCommand(_ => { }, param => param is int i && i > 0);

        // Act & Assert
        Assert.True(command.CanExecute(1));
        Assert.False(command.CanExecute(0));
        Assert.False(command.CanExecute(-1));
        Assert.False(command.CanExecute("not an int"));
    }

    [Fact]
    public void Execute_MultipleExecutions_ExecutesEachTime()
    {
        // Arrange
        var executionCount = 0;
        var command = new RelayCommand(_ => executionCount++);

        // Act
        command.Execute(null);
        command.Execute(null);
        command.Execute(null);

        // Assert
        Assert.Equal(3, executionCount);
    }

    [Fact]
    public void Constructor_WithExecuteOnly_CreatesValidCommand()
    {
        // Arrange & Act
        var command = new RelayCommand(_ => { });

        // Assert
        Assert.NotNull(command);
        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void Constructor_WithExecuteAndCanExecute_CreatesValidCommand()
    {
        // Arrange & Act
        var command = new RelayCommand(_ => { }, _ => true);

        // Assert
        Assert.NotNull(command);
    }
}
