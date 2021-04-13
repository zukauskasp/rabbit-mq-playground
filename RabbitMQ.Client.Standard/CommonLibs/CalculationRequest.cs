using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class CalculationRequest
    {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public RequestType Request { get; }

        private CalculationRequest()
        {

        }

        public CalculationRequest(int number1, int number2, RequestType request)
        {
            Number1 = number1;
            Number2 = number2;
            Request = request;
        }
    }
}
