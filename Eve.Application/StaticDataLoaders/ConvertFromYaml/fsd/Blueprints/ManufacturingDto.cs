using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eve.Application.StaticDataLoaders.ConvertFromYaml.fsd.Blueprints;
public class ManufacturingDto
{
    public int TypeId { get; set; }
    public int Quantity { get; set; }
    public int Time { get; set; }
    public int MaxProductionLimit { get; set; }

    public List<ProductSkillDto> Skills { get; set; }
    public List<ProductMaterialDto> Materials { get; set; }

    public override string ToString()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(this, options);
    }
}

public class ProductMaterialDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int TypeId { get; set; }
    public int ProductId { get; set; }
}

public class ProductSkillDto
{
    public int Id { get; set; }
    public int Level { get; set; }
    public int TypeId { get; set; }
    public int ProductId { get; set; }
}