using MIBO.Models;

namespace MIBO.Services;

public interface IMappingEngine
{
    string Map(string sourceJson, MappingConfig config);
}
