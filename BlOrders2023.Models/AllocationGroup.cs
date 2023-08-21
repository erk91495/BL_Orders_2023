using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
[Table("tbl_AllocationGroups")]
public class AllocationGroup
{
    #region Properties
    public int ID { get; set; }
    public string GroupName { get; set; } = "";
    public List<int> ProductIDs { get; set; } = new();
    public int AllocationOrder { get; set; }
    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    #endregion Constructors

    #region Methods
    #endregion Methods
}
