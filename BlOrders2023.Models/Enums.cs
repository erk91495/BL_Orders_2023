using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models.Enums;

public enum OrderStatus
{
    Ordered,
    Filling,
    Filled,
    Invoiced,
    Complete,
}

public enum ShippingType
{
    [Description("No Type")]
    NoType,
    Pickup,
    Delivery,
}

public enum States
{
    AL,
    AK,
    AZ,
    AR,
    CA,
    CO,
    CT,
    DE,
    FL,
    GA,
    HI,
    ID,
    IL,
    IN,
    IA,
    KS,
    KY,
    LA,
    ME,
    MD,
    MA,
    MI,
    MN,
    MS,
    MO,
    MT,
    NE,
    NV,
    NH,
    NJ,
    NM,
    NY,
    NC,
    ND,
    OH,
    OK,
    OR,
    PA,
    RI,
    SC,
    SD,
    TN,
    TX,
    UT,
    VT,
    VA,
    WA,
    WV,
    WI,
    WY
}

public enum CustomerAllocationType
{
    Grocer = -1,
    Gift,
}

public enum  AllocatorMode
{
#if DEBUG
    Test = -1,
#endif //DEBUG
    Gift,
    Grocer,
    Both,
}

public enum BoxType
{
    Unknown,
    BBox,
    CBox,
    NonGMO_BBox,
    NonGMO_CBox,
    BreastBox,
    NonGMOBreastBox,
}

public enum InventoryReconciliationAction
{
    None,
    Added,
    Removed,
}

public enum DiscountTypes
{
    [Description("Set Price")]
    SetPrice,
    [Description("Percent Off")]
    PercentOff,
    //DollarsOff,    
}

public enum ReportFunctions
{
    Display,
    Print,
    Email
}