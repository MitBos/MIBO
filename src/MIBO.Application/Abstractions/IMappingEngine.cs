using MIBO.Domain.Models;

namespace MIBO.Application.Abstractions;

public interface IMappingEngine
{
    string Map(string sourceJson, MappingConfig config);
}
