/**
 * Copyright 2017 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Activities;
using System.Diagnostics.Contracts;
using biz.dfch.CS.Commons;
using Net.Appclusive.Workflows.Public;

namespace Net.Appclusive.Workflows
{
    public sealed class ReturnResult : NativeActivity, IBuildModel
    {
        [RequiredArgument]
        public InArgument<long> ParentItemId { get; set; }

        [RequiredArgument]
        public InArgument<string> ModelName { get; set; }

        [RequiredArgument]
        public InArgument<DictionaryParameters> Configuration { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            Contract.Assert(null != context);

            context.Track(new ReturnResultTrackingRecord(context.WorkflowInstanceId)
            {
                Result = "this is the end",
            });
        }
    }
}
