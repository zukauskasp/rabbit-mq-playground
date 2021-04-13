using RabbitMQ.Client.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class CalculationRequest
    {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public RequestType RequestType { get; set; }

        private CalculationRequest()
        {

        }

        public CalculationRequest(int number1, int number2, RequestType request)
        {

        }
    }
}
