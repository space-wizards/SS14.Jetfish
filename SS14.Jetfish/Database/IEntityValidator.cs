namespace SS14.Jetfish.Database;

public interface IEntityValidator
{
    void Validate();
}

public class EntityValidationFailedException : Exception
{
    public EntityValidationFailedException(string type, string message) : base($"{type} failed validation: {message}")
    {

    }
}