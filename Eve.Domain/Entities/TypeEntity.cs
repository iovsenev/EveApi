using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.Text.Json;
using Eve.Domain.Entities.Products;

namespace Eve.Domain.Entities;
public class TypeEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } = "";
    public bool Published { get; set; }
    public float? Capacity { get; set; }
    public float? Mass { get; set; }
    public float? PackagedVolume { get; set; }
    public int? PortionSize { get; set; }
    public float? Radius { get; set; }
    public float? Volume { get; set; }
    public bool IsProduct { get; set; } = false;

    public int? IconId { get; set; }
    public IconEntity Icon { get; set; }

    public int? MarketGroupId { get; set; }
    public MarketGroupEntity MarketGroup { get; set; }

    public int GroupId { get; set; }
    public GroupEntity Group { get; set; }

    public ProductEntity Product { get; set; }

    public List<ReprocessMaterialEntity> ReprocessComponents { get; set; } = new List<ReprocessMaterialEntity>();

    public override string ToString()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        return  JsonSerializer .Serialize(this, options);
    }
}

public class DogmaEffect
{
    public int AttributeId { get; set; }
    public int Value { get; set; }
}

public class DogmaAttribute
{
    public int EffectId { get; set; }
    public bool IsDefault { get; set; }
}