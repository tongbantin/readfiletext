using System;
using System.Collections.Generic;
using System.Text;

namespace txt.import.Class
{
    [Serializable]
    class CustomException : Exception
    {
        public CustomException()
        {

        }

        public CustomException(string name)
            : base(String.Format("Error:", name))
        {

        }

    }
}
