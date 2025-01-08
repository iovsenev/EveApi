namespace Eve.Domain.Interfaces;

public interface IEntityLoader
{
    Task Run(CancellationToken token);
}