using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Domain.Entities;
public class ReprocessMaterialEntity
{
    public int TypeId { get; set; }
    public TypeEntity Type {  get; set; }

    public int MaterialId { get; set; }
    public TypeEntity Material { get; set; }

    public int Quantity {  get; set; }     

}
