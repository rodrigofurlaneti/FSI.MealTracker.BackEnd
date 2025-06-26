using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.MealTracker.Application.Dtos
{
    public class ConsumptionDto : BaseDto
    {
        public long UserId;
        public long MealId;
        public long FoodId;
        public DateTime ConsumptionDate;
        public int Quantity;
    }
}
