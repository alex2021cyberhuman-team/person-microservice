using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Conduit.Person.DataAccessLayer;

public class Neo4JDriverFactory
{
    private readonly IOptionsMonitor<Neo4JOptions> _optionsMonitor;

    public Neo4JDriverFactory(
        IOptionsMonitor<Neo4JOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public IDriver CreateDriver()
    {
        var options = _optionsMonitor.CurrentValue;
        var authToken = AuthTokens.Basic(options.Username, options.Password);
        var driver = GraphDatabase.Driver(options.Uri, authToken);
        return driver;
    }
}
