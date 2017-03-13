using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;

namespace Net.Appclusive.Workflows
{

    public sealed class Sleeper : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<int> Seconds { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var seconds = context.GetValue(this.Seconds);
            //System.Threading.Thread.Sleep(seconds * 1000);
            Task.Delay(seconds * 1000);
        }
    }
}
