//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KindergardenFood.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Food_Norm
    {
        public int Id { get; set; }
        public int Category { get; set; }
        public int Food_ID { get; set; }
        public double Norm_value { get; set; }
        public Nullable<System.DateTime> Norm_date { get; set; } = DateTime.Now;
    
        public virtual Categories Categories { get; set; }
        public virtual Food Food { get; set; }
    }
}
