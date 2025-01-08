using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eve.Domain.Entities;
public class GroupEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Published { get; set; }


    public int CategoryId { get; set; }
    public CategoryEntity Category { get; set; }

    public List<TypeEntity> Types { get; set; } = new List<TypeEntity>();
}
