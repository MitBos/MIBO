// Summary: Abstraction for mapping source JSON into a transformed shape using configured field mappings.
using MIBO.Domain.Models;

namespace MIBO.Application.Abstractions;

public interface IMappingEngine
{
    string Map(string sourceJson, MappingConfig config);
}

