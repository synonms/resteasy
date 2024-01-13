namespace Synonms.RestEasy.Core.Environment;

public interface IDateProvider
{
    DateTime Now();
    
    DateOnly Today();
}