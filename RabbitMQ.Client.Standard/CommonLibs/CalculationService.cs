using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibs
{
    public class CalculationService
    {
        public CalculationResponse Calculate(CalculationRequest request)
        {
            if (request.Request == RequestType.Add)
            {
                return new CalculationResponse(request.Number1 + request.Number2);
            }

            else if (request.Request == RequestType.Substract)
            {
                return new CalculationResponse(request.Number1 - request.Number2);
            }

            return new CalculationResponse(0);
        }
    }
}
