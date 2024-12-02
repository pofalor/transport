using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport
{
    public class Element
    {
        public int Value { get; set; }

        /// <summary>
        /// Тут хранится потенциал
        /// </summary>
        public int? Potential { get; set; }

        /// <summary>
        /// Тут хранится значение отправляемого груза
        /// </summary>
        public int Weight { get; set; }

        public bool IsPotentialNegative { get; set; }

        /// <summary>
        /// Индекс по строкам(N)
        /// </summary>
        public int IndexRow { get; set; }

        /// <summary>
        /// Индекс по столбцам(M)
        /// </summary>
        public int IndexCol { get; set; }

        public override string ToString()
        {
            return this.Value.ToString()+ "{" + this.Weight.ToString() + "}";
        }

    }
}
