using System.Runtime.Serialization;

namespace Conduit.Person.DataAccessLayer;

public class Neo4JApplicationException : ApplicationException
{
    public Neo4JApplicationException()
    {
    }

    protected Neo4JApplicationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    public Neo4JApplicationException(
        string? message) : base(message)
    {
    }

    public Neo4JApplicationException(
        string? message,
        Exception? innerException) : base(message, innerException)
    {
    }
}
