using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBrix.Cloning.Tests.Mocks
{
    internal class SelfReferencingMock
    {
        public SelfReferencingMock Other { get; set; }
    }
}
