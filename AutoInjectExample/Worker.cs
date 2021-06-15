using AutoInjectCTOR;
using AutoInjectExample.Services;

namespace AutoInjectExample
{
    [InjectCtor]
    public partial class Worker
    {
        readonly ServiceA serviceA;
        readonly ServiceB serviceB;
        readonly ServiceC serviceC;

        public void Execute()
        {
            serviceA.Work();
            serviceB.Work();
            serviceC.Work();
        }
    }
}
